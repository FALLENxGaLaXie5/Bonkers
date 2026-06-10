# BloksResetterSystem

Resets bloks (back to pool / starting state) — used to clear/reset the arena between rounds or generation passes.

## Key files

- `Runtime/` — the resetter runtime (asmdef `Bonkers.BloksResetter.Runtime`).
- `Editor/` — editor tooling for the resetter.

## Depends on / Used by

- **BlokControl** (pool/active bloks); invoked from level/generation flow.

## Language

> Stub.

**Reset**: Returning all active bloks to their pooled/initial state.
