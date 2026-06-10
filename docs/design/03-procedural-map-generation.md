# 03 — Procedural Map Generation (design sketch)

> **PROPOSAL — forward-looking; not as-built.** Current architecture = [`CONTEXT-MAP.md`](../../CONTEXT-MAP.md) + the per-module `context.md` files. **Verify against live code before implementing.** This is the *how-to-build* for [`docs/ideas/03-procedural-map-generation.md`](../ideas/03-procedural-map-generation.md) — read that first for the *why/what*.

Source idea: [`ideas/03`](../ideas/03-procedural-map-generation.md) · Prereq refactors: [`ARCHITECTURE-REFACTORS.md` #5](../ARCHITECTURE-REFACTORS.md) (pool internals), [#7](../ARCHITECTURE-REFACTORS.md) (archive dead generators) · Blocking bugs: [`BUGS.md` #9](../BUGS.md) (A* rescan), [#10](../BUGS.md) / [#12](../BUGS.md) (pooling robustness under dense generated maps).

---

## 1. Reframe: what we actually have

The "ML procedural generation" in `Content Generation/` is **not a generator the game calls** — it is an **editor-only level baker**. Naming it honestly changes the whole plan.

Verified in code:

| Claim | Evidence |
| --- | --- |
| The live entry point is `GeneratedDataSystem`, and it is editor-only | `Data/Generated Content Data/GeneratedDataSystem.cs` — the **entire file** is inside `#if UNITY_EDITOR`; uses `EditorSceneManager.NewScene`, `PrefabUtility.InstantiatePrefab`, `AssetDatabase.CreateAsset`. |
| The map payload format is editor-only too | `GeneratedContentData.cs` and `ContentLevelMapping.cs` are **also** `#if UNITY_EDITOR`. There is no runtime map data type. |
| It is the only caller of the `CoreLogic` handoff | `GenerateNewLevelScene()` is the sole caller of `SetEnvironmentAsParent` / `SetCameraPosition` / `SetupNewPatrolPoints` / `SetupPathfinder` / `ReferenceNewBlokSpawnSystem`. |
| The A* seam is dead | `CoreLogic.SetupPathfinder(mapSize)` (`CoreLogic.cs:42`) is a fully commented-out no-op, yet `GenerateNewLevelScene` calls it at the point where it *must* rescan. |
| Most of the module is dead | `Runtime/Brains/Generator 11..22`, `Generator Multi Chain*`, `Test/*` — not reachable from `GeneratedDataSystem`. Archivable (refactor #7). |

**Implication:** the ML brains author content **in the editor**, bake a `GeneratedContentData` + a per-level `BlokSpawnSystem.asset`, and save a `.unity` scene that ships like a hand-built level. That is a legitimate authoring workflow — but **runtime "endless" generation does not exist today and is green-field.** Nothing in `Content Generation/` runs in a player build.

So the real questions from `ideas/03` collapse to two seams:

1. A **generator abstraction** that classic PCG, the existing ML baker, and a future runtime path can all sit behind.
2. The **CoreLogic / A\* rescan seam** — currently dead — which *every* size-changing generator (baked or runtime) depends on.

---

## 2. Proposed architecture: generate-and-test behind one interface

The spine is **classic PCG + a validator suite**, with ML as an optional ranker — exactly the hybrid `ideas/03 §2C` recommends. Four pieces:

```
  GenerationRecipe (SO)                 IMapGenerator                 Validator suite
  ┌───────────────────┐   recipe   ┌───────────────────┐  MapDraft  ┌──────────────────┐
  │ size / density    │──────────▶ │ Cellular / BSP /  │──────────▶ │ connectivity     │
  │ technique         │            │ WFC / DrunkWalk   │            │ spawn reach      │
  │ blok-mix weights  │            │  (+ symmetry      │            │ bonk-lane length │
  │ symmetry / theme  │            │   post-process)   │            │ blok budget      │
  │ seed              │            └───────────────────┘            └──────────────────┘
  └───────────────────┘                    ▲                                │
            │ seed → System.Random          │ reroll on fail (new sub-seed)  │ pass / fail + score
            └───────────────────────────────┴────────────────────────────────┘
                                             │ best passing draft
                                             ▼
                          ┌──────────────────────────────────────┐
                          │ MapDraft  (engine-free grid + spawns) │
                          └──────────────────────────────────────┘
                               │ bake (editor)         │ realize (runtime — future)
                               ▼                        ▼
                    GeneratedContentData +      live blok spawn into the
                    BlokSpawnSystem.asset       Core scene (no AssetDatabase)
                    + saved .unity scene
                               │
                               ▼  both paths converge on CoreLogic:
            SetEnvironmentAsParent · SetCameraPosition · SetupNewPatrolPoints
                  · ReferenceNewBlokSpawnSystem · **SetupPathfinder (must rescan)**
```

### 2.1 `MapDraft` — the engine-free intermediate

The keystone. A plain C# (no `UnityEngine`, no `#if UNITY_EDITOR`) result type:

- `int Size`
- `BlokKind[,] Cells` (enum: `Empty`, `Immovable`, `Basic`, `Bomb`, `Glass`, `Ice`, `Wood`, `Spawner`)
- `List<Vector2Int> PlayerSpawns`
- `int Seed`

Why a new type instead of reusing `GeneratedContentData`: that class is `#if UNITY_EDITOR` and `AssetDatabase`-bound, so it can never be a runtime payload. `MapDraft` is testable in EditMode without a scene and is the single thing every generator emits and every validator reads. A thin **adapter** converts `MapDraft → GeneratedContentData` for the editor bake path, keeping the existing baker intact.

### 2.2 `IMapGenerator` — pluggable techniques

```csharp
public interface IMapGenerator
{
    MapDraft Generate(GenerationRecipe recipe, System.Random rng);
}
```

| Generator | Strength | First/Later |
| --- | --- | --- |
| `DrunkardWalkGenerator` | connectivity is *guaranteed by construction* (it's walked) — cheapest validator story | **First** (MVP) |
| `CellularAutomataGenerator` | organic open arenas; needs flood-fill cleanup | First-wave |
| `BspGenerator` | structured "arena of rooms" | Later |
| `WfcGenerator` | designer control via tile/adjacency set; highest ceiling | Last (after tool + validators exist) |

`SymmetryPostProcess` is **not** a generator — it's a `MapDraft → MapDraft` transform (mirror/rotate a quadrant) applied after generation for fair versus maps.

### 2.3 `GenerationRecipe` — the SO knob panel

A `ScriptableObject` (matches the project's SO-config convention) holding: `size`, `density`, `technique` (enum → picks the `IMapGenerator`), `blokMixWeights` (per-`BlokKind`), `symmetry` (None/Mirror/Rotational), `theme`, `playerSpawnCount`, `seed` (`int`, `-1` = random), and the **validator thresholds** (min lane length, blok budget, reroll cap). Mode presets ("Open Brawl", "Fortress", "Symmetric Versus", "Chaos") are just authored recipe assets under `Assets/ScriptableObjects/`.

### 2.4 Validator suite + auto-reroll

Each validator is `bool Validate(MapDraft, GenerationRecipe, out string reason)` (+ optional `float Score` for ranking):

| Validator | Checks | Notes |
| --- | --- | --- |
| Connectivity | flood-fill: one walkable region (or N intentional pockets) | hardest constraint; cheap |
| Spawn reachability | every `PlayerSpawn` reaches the largest open region | "no player boxed in" |
| Bonk-lane length | longest straight clear run ≥ threshold | the core verb needs room |
| Blok budget | per-type counts within recipe weights ± tolerance | "not 90% bombs" |

Driver: `GenerationController.Build(recipe)` loops `generate → validate`, deriving each attempt's sub-seed deterministically from the recipe seed, up to `rerollCap`; returns the best passing draft (or the highest-scoring draft + a loud warning if none pass). This is the **generate-and-test** core.

---

## 3. Determinism & seeding

One rule: **all randomness flows through a single `System.Random` seeded from the recipe.** No `UnityEngine.Random` inside generators/validators (it's global, non-reproducible, and order-dependent across frames). Reroll N derives a sub-seed (`new System.Random(baseSeed unchecked* + n)`) so the same recipe+seed always yields the same map *and* the same reroll sequence. Seeds are shareable (`ideas/03 §3`). A property-style EditMode test ("same seed ⇒ identical `MapDraft`") locks this in.

---

## 4. Editor authoring tool

An `EditorWindow` (`Editor/MapGenerationWindow`) — the thing that makes it usable, per `ideas/03 §3`:

- **Recipe field** + **live preview**: draw the `MapDraft` to a texture; "Regenerate" and a seed scrubber.
- **Validator readout**: pass/fail per validator with the failing `reason` string.
- **Brush / lock-region overrides**: paint/erase a `BlokKind`, mark a region locked, then "regenerate the rest" (generators honor a locked mask in the draft). Human-in-the-loop beats fully-auto.
- **Bake button**: `MapDraft → GeneratedContentData` adapter → reuse the *existing* `GeneratedDataSystem.GenerateNewLevelScene` path so we inherit prefab instantiation, camera, patrol points, and the spawn-system asset for free.
- **Analytics overlay** (later): openness / bonk-lane / reachability heatmaps.

This reuses the proven baker as the export backend rather than reinventing scene authoring.

---

## 5. The CoreLogic / A* rescan seam (unblocks everything)

`SetupPathfinder` being a no-op means generated/resized levels pathfind against a **stale, design-time-sized** GridGraph. Hand-built levels only work because the graph happens to match. **This must be fixed before any size-varying generator is usable** — it gates the whole feature, baked or runtime. (`BUGS.md #9`.)

Concrete sketch (verify against the A\* Pathfinding Project API for Unity 6 — `width`/`depth`/`SetDimensions` naming has changed across versions):

```csharp
public void SetupPathfinder(int mapSize)
{
    GridGraph graph = pathfinder.data.gridGraph;
    if (graph == null) { Debug.LogError("CoreLogic: no GridGraph on AstarPath."); return; }

    graph.SetDimensions(mapSize, mapSize, graph.nodeSize);   // or width/depth = mapSize (API-dependent)
    float c = mapSize / 2f - 0.5f;                            // match SetCameraPosition's centering
    graph.center = new Vector3(c, c, 0f);
    graph.UpdateTransform();
    AstarPath.active.Scan(graph);                            // editor bake: synchronous; runtime: see below
}
```

Notes:
- **Centering must agree with the camera.** `GenerateNewLevelScene` uses `mapSize / 2f - 0.5f` for the camera; the graph center must use the same origin or enemies path to the wrong cells.
- **Editor bake:** a synchronous `Scan` is fine (it's a one-time author step).
- **Runtime (future):** `Scan` stalls the main thread — use `ScanAsync`/coroutine over a load screen. Budget matters (`ideas/03 §4`).
- This is a **missing seam, not merely a bug**: re-centering the graph is the natural extension point both the baker and a runtime realizer need. Restoring it also unblocks the existing ML baker, which already calls it expecting a rescan.

---

## 6. Authoring-time vs runtime — different robustness bars

Two use-cases (`ideas/03 §4`) share the spine but demand different things from the validators:

| | Authoring-time bake (recommended first) | Runtime "endless" (later) |
| --- | --- | --- |
| When | designer generates, curates, saves a few good scenes | fresh map each round (Infinite Wave) |
| Bad-seed tolerance | high — a human reviews before ship | **zero** — a broken arena ships live |
| Validator bar | advisory; human is the final filter | strict; **must** guarantee playable or reroll/fallback |
| Perf budget | generous (editor, offline) | hard (<1 frame or async over load screen) |
| Data path | `MapDraft → GeneratedContentData → .unity` | `MapDraft → realize directly into Core scene`, no `AssetDatabase` |
| Pooling stress | mild | high — dense maps surface `BUGS.md #10/#12` (KeyNotFound, infinite spawn spin) → do refactor #5 first |

Same generators and validators; runtime just turns the validators from *advisory* to *gatekeeping* and adds a guaranteed-good **fallback recipe** if `rerollCap` is exhausted.

---

## 7. Where ML-Agents fits (one option, not the spine)

The existing brains stay as **one `IMapGenerator` among several** (wrap the baker behind the interface), and the higher-value role is a **learned ranker/critic** inside generate-and-test: classic PCG produces many cheap candidates, a learned evaluator scores "fun/fairness" to pick among the passing ones (`ideas/03 §2B/§2C`). ML is never the only path — it's expensive to retrain, non-deterministic, and hard to debug. If determinism is required for a given mode, exclude ML generators from that recipe.

---

## 8. Migration roadmap

| Step | Work | Unblocks |
| --- | --- | --- |
| 0 | Fix `SetupPathfinder` rescan (`BUGS.md #9`); archive dead generators (refactor #7) | everything below |
| 1 | `MapDraft` + `IMapGenerator` + `GenerationRecipe` SO; `DrunkardWalkGenerator`; EditMode determinism test | a testable generator core |
| 2 | Validator suite + `GenerationController` reroll; `MapDraft → GeneratedContentData` adapter | generate-and-test; reuse the baker to ship a map |
| 3 | `MapGenerationWindow` (preview, seed scrub, validator readout, bake) | designer usability |
| 4 | `CellularAutomata` + `Bsp`; `SymmetryPostProcess`; mode-preset recipe assets | variety + fair versus |
| 5 | Brush / lock-region overrides | human curation |
| 6 | `WfcGenerator`; optional ML ranker | quality ceiling |
| 7 | Runtime realizer (no `AssetDatabase`) + strict validators + async scan + fallback recipe | endless mode |

Authoring-time first (low risk); runtime endless only once validators are trustworthy.

---

## 9. Risks

- **A\* API drift** — the rescan sketch must be checked against the installed A\* Pathfinding Project version; graph-resize API has changed across releases. Highest-uncertainty item.
- **Pooling under dense maps** — `BlokPool` is fragile (`BUGS.md #10/#12`, refactor #5); generated maps will hit packed-arena edge cases hand-built levels never did. Do refactor #5 before runtime.
- **Editor/runtime data split** — keeping `GeneratedContentData` editor-only while `MapDraft` is runtime-safe is the right boundary, but the adapter must stay one-directional (draft → baked) to avoid leaking `AssetDatabase` into runtime.
- **WFC tile-set authoring cost** — open question in `ideas/03`; defer until the tool/validators exist.
- **ML scope creep** — easy to over-invest in brains; keep it behind `IMapGenerator`/ranker so it can't become load-bearing.

---

## 10. Smallest first step

**Fix `SetupPathfinder` (`CoreLogic.cs:42`, `BUGS.md #9`)** and re-bake one existing generated scene to confirm enemies pathfind on a non-default map size. It's the gate for every other step, it's a handful of lines, and it independently repairs the current ML baker. Land it in its own commit, then start step 1 (`MapDraft` + a drunkard's-walk generator + a determinism test) with zero scene dependencies.

---

## ⚔️ Adversarial Challenge

Red-team pass. This is the strongest of the three docs — the reframe (editor baker, not runtime generator) is correct and verified, and the "fix the seam first" sequencing is right. The bite is scope breadth: four generators + WFC + ML ranker + a runtime path is a lot of spine for a feature with no shipped runtime caller today.

| Concern | Severity | Why it bites | Cheaper / safer alternative |
| --- | --- | --- | --- |
| Generator *suite* (Drunkard + Cellular + BSP + WFC) behind `IMapGenerator` | Med | Four techniques is a research project, not a feature. The interface is cheap; the four *implementations* + per-technique validator tuning is the real cost, and three of four are "later/last." | Ship **DrunkardWalk only** (connectivity free by construction) through bake. Add a second generator *only* if the first produces boring maps. The interface costs nothing; don't pre-build implementations behind it. |
| Validator suite as 4 separate validators | Med | Connectivity (flood-fill) does most of the work; spawn-reachability is a special case of it; bonk-lane + blok-budget are advisory at authoring time (human filters). Building all four before the first baked map is YAGNI. | One flood-fill connectivity check gates the MVP. Reachability falls out of the same flood-fill. Bonk-lane/budget are warnings in the editor window, not hard gates, until runtime mode exists. |
| `GenerationRecipe` SO with full knob panel (weights, symmetry, theme, thresholds) | Low | Designing the complete recipe schema before one generator runs risks fields nothing reads yet. | Start with `size`, `technique`, `seed`. Add weights/symmetry/thresholds as the generators and validators that consume them land. |
| A* rescan sketch shipped as written | High | The §5 code is explicitly "verify against the installed A\* version" and uses `SetDimensions`/`width`/`depth` guesses. Shipping it unverified re-breaks the *currently-working* hand-built levels, which rely on the graph matching. | Verify the exact A\* Pathfinding Project API in this project's installed version **in-editor** before writing the body. Re-bake one existing scene and confirm enemies path before touching any generator. |

- **The centering-mismatch warning is verified and load-bearing — surface it harder.** §5 warns the graph center must match the camera; confirmed: `GeneratedDataSystem.cs:125` centers the camera at `MapSize / 2f - 0.5f`, while the commented-out `SetupPathfinder` body (`CoreLogic.cs:45,52`) used `mapSize / 2f` with **no** `-0.5f` offset. So the dead code, if naively un-commented, would center the graph half a cell off the camera and enemies *would* path to wrong cells. This isn't hypothetical — it's the exact trap baked into the existing commented code. The fix must use `mapSize / 2f - 0.5f` to agree with line 125.
- **"Fix the seam first" is correct, but the seam fix alone doesn't prove the baker works.** Step 0 fixes `SetupPathfinder`; the smallest-first-step says re-bake one scene to confirm. Good — but `GeneratedDataSystem` is `#if UNITY_EDITOR` and drives `EditorSceneManager.NewScene` + `AssetDatabase.CreateAsset`. Confirm the *whole* bake path still runs end-to-end on this Unity 6 version (the ML brains may not even be trainable/runnable as-is) before assuming the baker is a reliable export backend for step 2's adapter.
- **`MapDraft` is the right keystone — no challenge.** Engine-free, EditMode-testable, one type every generator emits and every validator reads. The split from `GeneratedContentData` (verified `#if UNITY_EDITOR`) is correct and is the single most defensible call in the doc. Build this first; it's where the determinism test (§3) lives and it has zero scene dependencies.
- **Runtime "endless" mode (step 7) should be cut from this doc's scope, not just deferred.** There is no runtime generation caller today (verified — entire `Content Generation/` is editor-only), and runtime adds async scan + strict validators + fallback recipe + pooling robustness (refactor #5, BUGS #10/#12) — each its own project. Carrying it as "step 7" invites designing the spine *for* runtime (engine-free `MapDraft`, etc.) before authoring-time has shipped one usable map. The engine-free `MapDraft` is justified on testability alone; don't let the runtime maybe-future drive any *other* decision.

**Verdict:** proceed with revisions — verify the A\* API in-editor and fix `SetupPathfinder` (with the `-0.5f` offset) first, ship `MapDraft` + DrunkardWalk + one connectivity validator + the bake adapter as the MVP, and descope runtime "endless" out of this doc until authoring-time has produced shippable maps.
