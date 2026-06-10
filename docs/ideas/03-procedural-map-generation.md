# Fully Procedural Map Generation Tool — Big Brainstorm

**Size:** L (program of work, not one task) · **Confidence:** loose / exploratory · **Touches:** `Content Generation/`, `BlokControl/`, `Core/`, `Grid/`, `Enemy Spawn Management/`.

This is a **spitball dump** — the goal is to map the whole possibility space for a real procedural-generation tool, not to commit to one approach. Pick and combine later.

---

## 0. What we already have
- An **ML-Agents** generation path (`Content Generation/`): agents ("brains") that learn to place bloks/players, with a heuristic UI for manual authoring and a `GeneratedContentData` serialized output replayed into a level. Plus ~15 abandoned generator iterations (archive them — refactor #7).
- A grid arena, object-pooled bloks, patrol points, and an A* graph that currently **isn't re-scanned for generated sizes** (`BUGS.md` #9 — a hard prerequisite for *any* generation that changes map size).

So the question isn't "can we generate" — it's "what's the best authoring model, and how much do we lean on ML vs. classic PCG?"

---

## 1. Framing: what makes a *good* Bonkers map?
Generation is only as good as its objectives. Candidate quality criteria to encode (as ML reward or as PCG constraints):
- **Solvable / fair:** every player spawn can reach the action; no player boxed in at start.
- **Connected:** open space is traversable (flood-fill check); no isolated pockets unless intentional.
- **Bonk lanes:** long straight runs so bloks can be pushed into enemies — the core verb needs room to work.
- **Cover & chokepoints:** immovable bloks create tactics without turning it into a maze.
- **Symmetry / fairness** for versus (mirror or rotational), vs. **asymmetry** for chaos/co-op.
- **Blok-type mix:** a readable distribution (not 90% bombs); special bloks placed where they create interesting chains (bomb next to a cluster, ice on a long lane).
- **Density curve:** open at spawns, denser in the middle.
- **Aesthetic coherence:** themed tiles, not noise.

Write these down as a checklist first; every generator below is judged against them.

---

## 2. The menu of generation techniques (mix & match)

### A. Classic PCG (cheap, controllable, debuggable) — recommended backbone
1. **Noise fields** (Perlin/Simplex) → threshold into wall/floor density. Fast, organic; needs post-processing for connectivity.
2. **Cellular automata** (the cave-gen classic): random fill → smoothing passes → flood-fill to keep the largest region. Great for organic open arenas.
3. **BSP / room-and-corridor:** recursively split the space into rooms joined by corridors. Good for structured, "arena of rooms" layouts.
4. **Wave Function Collapse (WFC):** author a small set of tile patterns + adjacency rules; WFC stitches them into coherent, hand-authored-*looking* maps. **Strong fit** for a tile/blok game — you get designer control via the tile set, and infinite variety. Probably the highest-ceiling classic option.
5. **Template stamping / chunk assembly:** hand-author "chunks" (a bomb cluster, an ice lane, a fort of immovables) and randomly place/rotate/mirror them, then connect. Fast path to "designed-feeling" maps with low risk.
6. **Agent-based "digger" / drunkard's walk:** carve paths with random walkers — cheap way to guarantee connectivity (it's literally walked).
7. **Voronoi regions:** partition into zones, assign each a theme/blok-mix. Good for variety + readable structure.
8. **Symmetry operator:** generate a quadrant/half, then mirror/rotate for fair versus maps — a *post-process* applicable to any technique above.

### B. ML-Agents (what exists) — keep as an *option*, not the only path
- Pros: can learn to satisfy fuzzy "fun" objectives if you can express the reward; you've already invested.
- Cons: hard to control precisely, slow to iterate, non-deterministic, heavy to retrain when design changes; debugging "why did it place that" is painful.
- **Best role:** a *finisher/critic* on top of classic PCG (e.g. ML scores or nudges a PCG candidate), or a "surprise me" mode — not the primary generator. Or use a learned **evaluator** (predicts fun/fairness) to rank many cheap PCG candidates ("generate-and-test").

### C. Hybrid (likely the sweet spot)
**Generate-and-test:** a fast classic generator (WFC or cellular automata) produces many candidates → a cheap **evaluator** scores them against §1 criteria (connectivity via flood-fill, lane length, fairness, blok-mix) → keep the best. The evaluator can be hand-written rules first, ML later. This gives control *and* quality without betting everything on training.

---

## 3. The TOOL itself (this is what makes it powerful)
A generator is only useful if it's authorable and inspectable. Build an **editor tool** (EditorWindow + runtime preview), not just a runtime call.

Features to spitball:
- **Seeded & deterministic:** every map = a seed. Type a seed, get the same map. Shareable seeds.
- **Live preview** in the editor with a regenerate button; scrub seeds.
- **Parameter panel** as a ScriptableObject "GenerationRecipe": size, density, technique, blok-mix weights, symmetry, spawn count, theme. (Matches the project's SO-config convention.)
- **Constraints/validators** run on output with pass/fail readout: connectivity, min lane length, spawn reachability, blok budget. Reject & reroll on fail.
- **Brush/override layer:** generate, then hand-tweak — paint/erase bloks, lock regions and regenerate the rest. Human-in-the-loop beats fully-auto.
- **Export to the existing pipeline:** write a `GeneratedContentData` and hand sizing to `CoreLogic` + `BlokSpawnSystem`; re-scan the A* graph (fix `BUGS.md` #9).
- **Difficulty/mode presets:** "Open Brawl", "Maze", "Fortress", "Symmetric Versus", "Chaos".
- **Analytics overlay:** heatmaps of openness, predicted bonk-lanes, reachability — visualize the §1 criteria.

---

## 4. Runtime vs. authoring time
Two distinct use-cases — decide which you want (maybe both):
- **Authoring-time:** designer generates, curates, saves a handful of good maps that ship. Lower risk, higher quality. (Recommended first.)
- **Runtime "endless":** generate a fresh map each round (great for Infinite Wave / replayability). Needs robust validators so a bad seed never ships a broken arena, and fast generation (<1 frame budget or async over a load screen).

---

## 5. Dynamic / gameplay-driven generation (further out)
- **Reactive arenas:** map shifts mid-match — walls rise/fall, sections collapse, hazards spread (tie to puddles).
- **Difficulty-adaptive:** generation reacts to how players are doing (struggling → more open + fewer enemies).
- **Destructible-aware:** since bloks break, factor the *post-destruction* arena into quality (it shouldn't degenerate into an empty box).
- **Set-piece injection:** guarantee one "interesting" feature per map (a bomb chain, a giant ice lane).

---

## 6. Suggested roadmap (low-risk → high-ceiling)
1. **Unblock:** fix A* re-scan (`BUGS.md` #9); archive dead generators (refactor #7). Define the §1 quality checklist.
2. **MVP generator:** cellular-automata or template-stamping behind a `GenerationRecipe` SO + an EditorWindow with seed + live preview + connectivity validator. Export into the existing pipeline.
3. **Add validators + generate-and-test** (rule-based evaluator). Add symmetry post-process for versus.
4. **WFC** as a higher-quality technique once the tool/validators exist.
5. **Brush/override layer** for human curation.
6. **Optional ML** as a critic/ranker or "surprise me" mode — only if §2B earns its keep.
7. **Runtime endless mode** once validators are trustworthy.

## Open questions
- Tile/blok set size for WFC — how many distinct adjacency-constrained pieces do we author?
- Is map size fixed-per-mode or a generation parameter? (Affects camera — doc 01.)
- Do enemies/powerups generate *with* the map or spawn dynamically after? (Currently dynamic via `EnemySpawnManagement` — keep that, generate only static layout + spawn points first.)
- Reuse the ML `GeneratedContentData` format as the universal "map" payload so all generators (classic + ML) share one export?

## Related
- `Assets/Scripts/Bonkers/Content Generation/context.md`, refactor #7, `BUGS.md` #9, doc 01 (camera/spawns).
