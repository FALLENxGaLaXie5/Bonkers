# Movement

Grid-step movement for players and AI. Players move in discrete cells via a detached **MovePoint**; a **boost** system gives a temporary speed burst that depreciates back to base.

## Key files

- `PlayerMovement.cs` — discrete grid movement (`MovePointHorizontal/Vertical`), rotation/facing, boost (`StartBoostEffect`, `SlowDown`, `CoolBoost`), speed get/set with clamp. Owns a `BoostBar`.
- `BoostBar.cs` — UI bar showing available boost.
- `AnimationSpeedControl.cs` — drives animation speed from movement.
- AI movement — `IAIMovement` + `AIPathfinderMovement` (A*) and `AISingleSpaceMovement` (cell-by-cell).

## Key behaviors / gotchas

- In `Awake`, `MovePoint` and (in `Start`) the boost bar's parent are **detached from the player** (`SetParent(null)`) so they don't inherit player rotation. They are repositioned each frame. They must be cleaned up explicitly on death (see `docs/BUGS.md`).
- `CanMove` lets a player walk into a cell occupied by a blok only if that blok is moving faster than the player (push-through).
- `SetMoveSpeed` clamps to `[0, baseSpeed + boostEffectSpeed]`.

## Depends on

- **BlokControl** (`IMoveableBlokControl` for push checks), **Input/Control** (callers).

## Used by

- **Control** (player + AI), **ItemDrops/Control** (puddles tween `SetMoveSpeed`).

## Language

> Stub — sharpen via grill-with-docs.

**MovePoint**: An invisible target cell the player lerps toward; detached from the player transform so rotation doesn't affect it.

**Boost**: A consumable speed bonus (`boostAvailable`) that recharges over time (`CoolBoost`) and depreciates after use (`SlowDown`).
