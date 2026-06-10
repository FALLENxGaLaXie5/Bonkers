# Feature Ideas — Grab Bag

**Confidence:** loose. A running list of feature spitballs that came up while reading the codebase. Each: one-liner + why it fits Bonkers + rough size. Promote the good ones into their own doc/plan.

## Game modes
- **Infinite Wave polish** (S–M): you already have `Level_InfiniteWaveSpawn`. Add scaling difficulty, wave counter, between-wave shop (spend score on powerups), high-score table.
- **2v2 Team mode** (M): once 4-player lands (doc 01). Shared score, friendly-fire toggle, team-colored bloks.
- **King of the Hill / Zone control** (M): a moving zone you hold by bonking rivals out of it.
- **Last-Blok-Standing / Sudden Death** (S): shrinking arena (ties to dynamic generation) forces confrontation.
- **Co-op survival** (M): all players vs. escalating enemy waves on a generated map.
- **Time Attack / Score Attack** (S): clear N enemies fastest, or max score in 90s.

## Core-loop depth
- **Blok combos / chains** (M): chain reactions (bomb → ignites adjacent bombs; ice → long slides into clusters). Reward multi-hits with score multipliers (`Score/BlokHitTracker` already tracks attribution).
- **More blok types** (S each, once refactor #1 lands): sticky, teleporter, mirror (reflects a push), heavy (needs boost to move), spawner variants.
- **Charged bonk / aiming** (M): hold to charge a stronger push; telegraph direction.
- **Player abilities per character** (M): Grubber dashes, Slimo leaves a trail, Turb has a body slam (`TurbBodySensor` hints at this). Pick at character select (doc 02).

## Powerups & hazards (build on `ItemDrops/`)
- **More powerups** (S each): speed, shield/invuln (re-use `PlayerHealth.SetInvincible`), magnet (pull bloks), bomb-immunity, multi-push.
- **More environment effectors** (S each): the puddle system (`PuddleDrop` + `IEnvironmentEffector`) generalizes well — fire (DoT), ice patch (slide), mud (stick), conveyor (forced drift), wind.
- **Hazard escalation** over a match to ramp tension.

## Progression / meta
- **Unlockables**: characters, color palettes, blok skins, arenas earned by play.
- **Local profiles + stats**: wins, bonks landed, favorite character (use `PlayerPrefs`, not the vestigial `RPG_Game` saving — refactor #7).
- **Daily seed challenge** (ties to procedural generation doc 03): everyone plays the same generated map/seed, compare scores.

## Juice / game feel (cheap, high impact — reuse `Effects/`)
- Hit-stop / freeze-frame on big bonks; screen shake (with an accessibility toggle).
- Score popups already exist (`EnemyHealth` spawns score text) — add combo flair, color by player.
- Dynamic music that intensifies with action; stingers on kills/wins (extend `Audio/`).
- Camera punch/zoom on impactful moments.
- Trails, squash-and-stretch on movement (Squeeze effects already exist).

## Accessibility (worth doing early)
- Colorblind-friendly player palettes; per-player icons not just color.
- Screen-shake + flashing toggles (puddle pulse / fades can be intense).
- Remappable controls; adjustable game speed for younger/older players.

## Tech / quality-of-life
- **Replays / killcam** via deterministic seeds + input recording (synergizes with procedural seeds).
- **Spectator / demo-attract mode** (AI plays behind the menu — doc 02).
- **In-editor "spawn a test arena" tool** for fast iteration.
- **Photo/GIF capture** of chaotic moments for sharing.

## Long shots
- Online multiplayer (large — local-first design helps but netcode is a project of its own).
- Level editor for players (the generation tool from doc 03 is halfway to this).
- Mod/Steam Workshop support for blok types & arenas.

---
When one of these graduates from "fun idea" to "we're doing it," give it its own `docs/ideas/NN-*.md` or turn it into a plan with the `to-prd` / `to-issues` skills.
