# 02 · Menu Overhaul — Design Sketch

> **PROPOSAL — forward-looking; not as-built. Current architecture = [`CONTEXT-MAP.md`](../../CONTEXT-MAP.md) + per-module `context.md`. Verify against live code before implementing.**

Sketches *how to build* [`docs/ideas/02-menu-overhaul.md`](../ideas/02-menu-overhaul.md). Read that first for the *why/what*.

**Size:** L (M if Options/juice deferred) · **Touches:** `SceneManagement/`, `Control/` (join flow), `Core/PauseMenu`, new `Bonkers.MenuFlow` assembly.
**Depends on:** [`01-four-player.md`](./01-four-player.md) (`SessionController` owns join/player state — this doc does **not** redesign it). Fixes [`BUGS.md`](../BUGS.md) #14.

---

## Problem with today's flow

Scene transitions have **no single owner**, and fades have a lifecycle bug:

| Concern | Lives today | Issue |
| --- | --- | --- |
| Fade in/out | `SceneManagement/Fader.cs` | Caches `canvasGroup` in `Start` (`Fader.cs:11`) → NRE if a fade fires first (BUGS #14) |
| Scene load from menu | `Portal.cs:31` + `MainMenu.cs:21` | Portal is a `DontDestroyOnLoad` singleton driving fade+load by **scene index** |
| Scene load from join | `PlayerConfigurationSystem.cs:173` | The join SO calls `SceneManager.LoadScene` itself — load coupled to player config |
| Scene load from pause | `PauseMenu.cs:57` | `SceneManager.LoadScene` again, by name, no fade |

Three independent paths to "leave this scene", two of them bypassing the fader. The overhaul unifies them behind **one service**.

---

## Target architecture

A new leaf assembly `Bonkers.MenuFlow` with two responsibilities, kept separate:

1. **`SceneFlow`** — the *only* thing that loads scenes. Owns the fader, does `FadeOut → LoadSceneAsync → FadeIn`. Replaces `Portal` + the raw `SceneManager.LoadScene` calls in `PlayerConfigurationSystem` and `PauseMenu`.
2. **`MenuStateMachine`** — drives *screen* navigation within the menu scene (Title → Play → Join → Character/Level select), and the Pause overlay in-game. Calls `SceneFlow` only at the boundaries (start game, quit to menu).

```
                 ┌───────────────────────── MenuStateMachine ─────────────────────────┐
                 │                                                                      │
   [Title] ──Play──▶ [Play Menu] ──Versus──▶ [Join / Setup] ──all ready──▶ [Char/Color]│
     │  │              │  │                    (1–4 cards)        countdown      select  │
     │  │              │  └─HowToPlay─▶[Controls]                                   │     │
     │  │              └─Options─▶[Options]◀──────────────────────────────────────┐│     │
     │  └─Options─▶[Options] ─(Audio/Video/Access)                                ││     │
     │  └─Quit─▶ Application.Quit                                            [Level/Mode] │
     │                                                                            │      │
     └────────────────────────────────────────────────────────────────────┐     ▼      │
                                                                           SceneFlow.Load(Game)
                 ┌──────────── in-game (Pause overlay, same state machine) ──┴──────────┐
                 │  [Game] ◀─Resume─ [Pause] ─Restart─▶ SceneFlow.Reload                 │
                 │                     │   └─Options─▶[Options]  └─Quit─▶ SceneFlow.Load(Menu)
                 └──────────────────────────────────────────────────────────────────────┘
```

### `SceneFlow` (the smallest, highest-value piece)

A scene-persistent service (MonoBehaviour singleton **or** a runtime SO + bootstrap object — match the team's preference; SO is consistent with the event-bus style). Public surface:

```csharp
// Bonkers.MenuFlow
public interface ISceneFlow {
    void Load(SceneRef target);          // fade out → async load → fade in
    void Reload();                       // current scene (Pause ▸ Restart)
    Coroutine FadeTo(float alpha, float t);
}
```

- Owns a `Fader`; **fixes BUGS #14** by caching the `CanvasGroup` in `Awake` (or lazy-getting on first use) instead of `Start` (`Fader.cs:11`).
- Replaces `Portal`'s int-index API with a typed `SceneRef` (enum or addressable/scene-asset wrapper) so menus stop passing magic indices (`MainMenu.cs:11-12`).
- Raises a `Void`/`Int` event SO on load-complete so HUD/spawn systems can react via the existing bus instead of polling.

### `MenuStateMachine`

- Plain C# state enum + a `MonoBehaviour` host that shows/hides screen panels (uGUI today; UI Toolkit migration is out of scope — see [`05-ui-toolkit-migration.md`](./05-ui-toolkit-migration.md)).
- One `MenuScreen` per state (Title, PlayMenu, Join, CharacterSelect, LevelSelect, Options, Controls, Pause). Each screen owns its panel + first-selected control for gamepad/keyboard focus parity.
- Navigation events (`Back`, `Confirm`) routed from the Input System UI module; every transition can fire an audio + tween hook (juice, below).

---

## Relationship to four-player `SessionController`

The Join/Setup screen is the **centerpiece** and is shared with [`01-four-player.md`](./01-four-player.md). Division of labour:

| Concern | Owner | Note |
| --- | --- | --- |
| Detect joins, hold `PlayerConfiguration` list, ready state | **`SessionController`** (doc 01) | Replaces the runtime-state-on-SO smell in `PlayerConfigurationSystem` (BUGS #7) |
| Load the game scene once all ready | **`SceneFlow`** (this doc) | Moves the `SceneManager.LoadScene` out of `PlayerConfigurationSystem.cs:173` |
| Render 1–4 player **cards**, character/color pick, countdown | **MenuFlow** Join screen | Consumes `SessionController` state; today this is `PlayerSetupMenuController` + `SpawnPlayerSetupMenu` |

So: `MenuFlow` *renders and navigates*, `SessionController` *owns player state*, `SceneFlow` *changes scenes*. Don't let the Join screen call `SceneManager` directly — it asks `SessionController` "all ready?" then asks `SceneFlow` to load.

> Today `SpawnPlayerSetupMenu.cs:17` finds `"MainLayout"` by name and instantiates one menu prefab per joined `PlayerInput`. The 4-card rebuild should let the Join screen own a fixed 4-slot layout and bind slots to configs, rather than spawning a menu per player. Track via doc 01.

---

## Screen inventory: current vs proposed

| Screen | Current state | Proposed |
| --- | --- | --- |
| Title / Main menu | `MainMenu.cs` — single + two-player buttons, magic scene indices | Title state; Play/HowToPlay/Options/Quit; no hard-coded indices |
| Player join / setup | `PlayerSetupMenuController` + `SpawnPlayerSetupMenu`, menu-per-player | 1–4 fixed cards bound to `SessionController`; live device hints; ready ✓ |
| Character / color select | Color pick only (`SetColor`), per-player panel | Per-card character + color (≥4 distinct), gamepad/keyboard focus |
| Level / mode select | none (indices baked into Title) | Versus first; Infinite Wave (`Level_InfiniteWaveSpawn`) + Quick Play later (doc 03) |
| How to Play / controls | none | Per-scheme card: WASD / arrows / gamepad |
| Options | none | Audio / Video / Accessibility (below) |
| Pause | `PauseMenu.cs` — Resume/Quit, no fade, no Options | Resume · Restart · Options · Quit-to-Menu, all via `SceneFlow` |

---

## Options screen scope

PlayerPrefs-backed (`RPG_Game` save is vestigial — don't revive; refactor #7). Read on boot, apply live, persist on change.

| Group | Settings |
| --- | --- |
| Audio | Master / SFX / Music volume (→ `AudioMixer` exposed params) |
| Video | Resolution, fullscreen mode, VSync |
| Accessibility | Colorblind palettes, screen-shake toggle, reduced-flashing (puddle/pulse), text size |

Accessibility toggles are the cross-cutting ones: screen-shake + reduced-flashing should gate the `Effects` tweens (e.g. `ShakeScaleEffect`, `ColorLerpEffect`, the puddle pulse) — a single `MenuSettings` SO other systems read, not menu-local bools.

---

## Juice — reuse existing tweens

The menu should feel like the game. Reuse `Bonkers.Effects` `TweenEffect<T>` assets rather than new animation code:

| Moment | Reuse |
| --- | --- |
| Button hover/select "bonk" | `GrowScaleEffect`, `SqueezeScaleEffectX/Y`, `ShakeScaleEffect` |
| Card joins / ready stamp | `GrowScaleEffect` + `ColorLerpEffect` |
| Title bloks bonk into place | existing blok prefabs + `GrowScaleEffect`/`ShakeRotationEffect` |
| Every navigation sound | `Audio/SimpleAudioEvent` SOs |

> Caveat: `TweenEffect.StopEffect` is a no-op by default (`TweenEffect.cs:18`) and the fade tweens aren't `SetLink`ed (BUGS #1, #4). Menu elements are long-lived so this is lower-risk than in-game, but bind hover tweens to a kill on deselect to avoid stacking.

---

## Risks

| Risk | Mitigation |
| --- | --- |
| `Portal` is `DontDestroyOnLoad` singleton — replacing it mid-flight can double-load or orphan the fader | Extract `SceneFlow` to wrap the *same* fade+load coroutine first; cut `Portal` over screen-by-screen |
| Join screen entangled with `PlayerConfigurationSystem` runtime state (BUGS #7) | Land doc 01's `SessionController` (or its `[NonSerialized]` fix) **before** the 4-card rebuild |
| uGUI focus/navigation parity across keyboard + gamepad | Each `MenuScreen` declares its first-selected control; test all schemes |
| Scope creep (Options + juice + 4 cards at once) | Ship in the order below; flow correctness before polish |
| BUGS #8 (`StartLevel` with zero players) | `SessionController` requires `count > 0 && all ready` before `SceneFlow.Load` |

---

## Smallest first step

**Extract `SceneFlow` and fix the fader.** Highest value, lowest blast radius, unblocks the rest:

1. New `Bonkers.MenuFlow` asmdef; move/wrap `Portal`'s `FadeOut → LoadSceneAsync → FadeIn` into `SceneFlow`. Cache `CanvasGroup` in `Awake` (fixes **BUGS #14**).
2. Repoint `MainMenu`, `PauseMenu`, and `PlayerConfigurationSystem`'s load call at `SceneFlow.Load(SceneRef)` — delete the three raw `SceneManager.LoadScene` sites.
3. *Then* the `MenuStateMachine` + screen panels.
4. *Then* the 4-card Join rebuild (with doc 01's `SessionController`).
5. *Then* Options + juice.

Steps 1–2 are an **S** refactor with no visible behavior change beyond consistent fades — a safe first commit.

---

## ⚔️ Adversarial Challenge

Red-team pass. The "extract `SceneFlow` first" instinct is right and the lowest-risk slice; the over-engineering risk is the new assembly + state machine arriving before they earn their keep.

| Concern | Severity | Why it bites | Cheaper / safer alternative |
| --- | --- | --- | --- |
| New `Bonkers.MenuFlow` assembly up front | Med | A whole asmdef (+ GUID-ref editing across consumers, the project's stated friction) for what starts as one `SceneFlow` class + a few panels. Premature module boundary. | Put `SceneFlow` in `SceneManagement/` (where `Fader`/`Portal` already live) first. Spin out `Bonkers.MenuFlow` only once the state machine + screens exist and the boundary is obvious. |
| `MenuStateMachine` as a designed system | Med | Title↔Play↔Options is plain panel show/hide; a state machine here is abstraction ahead of need. | Enum + `switch` over panel GameObjects. Promote to a real FSM **only** if Join→Char→Level back-out tangles (it might — see below). |
| `SceneFlow` as a stateless SO | Med | The doc waves at "MonoBehaviour or SO, match team preference," but an SO that owns a `Coroutine`/`Fader` ref is the BUGS #7 smell again, and SOs can't run coroutines (`StartCoroutine` needs a MonoBehaviour). The "stateless SO" still needs a scene-placed runner — so why the SO. | Plain scene-placed MonoBehaviour that owns the `Fader`. No DDOL if every scene that fades has one; raise a Void event on load-complete for cross-scene reaction. Skip the ADR until it's actually contentious. |
| `SceneRef` typed wrapper replacing int indices | Low | Real improvement, but a scene-asset/enum wrapper is easy to over-build (addressables, validation). | A simple `enum SceneRef` → name/index lookup table is enough; don't reach for addressables. |

- **Hard dependency on doc 01 is a sequencing hazard, not just a note.** §"Relationship" and the Risks table both gate the 4-card Join rebuild on `SessionController` (doc 01) landing first — but doc 01's `SessionController` is itself flagged for descoping (see its Adversarial Challenge: do step 0 / `[NonSerialized]` first, defer the DDOL controller). So the menu overhaul's centerpiece is gated on a piece that *should* shrink. **Resolve doc 01's scope before committing the Join-screen rebuild**, or the menu work blocks on a moving target.
- **`SceneFlow` extraction does have blast radius despite the "no visible behavior change" claim.** `Portal` is a `DontDestroyOnLoad` singleton ([`Portal.cs:31`]) — the doc's own Risks row admits replacing it mid-flight can double-load or orphan the fader. Steps 1–2 touch three load sites (`MainMenu.cs:21`, `PauseMenu.cs:57`, `PlayerConfigurationSystem.cs:173`, all verified to call `SceneManager.LoadScene` directly). That's an S refactor only if `Portal` and `SceneFlow` don't coexist as two DDOL fader-owners during the cutover — wrap the *same* coroutine and delete `Portal` in the same commit, don't run both.
- **Fader fix is correctly the keystone and is cheap.** Caching `CanvasGroup` in `Awake` instead of `Start` (BUGS #14, `Fader.cs:11`) is a 1-line fix that can ship *today*, independent of the whole overhaul. Do it first, standalone — don't let it wait on `SceneFlow`.
- **Juice reuse is the right move but the caveat is load-bearing.** `TweenEffect.StopEffect` is a no-op (BUGS #4) and fades aren't `SetLink`ed (BUGS #1). Hover tweens that don't kill-on-deselect will stack on menu elements (which, unlike puddles, are long-lived and re-hovered constantly) — this is *more* likely to bite in a menu than in-game. Bind kill-on-deselect from the start, not as polish.

**Verdict:** proceed with revisions — ship the standalone Fader fix + `SceneFlow` (in `SceneManagement/`, MonoBehaviour) first, hold the `Bonkers.MenuFlow` assembly and state machine until screens exist, and don't start the 4-card Join rebuild until doc 01's scope is settled.
