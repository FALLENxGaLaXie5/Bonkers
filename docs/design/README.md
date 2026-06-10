# Design Docs (forward-looking architecture proposals)

> [!WARNING]
> **These are PROPOSALS, not as-built documentation.** Nothing here is implemented yet.
> If you are an agent (or human) doing *actual implementation*, your source of truth for
> the **current** architecture is [`CONTEXT-MAP.md`](../../CONTEXT-MAP.md) and the per-module
> `context.md` files — **not** this folder. Treat every file here as "how we *might* build X",
> to be verified against the live code before you touch anything.

## What this folder is

For each brainstorm in [`docs/ideas/`](../ideas/) (the *why / what*), this folder holds a
matching **architecture sketch** (the *how*): proposed module boundaries, data shapes, new
ScriptableObjects/assemblies, migration order, risks, and a smallest-first-step. They were
drafted by architecture-planning agents and then adversarially challenged (see the
**⚔️ Adversarial Challenge** section at the bottom of each doc).

## Doc tree

| # | Design doc | Sketches how to build… | Source idea |
| --- | --- | --- | --- |
| 01 | [`01-four-player.md`](./01-four-player.md) | 1–4 player local multiplayer (N-player join, spawns, HUD, camera) | [ideas/01](../ideas/01-four-player-multiplayer.md) |
| 02 | [`02-menu-overhaul.md`](./02-menu-overhaul.md) | Menu/flow system + scene-transition service + join cards | [ideas/02](../ideas/02-menu-overhaul.md) |
| 03 | [`03-procedural-map-generation.md`](./03-procedural-map-generation.md) | Seeded generate-and-test map tool (recipe SO, validators) | [ideas/03](../ideas/03-procedural-map-generation.md) |
| 04 | [`04-feature-depth.md`](./04-feature-depth.md) | Core-loop depth: blok combos, powerups, game modes, juice | [ideas/04](../ideas/04-feature-ideas.md) |
| 05 | [`05-ui-toolkit-migration.md`](./05-ui-toolkit-migration.md) | Migrating UI from uGUI → UI Toolkit (USS/UXML) + **cost-benefit** | [ideas/05](../ideas/05-ui-toolkit-migration.md) |

## How this relates to the other docs

```
docs/
├── ideas/          ← WHY / WHAT  (brainstorms, options, open questions)
│   └── NN-*.md
├── design/         ← HOW (proposed)  ← YOU ARE HERE
│   └── NN-*.md     ← one per idea; verify vs live code before building
├── adr/            ← DECIDED (hard, irreversible architecture decisions)
├── BUGS.md         ← current bug ledger
└── ARCHITECTURE-REFACTORS.md  ← refactor backlog (prereqs for several of these)
CONTEXT-MAP.md      ← AS-BUILT current architecture (source of truth)
```

**Promotion path:** idea → design sketch (here) → ADR (if a hard decision is made) → implementation.
When a design doc graduates into real work, link the resulting ADR/PR from the top of that doc.
