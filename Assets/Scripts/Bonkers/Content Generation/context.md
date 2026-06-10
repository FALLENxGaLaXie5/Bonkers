# ContentGeneration

ML-Agents **procedural level generation**. Agents ("brains") learn to place bloks/players to author a level, separate from the hand-built levels in `Scenes/Levels/`. Scene: `Scenes/Bonkers Level Generation/ML Content Generation.unity`.

> This folder has many experimental iterations (`Brains/Generator 11..22`, `Generator Multi Chain*`, `Test*`). They are **research scratch** — the current/canonical path is the `Runtime/Scripts/` code below. Treat the numbered generators as history.

## Key files (canonical)

- `Runtime/Scripts/Components/`
  - `ContentGenerationAgent.cs` + `...FourDirectional` / `...EightDirectional` — the ML agents.
  - `ContentGenerationDecisionRequester.cs` — drives agent decision cadence.
  - `BlokLevelGeneration.cs`, `PlayerSpawnLevelGeneration.cs` — translate agent actions into placed bloks / spawn points.
- `Runtime/Scripts/Data/`
  - `ContentGeneratorData.cs`, `Generated Content Data/` (`GeneratedContentData`, `GeneratedDataSystem`, `ContentMappingArrayEditorData`) — config + the serialized generated map.
- `Runtime/Scripts/Utility/` — `ArrayUtilities`, `BlokMapping`, `ContentLevelMapping`, `ContentGenerationAgentUtility`.
- `Runtime/Scripts/UI/BlokGenerationHeuristicUI.cs` — manual/heuristic authoring UI.
- `Test/` — `MoveToGoalAgent`, `FoodAgent`, `Goal`, `Wall` — ML-Agents "hello world" sample, unrelated to shipping.

## Depends on

- `com.unity.ml-agents`, **BlokControl** (places bloks), **Core** (hands off map sizing).

## Used by

- The generation scene; output feeds `BlokSpawnSystem` / `CoreLogic`.

## Language

> Stub — sharpen via grill-with-docs.

**Brain / Agent**: An ML-Agents `Agent` that emits placement actions to author a level.

**Generated Content Data**: The serialized grid/mapping the agent produces, replayed to build a playable level.

**Heuristic mode**: Human-driven generation (via UI) instead of the trained policy.
