# Core

The per-level **hub**. `CoreLogic` is the wiring point that hooks a level's spawner, camera, A* graph, and patrol points together; the rest is level lifecycle (pause, end, points, persistence).

## Key files

- `CoreLogic.cs` — `SetEnvironmentAsParent`, `SetCameraPosition`, `SetupNewPatrolPoints`, `ReferenceNewBlokSpawnSystem`, `SetupPathfinder` (**currently commented out** — graph is not re-centered/scanned for generated levels; see docs/BUGS.md).
- `Configuration.cs`, `BloksConfigurationControl.cs` — level/blok configuration.
- `PauseMenu.cs` — pause UI/state.
- `LevelEnding.cs` — win/lose / level-complete flow.
- `PointsEvent.cs`, `SpawnBlokTracking.cs` — scoring + spawn-blok bookkeeping.
- `DontDestroyOnLoad.cs`, `RendererScript.cs` — persistence + render helpers.

## Depends on

- **BlokControl** (`BlokSpawnSystem`/`BlokSpawner`), **Grid** (`PatrolPoints`), **Pathfinding** (`AstarPath`).

## Used by

- **ContentGeneration** hands generated map sizing here; level scenes reference `CoreLogic`.

## Language

> Stub — sharpen via grill-with-docs.

**Core / CoreLogic**: The single per-level coordinator other systems are wired into at level start.

**Environment**: The parent transform that spawned bloks/objects are nested under.

**Patrol Points**: Background grid cells used as AI patrol targets.
