# Control

Per-character control scripts (human + AI) and the **local-multiplayer join/configuration flow**.

## Key files

### Player control & characters
- `PlayerControl.cs`, `PlayerControlManagement.cs`, `Player2Control.cs` — core player control wiring.
- Character scripts — `GrubberControl`, `GhostlyGrubberControl`, `TarSlimoControl`, `ToxicSlimoControl`, `TurbControl`.
- `PlayerEffects.cs`, `PlayerPowerupControl.cs`, `PlayerEnvironmentEffectorsControl.cs` — reactions to powerups and ground hazards (puddles).
- `Input/PlayerInputHandler.cs` — bridges Input System to control.

### AI
- `AIControl.cs`, `AIControlPathfinder.cs`, `AISingleSpaceMovementControl.cs`, `AIVision.cs` — enemy AI driven by A* pathfinder.
- `ActivateAIBrain.cs`, `ActivateDumbAIBrain.cs` — toggle AI behavior tiers.
- `Eater.cs` — eating behavior.

### Join / configuration flow
- `PlayerConfigurationSystem.cs` (SO) — listens to raw Input System events, detects join presses per device (WASD / arrows / gamepad), instantiates the right `PlayerInput` prefab, tracks `PlayerConfiguration` list, ready-up + `StartLevel`.
- `PlayerConfiguration.cs`, `ConfigurationSetup.cs`, `KeyboardSetup.cs`, `GamepadSetup.cs` — per-player + per-device config records.
- `PlayerSetupMenuController.cs`, `SpawnPlayerSetupMenu.cs` — the join-screen UI.
- `InitializeLevel.cs` — spawns configured players into a level.

## Depends on

- **Movement, Combat, ItemDrops, Input, Events**.

## Used by

- **Core** (level init), **EnemySpawnManagement** (AI enemies).

## Notes / risk

- `PlayerConfigurationSystem` is a **ScriptableObject holding runtime state** (`playerConfigs`, device lists). SO state can persist across play sessions in-editor — see `docs/BUGS.md`.

## Language

> Stub — sharpen via grill-with-docs.

**Player Configuration**: A joined player's identity (index, device, control scheme, color, ready state) created at the join screen and consumed when the level loads.

**Control Scheme**: `Keyboard` (WASD), `Keyboard2` (arrows), `Controller` (gamepad).

**Grubber / Slimo / Turb**: Playable/AI character archetypes.

**Environment Effector**: A ground hazard (puddle) the player control reacts to on enter/exit.
