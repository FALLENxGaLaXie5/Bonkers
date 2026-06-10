# 04 — Feature Depth (architecture sketch)

**PROPOSAL — forward-looking; not as-built. Current architecture = [`CONTEXT-MAP.md`](../../CONTEXT-MAP.md) + per-module `context.md`. Verify against live code before implementing.**

Source idea: [`docs/ideas/04-feature-ideas.md`](../ideas/04-feature-ideas.md) — a grab-bag. This doc organizes it into themes and sketches the *seams* each theme rides on, so the grab-bag graduates without 5 new parallel folders per blok or a hardcoded mode `switch`.

The recurring move: **two refactors unlock most of this depth cheaply.**
- [`ARCHITECTURE-REFACTORS.md`](../ARCHITECTURE-REFACTORS.md) **#1** — `BlokTypeDefinition` SO. New blok types + combos build *on* it, not on more subclasses.
- The **environment-effector seam** (`IEnvironmentEffector` + `PlayerEnvironmentEffectorsControl`) — the natural extension point for powerups/hazards.

Anything touching DOTween must obey the **tween-lifecycle rule** ([REFACTORS #4](../ARCHITECTURE-REFACTORS.md), [BUGS #1/#4](../BUGS.md)) — see §5.

Size = S/M/L. Value = subjective pull.

---

## 1. Core-loop depth — blok combos & chains

Today there is **no chain reaction**: `BombBlokInteraction.BlokHit` just `SetMoving(true, dir)` — bombs don't ignite neighbours, ice doesn't reward cluster slides. Combos are the highest-leverage depth because the bloks, the grid, and score attribution already exist; only the *reaction graph* is missing.

The clean shape: combos are a **behaviour on `BlokTypeDefinition`** (REFACTORS #1), not new interaction subclasses. A definition declares an optional `IChainReaction` (e.g. `IgniteNeighbours`, `SlideCluster`), the break/impact path raises a **`BlokChainEvent`** (new `BaseGameEvent<BlokChainContext>`), and a `ComboTracker` listens to award multipliers. Attribution rides existing `Score/BlokHitTracker` — the chain context carries the originating `playerId` so the whole cascade credits one player.

| Feature | What it needs | Depends-on refactor | Size | Value |
| --- | --- | --- | --- | --- |
| Bomb→bomb ignite | `IChainReaction` on bomb def; neighbour query (grid radius); raise `BlokChainEvent` | #1 (def composes it) | M | High |
| Ice-slide clusters | Ice def reports cells crossed; tally hits in one slide | #1 | M | High |
| Combo score multiplier | `ComboTracker` listens `BlokChainEvent`, feeds `BlokHitTracker`/`PlayerScore` | — (event seam) | S | High |
| Combo flair (popups/SFX) | Effects bundle keyed off chain depth | tween rule §5 | S | Med |

> **Why event, not direct call:** a cascade can span pooled bloks owned by different systems. A `BlokChainEvent` keeps `Score`, `Effects`, and `Audio` decoupled (per the [Events](../../Assets/Scripts/Bonkers/Events/context.md) preference) and avoids re-entrancy when bomb A ignites B which ignites A.

---

## 2. New blok types & per-character abilities

**Blok types after REFACTORS #1 are a data asset, not five files.** Confirmed empty ceremony today: `BasicBlokHealth` is an empty subclass; `WoodBlokHealth` only adds `droppable.SpawnDrop()`. Folding HP/fade/effect-list into `BlokTypeDefinition` means *sticky / mirror / heavy / teleporter* become a definition + maybe one tiny strategy, not a sweep across `BlokHealth/`, `BlokInteraction/`, `BlokEffects/`.

Per-character abilities are a **separate axis** — they belong on the player controllers, not bloks. The honest path is a small `ICharacterAbility` hook invoked by an ability input, deliberately scoped *under* the controller-unification refactor ([REFACTORS #6](../ARCHITECTURE-REFACTORS.md)) so abilities don't get copy-pasted across `GrubberControl`/`TarSlimoControl`/etc.

| Feature | What it needs | Depends-on refactor | Size | Value |
| --- | --- | --- | --- | --- |
| Sticky / heavy / mirror bloks | Data on `BlokTypeDefinition` + 1 strategy each | #1 | S each | Med |
| Teleporter / spawner variants | Definition + paired interaction (mirror existing `SpawnBlokInteraction`) | #1 | S–M | Med |
| Charged bonk / aim telegraph | Hold-to-charge in `PlayerCombat`; push strength scales charge | — | M | High |
| Per-character ability (dash/slam/trail) | `ICharacterAbility` on controller base; ability input | #6 (do base first) | M | High |

> `TurbBodySensor` / `WoodenBlokBonks` already hint at specialized collision — a slam ability can reuse that sensor rather than a new one.

---

## 3. Powerups & hazards — the effector seam

This is the cleanest extension point in the repo. `IEnvironmentEffector.AttemptGetEffector()` hands a `ScriptableObject` to `PlayerEnvironmentEffectorsControl`, which currently special-cases `PuddleDrop` (`effector as PuddleDrop`). **Generalize the reaction:** an effector SO exposes *what it modifies* (speed delta, visual effects, optional DoT) so `PlayerEnvironmentEffectorsControl` stops hard-casting and just applies whatever the effector declares. Fire/ice-patch/mud/conveyor/wind then become **new effector SOs + prefab**, zero controller edits.

Powerups (`Powerup.cs`: `Shield`/`Stamina`, `Spawn` + `Modifier`) are the **timed, player-attached** cousin — same modifier vocabulary, opposite lifetime. A shared `IPlayerModifier` (apply / tick / revert) lets a magnet, shield, multi-push, or fire-DoT be authored once and used by both a pickup and a ground effector.

| Feature | What it needs | Depends-on refactor | Size | Value |
| --- | --- | --- | --- | --- |
| Generalize effector reaction | Effector SO declares modifiers; drop the `as PuddleDrop` cast | — (but fixes [BUGS #5](../BUGS.md) overlap-counter while there) | M | High |
| Fire / ice-patch / mud / conveyor / wind | One effector SO + prefab each | generalize above | S each | High |
| Speed / shield / magnet / multi-push powerups | `IPlayerModifier`; extend `Powerup.PowerupType` | shares §3 modifier vocab | S each | High |
| Hazard escalation over a match | Mode controller (§4) raises spawn rate/severity | §4 GameMode | S | Med |

> **Fix-while-you-extend:** generalizing the effector is the right moment to convert `_isSpeedModified` (a single bool, [BUGS #5](../BUGS.md)) into an active-effector **set/counter**, so overlapping puddles/patches stack and only restore speed on the *last* exit.

---

## 4. Game modes — shared mode infrastructure

The ideas list five modes (Infinite Wave polish, 2v2 team, KotH, last-blok-standing, time/score attack). Built naively each forks `CoreLogic` and the spawn loop. Instead introduce a **`GameMode` ScriptableObject** (win condition, scoring rules, spawn cadence, team config) read by a scene-lifetime **`GameModeController`** that subscribes to existing events (kills, blok breaks, timer) and raises a `MatchEndEvent`. `CoreLogic` stays the per-level *wiring* hub; the mode controller owns *rules*.

This depends on a clean session boundary — modes need a reliable "who's playing / what team" source, which is exactly [REFACTORS #2](../ARCHITECTURE-REFACTORS.md) (stop storing runtime state on SOs, [BUGS #7](../BUGS.md)) and 4-player readiness ([design 01](./01-four-player.md)). Team modes additionally need a `teamId` on the player config.

| Feature | What it needs | Depends-on refactor | Size | Value |
| --- | --- | --- | --- | --- |
| `GameMode` SO + `GameModeController` | Win-condition interface; `MatchEndEvent`; reads existing kill/break events | #2 (session state) | M | High |
| Infinite Wave polish | Wave counter, scaling, between-wave shop, high-score (`PlayerPrefs`) | builds on controller | S–M | High |
| 2v2 team mode | `teamId` on config; shared score; friendly-fire toggle | #2 + 4-player (design 01) | M | Med |
| KotH / zone control | Moving zone = an `IEnvironmentEffector` that scores occupancy | §3 seam | M | Med |
| Last-blok-standing / time-attack | Win condition only (mostly config) | controller | S | Med |

---

## 5. Juice & game feel — done safely

All the cheap wins (hit-stop, screen shake, camera punch, trails, squash-stretch, combo popups) reuse [Effects](../../Assets/Scripts/Bonkers/Effects/context.md) — **but** the current `TweenEffect` family is a known leak source. Adding more juice without the rule multiplies [BUGS #1/#4](../BUGS.md).

**Tween-lifecycle rule (adopt before adding effects — [REFACTORS #4](../ARCHITECTURE-REFACTORS.md)):**
1. Every `ExecuteEffect` **must** `.SetLink(component.gameObject)` (or store the `Tween` for `StopEffect` to `Kill()`). Manual `DOTween.To` setters that capture a renderer outlive a destroyed target otherwise.
2. Any **looping** effect (player pulse/tint) **must** override `StopEffect` to kill + reset to baseline — the base no-op silently fails to revert ([BUGS #4](../BUGS.md)).
3. Add a `protected Tween Track(Tween t, T c)` helper that links + records, and route all effects through it.

| Feature | What it needs | Depends-on refactor | Size | Value |
| --- | --- | --- | --- | --- |
| Tween rule + `Track` helper | One edit to `TweenEffect<T>`; audit existing effects | #4 (this *is* #4) | S | High |
| Hit-stop / freeze-frame, camera punch | Time-scale pulse on big bonk/combo (listen `BlokChainEvent`) | §1 event | S | High |
| Screen shake + accessibility toggle | Reuse `ShakeScaleEffect`; gate behind setting | §6 settings | S | High |
| Trails / squash-stretch | Reuse `Squeeze*` effects on movement | tween rule | S | Med |

---

## 6. Progression / meta & accessibility

Lightweight and mostly orthogonal — a thin **`PlayerProfile` (PlayerPrefs)** service plus a **`GameSettings` SO** that the rest of the systems *read*. Explicitly avoid the vestigial `RPG_Game` saving ([REFACTORS #7](../ARCHITECTURE-REFACTORS.md)).

| Feature | What it needs | Depends-on refactor | Size | Value |
| --- | --- | --- | --- | --- |
| Local stats / unlockables | `PlayerProfile` over `PlayerPrefs`; unlock gates read it | avoid #7 RPG_Game | S–M | Med |
| Daily seed challenge | Shared seed from [design 03](./03-procedural-map-generation.md) generation | design 03 | S | Med |
| Colorblind palettes + per-player icons | Icon alongside color in player config / HUD | 4-player (design 01) | S | High |
| Shake/flash toggles, game-speed, remap | `GameSettings` SO read by Effects/camera/Input | — | S | High |

> Accessibility toggles are worth wiring **early** because §5 juice (shake, puddle pulse, fades) is exactly what they gate — adding the toggle after the juice means retrofitting reads everywhere.

---

## Recommended sequencing

1. **Tween-lifecycle rule (§5.1)** — S, fixes live [BUGS #1/#4](../BUGS.md), unblocks all juice. *(safe, do anytime)*
2. **`BlokTypeDefinition` ([REFACTORS #1](../ARCHITECTURE-REFACTORS.md))** — the keystone for §1 combos and §2 blok types. Don't add blok content before it.
3. **Generalize the effector seam (§3)** — unlocks every powerup/hazard *and* fixes [BUGS #5](../BUGS.md).
4. **Bomb→bomb combo + `ComboTracker` (§1)** — the first headline depth feature, once #2 lands.
5. **`GameMode` controller (§4)** — after [REFACTORS #2](../ARCHITECTURE-REFACTORS.md) session split; Infinite Wave polish is its first payload.
6. Progression/accessibility (§6) trickle in alongside.

### Smallest first step
Adopt the **tween-lifecycle rule** and add the `Track(Tween, T)` helper to `TweenEffect<T>` (§5). It's a single-file, low-risk change that closes two real bugs and is the prerequisite for every juice feature in this doc — value before any new content lands.

---

## ⚔️ Adversarial Challenge

Red-team pass: the doc is mostly sound, but it stacks every headline feature on **two refactors that don't exist yet** and leans on a "one event seam fixes everything" optimism that hides real sequencing and re-entrancy cost.

| Concern | Severity | Why it bites | Cheaper / safer alternative |
| --- | --- | --- | --- |
| Whole doc is gated on `BlokTypeDefinition` (#1) + session-state split (#2), neither built | High | §1, §2, §4 all say "depends-on #1/#2". If the keystone refactor slips, *every* headline feature slips with it — single point of failure | Land **one** vertical slice (bomb→bomb via event only, no def) to prove value before committing to the SO rewrite |
| `BlokChainEvent` as the combo backbone | Med | New `BaseGameEvent<T>` instances already leak listeners across scene loads ([BUGS #23](../BUGS.md)). A high-frequency cascade event multiplies that, plus per-cascade GC | Direct neighbour query + a frame-scoped visited-set; only promote to an event when a *second* system (Audio) actually needs it |
| "Combos credit one player via `playerId` in chain context" | Med | Attribution rides `BlokHitTracker`, but ghost/duplicate players are a live bug ([BUGS #7](../BUGS.md)). Mis-attribution lands on top of an unreliable player identity | Fix #7 (session state) *before* any scoring-attribution feature, not in parallel |
| `IPlayerModifier` (apply/tick/revert) shared by powerups + effectors | Med | This is the same revert-never-fires failure as [BUGS #4](../BUGS.md)/#5 — a generic tick/revert contract with no lifecycle owner will reproduce the puddle bug at scale | Ship the [BUGS #5](../BUGS.md) effector counter first; let the *concrete* fix define the abstraction, don't design the interface up front |
| `GameMode` SO for 5 modes | Low | Real YAGNI risk for the team/shop fields; only Infinite Wave exists today | Doc already concedes this — start at one `IWinCondition` + `MatchEndEvent`, no team config |

- **The keystone is a bottleneck, not just a dependency.** §1.20, §2.36, §4.70 all route through `BlokTypeDefinition` or session-split. That's elegant on paper but means zero shippable depth until a large, untested SO refactor lands. The doc's own "smallest first step" (tween rule) is correctly off this critical path — but everything *fun* is on it. Recommend an explicit escape hatch: bomb-ignite as a direct neighbour query on `BombBlokInteraction.cs:12` (currently a bare `SetMoving(true, ...)`), event-free, to bank a headline feature while #1 incubates.
- **The event-vs-direct-call argument is half-right.** §1's re-entrancy worry (A ignites B ignites A) is real, but an event bus doesn't solve it — `Raise` is synchronous, so a listener that re-breaks a blok still re-enters within the same stack. You need a frame-scoped visited-set regardless; the event only buys decoupling, which you don't need until Audio/Effects subscribe. Verified: `BaseGameEvent.Raise` is a plain synchronous `Action<T>` invoke ([BUGS #23](../BUGS.md) context).
- **"Fix-while-you-extend" the effector (§3) understates the blast radius.** `PlayerEnvironmentEffectorsControl.cs:48,78` hard-cast `effector as PuddleDrop` and the bool guard is at `:17`/`:52`/`:73`. Generalizing the reaction *and* converting `_isSpeedModified` to a counter *and* adding a modifier vocabulary in one commit is three changes to a load-bearing file with live bugs. Do the [BUGS #5](../BUGS.md) counter as its own reviewed commit first, then generalize.
- **Verify before building:** confirmed none of `BlokTypeDefinition`, `IChainReaction`, `GameMode`, `IPlayerModifier` exist (grep is clean). `BasicBlokHealth.cs:7`/`WoodBlokHealth.cs:5` empty-ceremony claim holds. This is a sketch — treat sizes as optimistic; each "S" assumes the refactor it rides on is already free.

**Verdict:** proceed with revisions — decouple at least one headline feature (bomb-ignite) from the keystone refactor so depth ships before the big SO rewrite, and land [BUGS #5](../BUGS.md)/#7 before any attribution-dependent work.
