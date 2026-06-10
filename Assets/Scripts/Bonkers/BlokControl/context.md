# BlokControl

Everything about **bloks** — the pushable grid objects that are the heart of the game. Each blok *type* is composed from parallel implementations (control + health + movement + interaction + effects) rather than inheritance, so adding a type means adding a file in each folder + a prefab, not editing a switch.

## Key files

- `BlokControl.cs` — base/shared blok control.
- `MoveableBlokControl.cs` + `IMoveableBlokControl.cs` — sliding/movement of a bonked blok; `HitEnemy`, `IsMoving`, `GetCurrentSpeed` consumed by `PlayerMovement` and the spawner.
- `BlokHealth/` — `BlokHealth` (abstract base) + per-type `BasicBlokHealth`, `BombBlokHealth`, `GlassBlokHealth`, `IceBlokHealth`, `WoodBlokHealth`, `SpawnBlokHealth`. Raises `OnBreakBlok` / `OnRespawnBlok`; requires `BlokDestroyIntoPoolHelper`.
- Per-type control folders — `Basic Blok Control/`, `Bomb Blok Control/`, `Glass Blok Control/`, `Ice Blok Control/`, `Immovable Blok Control/`, `Spawn Blok Control/`, `Wooden Blok Control/`.
- `Blok Animation Control/` — `BlokAnimationControl`, `SpawnBlokAnimationControl`.
- `Spawning/` — `BlokSpawnSystem` (SO that picks positions + spawns), `BlokSpawner` (MonoBehaviour coroutine loop), `BlokDestroyIntoPoolHelper` (returns broken bloks to pool).
- `Spawning/BlokPooling/` — `BlokPool` (Odin-serialized dict of pooling-data → pool root) + `BlokPoolingData/` config assets (`IndividualBlokPoolingData`, `BlokPoolingGeneration`, ...).

## Depends on

- **Combat** — `IBlokInteraction` strategies live there; health drives interactions.
- **Effects** — spawn/break effects (`TweenEffect<Transform>`).
- **Events** — `SpawnBlokReportingEvent`, respawn notifications.
- **2DDestruction** — `BreakOnOrder` / `AnimateFragmentOut` fragment the sprite on break.

## Used by

- **Core** (`CoreLogic.ReferenceNewBlokSpawnSystem`), **ContentGeneration** (places bloks), **Movement** (`CanMove` checks moving bloks), **Combat** (interactions).

## Flow (spawn → bonk → break → repool)

`BlokSpawner` loop → `BlokPool.GetPooledBlokToSpawn` → `BlokSpawnSystem.SpawnBlok` (find free cell, activate, fade in) → player bonk via Combat `IBlokInteraction` → `BlokHealth.BreakBlok` → `BreakOnOrder` shards + `BlokDestroyIntoPoolHelper` returns to pool (or destroys on fail).

## Language

> Stub — sharpen via grill-with-docs.

**Blok**: The pushable grid object players bonk. Types: Basic, Bomb, Glass, Ice, Wood, Spawn, Immovable.

**Spawn Blok**: A blok that itself spawns enemies/content; reports state via `SpawnBlokReportingEvent`.

**Pool / Pooling Data**: Bloks are reused from per-type object pools, not instantiated per spawn. `IndividualBlokPoolingData` keys a pool.

**Break vs Destroy**: _Break_ = shatter into fragments and attempt to return to pool. _Destroy_ = actually remove the GameObject (pooling fallback).
