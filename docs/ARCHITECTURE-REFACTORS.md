# Architecture Refactors (future)

Opportunities to make the codebase easier to extend, less error-prone, and more AI-navigable. **None are urgent** — this is a backlog to pull from, ordered roughly by value-for-effort. Each notes the smell, the payoff, and a sketch. Discuss before doing the big ones; the user owns architecture.

Legend: 💎 high value · ⚖️ medium · 🧹 cleanup. Effort: S/M/L.

---

## 💎 1. Tame the blok-type boilerplate (M)
**Smell:** A blok type (Basic, Bomb, Glass, Ice, Wood, Spawn, Immovable) is spread across ~5 parallel folders — `BlokControl/<Type> Blok Control/`, `BlokControl/BlokHealth/<Type>BlokHealth`, `Combat/BlokInteraction/<Type>BlokInteraction`, `Effects/BlokEffects/<Type>BlokEffects`, plus a prefab. Adding a type means touching all of them and wiring a prefab; easy to miss one.

**Payoff:** Adding/tuning a blok type becomes a single data asset + maybe one script. Fewer "forgot to add the X for the new type" bugs.

**Sketch:** Introduce a `BlokTypeDefinition` ScriptableObject that references the interaction strategy, health profile, effect bundle, and pooling data for a type. Keep the strategy interfaces (`IBlokInteraction`, etc.) but let the definition compose them, so the prefab just holds a reference to its definition. Consider whether health/effects can be data-driven (HP, fade time, effect list) rather than a subclass per type — several `*BlokHealth` subclasses may differ only in values.

> **Strengthened (2026-06-10 review):** the health axis is mostly empty ceremony — `BasicBlokHealth` is a *completely empty* subclass; `WoodBlokHealth` adds only a single `droppable.SpawnDrop()` before `base.BreakBlok()`. Bump this up: collapsing `*BlokHealth` into one `BlokHealth` reading an HP value + optional drop reference is a low-risk first slice that de-risks the bigger `BlokTypeDefinition` SO by proving the data-driven pattern on the cheapest axis.

---

## 💎 2. Stop storing runtime state on ScriptableObjects (M)
**Smell:** `PlayerConfigurationSystem` (and to a lesser degree `BlokSpawnSystem`) are SOs that hold live, mutating gameplay state. SO state persists across play sessions in-editor and across scene loads, producing stale/ghost data (see `BUGS.md` #7).

**Payoff:** Eliminates a whole class of "works on first Play, breaks on second" bugs.

**Sketch:** Split each SO into **config (immutable, authored)** vs **runtime state (reset on enable / scene-scoped)**. Mark runtime collections `[NonSerialized]` and clear them in an explicit `Initialize()`. Or move join/session state to a scene-lifetime MonoBehaviour (a `SessionController`) and keep the SO as pure config.

> **Strengthened (2026-06-10 review) — this is the keystone move.** `PlayerConfigurationSystem` fuses *three* responsibilities on one persisted asset: authored config, live device-join/registry state (`keyboardDevices`/`gamepadDevices`, subscribes `InputSystem.onEvent`), **and** scene transitions (`SceneManager.LoadScene`). Extracting a scene-lifetime **`SessionController`** that owns join/device state, player spawning, HUD-slot wiring, and scene loads — driven by one `PlayerConfiguration`-parameterized player prefab — simultaneously discharges this item, unblocks 4-player (#11/#12 below), and creates the dependency-injection seam that makes testing (#16 below) possible. See `docs/design/01-four-player.md`.

---

## ⚖️ 3. Consolidate the two event-bus systems (S–M)
**Smell:** `Events/` has both the parameterized `BaseGameEvent<T>` family and a `Legacy*` UnityEvent-based family. Two ways to do the same thing invites inconsistency.

**Payoff:** One mental model; new contributors (and AI) don't have to learn both.

**Sketch:** Pick the parameterized family as canonical (it's the documented preference). Migrate `Legacy*` listeners opportunistically when you touch them, then delete the legacy classes once unused. Track remaining legacy usages in an issue.

---

## ⚖️ 4. A consistent tween lifecycle (S)
**Smell:** `TweenEffect` subclasses are inconsistent about cleanup — `ShakeScaleEffect` uses `.SetLink(gameObject)`; the fades don't. `StopEffect` defaults to a silent no-op, so "stoppable" effects silently aren't. Root cause of `BUGS.md` #1, #4.

**Payoff:** Effects can't outlive their targets; stop/revert actually works.

**Sketch:** Establish a rule in `TweenEffect<T>`: every `ExecuteEffect` must `.SetLink(component.gameObject)` (or store the `Tween` for `StopEffect` to `Kill`). Consider a protected helper `protected Tween Track(Tween t, T c)` that links + records. Make `StopEffect` abstract-ish or at least loudly document that loops must override it.

---

## ⚖️ 5. Replace the hand-rolled pool internals (M)
**Smell:** `BlokPool` stores pools as child GameObjects and pulls `transform.GetChild(0)` / checks `childCount` as a makeshift stack. It's an `Odin SerializedMonoBehaviour` dict keyed by SO. Works, but fragile (KeyNotFound, empty-pool branches, `Random.Range` on shrinking lists) and hard to reason about.

**Payoff:** Fewer pool edge cases (`BUGS.md` #10–#13); clearer ownership of active vs pooled bloks.

**Sketch:** Wrap Unity's `UnityEngine.Pool.ObjectPool<T>` (or a small typed pool class) per blok type, behind the current `BlokPool` API so callers don't change. Keep the SO-keyed lookup but store typed pools, not transforms.

---

## ⚖️ 6. Unify player/enemy character controllers (M–L)
**Smell:** Many near-parallel control scripts — `GrubberControl`, `GhostlyGrubberControl`, `TarSlimoControl`, `ToxicSlimoControl`, `TurbControl`, plus `AIControl` / `AIControlPathfinder` / `AISingleSpaceMovementControl`. Likely substantial duplication.

**Payoff:** New characters/enemies become small; shared movement/combat/animation wiring lives once.

**Sketch:** Extract a `CharacterControllerBase` (movement + health + animation hookup) and let each archetype override only its unique behavior. For AI, separate **brain** (decide) from **mover** (execute) cleanly — `IAIMovement` already hints at this; push more shared logic into a base brain.

---

## 🧹 7. Remove vestigial / scratch code from the build (S)
- `RPG_Game/` — leftover RPG-tutorial saving/scene code, not part of Bonkers. Verify no scene references it, then delete (see its `context.md`).
- `TestCode/` — scratch pathfinding/movement; ensure it's not in any build and consider moving out of `Assets/` or behind an editor-only asmdef.
- `Content Generation/Runtime/Brains/Generator 11..22`, `Generator Multi Chain*`, `Test*` — many dead ML iterations. Archive to a branch or a `_Archive/` folder so the live generation path is obvious.

**Payoff:** Smaller surface area; AI/devs stop reading dead code; faster compiles.

---

## 🧹 8. Centralize death/cleanup ownership (S)
**Smell:** Detached helper objects (MovePoint, BoostBar) and effects are cleaned up ad hoc; some leak on death (`BUGS.md` #6).

**Payoff:** No orphans; predictable teardown.

**Sketch:** Give the player a single teardown path (subscribe `PlayerMovement`/effects to `onPlayerDeath`, or an `OnDestroy`) that disposes everything it spawned/detached.

---

## 🧹 9. Naming & layer/config hygiene (S)
- The Combat asmdef is named `RPG.Combat` while everything else is `Bonkers.*`. Rename to `Bonkers.Combat` (update GUID refs) for consistency — low risk, do it in a dedicated commit.
- LayerMasks, radii, and timings are sprinkled through spawn/movement/combat as literals. Consider gathering tunables into the existing SO config assets so designers tune in one place (matches the project's stated convention).

---

---

# New items (2026-06-10 adversarial architecture review)

A second autonomous pass attacked the assembly graph, content-generation path, 4-player blockers, and testability. None of these overlap #1–#9 above.

## 💎 10. Extract a scene-lifetime `SessionController` (the keystone) — M
Covered as a strengthening note on **#2** above, but called out here because it's the single highest-leverage move: it discharges #2, unblocks #11/#12, and creates the DI seam #16 needs. One move, three payoffs. See `docs/design/01-four-player.md`.

## 💎 11. HUD/score wired by positional child index — caps player count — M
**Smell:** `PlayerInputHandler.InitializePlayer` does `FindObjectOfType<PauseMenu>().transform.GetChild(playerNum-1)` to find the score text. Player count is implicitly bounded by hand-placed UI children, and identity is positional (fragile to reordering). A distinct 4-player blocker from the join-flow ones.
**Payoff:** N-player HUD becomes data-driven instead of "add a 3rd/4th child by hand and hope the index lines up."
**Sketch:** the join flow spawns a HUD slot per `PlayerConfiguration` and passes the slot reference directly to `PlayerScore.SetupScoreUI` — no `GetChild(index)`, no `FindObjectOfType`. (Intersects the UI Toolkit question — see `docs/design/05-ui-toolkit-migration.md`.)

## 💎 12. Two incompatible input stacks on the live player path — M
**Smell:** `PlayerControl` (P1) is event-driven off `PlayerInputHandler` (new Input System); `Player2Control` reads `Input.GetAxisRaw("Horizontal2")`/`KeyCode.Return` (legacy Input Manager) in `Update`. The join flow is new-Input-System-only, so `Player2Control` is an orphan path that can't be device-paired or scaled.
**Payoff:** one input model is a hard prerequisite for N-player device pairing; collapses P1/P2 divergence.
**Sketch:** make `PlayerControl` the single controller driven by `PlayerConfiguration`; delete `Player2Control` and the `"Horizontal2"/"Vertical2"` axes. (Concrete first cut of #6, scoped to players.)

## 💎 13. Content generation is an **editor-only level baker**, not runtime generation — L (decision, not a quick fix)
**Smell:** `GeneratedDataSystem` — the only caller of the `CoreLogic` handoff — is wrapped entirely in `#if UNITY_EDITOR` and uses `EditorSceneManager`/`PrefabUtility`/`AssetDatabase`. The ML brains author content in-editor and bake it to assets + a saved scene. There is **no runtime generation path**; the CONTEXT-MAP/CLAUDE framing ("ML-Agents procedurally generates level content") implies a runtime capability that doesn't exist.
**Payoff:** reframes the module as a *tooling pipeline*, not a gameplay system — prevents wasted effort wiring it into runtime and grounds `docs/design/03-procedural-map-generation.md`.
**Sketch:** rename the context to "Level Authoring / Baking", document the editor-only constraint in its `context.md`, separate the editor handoff from any future runtime intent. A "decide what this module *is*" conversation.

## ⚖️ 14. `CoreLogic.SetupPathfinder` is a fully commented-out no-op — missing seam — S–M
**Smell:** `CoreLogic.cs:42-54` body is entirely commented out, yet `GeneratedDataSystem` calls it expecting the A* graph to be re-centered/scanned for the baked map size. Architecturally this is a *missing binding* between data-driven level sizing (`mapSize` from `GeneratedContentData`) and the static authored pathfinding graph. (BUGS #9 tracks the symptom; this is the design gap.)
**Payoff:** closes the gap between "generated map size" and "navigable map" — makes generated levels actually playable with AI.
**Sketch:** implement the body — resize/recenter the `GridGraph` from `mapSize` and `AstarPath.Scan()` at bake time; track graph dimensions as data alongside `mapSize`.

## ⚖️ 15. No test seams anywhere — gameplay logic welded to MonoBehaviour/scene/statics — M (ongoing)
**Smell:** no test asmdefs exist (despite `com.unity.test-framework` installed), and the testable logic is unreachable from a test: ~40 `FindObjectOfType`/`GameObject.Find` calls, rules living in `Update()`/coroutines, spawn/pool math reading `transform.childCount`. Nothing is a plain POCO.
**Payoff:** a few extracted POCOs make the highest-churn rules (movement redirect, score, spawn placement) regression-safe — directly de-risks the 4-player and pooling refactors.
**Sketch:** add one EditMode test asmdef; extract 2–3 pure helpers (`MovementResolver`, `SpawnPlacement`, score math) and unit-test them. Replacing `FindObjectOfType` with injected refs is the same seam testing needs.

## 🧹 16. `PluginsAssemby` god-wrapper (and its typo) is a hidden hard dependency — S
**Smell:** `Assets/Plugins/PluginsAssemby.asmdef` (sic — misspelled, no namespace, no references list = "reference everything") is referenced directly by `Bonkers.Core`, `Bonkers.Control`, and `Bonkers.EnemySpawnManagement`. Three core assemblies take a hard dependency on a catch-all plugin bag.
**Payoff:** restores the "leaf plugins, explicit deps" model; plugin churn stops recompiling Core/Control.
**Sketch:** reference the specific plugin asmdefs directly; delete/scope `PluginsAssemby`. Fix the spelling in the same commit as the `RPG.Combat`→`Bonkers.Combat` rename (#9).

## 🧹 17. Naming/structure traps: two "Misc" assemblies + a placeholder asmdef name — S–M
**Smell:** (a) two Misc asmdefs — `Scripts/Bonkers/Misc/Runtime` (`Bonkers.Misc.Runtime`) **and** a separate `Scripts/Misc/Bonkers.Misc.asmdef` (`Bonkers.Misc`) sitting *outside* the Bonkers tree, the latter a junk-drawer (`CoinLogic`, `Grid.cs`, `TileOccupation`, `Waypoint`, a `PutIntoFolderLater/` folder, Unite-2017 examples) that's nonetheless a load-bearing hub referenced by BlokControl/Control/ContentGeneration/Core; (b) `Animation/Bonkers.Animation.asmdef` is literally named `"NewAssembly"`.
**Payoff:** removes a navigation trap (two things named Misc; real domain types like `Grid`/`TileOccupation` buried in scratch) and a confusing DLL identity.
**Sketch:** promote real types into `Bonkers.Grid`, move `CoinLogic` to `Drops`, delete the Unite examples + `PutIntoFolderLater`, collapse to one Misc; rename `NewAssembly`→`Bonkers.Animation` in a dedicated commit.

---

## Cross-cutting note: 4-player readiness
Several systems hardcode two players (`Player2Control`, `CheckKeyboard1Input`/`CheckKeyboard2Input`, fixed control schemes, positional-child-index HUD). Refactors #2/#6 — and concretely the `SessionController` (#10), HUD slots (#11), and input-stack unification (#12) — are prerequisites for clean 4-player support. See `docs/ideas/01-four-player-multiplayer.md` and `docs/design/01-four-player.md`.
