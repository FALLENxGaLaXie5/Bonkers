# RPG_Game  (vestigial)

Leftover code from an RPG tutorial project (saving system, persistent object spawner, scene-saving wrapper). **Not part of Bonkers' core loop.** Flagged as a removal/cleanup candidate — see `docs/ARCHITECTURE-REFACTORS.md`.

## Key files

- `Core(RPG)/PersistentObjectSpawner.cs`
- `Saving(RPG)/` — `ISaveable`, `SaveableEntity`, `SavingSystem`, `SerializableVector3`.
- `SceneManagement(RPG)/SavingWrapper.cs`.

## Status

Verify nothing in shipping scenes references these before deleting. If save/load is wanted later, this is a starting point; otherwise remove to cut confusion.

## Language

> N/A — not domain code.
