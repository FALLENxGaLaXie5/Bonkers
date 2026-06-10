# Effects

Reusable, designer-authorable visual effects built on **DOTween**, exposed as ScriptableObjects so they can be dropped onto bloks, puddles, and players in the inspector.

## Key files

- `TweenEffects/TweenEffect.cs` — abstract generic base `TweenEffect<T> : ScriptableObject where T : Component`. `ExecuteEffect(T, Action)` runs it; `StopEffect(T, Action)` reverts it (default **no-op** — override for stoppable/looping effects).
- Concrete tweens — `FadeInEffect`, `FadeOutEffect` (target `SpriteRenderer`); `GrowScaleEffect`, `ShrinkScaleEffect`, `ShakeScaleEffect`, `ShakeRotationEffect`, `SqueezeScaleEffectX/Y`, `ColorLerpEffect` (target `Transform` / `SpriteRenderer`).
- `BlokEffects/` — per-blok-type effect bundles: `BlokEffects` (base) + `BasicBlokEffects`, `BombBlokEffects`, `GlassBlokEffects`, `IceBlokEffects`, `WoodBlokEffects`, `SpawnBlokEffects`.

## Gotcha (see docs/BUGS.md)

- `ShakeScaleEffect` uses `.SetLink(gameObject)` so DOTween auto-kills it on destroy. **`FadeInEffect`/`FadeOutEffect` do not** — they use `DOTween.To` with manual setters and no link, so they can throw `MissingReferenceException` if the target is destroyed mid-tween (the puddle fade issue).
- `StopEffect` defaulting to no-op means looping player effects can't be reverted unless the concrete effect overrides it.

## Depends on

- DOTween, Odin (`EnumToggleButtons`).

## Used by

- **BlokControl** (spawn/break), **ItemDrops** (puddles, pickups), **Control** (player visual effects).

## Language

> Stub — sharpen via grill-with-docs.

**Tween Effect**: A ScriptableObject describing one DOTween animation (speed, ease) runnable against any matching `Component`.

**Execute vs Stop**: `ExecuteEffect` plays; `StopEffect` reverts. Stop only does something if the concrete effect implements it.
