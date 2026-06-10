# ItemDrops  (asmdef: Bonkers.Drops)

Things that drop into the arena and affect players: **food drops**, **powerups**, and **puddles** (a kind of **environment effector**).

## Key files

- Interfaces — `IDroppableObject`, `IPickupable`, `IEnvironmentEffector`.
- `BlokDroppable.cs` — bloks that drop something when broken.
- `FoodDrop.cs` — collectible food.
- `Powerup.cs`, `PowerUpPickup.cs` — powerup items + pickup logic.
- `PlayerDropDetector.cs`, `PlayerPickupGrabber.cs`, `PlayerEnvironmentEffectorGrabber.cs` — player-side trigger detectors that raise enter/exit events.
- `Puddles/` —
  - `PuddleBehavior.cs` (abstract: `WaitToDestroy` + `StartWaitingToDestroy`), `PuddleDrop.cs` (SO: spawns puddle, grow/fade-in/shrink/fade-out effects, player visual effects, `EffectStrength`, `PuddleLife`).
  - `Tar Puddle/TarPuddleBehavior.cs` — slows + (per commits) pulses the player.
  - `ToxicPuddle/ToxicPuddleBehavior.cs` — can spawn a new toxic slimo on expiry.

## Flow

`PuddleDrop.Spawn` → instantiate prefab, play grow + fade-in (fade-in callback starts the life timer) → on expiry `WaitToDestroy` calls `DestroyPuddle` (shrink+destroy, fade-out). Player `...EffectorGrabber` fires enter/exit → `PlayerEnvironmentEffectorsControl` tweens move speed + applies/stops player visual effects.

## Gotcha (see docs/BUGS.md)

- `DestroyPuddle` runs shrink (destroys GameObject on complete) and fade-out concurrently → fade-out can outlive the object. Fade effects aren't `SetLink`ed.
- Overlapping puddles: exiting one restores full speed while the player still stands in another (`_isSpeedModified` guard is boolean, not a count).
- Player visual effects use `StopEffect`, which is no-op by default → effect may not revert.

## Depends on

- **Effects** (tweens), **Movement** (speed), **Events**, **Control** (grabbers/reactions).

## Language

> Stub — sharpen via grill-with-docs.

**Environment Effector**: A ground hazard that modifies a player while standing in it (currently puddles). Exposes itself via `IEnvironmentEffector.AttemptGetEffector()`.

**Puddle**: A timed environment effector (Tar = slow/pulse, Toxic = slow + may spawn a slimo).

**Pickup vs Drop**: A _drop_ appears in the world; a _pickup_ is the act/component of collecting it.
