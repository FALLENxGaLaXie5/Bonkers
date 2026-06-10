# Architecture Refactors (future)

Opportunities to make the codebase easier to extend, less error-prone, and more AI-navigable. **None are urgent** — this is a backlog to pull from, ordered roughly by value-for-effort. Each notes the smell, the payoff, and a sketch. Discuss before doing the big ones; the user owns architecture.

Legend: 💎 high value · ⚖️ medium · 🧹 cleanup. Effort: S/M/L.

---

## 💎 1. Tame the blok-type boilerplate (M)
**Smell:** A blok type (Basic, Bomb, Glass, Ice, Wood, Spawn, Immovable) is spread across ~5 parallel folders — `BlokControl/<Type> Blok Control/`, `BlokControl/BlokHealth/<Type>BlokHealth`, `Combat/BlokInteraction/<Type>BlokInteraction`, `Effects/BlokEffects/<Type>BlokEffects`, plus a prefab. Adding a type means touching all of them and wiring a prefab; easy to miss one.

**Payoff:** Adding/tuning a blok type becomes a single data asset + maybe one script. Fewer "forgot to add the X for the new type" bugs.

**Sketch:** Introduce a `BlokTypeDefinition` ScriptableObject that references the interaction strategy, health profile, effect bundle, and pooling data for a type. Keep the strategy interfaces (`IBlokInteraction`, etc.) but let the definition compose them, so the prefab just holds a reference to its definition. Consider whether health/effects can be data-driven (HP, fade time, effect list) rather than a subclass per type — several `*BlokHealth` subclasses may differ only in values.

---

## 💎 2. Stop storing runtime state on ScriptableObjects (M)
**Smell:** `PlayerConfigurationSystem` (and to a lesser degree `BlokSpawnSystem`) are SOs that hold live, mutating gameplay state. SO state persists across play sessions in-editor and across scene loads, producing stale/ghost data (see `BUGS.md` #7).

**Payoff:** Eliminates a whole class of "works on first Play, breaks on second" bugs.

**Sketch:** Split each SO into **config (immutable, authored)** vs **runtime state (reset on enable / scene-scoped)**. Mark runtime collections `[NonSerialized]` and clear them in an explicit `Initialize()`. Or move join/session state to a scene-lifetime MonoBehaviour (a `SessionController`) and keep the SO as pure config.

---

## ⚖️ 3. Consolidate the two event-bus systems (S–M)
**Smell:** `Events/` has both the parameterized `BaseGameEvent<T>` family and a `Legacy*` UnityEvent-based family. Two ways to do the same thing invites inconsistency.

**Payoff:** One mental model; new contributors (and AI) don't have to learn both.

**Sketch:** Pick the parameterized family as canonical (it's the documented preference). Migrate `Legacy*` listeners opportunistically when you touch them, then delete the legacy classes once unused. Track remaining legacy usages in an issue.

---

## ⚖️ 4. A consistent tween lifecycle (S)
**Smell:** `TweenEffect` subclasses are inconsistent about cleanup — `ShakeScaleEffect` uses `.SetLink(gameObject)`; the fades don't. `StopEffect` defaults to a silent no-op, so "stoppable" effects silently aren't. Root cause of `BUGS.md` #1, #4.

**Payoff:** Effects can't outlive their targets; stop/revert actually works.

**Sketch:** Establish a rule in `TweenEffect<T>`: every `ExecuteEffect` must `.SetLink(component.gameObject)` (or store the `Tween` for `StopEffect` to `Kill`). Consider a protected helper `protected Tween Track(Tween t, T c)` that links + records. Make `StopEffect` abstract-ish or at least loudly document that loops must override it.

---

## ⚖️ 5. Replace the hand-rolled pool internals (M)
**Smell:** `BlokPool` stores pools as child GameObjects and pulls `transform.GetChild(0)` / checks `childCount` as a makeshift stack. It's an `Odin SerializedMonoBehaviour` dict keyed by SO. Works, but fragile (KeyNotFound, empty-pool branches, `Random.Range` on shrinking lists) and hard to reason about.

**Payoff:** Fewer pool edge cases (`BUGS.md` #10–#13); clearer ownership of active vs pooled bloks.

**Sketch:** Wrap Unity's `UnityEngine.Pool.ObjectPool<T>` (or a small typed pool class) per blok type, behind the current `BlokPool` API so callers don't change. Keep the SO-keyed lookup but store typed pools, not transforms.

---

## ⚖️ 6. Unify player/enemy character controllers (M–L)
**Smell:** Many near-parallel control scripts — `GrubberControl`, `GhostlyGrubberControl`, `TarSlimoControl`, `ToxicSlimoControl`, `TurbControl`, plus `AIControl` / `AIControlPathfinder` / `AISingleSpaceMovementControl`. Likely substantial duplication.

**Payoff:** New characters/enemies become small; shared movement/combat/animation wiring lives once.

**Sketch:** Extract a `CharacterControllerBase` (movement + health + animation hookup) and let each archetype override only its unique behavior. For AI, separate **brain** (decide) from **mover** (execute) cleanly — `IAIMovement` already hints at this; push more shared logic into a base brain.

---

## 🧹 7. Remove vestigial / scratch code from the build (S)
- `RPG_Game/` — leftover RPG-tutorial saving/scene code, not part of Bonkers. Verify no scene references it, then delete (see its `context.md`).
- `TestCode/` — scratch pathfinding/movement; ensure it's not in any build and consider moving out of `Assets/` or behind an editor-only asmdef.
- `Content Generation/Runtime/Brains/Generator 11..22`, `Generator Multi Chain*`, `Test*` — many dead ML iterations. Archive to a branch or a `_Archive/` folder so the live generation path is obvious.

**Payoff:** Smaller surface area; AI/devs stop reading dead code; faster compiles.

---

## 🧹 8. Centralize death/cleanup ownership (S)
**Smell:** Detached helper objects (MovePoint, BoostBar) and effects are cleaned up ad hoc; some leak on death (`BUGS.md` #6).

**Payoff:** No orphans; predictable teardown.

**Sketch:** Give the player a single teardown path (subscribe `PlayerMovement`/effects to `onPlayerDeath`, or an `OnDestroy`) that disposes everything it spawned/detached.

---

## 🧹 9. Naming & layer/config hygiene (S)
- The Combat asmdef is named `RPG.Combat` while everything else is `Bonkers.*`. Rename to `Bonkers.Combat` (update GUID refs) for consistency — low risk, do it in a dedicated commit.
- LayerMasks, radii, and timings are sprinkled through spawn/movement/combat as literals. Consider gathering tunables into the existing SO config assets so designers tune in one place (matches the project's stated convention).

---

## Cross-cutting note: 4-player readiness
Several systems hardcode two players (`Player2Control`, `CheckKeyboard1Input`/`CheckKeyboard2Input`, fixed control schemes). Refactors #2 and #6 are prerequisites for clean 4-player support — see `docs/ideas/01-four-player-multiplayer.md`.
