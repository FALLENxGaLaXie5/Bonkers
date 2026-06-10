# Bug Tracker

Suspected/known bugs found during a static read of the codebase (2026-06-07, autonomous pass). These are **candidates** — none were reproduced in-editor. Each has a `file:line`, a severity, why it's suspicious, and a fix sketch. Verify in Play mode before fixing.

Severity: 🔴 High (likely crash / breaks a feature) · 🟠 Medium (wrong behavior, edge cases) · 🟡 Low (leak / polish / unlikely path).

Status: `[ ]` open · `[x]` fixed/verified-not-a-bug.

## Index

| # | Sev | Area | One-liner | Where |
| --- | --- | --- | --- | --- |
| 1 | 🔴 | Effects | Fade tweens not `SetLink`ed → exception when target destroyed mid-fade | `FadeIn/OutEffect.cs:15` |
| 2 | 🔴 | Puddles | `DestroyPuddle` destroys object while fade-out still running | `PuddleDrop.cs:41` |
| 3 | 🟠 | Puddles | `Spawn`/`DestroyPuddle` have no null guards | `PuddleDrop.cs:31` |
| 4 | 🟠 | Effects | Player visual effects never revert (`StopEffect` no-op) | `TweenEffect.cs:18` |
| 5 | 🟠 | Puddles | Overlapping puddles restore full speed too early | `PlayerEnvironmentEffectorsControl.cs:46` |
| 6 | 🟠 | Lifecycle | MovePoint & BoostBar orphaned on player death | `PlayerMovement.cs:58` |
| 7 | 🔴 | Control | Runtime state on serialized SO → ghost players | `PlayerConfigurationSystem.cs:24` |
| 8 | 🟡 | Control | `StartLevel` can start with zero players | `PlayerConfigurationSystem.cs:168` |
| 9 | 🟠 | Pathfinding | A* graph never re-scanned for generated/resized levels | `CoreLogic.cs:42` |
| 10 | 🟠 | Pooling | `GetPooledBlokToSpawn` can throw `KeyNotFoundException` | `BlokPool.cs:21` |
| 11 | 🟡 | Spawning | `SpawnBlok` dereferences `BlokHealth` unchecked | `BlokSpawnSystem.cs:108` |
| 12 | 🟠 | Spawning | `SpawnBlok` can spin forever and strand a pooled blok | `BlokSpawnSystem.cs:85` |
| 13 | 🟡 | Spawning | `BlokSpawner` NREs if no spawn system set before `Start` | `BlokSpawner.cs:34` |
| 14 | 🟠 | Scene | `Fader` NREs if a fade is called before `Start` | `Fader.cs:11` |
| 15 | 🟡 | Combat | `PlayerHealth.Die` may deref null `audioSource` | `PlayerHealth.cs:52` |
| 16 | 🟡 | Combat | New material instance per enemy (churn) | `EnemyHealth.cs:41` |
| 17 | 🟡 | Control | Input routed via `*.current` not the event device | `PlayerConfigurationSystem.cs:77` |

**Suggested first pass:** #1–#5 (one puddle/fade sweep) and #7 (ghost players). Detail below.

---

## Effects & Puddles (the recently-worked area — "fade effects having issues")

### [ ] 🔴 1. Fade tweens aren't killed when their target is destroyed
- **Where:** `Effects/TweenEffects/FadeInEffect.cs:15`, `FadeOutEffect.cs:15`; triggered from `ItemDrops/Puddles/PuddleDrop.cs:41-45`.
- **Why:** Both fades use `DOTween.To(getter, setter, …)` with no `.SetLink(gameObject)`. Compare `ShakeScaleEffect.cs:20`, which *does* link and is auto-killed on destroy. The setter captures `spriteRenderer`; when the puddle GameObject is destroyed mid-fade the tween keeps ticking and the setter touches a destroyed object → `MissingReferenceException`. This is the most likely source of the "fade effects having issues" note.
- **Fix sketch:** add `.SetLink(spriteRenderer.gameObject)` to the fade tweens (and any other `TweenEffect` using manual `DOTween.To`). Optionally store the `Tween` and `Kill()` it in a `StopEffect` override.

### [ ] 🔴 2. `DestroyPuddle` destroys the object while the fade-out is still running
- **Where:** `ItemDrops/Puddles/PuddleDrop.cs:41-45`.
- **Why:** `puddleShrinkEffect` runs with an `OnComplete` that calls `Destroy(transform.gameObject)`, while `puddleFadeOutEffect` runs concurrently on the sprite. If shrink finishes first (different speeds/eases), the fade-out tween is orphaned on a destroyed renderer → see bug #1. Even with #1 fixed, the visual is non-deterministic (which finishes first).
- **Fix sketch:** drive destruction off whichever effect is meant to be authoritative — e.g. only `Destroy` after *both* complete (sequence/`DOTween.Sequence`), or fade-out then shrink+destroy in a sequence.

### [ ] 🟠 3. `PuddleDrop.Spawn` / `DestroyPuddle` have no null guards
- **Where:** `ItemDrops/Puddles/PuddleDrop.cs:31-45`.
- **Why:** `puddleGrowEffect`, `puddleFadeInEffect`, `puddleShrinkEffect`, `puddleFadeOutEffect`, `puddle.GetComponent<SpriteRenderer>()`, and `GetComponent<PuddleBehavior>()` are all dereferenced unchecked. A missing effect asset or a sprite on a child object → NRE.
- **Fix sketch:** null-check each effect before `ExecuteEffect`; use `GetComponentInChildren<SpriteRenderer>()` if the sprite isn't on the root; guard `puddleBehavior`.

### [ ] 🟠 4. Player visual effects never revert (`StopEffect` is a no-op)
- **Where:** `Effects/TweenEffects/TweenEffect.cs:18-21` (default `StopEffect` does nothing) → `PuddleDrop.StopPlayerVisualEffects` (`PuddleDrop.cs:55`) → `Control/PlayerEnvironmentEffectorsControl.cs:97`.
- **Why:** When the player leaves a puddle, `StopPlayerVisualEffects` calls `StopEffect` on each visual effect, but the base implementation does nothing. If the "pulse"/tint effect is a loop (or just left the sprite mid-tween), it won't revert — the player can stay pulsing/tinted after leaving the puddle.
- **Fix sketch:** override `StopEffect` on the player-visual effects to `Kill()` the tween and reset alpha/scale/color to baseline; store the running `Tween` per target.

### [ ] 🟠 5. Overlapping puddles restore full speed too early
- **Where:** `Control/PlayerEnvironmentEffectorsControl.cs:46-98`.
- **Why:** `_isSpeedModified` is a single bool. Standing in puddle A (slowed), stepping into overlapping puddle B does nothing (guard returns at :52). Leaving B (still inside A) restores `_originalSpeed` at :88-94 — player is now full-speed while still standing in tar.
- **Fix sketch:** track active effectors in a set/counter; only restore speed when the last one is exited. Recompute target speed from the strongest active effector.

---

## Lifecycle / cleanup

### [ ] 🟠 6. MovePoint and BoostBar are orphaned when the player dies
- **Where:** `Movement/PlayerMovement.cs:58-59` (movePoint `SetParent(null)`), `:68-69` (boost bar parent `SetParent(null)`); death in `Combat/PlayerHealth.cs:47-57` only `Destroy(gameObject)` for the player.
- **Why:** Both objects are intentionally detached from the player so rotation doesn't affect them, but nothing destroys them on death. Each player death leaves an orphan MovePoint and a floating boost bar in the scene.
- **Fix sketch:** in `PlayerHealth.Die` (or an `OnDestroy`/`onPlayerDeath` subscriber in `PlayerMovement`) destroy `movePoint` and call `DestroyBoostBar()`.

### [ ] 🔴 7. `PlayerConfigurationSystem` stores runtime state on a serialized SO field
- **Where:** `Control/PlayerConfigurationSystem.cs:24` — `public List<PlayerConfiguration> playerConfigs`.
- **Why:** It's a `public` field on a `ScriptableObject`, so it serializes into the asset and **persists across play sessions in the editor** (and potentially across scene loads). Joined players, ready states, and configs can carry over, producing ghost/duplicate players unless `ClearPlayerConfigs()`/`ClearDevicesBeingUsed()` are reliably called. `keyboardDevices`/`gamepadDevices` are at least private, but the SO instance itself survives domain reloads.
- **Fix sketch:** mark runtime collections `[System.NonSerialized]`; explicitly `Clear()` all three in `Initialize()`; consider moving join state to a scene MonoBehaviour or a runtime-only SO that's reset on enable.

### [ ] 🟡 8. `StartLevel` can start with zero players
- **Where:** `Control/PlayerConfigurationSystem.cs:168-175`.
- **Why:** `playerConfigs.All(p => p.IsReady)` returns `true` for an empty list, so a stray `StartLevel` call with no joined players would load the gameplay scene.
- **Fix sketch:** require `playerConfigs.Count > 0 && playerConfigs.All(...)`.

---

## Pathfinding / level setup

### [ ] 🟠 9. A* graph is never re-centered/scanned for generated or resized levels
- **Where:** `Core/CoreLogic.cs:42-54` — `SetupPathfinder` body is entirely commented out.
- **Why:** For ML-generated / differently-sized levels the grid graph keeps its design-time center & bounds, so enemies may fail to pathfind on procedurally generated maps. Hand-built levels happen to work because the graph matches.
- **Fix sketch:** restore graph re-center + `Scan()` (or `pathfinder.Scan()`), driven by the actual map size, after generation. Confirm with the A* Pathfinding Project API for Unity 6.

---

## Spawning / pooling

### [ ] 🟠 10. `BlokPool.GetPooledBlokToSpawn(data, parent)` can throw `KeyNotFoundException`
- **Where:** `BlokControl/Spawning/BlokPooling/BlokPool.cs:21`.
- **Why:** `BlokPools[individualBlokPoolingData]` is indexed before any `ContainsKey` check. A spawn-system entry whose pooling data has no pool → exception (the other overload and `AttemptToPoolBlok` *do* guard).
- **Fix sketch:** `if (!BlokPools.ContainsKey(data)) { LogWarning; return null; }` first.

### [ ] 🟡 11. `SpawnBlok` dereferences `BlokHealth` without a null check
- **Where:** `BlokControl/Spawning/BlokSpawnSystem.cs:108`.
- **Why:** `blokToSpawn.GetComponent<BlokHealth>().InvokeRespawnBlok()` — NRE if a pooled prefab lacks a `BlokHealth`.
- **Fix sketch:** `TryGetComponent` and guard, or require it via the pooling data contract.

### [ ] 🟠 12. `SpawnBlok` can spin forever and strand a pooled blok
- **Where:** `BlokControl/Spawning/BlokSpawnSystem.cs:85-111`.
- **Why:** The `do { … } while (blokCollider)` retries every 1s until it finds a wall-free cell. If the arena is fully packed it never resolves: the coroutine leaks and the blok it already pulled from the pool (`BlokSpawner.cs:36`) is never returned. Also the `WaitForSeconds(1f)` happens *before* the first usable position is committed, adding latency.
- **Fix sketch:** cap retries (e.g. N attempts), and on failure return the blok to the pool (`BlokPool.AttemptToPoolBlok`) and abort.

### [ ] 🟡 13. `BlokSpawner` NREs if no spawn system is set before `Start`
- **Where:** `BlokControl/Spawning/BlokSpawner.cs:34`.
- **Why:** `BeginSpawningBloks` reads `spawnSystem.MinTimeBetweenSpawns` in a coroutine started in `Start`. If `CoreLogic.ReferenceNewBlokSpawnSystem` hasn't run and no asset is serialized, NRE.
- **Fix sketch:** guard for null `spawnSystem` (wait until set), or ensure wiring order.

---

## Misc null/race

### [ ] 🟠 14. `Fader` NREs if a fade is called before `Start`
- **Where:** `SceneManagement/Fader.cs:11` (caches `canvasGroup` in `Start`), used at `:14-38`.
- **Why:** No `GetComponent` in `Awake`; another script's `Start`/`Awake` calling `FadeOutImmediate`/`FadeOutIn` first → NRE. Execution order between `Start`s isn't guaranteed.
- **Fix sketch:** cache `canvasGroup` in `Awake`, or lazily `GetComponent` on first use.

### [ ] 🟡 15. `PlayerHealth.Die` dereferences a possibly-null `audioSource`
- **Where:** `Combat/PlayerHealth.cs:52-54` (set in `Start` via `GetComponentInChildren<AudioSource>()`).
- **Why:** If the player has no child `AudioSource`, or dies before `Start`, `audioSource.transform` NREs.
- **Fix sketch:** null-check before playing; cache in `Awake`.

### [ ] 🟡 16. New material instance created per enemy
- **Where:** `Combat/EnemyHealth.cs:41` — `GetComponentInChildren<SpriteRenderer>().material`.
- **Why:** Accessing `.material` instantiates a per-renderer material (dissolve needs it), but with many enemies this churns materials each spawn. Fine for small counts; watch it for infinite-wave mode.
- **Fix sketch:** acceptable for now; if profiling shows churn, use a `MaterialPropertyBlock` for `_Fade` instead of a material instance.

### [ ] 🟡 17. `PlayerConfigurationSystem` routes input via `Keyboard.current` instead of the event's device
- **Where:** `Control/PlayerConfigurationSystem.cs:77-92, 105-118, 131-139`.
- **Why:** `OnCheckInput` receives the specific `inputDevice`, but the Check* methods use `Keyboard.current`/`Gamepad.current`. With multiple keyboards this misattributes join presses. Harmless with the typical one-keyboard-two-schemes setup.
- **Fix sketch:** pass and use the `keyboard`/`gamepad` argument already in scope.

---

_When you fix one, tick its box and append "fixed in `<commit>`" so this stays a real ledger._
