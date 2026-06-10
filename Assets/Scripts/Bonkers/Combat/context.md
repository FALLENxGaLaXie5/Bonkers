# Combat

Health and combat for **everything that can be hit** — players, enemies, and bloks — plus the per-blok-type **interaction strategies**. The asmdef is historically named `RPG.Combat`; the namespace is `Bonkers.Combat`.

## Key files

- Shared interfaces — `IHealth`, `IEnemyCombat`, `IEnemyHealth`. Implemented by players, enemies, and bloks alike.
- `PlayerHealth.cs` — player HP, `invincible` test flag, `Die()` (detaches death audio, raises `onPlayerDeath`, destroys player).
- `EnemyHealth.cs` — enemy HP, hit cooldown (`timeBetweenHits`), dissolve-shader death, score-text spawn, disables AI on death.
- `EnemyCombat.cs` — enemy attack behavior.
- `PlayerCombat.cs` — player bonk/attack entry.
- `TurbBodySensor.cs`, `WoodenBlokBonks.cs` — specialized collision sensors.
- `BlokInteraction/` — strategy pattern: `IBlokInteraction` + `BlokInteraction` (base) + `BasicBlokInteraction`, `BombBlokInteraction`, `GlassBlokInteraction`, `IceBlokInteraction`, `SpawnBlokInteraction`, `WoodenBoxBlokInteraction`. Each decides what happens when a player bonks that blok (move it vs impact-break it vs nothing).

## Depends on

- **BlokControl** — `IMoveableBlokControl` to move bloks; health pairs with blok types.
- **Movement** — facing direction, move speed comparisons.
- **Events / Effects / Pathfinding** — death VFX, score events, AI disable.

## Used by

- **Control** (player/AI deal & take damage), **BlokControl** (break flow), **EnemySpawnManagement** (enemy health/combat).

## Language

> Stub — sharpen via grill-with-docs.

**Bonk**: A player striking a blok via an `IBlokInteraction`. If a bonkable sits behind the blok, the blok *impacts* (breaks); otherwise it starts *moving*.

**Bonkable**: A layer/collider a moving blok can strike and damage (`bonkableLayers`).

**Hittable / hit cooldown**: Enemies have an i-frame window (`timeBetweenHits`) during which `TakeDamage` is ignored.
