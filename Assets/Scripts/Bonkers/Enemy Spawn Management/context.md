# EnemySpawnManagement

Spawns enemies into a level via per-type **holders** at **spawn points**, driven by an `EnemySpawnSystem`.

## Key files

- `EnemySpawnSystem.cs` — the spawn driver (timing, selection).
- `EnemySpawner.cs` — runs the spawn loop at runtime.
- `SpawnPoint.cs` — a location enemies can appear.
- `IEnemyHolder.cs` + holders — `EnemyHolder`, `GrubberHolder`, `GhostlyGrubberHolder`, `SlimoHolder`, `ToxicSlimoHolder`, `TurbHolder`. Each wraps how a given enemy archetype is produced.
- `EnemyHolderBroadcaster.cs` — broadcasts holder/spawn events.

## Depends on

- **Control** (AI control on spawned enemies), **Combat** (enemy health/combat), **Animation**, **Events**.

## Used by

- Level scenes (esp. `Level_InfiniteWaveSpawn`).

## Language

> Stub — sharpen via grill-with-docs.

**Holder**: A per-enemy-type factory/wrapper that knows how to produce one archetype.

**Spawn Point**: A world location where enemies can be spawned.

**Wave**: A timed batch of enemy spawns (infinite-wave mode).
