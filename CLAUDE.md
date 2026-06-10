# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Working mode (read first)

This project runs under a **low-tier plan — optimize for token economy.** Be brief, do less, retain knowledge outside the chat.

- **Role:** brainstorming partner + targeted implementer, *not* an autonomous agent. The user owns architecture decisions, reviews all code, and often writes the code themselves with Claude assisting alongside.
- **Default to small moves:** narrow, surgical edits to the files already in scope. Do not fan out across the repo, read large files in full, or spawn subagents unless explicitly asked.
- **Be concise:** short answers, minimal preamble, lead with the change or the idea. Skip restating what the user already knows.
- **Prefer context maps over exploration:** start at [`CONTEXT-MAP.md`](./CONTEXT-MAP.md) (root index of every `Bonkers.*` context), then read the relevant per-folder `context.md` to orient *before* scanning the tree. Keep the nearby `context.md` current when you change a module. The `## Language` sections are intentionally stubs until grilled.
- **Handoff-first:** the user must be able to continue solo if their daily tokens run out. After any non-trivial work, leave a short written handoff (what changed, why, what's left) and keep the relevant `.md`/`context.md` docs current so the knowledge survives outside the conversation.

## Navigation & docs (read before exploring)

- **`CONTEXT-MAP.md`** (root) — the index of all contexts + how they relate. Your first stop.
- **`Assets/Scripts/Bonkers/<Module>/context.md`** — per-context orientation map (purpose, key files, deps, domain `Language`). Update the one next to code you touch.
- **`docs/`** — living knowledge that must survive outside the chat:
  - `docs/BUGS.md` — known/suspected bugs with `file:line`, severity, and fix sketch. Add to it when you spot something; check it off when fixed.
  - `docs/ARCHITECTURE-REFACTORS.md` — future refactor opportunities (not yet actioned).
  - `docs/ideas/` — feature/architecture brainstorms (4-player, menu overhaul, procedural generation, etc.).
  - `docs/adr/` — Architecture Decision Records (create lazily, only for hard-to-reverse, surprising, real-trade-off decisions).

## Agent skills (Matt Pocock skill config)

These engineering skills are installed; this is the per-repo config they assume.

- **Issue tracker:** local markdown (solo project, no shared issue tracker). `to-issues` / `triage` / `to-prd` should write issues as markdown files under `docs/` (e.g. `docs/issues/` or alongside `docs/BUGS.md`) rather than calling `gh`. Confirm with the user before assuming GitHub Issues.
- **Triage labels:** default five roles (`needs-triage`, `needs-info`, `ready-for-agent`, `ready-for-human`, `wontfix`) used inline in the markdown.
- **Domain docs:** **multi-context** repo — `CONTEXT-MAP.md` at root points to per-folder `context.md` glossaries; ADRs in `docs/adr/`. `improve-codebase-architecture`, `grill-with-docs`, and `zoom-out` should read those.
- **Don't grill unprompted:** `grill-with-docs` fills the `## Language` stubs — only run it when the user asks.

## Project

Bonkers is a 2D local-multiplayer arcade game built in **Unity 6 (6000.3.15f1)** with the **Universal Render Pipeline (URP)**. Players push and "bonk" blocks (bloks) around a grid to damage enemies; ML-Agents is used to procedurally generate level content.

## Engine & Tooling

- **Unity version: 6000.3.15f1** — must match (`ProjectSettings/ProjectVersion.txt`). Open the project from Unity Hub; the editor regenerates the `.sln`/`.csproj` files (all gitignored).
- There is no command-line build/lint/test pipeline checked in. Build via **File ▸ Build Settings**, or headless:
  `Unity -batchmode -quit -projectPath . -buildTarget StandaloneWindows64 -executeMethod <BuildClass.Method>`
- Tests: `com.unity.test-framework` is installed. Run via **Window ▸ General ▸ Test Runner** (EditMode/PlayMode). Note: `Assets/Scripts/Bonkers/TestCode/` is experimental gameplay/pathfinding scratch code, *not* a unit-test suite.
- Key third-party plugins (`Assets/Plugins/`, referenced by GUID in asmdefs): **Odin Inspector** (Sirenix), **Animancer** + **Animancer.FSM** (animation state machines), **DOTween** (Demigiant), **CodeMonkey** utilities, and the **A\* Pathfinding Project** (`Pathfinding` namespace, `AstarPath`) for enemy AI.

## Cross-platform (PC + Mac)

This repo is worked on from both Windows and macOS. Keep both environments in sync:
- `ProjectSettings/EditorSettings.asset` sets `m_LineEndingsForNewScripts: 0` (OS Native). There is no `.cs` text normalization in `.gitattributes`, so be deliberate about line endings to avoid noisy diffs across machines.
- Binary assets (models, audio, images, fonts) are tracked with **Git LFS** (see `.gitattributes`) — ensure `git lfs` is installed on both machines before cloning/pulling.
- Use forward-slash or OS-agnostic paths in any tooling; never hardcode `C:\...` or `/Users/...` into committed code.

## Architecture

### Modular assembly definitions
Code is split into many `Bonkers.*` assemblies (`.asmdef`), each a folder under `Assets/Scripts/Bonkers/`. Assemblies reference each other **by GUID**, so adding a cross-module dependency means editing the consumer's `.asmdef` references list, not just adding a `using`. Major modules: `Core`, `Control`, `Combat` (asmdef name is `RPG.Combat`), `Movement`, `BlokControl`, `Grid`, `Events`, `Effects`, `Drops`, `Score`, `Input`, `SceneManagement`, `ContentGeneration`, `EnemySpawnManagement`. Editor-only code lives in sibling `Editor/` folders with their own asmdefs.

### ScriptableObject event bus (primary decoupling mechanism)
Cross-system communication goes through ScriptableObject-based events rather than direct references:
- `Bonkers.Events/ParameterizedEvents/BaseGameEvent<T>` (`ScriptableObject` with a C# `Action<T>` + `Raise(T)`) and paired `BaseGameEventListener` MonoBehaviours. Concrete instances exist for `Int`, `Void`, `SpawnBlokHealth`, etc.
- A `Legacy*` UnityEvent-based variant (`GameEvent`, `GameEventWithGameObject`, `GameEventWithVector3`) predates the parameterized one.
- Event *assets* live in `Assets/ScriptableObjects/Events/`. When wiring new behavior, prefer raising/listening to an existing event SO over adding a hard reference between modules.

### Blok type system (strategy pattern)
Each blok type (Basic, Bomb, Glass, Ice, Wood, Spawn, Immovable) is composed from parallel interface implementations rather than inheritance:
- Interaction: `IBlokInteraction` → `BasicBlokInteraction`, `BombBlokInteraction`, etc. (`Combat/BlokInteraction/`)
- Movement: `IMoveableBlokControl` → `MoveableBlokControl` (`BlokControl/`)
- Health: `BlokHealth` base → `BasicBlokHealth`, `BombBlokHealth`, … (`BlokControl/BlokHealth/`)
- Combat/health interfaces `IHealth`, `IEnemyCombat`, `IEnemyHealth` are shared across players, enemies, and bloks (`Combat/`).
Adding a new blok type means adding a matching implementation in each of these folders plus a prefab, not editing a central switch.

### Block spawning & pooling
Bloks are object-pooled, not instantiated per use. `BlokSpawnSystem` (a ScriptableObject) drives spawning; `BlokSpawner`/`BlokPool` (`BlokControl/Spawning/`) manage the pool, with pooling config data assets. `Core/CoreLogic.cs` is the per-level hub that wires the spawn system, camera, environment parent transform, A\* graph, and patrol points together.

### Players, input & control
Local multiplayer uses Unity's **Input System** (`Input/PlayerControls.inputactions`) with a player-config/join flow (`Control/PlayerConfiguration*.cs`, `PlayerSetupMenuController`, `SpawnPlayerSetupMenu`). Per-character control scripts live in `Control/` (e.g. `GrubberControl`, `TarSlimoControl`, `ToxicSlimoControl`, `TurbControl`), with AI variants (`AIControl`, `AIControlPathfinder`, `AIVision`) for enemies driven by the A\* pathfinder.

### ML-Agents content generation
`com.unity.ml-agents` is used for procedural level generation (`Scripts/Bonkers/Content Generation/`, scene `Scenes/Bonkers Level Generation/ML Content Generation.unity`), separate from the playable levels in `Scenes/Levels/` (`Level 1`–`Level 9`, `Level_InfiniteWaveSpawn`).

## Conventions

- Namespaces follow the folder/asmdef (`Bonkers.Control`, `Bonkers.BlokControl`, …); the Combat folder's assembly is historically named `RPG.Combat`.
- Designer-facing tunables and wiring live in ScriptableObject assets under `Assets/ScriptableObjects/`; gameplay logic reads from them rather than hardcoding values.
- When you add/rename a script, the corresponding `.meta` file must be committed alongside it (meta files are intentionally tracked).
