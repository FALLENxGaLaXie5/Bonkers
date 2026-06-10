# Grid

The background tile grid that AI uses for patrolling. Small context.

## Key files

- `PatrolPoints.cs` — holds/repopulates background tiles (`RepopulateBackGroundTiles(levelSize)`), queried for AI patrol targets.

## Depends on / Used by

- Used by **Core** (`CoreLogic.SetupNewPatrolPoints`) and AI in **Control**/**Movement**.

## Language

> Stub.

**Patrol Point**: A background grid cell an enemy can path to when patrolling.
