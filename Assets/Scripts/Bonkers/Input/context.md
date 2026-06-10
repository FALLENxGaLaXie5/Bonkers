# Input

Unity **Input System** bindings for local multiplayer. Mostly generated code — treat as generated, edit the `.inputactions` asset, not the `.cs`.

## Key files

- `PlayerControls.cs` — generated wrapper for `PlayerControls.inputactions` (the action maps + control schemes: Keyboard / Keyboard2 / Controller).
- `InputMaster.cs` — input access helper.
- `PlayerControls.inputactions` (asset) — the source of truth; regenerate `PlayerControls.cs` from it.

## Used by

- **Control** (`PlayerInputHandler`, `PlayerConfigurationSystem`).

## Language

> Stub.

**Control Scheme**: A device+binding set (`Keyboard` = WASD, `Keyboard2` = arrows, `Controller` = gamepad).
