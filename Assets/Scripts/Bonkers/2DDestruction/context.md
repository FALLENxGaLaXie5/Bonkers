# 2DDestruction  (asmdef: Destruction)

Vendored 2D sprite-fracturing system. Breaks a sprite into polygon fragments (via the bundled Clipper library) and animates them out. Bloks use this for their shatter effect.

> Largely third-party / adapted code. Prefer not to rewrite; wrap it from BlokControl.

## Key files

- `Breakable.cs`, `BreakForce.cs`, `BreakOnOrder.cs` — entry points; `BreakOnOrder.BreakBlok()` + `FadeTime` are what `BlokHealth` calls.
- `SpriteExploder.cs`, `ExplodableAddon.cs`, `ExplodableFragments.cs`, `ExplodeOnClick.cs` — fracture generation.
- `AnimateFragmentOut.cs` — fades/animates individual shards (consumed by `BlokHealth.DestroyBlok`).
- `BlokFragmentGenerationHelper.cs`, `BlokFragmentMaterialModificationHelper.cs`, `FragmentDataStorage.cs`, `Reset.cs`, `ClipperHelper.cs`, `clipper_library/clipper.cs`.

## Used by

- **BlokControl** (`BlokHealth` → `BreakOnOrder` / `AnimateFragmentOut`).

## Language

> Stub.

**Fragment / Shard**: A polygon piece a broken blok sprite splits into.

**Break Order**: A command (`BreakOnOrder`) that triggers fracture + fade-out.
