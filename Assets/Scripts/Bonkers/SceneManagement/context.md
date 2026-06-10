# SceneManagement

Screen transitions and scene navigation.

## Key files

- `Fader.cs` — `CanvasGroup` alpha fade in/out coroutines (`FadeOut`, `FadeIn`, `FadeOutIn`, `FadeOutImmediate`). Note: `canvasGroup` is cached in `Start` — calling a fade before `Start` NREs (see docs/BUGS.md).
- `MainMenu.cs` — main menu navigation.
- `Portal.cs` — in-scene portals / scene transitions.

## Depends on / Used by

- Used by **Core** level flow, menus.

## Language

> Stub.

**Fade**: A timed `CanvasGroup` alpha transition used to mask scene/level changes.

**Portal**: A trigger that moves the player to another scene/location.
