# Score

Per-player scoring and attribution of blok hits to the player responsible.

## Key files

- `PlayerScore.cs` — a player's running score.
- `BlokHitTracker.cs` — tracks which player last hit a blok so kills/breaks credit the right player.

## Depends on / Used by

- Listens to **Combat**/**BlokControl** events; surfaced via **Core** (`PointsEvent`) and UI.

## Language

> Stub.

**Score Attribution**: Crediting a blok break / enemy kill to the player whose bonk caused it (via `BlokHitTracker`).
