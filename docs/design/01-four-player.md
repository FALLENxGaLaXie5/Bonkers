# Four-Player Local Multiplayer — Design Sketch

> **PROPOSAL — forward-looking; not as-built.** Current architecture = [`CONTEXT-MAP.md`](../../CONTEXT-MAP.md) + the per-module `context.md` files. Verify against live code before implementing.

The *why/what* lives in [`docs/ideas/01-four-player-multiplayer.md`](../ideas/01-four-player-multiplayer.md). This is the *how-to-build*. **Size: L.** Gated on refactors [#2](../ARCHITECTURE-REFACTORS.md#-2-stop-storing-runtime-state-on-scriptableobjects-m) / [#6](../ARCHITECTURE-REFACTORS.md#-6-unify-playerenemy-character-controllers-ml) and bugs [#7](../BUGS.md) / [#17](../BUGS.md).

---

## 1. Target architecture

Three moves, in dependency order:

1. **`SessionController` (scene-lifetime MonoBehaviour)** — owns everything that is *live, mutating session state* today smeared across the config SO: device-join detection, the device/scheme registry, the joined-player list, HUD-slot wiring, player spawning, and scene loads. Lives in the join scene as `DontDestroyOnLoad` and carries config into the level scene (replaces the SO-as-bus pattern). This is the [#2](../ARCHITECTURE-REFACTORS.md) "move join/session state to a scene MonoBehaviour" option, made concrete.
2. **`PlayerConfigurationSystem` → pure config SO** — keeps only authored, immutable data (prefab ref, scene name, color palette, max players). No `playerConfigs` list, no device lists, no `InputSystem.onEvent`, no `SceneManager.LoadScene`. Kills the ghost-player class of bug at the root ([#7](../BUGS.md)).
3. **One `PlayerConfiguration`-driven player prefab** — collapse the three scheme prefabs (WASD / Arrows / Gamepad) and `Player2Control` into a single prefab whose behavior is parameterized by its `PlayerConfiguration` (index, device, scheme, color). Prereq: unify the control stack ([#6](../ARCHITECTURE-REFACTORS.md), see §3).

```
join scene                          level scene
┌──────────────────────────┐        ┌─────────────────────────────┐
│ SessionController         │  carries   │ SessionController (DDOL)     │
│  • InputSystem.onEvent    │  configs   │  • Spawn loop over configs  │
│  • device/scheme registry │ ───────▶   │  • spawn-point list[i]      │
│  • List<PlayerConfig>     │        │  • HUD slot[i] wiring        │
│  • LoadScene(level)       │        └─────────────────────────────┘
└──────────────────────────┘
        reads ▼ (pure data)
   PlayerConfigurationSystem (SO): playerPrefab, sceneToLoad, colorPalette[], maxPlayers
```

`InitializeLevel.Start` ([`InitializeLevel.cs:25-36`](../../Assets/Scripts/Bonkers/Control/InitializeLevel.cs)) folds into the `SessionController` spawn loop; the existing `#if UNITY_EDITOR` play-mode `ClearPlayerConfigs()` hack ([`InitializeLevel.cs:51-63`](../../Assets/Scripts/Bonkers/Control/InitializeLevel.cs)) disappears once state is no longer on the SO.

---

## 2. Every 2-player assumption to remove

| # | Assumption | Where | Fix |
| --- | --- | --- | --- |
| A | HUD/score wired by positional child index — caps players at authored slots | [`PlayerInputHandler.cs:58`](../../Assets/Scripts/Bonkers/Control/Input/PlayerInputHandler.cs) `FindObjectOfType<PauseMenu>().transform.GetChild(playerNum-1)` | `SessionController` owns an explicit `hudSlots[]`/`scoreSlots[]` list; spawn loop assigns `slots[i]`. No `FindObjectOfType`, no positional child lookup. (See §5 HUD shape.) |
| B | Legacy second-player controller on the live path | [`Player2Control.cs`](../../Assets/Scripts/Bonkers/Control/Player2Control.cs) (whole file) uses `Input.GetAxisRaw("Horizontal2")`, `KeyCode.Return` in `Update` | Delete; route P2 through the unified new-Input-System path (§3). |
| C | Hardcoded keyboard-split join checks | [`PlayerConfigurationSystem.cs:59-60`](../../Assets/Scripts/Bonkers/Control/PlayerConfigurationSystem.cs) `CheckKeyboard1Input` + `CheckKeyboard2Input` | Single parameterized keyboard-join check over a scheme table (WASD, Arrows); generalizes to "next free keyboard scheme". |
| D | Three scheme-specific prefabs | [`PlayerConfigurationSystem.cs:17-19`](../../Assets/Scripts/Bonkers/Control/PlayerConfigurationSystem.cs) WASD/Arrows/Gamepad prefabs | One prefab + `PlayerInput.Instantiate(prefab, controlScheme, pairWithDevice)` with scheme chosen from the join. |
| E | Join routed via `*.current`, not the event device | [`PlayerConfigurationSystem.cs:77,82,105,110,133,137`](../../Assets/Scripts/Bonkers/Control/PlayerConfigurationSystem.cs) use `Keyboard.current`/`Gamepad.current` | Use the `keyboard`/`gamepad` arg already in scope ([BUGS #17](../BUGS.md)). Correct multi-keyboard attribution is **required** at 4P, not cosmetic. |
| F | Spawn count tied to whatever `playerSpawns[]` is authored | [`InitializeLevel.cs:30`](../../Assets/Scripts/Bonkers/Control/InitializeLevel.cs) `playerSpawns[i]` — NRE if `configs.Length > spawns.Length` | Spawn-point list sized ≥ `maxPlayers` (4), or derived from map (doc 03 procedural). Clamp + assert. |
| G | `playerNum` 1-based, leaks into score | [`PlayerInputHandler.cs:52,58,63`](../../Assets/Scripts/Bonkers/Control/Input/PlayerInputHandler.cs) + [`PlayerScore.playerNum = 1`](../../Assets/Scripts/Bonkers/Score/PlayerScore.cs) default | Pass a 0-based index from the spawn loop; `PlayerScore` reads index from config, no default of `1`. |
| H | Runtime state serialized on the SO | [`PlayerConfigurationSystem.cs:24-27`](../../Assets/Scripts/Bonkers/Control/PlayerConfigurationSystem.cs) public `playerConfigs` + device lists | Moves to `SessionController` ([#2](../ARCHITECTURE-REFACTORS.md) / [BUGS #7](../BUGS.md)). |
| I | `StartLevel` accepts zero players | [`PlayerConfigurationSystem.cs:170`](../../Assets/Scripts/Bonkers/Control/PlayerConfigurationSystem.cs) `playerConfigs.All(...)` true on empty | `Count > 0 && All(IsReady)` ([BUGS #8](../BUGS.md)) — fix while in the file. |
| J | Camera framing assumed for ~2 | (no single line; level cameras authored per scene) | Single arena-framing camera (§6). |

Not a code assumption but a wiring one: the join UI ([`SpawnPlayerSetupMenu.cs`](../../Assets/Scripts/Bonkers/Control/SpawnPlayerSetupMenu.cs), `PlayerSetupMenuController`) needs 4 ready-cards.

---

## 3. Input unification (prerequisite)

Two incompatible stacks coexist on the live player path:

- **P1** — new Input System, event-driven via `PlayerInput.onActionTriggered` → [`PlayerInputHandler.cs:56,72-78`](../../Assets/Scripts/Bonkers/Control/Input/PlayerInputHandler.cs). Correct, N-ready.
- **P2** — legacy `UnityEngine.Input` polled in `Update` → [`Player2Control.cs:38,62,69`](../../Assets/Scripts/Bonkers/Control/Player2Control.cs). A dead end; `"Horizontal2"` axes don't generalize to player 3/4.

**Action:** all players on `PlayerInputHandler` + the `PlayerControls` asset, device/scheme assigned at join via `PlayerInput.Instantiate(..., pairWithDevice)` (already done for P1 schemes). Delete `Player2Control`. This is the player-facing half of [#6](../ARCHITECTURE-REFACTORS.md); a `CharacterControllerBase` is *nice-to-have* here but not on the critical path — the input unification is.

---

## 4. Data shapes

**`PlayerConfiguration`** (today: `PlayerInput`, `PlayerIndex`, `IsReady`, `Material PlayerColor` — [`PlayerConfiguration.cs:14-19`](../../Assets/Scripts/Bonkers/Control/PlayerConfiguration.cs)). Add:

```csharp
public sealed class PlayerConfiguration {
    public int           PlayerIndex { get; }      // 0-based, authoritative
    public PlayerInput   PlayerInput { get; }
    public InputDevice   Device      { get; }       // paired device (event device, not .current)
    public string        ControlScheme { get; }     // "Keyboard" | "Keyboard2" | "Controller"
    public bool          IsReady     { get; private set; }
    public PlayerColor   Color       { get; private set; } // palette entry, see §7
    public int?          TeamId      { get; private set; } // null = FFA; set in team mode (§8)
}
```

**Spawn points** — `SessionController` holds `List<Transform> spawnPoints` (length ≥ `maxPlayers`); spawn loop uses `spawnPoints[cfg.PlayerIndex]`. Replaces the per-scene `Transform[] playerSpawns` in [`InitializeLevel.cs:17`](../../Assets/Scripts/Bonkers/Control/InitializeLevel.cs). Procedural levels (doc 03) supply this list instead of authoring it.

**HUD slots** — `List<HudSlot> hudSlots` (length = `maxPlayers`), each `{ root, scorer, boostBar, colorSwatch }`. Spawn loop calls `hudSlots[i].Bind(player)` instead of `GetChild(i)`. `PlayerScore.SetupScoreUI` ([`PlayerScore.cs:33`](../../Assets/Scripts/Bonkers/Score/PlayerScore.cs)) takes the slot's scorer object — unchanged signature, just a non-positional source.

---

## 5. HUD / score wiring (the real blocker)

Today `InitializePlayer` reaches into the `PauseMenu` child hierarchy by index ([`PlayerInputHandler.cs:58`](../../Assets/Scripts/Bonkers/Control/Input/PlayerInputHandler.cs)) and forwards the discovered `Scorer` to `PlayerScore` ([`PlayerScore.cs:33-37`](../../Assets/Scripts/Bonkers/Score/PlayerScore.cs)). This *is* the hard N-player cap: only as many players as there are authored sibling slots, in fixed order.

Target: `SessionController` holds the slot list explicitly and binds `slots[index]` during the spawn loop. `PlayerInputHandler.InitializePlayer` loses its `FindObjectOfType`/`GetChild` and instead receives the already-bound slot (or just the `Scorer`). Authoring 4 HUD slots is then a scene-data change, not a code change.

---

## 6. Camera

| Option | Fit | Cost |
| --- | --- | --- |
| **(a) Static arena-framing** | Grid arena is bounded → frame the whole map. **Recommended default.** | S |
| **(b) Dynamic zoom to living-players bbox** | Nicer feel; ties to elimination | M (needs a follow rig + clamp to arena bounds) |
| (c) Split-screen | Shared-grid game — players must see each other. **Reject.** | L, wrong shape |

Start (a); upgrade to (b) only if the arena grows (procedural, doc 03). Either way it's **one** camera fed by the spawn loop / arena bounds, not per-player.

---

## 7. Color / identity palette

`SetPlayerColor` already exists ([`PlayerConfigurationSystem.cs:164`](../../Assets/Scripts/Bonkers/Control/PlayerConfigurationSystem.cs), `Material`-based). Promote the palette to authored config on the SO: an ordered `PlayerColor[]` of ≥4 distinct, colorblind-safe hues (e.g. Okabe–Ito: orange / sky-blue / vermillion / bluish-green). Assigned by join order, shown on the ready card and on the in-game sprite ([`PlayerInputHandler.cs:55`](../../Assets/Scripts/Bonkers/Control/Input/PlayerInputHandler.cs) `spriteRenderer.color`). Keep the `Material` path if the dissolve/tint shader needs it; otherwise a plain color + `MaterialPropertyBlock` avoids per-player material churn (cf. [BUGS #16](../BUGS.md)).

---

## 8. Team-mode / PvP forks

Keep FFA as the trunk; gate team logic behind `PlayerConfiguration.TeamId`:

- **2v2** — shared team score (sum per `TeamId`), team-colored cards, optional team-colored bloks.
- **Friendly-fire toggle** — a session flag read by combat when a moving blok hits a player; `null` TeamId = everyone hostile (FFA).
- **Player-vs-player bonking** — needs a damage rule: can a bonked blok damage another player? Decide *one* rule (e.g. FF-off teammates immune, FFA everyone vulnerable) before wiring, or it sprawls.

Defer all of this until 1–4 FFA is solid. It's data + a few `if (TeamId)` guards, not a new system — which is the point of carrying `TeamId` on the config now.

---

## 9. Risks

- **Device pairing edge cases** — controller connect/disconnect mid-match; `InputUser` re-pairing. Decide drop-out policy (lock at level start = simplest).
- **Multi-keyboard attribution** — [BUGS #17](../BUGS.md) must be fixed for two keyboard players to be told apart reliably.
- **Performance** — 4 players × effects × many bloks/enemies. Watch material-instance churn ([BUGS #16](../BUGS.md)) and pool sizing; difficulty scaling should raise blok/enemy counts, multiplying this.
- **Spawn-point / HUD-slot mismatch** — if a level authors fewer than `maxPlayers` of either, the spawn loop must clamp + warn, not NRE ([assumption F](#2-every-2-player-assumption-to-remove)).

---

## 10. Migration order (smallest-first, gated)

| Step | Work | Gate | Size |
| --- | --- | --- | --- |
| 0 | Fix [BUGS #7](../BUGS.md) (NonSerialize + clear) and [#8](../BUGS.md), [#17](../BUGS.md) in place — buys correctness without restructuring | — | S |
| 1 | Extract `SessionController`; move join/device/list state + `LoadScene` off the SO; SO becomes pure config ([#2](../ARCHITECTURE-REFACTORS.md)) | step 0 | M |
| 2 | Explicit `hudSlots[]` binding; delete `GetChild(playerNum-1)` ([assumption A](#2-every-2-player-assumption-to-remove)) | step 1 | S |
| 3 | Unify input: delete `Player2Control`, route all players through `PlayerInputHandler` ([#6](../ARCHITECTURE-REFACTORS.md) player half) | step 1 | M |
| 4 | Collapse 3 scheme prefabs → 1 `PlayerConfiguration`-driven prefab; generalize keyboard join check | step 3 | M |
| 5 | Spawn-point list ≥ 4; clamp/assert; **get 3 players running** | steps 2,4 | S |
| 6 | 4th player + 4-card join UI + palette of 4 | step 5 | S |
| 7 | Camera (a)→(b) if needed; difficulty scaling by count | step 6 | M |
| 8 | Team mode / FF / PvP fork | step 7 | M–L |

---

## Smallest first step

Do **step 0**: `[NonSerialized]` the three runtime collections on `PlayerConfigurationSystem` and `Clear()` them in an explicit init, plus the one-line `StartLevel` guard ([#8](../BUGS.md)). It's surgical, kills the ghost-player bug that *will* corrupt any N-player testing, and is the precondition for cleanly lifting state into `SessionController` (step 1). Get **3 players** working before chasing 4 + team mode.

---

## ⚔️ Adversarial Challenge

Red-team pass on the *how-to-build*. The plan is sound and well-sequenced; the bite is in scope and in two verified contradictions with live code.

| Concern | Severity | Why it bites | Cheaper / safer alternative |
| --- | --- | --- | --- |
| `SessionController` as `DontDestroyOnLoad` cross-scene owner | High | Trades the BUGS #7 stale-SO hazard for a stale-**DDOL** hazard: a survivor object holding `PlayerInput` refs across loads is the same class of "works on first Play, breaks on second" bug, plus duplicate-singleton-on-scene-reentry. | Land **step 0 only** first. Keep state on a `[NonSerialized]`, explicitly-reset SO; promote to a MonoBehaviour *only* if cross-scene carry actually proves necessary. Most of the value (kill ghost players) is in step 0. |
| Two keyboard players sharing one physical keyboard | High | §3/§4 assume "Keyboard" + "Keyboard2" are separate paired devices, but they're the **same** `Keyboard` device on two control schemes. `PlayerInput`'s device-pairing/`InputUser` model pairs *devices*, not schemes — two users on one keyboard is a known-awkward case that BUGS #17 does **not** fix (that's about attribution, not pairing). | Validate the one-keyboard-two-schemes path in a throwaway scene *before* committing to the unified-prefab design. It may force keeping scheme-on-prefab rather than pure device pairing. |
| Collapse 3 prefabs → 1 (step 4) | Med | `PlayerInput.Instantiate(prefab, controlScheme, pairWithDevice)` already works per-scheme today (verified `PlayerConfigurationSystem.cs:85,113,139`). The 3 prefabs may differ only in the assigned scheme + default action map — collapsing is real work for marginal payoff and risks regressing working join. | Defer step 4 past "3 players running" (step 5). If the prefabs are near-identical, a single prefab + scheme arg is trivial; if not, the collapse wasn't free and you'll have learned that cheaply. |
| `TeamId` carried on config "now" (§8) | Low | YAGNI — `int? TeamId` on the data shape before any team mode exists is premature. It's one nullable field, but it invites the team-mode fork to creep into the FFA trunk. | Add it in the same PR as the first team mode (step 8), not in §4's data shape. Costs nothing to defer. |

- **The DDOL claim contradicts the doc's own thesis.** §1 says `SessionController` "replaces the SO-as-bus pattern" and lives as `DontDestroyOnLoad` — but [`ARCHITECTURE-REFACTORS.md` #2](../ARCHITECTURE-REFACTORS.md) flags *persisted runtime state* as the smell, and a DDOL object is persisted runtime state by another name. The win is "reset reliably on a scene boundary," which a `[NonSerialized]`+`Initialize()` SO already gives you. Don't conflate "off the asset" with "must survive scene loads."
- **`InitializeLevel` folding is under-specified.** §1 says `InitializeLevel.Start` ([`InitializeLevel.cs:25-36`](../../Assets/Scripts/Bonkers/Control/InitializeLevel.cs)) "folds into the spawn loop," but `PlayerInputHandler.InitializePlayer` is what reaches `FindObjectOfType<PauseMenu>().transform.GetChild(playerNum-1)` ([`PlayerInputHandler.cs:58`](../../Assets/Scripts/Bonkers/Control/Input/PlayerInputHandler.cs), verified) and forwards the `Scorer`. The spawn loop must own slot binding *and* pass it into `InitializePlayer` — the doc names the symptom (assumption A) but the call-site rewrite spans two files, not one. Size that step as M, not S.
- **Sequencing trap: step 3 before step 2's HUD work is testable.** Deleting `Player2Control` (step 3) routes P2 through `PlayerInputHandler`, whose `InitializePlayer` still does `GetChild(playerNum-1)` until step 2 lands. Order is listed 2→3 but both gate on step 1; do 2 *fully* (explicit slots) before 3 or P2 will NRE/mis-bind on the positional lookup during the transition.
- **Camera (a) is the right call** — split-screen rejection is correct for a shared grid. No challenge; ship static arena framing and don't gold-plate (b) until procedural maps (doc 03) actually resize the arena.

**Verdict:** proceed with revisions — descope to step 0 + the SO `[NonSerialized]` fix first, prototype the two-keyboard pairing before committing to the unified prefab, and drop `TeamId` until team mode is real.
