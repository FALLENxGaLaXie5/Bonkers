# Migrate UI to Unity UI Toolkit (USS / UXML)

**Size:** L · **Confidence:** loose · **Depends on:** menu overhaul (idea 02) — best done *together*, not twice.

Goal: evaluate moving Bonkers' runtime UI from the current uGUI / Canvas stack (score text,
boost bars, join cards, menus, pause) to Unity's **UI Toolkit** — UXML for structure, **USS**
(CSS-like stylesheets) for styling, C# `UIDocument`/`VisualElement` for logic.

## Why even consider it
- **Styling reuse:** USS gives one place to theme buttons/cards/HUD (colors, hover "bonk" states)
  instead of per-prefab Canvas tweaks — pairs naturally with the menu overhaul (02) and the
  4-slot join cards (01).
- **Data binding & scale:** building an N-player HUD from a templated `VisualElement` is cleaner
  than hand-placing Canvas children (see the review finding that HUD is wired by positional child
  index — a 4-player blocker).
- **Tooling:** UI Builder (visual editor), runtime theming, and resolution-independent layout.

## Why it might NOT be worth it (the honest side)
- **In-world / world-space UI** (floating score popups, boost bars that track a player, puddle FX)
  is uGUI/world-space-canvas territory; UI Toolkit world-space is immature. We'd likely run a
  **hybrid** (UI Toolkit for screens/menus, uGUI or sprites for in-world) — two UI systems, not one.
- **DOTween/Animancer "juice"** is wired to Canvas/Transform today; UI Toolkit animates via USS
  transitions / `experimental.animation`, so existing button-bonk tweens don't port for free.
- **Migration cost** is real and the current uGUI works. This is a quality/scalability bet, not a fix.

## What a switch might get us (to be quantified in the design doc's cost-benefit)
- One themeable style layer; faster menu iteration; cleaner N-player HUD templating; fewer prefabs.
- Against: rebuild of every screen, hybrid complexity, team familiarity, plugin re-wiring.

## Open questions / spitballs
- Full migration, or **menus-only** UI Toolkit with uGUI kept for in-world HUD? (Likely the latter.)
- Does USS theming meaningfully help accessibility (colorblind palettes, text scaling) vs uGUI?
- Input: UI Toolkit + the new Input System event routing for multiple players/devices — gotchas?

## Smallest first step
Rebuild **one** screen (the pause menu or the title screen) in UI Toolkit as a spike, measure
effort + feel, and decide menus-only vs full before committing. The architecture + cost-benefit
analysis lives in [`docs/design/05-ui-toolkit-migration.md`](../design/05-ui-toolkit-migration.md).
