# Context Map

Bonkers is a 2D local-multiplayer arcade game (Unity 6 / URP). Players push and **bonk** **bloks** around a grid to damage enemies; ML-Agents procedurally generates level content.

Code is split into many `Bonkers.*` assemblies under `Assets/Scripts/Bonkers/`, each its own bounded context with a per-folder `context.md`. This file is the index: what each context is, where it lives, and how they relate.

> These `context.md` files are **skeletons** — the orientation map + key files are filled in, but the `## Language` glossaries are stubs to be sharpened later via the `grill-with-docs` skill. Keep the nearby `context.md` current when you change a module.

## Contexts

| Context | Path | What it is |
| --- | --- | --- |
| [Core](./Assets/Scripts/Bonkers/Core/context.md) | `Core/` | Per-level hub (`CoreLogic`) wiring spawner, camera, A* graph, patrol points; pause, level-end, points |
| [Control](./Assets/Scripts/Bonkers/Control/context.md) | `Control/` | Player + AI control scripts, local-multiplayer join/config flow, powerups, environment-effector reactions |
| [Combat](./Assets/Scripts/Bonkers/Combat/context.md) | `Combat/` | Shared health/combat interfaces, player/enemy health, blok-interaction strategies (asmdef is `RPG.Combat`) |
| [Movement](./Assets/Scripts/Bonkers/Movement/context.md) | `Movement/` | Grid-step player movement + boost, A* and single-space AI movement |
| [BlokControl](./Assets/Scripts/Bonkers/BlokControl/context.md) | `BlokControl/` | All blok types (control, health, movement) + object-pooled spawning |
| [Grid](./Assets/Scripts/Bonkers/Grid/context.md) | `Grid/` | Patrol points / background tile grid for AI |
| [Events](./Assets/Scripts/Bonkers/Events/context.md) | `Events/` | ScriptableObject event bus (parameterized + legacy UnityEvent variant) |
| [Effects](./Assets/Scripts/Bonkers/Effects/context.md) | `Effects/` | DOTween-based tween effects + per-blok-type effect bundles |
| [ItemDrops](./Assets/Scripts/Bonkers/ItemDrops/context.md) | `ItemDrops/` | Food drops, powerups, puddles, environment effectors |
| [Score](./Assets/Scripts/Bonkers/Score/context.md) | `Score/` | Per-player score + blok-hit tracking |
| [Input](./Assets/Scripts/Bonkers/Input/context.md) | `Input/` | Generated Input System bindings (`PlayerControls`, `InputMaster`) |
| [SceneManagement](./Assets/Scripts/Bonkers/SceneManagement/context.md) | `SceneManagement/` | Screen fading, main menu, scene portals |
| [ContentGeneration](./Assets/Scripts/Bonkers/Content%20Generation/context.md) | `Content Generation/` | ML-Agents procedural level generation (the "brains") |
| [EnemySpawnManagement](./Assets/Scripts/Bonkers/Enemy%20Spawn%20Management/context.md) | `Enemy Spawn Management/` | Enemy holders + spawn points + spawn system |
| [Animation](./Assets/Scripts/Bonkers/Animation/context.md) | `Animation/` | Animancer-driven enemy animation |
| [Audio](./Assets/Scripts/Bonkers/Audio/context.md) | `Audio/` | ScriptableObject audio events + blok audio |
| [2DDestruction](./Assets/Scripts/Bonkers/2DDestruction/context.md) | `2DDestruction/` | Sprite fracturing / breakable shards (vendored) |
| [BloksResetterSystem](./Assets/Scripts/Bonkers/BloksResetterSystem/context.md) | `BloksResetterSystem/` | Resets bloks to pooled/initial state between rounds |
| [Extensions](./Assets/Scripts/Bonkers/Extensions/context.md) · [Helpers](./Assets/Scripts/Bonkers/Helpers/context.md) · [Misc](./Assets/Scripts/Bonkers/Misc/context.md) | `Extensions/` `Helpers/` `Misc/` | Generic utilities (not domain code) |
| [RPG_Game](./Assets/Scripts/Bonkers/RPG_Game/context.md) | `RPG_Game/` | Vestigial RPG-tutorial code — removal candidate |
| TestCode | `TestCode/` | Scratch experiments — **not** shipped, **not** a test suite |

## Relationships

- **Control → Movement / Combat / ItemDrops / Input / Events** — player & AI control scripts drive movement, deal/take damage, react to pickups & puddles, read input, and raise events.
- **BlokControl → Combat / Effects / Events / 2DDestruction** — blok types compose an `IBlokInteraction` (Combat), a `BlokHealth`, a `MoveableBlokControl`, effect bundles (Effects), and break into shards (2DDestruction); spawning raises events.
- **Combat ↔ BlokControl / Movement** — `IMoveableBlokControl`, `IHealth`, `IEnemyCombat`, `IEnemyHealth` are shared across players, enemies, and bloks.
- **Core → BlokControl / Grid / Pathfinding** — `CoreLogic` re-wires the `BlokSpawnSystem`, patrol points, camera, and A* graph for each level.
- **ContentGeneration → BlokControl / Core** — ML brains place bloks/players to author a level, then hand sizing to `CoreLogic`/`BlokSpawnSystem`.
- **EnemySpawnManagement → Control / Combat / Animation** — holders spawn enemy prefabs that own AI control, health, and animation.
- **Effects / Events** — leaf contexts depended on widely; they depend on almost nothing themselves.

## Domain vocabulary (top level)

See per-context `## Language` sections for detail. The recurring nouns:

- **Blok** — the pushable grid object players bonk (types: Basic, Bomb, Glass, Ice, Wood, Spawn, Immovable).
- **Bonk** — the act of a player striking a blok (drives it into motion or impact).
- **Bonkable** — a layer/thing a moving blok can collide with and damage.
- **Slimo / Grubber / Turb / Tar / Toxic** — enemy and hazard archetypes.
- **Puddle / Environment Effector** — a ground hazard that modifies players who stand in it.
- **Holder** — a per-enemy-type spawn wrapper.
- **Brain** — an ML-Agents content-generation agent.
