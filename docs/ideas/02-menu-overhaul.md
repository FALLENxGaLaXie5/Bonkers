# Menu Overhaul

**Size:** M · **Confidence:** promising · **Touches:** `SceneManagement/` (`MainMenu`, `Fader`), `Control/` (join flow, `PlayerSetupMenuController`, `SpawnPlayerSetupMenu`), `Core/PauseMenu`.

Goal: turn the menus from functional into a polished, juicy front-end that sells the chaotic arcade vibe and scales to 1–4 players.

## Current pieces to build on
- `SceneManagement/MainMenu.cs` + `Fader.cs` (fades — note the `Start` race in `BUGS.md` #14).
- `Control/PlayerSetupMenuController.cs` + `SpawnPlayerSetupMenu.cs` (the join/ready flow).
- `Core/PauseMenu.cs` (in-game pause).

## Target structure
```
Title / Main Menu
 ├─ Play
 │   ├─ Local Versus  → Player Join/Setup → Character Select → Level/Mode Select → Game
 │   └─ (later) Infinite Wave / Co-op
 ├─ How to Play (controls per scheme: WASD / arrows / gamepad)
 ├─ Options (audio, video, controls, accessibility)
 └─ Quit
Pause (in-game): Resume · Restart · Options · Quit to Menu
```

## Player join / setup screen (the centerpiece)
- 4 player cards (empty → "press Start/Space/Enter to join" → joined, showing color + chosen character → ready ✓).
- Live device hints (which key/stick joins).
- Per-player **character select** and **color** (palette of ≥4 distinct colors).
- "All ready" → countdown → level. Back-out cleanly resets configs (ties to `BUGS.md` #7).

## Juice / polish ideas
- Animated title bloks that bonk into place; menu buttons that "bonk" on hover/select (reuse `Effects/TweenEffects` — Grow/Shake/Squeeze).
- Sound on every navigation (reuse `Audio/SimpleAudioEvent`).
- Background: a live demo arena (AI players bonking) behind the menu.
- Consistent fade transitions everywhere (fix `Fader` first; consider a single `SceneTransition` service).
- Controller + keyboard navigation parity; clear focus highlight; remappable in Options (later).

## Options worth having
- Master / SFX / Music volume.
- Resolution / fullscreen / vsync.
- Accessibility: colorblind palettes, screen-shake toggle, reduced-flashing (puddle/pulse effects), text size.

## Open questions
- Mode select now or later? (Versus first; Infinite Wave already exists as `Level_InfiniteWaveSpawn`.)
- Save options to disk — small `PlayerPrefs` layer, or revive `RPG_Game` saving? (Recommend `PlayerPrefs`; `RPG_Game` is vestigial, see refactor #7.)
- Per-level select vs. a single "Quick Play" that picks/gens a map (ties to doc 03).

## Smallest first step
A `SceneTransition`/menu service that owns fades + scene loads (fixing `Fader`), then rebuild the join screen as 4 scalable cards. Polish (juice/sound) layers on after the flow is solid.
