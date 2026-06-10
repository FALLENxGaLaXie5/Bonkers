# 05 — UI Toolkit Migration (cost-benefit + how-to-build)

> **PROPOSAL — forward-looking; not as-built.** Current architecture = [`CONTEXT-MAP.md`](../../CONTEXT-MAP.md)
> + per-module `context.md`. **Verify against live code before implementing.** Nothing here is built yet.

Source idea: [`docs/ideas/05`](../ideas/05-ui-toolkit-migration.md). This is the *UI-tech-choice layer*
**beneath** the four-player work ([`01-four-player.md`](./01-four-player.md)) and the menu overhaul
([`02-menu-overhaul.md`](./02-menu-overhaul.md)) — both of those rebuild screens, so the question
"uGUI or UI Toolkit?" is decided here, **once**, before either rebuilds anything.

**Verdict up front: Option B — menus/HUD on UI Toolkit, in-world stays uGUI/sprites — done *with* the
menu overhaul, not as a separate migration.** Rationale at the bottom.

---

## Where we are today (verified)

No project-level `.uxml`/`.uss` exists — every hit is in `Library/`/`Packages/` (editor tooling only).
**Zero runtime UI Toolkit today.** Everything is uGUI/Canvas + TMPro + DOTween. Confirmed files:

| UI element | Current tech | In-world / screen | File (verified) | Migration difficulty |
| --- | --- | --- | --- | --- |
| Score / HUD text | `TextMeshProUGUI`, **positional child wiring** | Screen (overlay) | `Score/PlayerScore.cs`, `Control/Input/PlayerInputHandler.cs:58` | **Hard** — coupling, see below |
| Player setup / join cards | uGUI `Button`/`GameObject` panels, TMP title | Screen | `Control/PlayerSetupMenuController.cs` | Medium |
| Main menu | uGUI buttons → `Portal` | Screen | `SceneManagement/MainMenu.cs` | Easy |
| Pause menu | uGUI; **also the HUD child container** | Screen | `Core/PauseMenu.cs` | Medium (entangled w/ HUD) |
| Screen fader | `CanvasGroup` alpha coroutine | Screen (fullscreen) | `SceneManagement/Fader.cs` | Easy (trivial in USS) |
| Boost bar | `Slider` + `Image` fill + `Gradient` | **In-world** (tracks player) | `Movement/BoostBar.cs` | **Keep uGUI** |
| Floating score popups / puddle FX | sprites / world-space canvas | **In-world** | (effects/world) | **Keep uGUI/sprites** |
| Heuristic gen UI | `UnityEngine.UI` | Screen (tool) | `Content Generation/.../UI/BlokGenerationHeuristicUI.cs` | Out of scope (tool) |

### The HUD coupling that motivates this (verified)

`PlayerInputHandler.InitializePlayer` finds the HUD slot by **positional child index**:

```csharp
// Control/Input/PlayerInputHandler.cs:58
GameObject scoreObject = FindObjectOfType<PauseMenu>().transform.GetChild(playerNum - 1).gameObject;
// then PlayerScore.SetupScoreUI(scoreObject) binds the TMP field
```

So the HUD is "player N → the Nth child of the PauseMenu transform." That is brittle, hard-codes player
count, and is a direct blocker for 1–4 players ([`01`](./01-four-player.md)). A **data-bound, templated**
HUD (one `VisualElement` template instanced per joined player, bound to a score source) is the clean fix —
and is the single strongest reason to consider UI Toolkit at all.

---

## Cost-Benefit Analysis

### Benefits

| Benefit | Who it helps | Confidence |
| --- | --- | --- |
| Templated N-player HUD (kills the `GetChild(playerNum-1)` coupling) | 4-player work, future modes | **High** |
| One USS theme layer (colors, hover/"bonk" states) vs per-prefab Canvas tweaks | Menu iteration, designer | High |
| Resolution-independent layout (flex), fewer anchor headaches | Multi-res, splitscreen | Medium |
| Accessibility from one stylesheet: colorblind palettes, text scaling (see below) | Players, a11y | Medium |
| Fewer prefabs / less scene wiring; structure lives in versioned UXML text | Maintainability, diffs | Medium |
| UI Builder visual editor + live USS reload → faster menu iteration | Solo dev velocity | Medium |

### Costs / risks

| Cost / risk | Size | Mitigation |
| --- | --- | --- |
| Rebuild every screen menu from scratch | **L** | Scope to menus/HUD only (Option B); reuse the menu-overhaul rebuild — don't migrate twice |
| **Hybrid = two UI systems** to reason about (UITK screens + uGUI in-world) | M | Draw a hard line: screen-space → UITK, anything tracking a world position → uGUI. Document it. |
| DOTween/Animancer "juice" doesn't port — UITK animates via **USS transitions / `experimental.animation`** | M | Re-author button/menu juice as USS transitions; keep DOTween only for in-world. Accept some feel-loss on menus. |
| Input System ↔ UITK event routing for **multiple players/devices** | **M–H** | See gotchas below — biggest unknown; de-risk in the spike |
| Team/solo familiarity ramp (flex layout, USS specificity, C# query API) | M | One-screen spike first; keep uGUI fallback until the spike proves out |
| World-space UITK is immature in Unity 6 | (avoided) | **Don't use it** — in-world stays uGUI/sprites by design |
| Re-test all menu navigation/focus for gamepad (Select/auto-focus) | M | UITK focus ring differs from uGUI `Selectable`; budget QA |

### Verdict — three options

| Option | Pros | Cons | Recommendation |
| --- | --- | --- | --- |
| **A — Stay on uGUI** | Zero migration; works today; juice already wired | HUD child-index coupling persists; theming stays per-prefab; menu overhaul rebuilds in old tech | No — leaves the 4-player HUD blocker unsolved |
| **B — Menus/HUD on UITK, in-world uGUI (hybrid)** ✅ | Fixes HUD coupling; one theme layer + a11y; rebuild happens *anyway* in the menu overhaul; avoids immature world-space UITK | Two UI systems; input-routing unknown; juice re-authored | **Recommended.** Best value because the menu overhaul already pays the rebuild cost — fold this in. |
| **C — Full migration (incl. in-world)** | One UI system end-to-end | Bets on immature world-space UITK; boost bars/popups regress; large risk for little gain | No — not worth it now; revisit if Unity's world-space UITK matures |

---

## Target hybrid architecture (Option B)

**Line in the sand:** *screen-space → UI Toolkit; anything anchored to a world position → uGUI/sprites.*

| Moves to UI Toolkit (UXML/USS/UIDocument) | Stays uGUI / sprites |
| --- | --- |
| Main menu, options, quit | Boost bars (track player) — `Movement/BoostBar.cs` |
| Player setup / join cards | Floating score popups |
| Pause menu (screen overlay) | Puddle / environment-effector FX |
| Score/HUD overlay (templated per player) | Any blok/world label |
| Fullscreen fader (USS opacity transition) | — |

**HUD shape:** one `UIDocument` per level holding a HUD root; a `PlayerHudSlot.uxml` template instanced
**per joined player** and bound to that player's score source (event-driven, not `GetChild`). When a player
joins, instance a slot; on leave, remove it. This deletes the positional-index coupling and scales 1→4 (or N)
for free — the concrete win that makes this doc worth doing. Source of player joins/scores is the same
`PlayerConfiguration*` flow the menu overhaul ([`02`](./02-menu-overhaul.md)) and four-player ([`01`](./01-four-player.md))
docs build on; this doc supplies the *view*, they supply the *data*.

### USS theming for accessibility

The a11y case is real and largely *free once everything reads from one stylesheet*:

- **Colorblind palettes:** define player/team colors as USS variables (`--player-1`, …); ship 2–3
  alternate stylesheets (deuteranopia/protanopia/tritanopia) and swap the active sheet at runtime.
  Per-prefab uGUI today would mean re-coloring every prefab by hand.
- **Text scaling:** a root `--ui-scale` / font-size variable propagated through USS lets one setting
  resize all menu/HUD text. uGUI needs per-component font handling.
- **Hover/"bonk" states:** `:hover`/`:focus` pseudo-classes centralize feedback styling.

This is a *benefit of the hybrid*, not a reason on its own — but it tilts the verdict toward B over A.

---

## Sequencing — do it ONCE, with the menu overhaul

**Hard dependency on [`02-menu-overhaul.md`](./02-menu-overhaul.md): build the new menus *directly in UI
Toolkit*.** If we ship the menu overhaul in uGUI first and migrate later, we rebuild the same screens twice.
Decision order:

1. **Spike** (this doc) — prove the tech + de-risk input routing.
2. **Adopt** UITK as the menu-overhaul's view layer (02), HUD template feeds four-player (01).
3. In-world UI is untouched throughout (stays uGUI).

If the spike fails (input routing too painful), fall back to Option A and do the menu overhaul in uGUI —
no sunk cost beyond the spike.

## Input System + UI Toolkit gotchas (de-risk these first)

Bonkers is **local multiplayer with per-player `PlayerInput`** — the riskiest part of UITK adoption:

- UITK routes UI events through a single `InputSystemUIInputModule`/event system by default; it is **not**
  natively per-player. Multiple players navigating *separate* join cards with separate devices is the
  classic pain point — verify which device drives UITK focus.
- The join/setup screen specifically needs **N independent cursors/focus** (each device controls its own
  card). Confirm whether one `UIDocument` can be driven per-player or whether join cards need a different
  approach (possibly keep the join screen on uGUI even under Option B if UITK can't do per-device focus cleanly).
- Gamepad navigation/focus model differs from uGUI `Selectable.Select()` (used in
  `PlayerSetupMenuController`); re-validate auto-focus and Ready-button focus.
- Pause is per-player today (`Core/PauseMenu.cs` subscribes to `Player.Pause`); ensure the UITK overlay
  doesn't capture/steal input from gameplay for the other players.

## Plugin re-wiring

- **DOTween:** menu juice (button bonk, panel slide) currently `DOScale`/`DOFade` on Transforms. UITK has no
  RectTransform — re-author as **USS transitions** (`transition: scale 0.1s`) toggled by adding/removing a
  class, or `VisualElement.experimental.animation` for code-driven tweens. Keep DOTween for in-world only.
- **Fader:** `Fader.cs` `CanvasGroup` alpha coroutine → a fullscreen `VisualElement` with a USS `opacity`
  transition (and fixes the cache-in-`Start` NRE noted in the SceneManagement `context.md`). Trivial win.
- **Animancer:** unaffected (enemy animation, not UI).
- **Odin:** inspector tooling — unaffected at runtime.

## Smallest first step (the spike)

Rebuild **one** screen in UI Toolkit and measure before committing. Pick the **pause menu** (self-contained,
screen-space, already entangled with HUD so it teaches us the HUD-template path) — or the **main menu** if you
want the lowest-risk warm-up.

**Build:** a `PauseMenu.uxml` + `bonkers-theme.uss` + a `UIDocument`-driven controller; wire one button to
`Portal`/resume; instance **one** `PlayerHudSlot.uxml` bound to a live `PlayerScore`.

**Measure (this is the deliverable):**
1. Effort: hours to rebuild vs the uGUI equivalent.
2. **Input routing:** can ≥2 players/devices navigate independently? (pass/fail — the gating question)
3. Juice parity: can USS transitions match the DOTween button feel? (subjective, note gaps)
4. Theming: swap one colorblind palette via stylesheet — confirm it's one-file.
5. Diff/iteration feel: UI Builder + live reload vs prefab editing.

Then decide **B vs A** with data. Do **not** touch in-world UI in the spike.

---

## ⚔️ Adversarial Challenge

Red-team pass: the doc argues honestly *against itself* in places, but the verdict (Option B) still over-reaches. The one genuine **fix** here — the HUD child-index coupling — is fully solvable in uGUI for a fraction of the cost. Everything else is a quality/velocity *bet* riding on an unproven input-routing assumption, justified largely by "we're rebuilding anyway."

| Concern | Severity | Why it bites | Cheaper / safer alternative |
| --- | --- | --- | --- |
| The motivating "blocker" is solvable in uGUI alone | High | `PlayerInputHandler.cs:58` `GetChild(playerNum-1)` is a *wiring* bug, not a tech limit. An event-bound, pooled HUD-slot prefab kills it with zero migration | Data-driven uGUI HUD: a `PlayerHudSlot` prefab instanced per join, bound to `PlayerScore` via an existing event. Same 1→4 win, days not weeks |
| Verdict B leans on "rebuilding menus anyway (02)" | High | That's a dependency on an *unbuilt* doc. If 02 ships even partly in uGUI, the "marginal cost" framing collapses and you migrate twice — the exact thing the doc warns against | Decouple: fix the HUD coupling now (uGUI), defer the UITK decision until 02 is actually scoped and the spike has run |
| Per-player input routing is the gate, yet unverified | High | Bonkers is per-player `PlayerInput`; UITK routes through one `InputSystemUIInputModule`. The doc itself flags join cards may *have* to stay uGUI — that's the screen most needing N-cursor focus | Spike this *in isolation* before any menu work; if it fails, the whole hybrid loses its headline (templated join cards) |
| Hybrid = two permanent UI systems | Med | "Screen vs world" line rots under deadline pressure; every new dev learns both flex/USS and uGUI anchors | Staying on uGUI keeps one mental model; the a11y/theming wins are real but achievable with uGUI palette SOs + TMP style sheets |
| Lost DOTween menu juice | Med | Current menu feel is `DOScale`/`DOFade`; USS transitions "cover most" is an untested claim | Accept feel-loss only after the spike measures it; don't pre-commit |

- **The strongest argument in the doc is the weakest under scrutiny.** The HUD coupling (`PlayerInputHandler.cs:58`, verified) is framed as "the single strongest reason to consider UI Toolkit at all" — but it's a positional-lookup smell, not a uGUI limitation. A uGUI `PlayerHudSlot` prefab instanced on join and bound via an event SO solves it identically. Once that fix is available in uGUI, the migration loses its one concrete *fix* and becomes a pure quality/velocity bet. That bet may still be worth it, but the doc should not sell it as solving the 4-player blocker.
- **"Rebuild anyway, so marginal cost is small" is circular.** It assumes 02 commits to UITK; 02 is itself unbuilt. The two docs lean on each other. Break the loop: ship the event-bound HUD in uGUI (banks the 4-player win immediately), then let the 02 spike decide UITK on its own merits with no HUD pressure.
- **Input routing is correctly named as the gate but mis-sequenced.** The doc puts the spike first (good) yet still prints "Verdict up front: Option B" (line 11) — deciding before the gating experiment runs. If the spike can't give two devices independent focus on join cards, Option B's flagship feature (per-player templated cards) is gone and you're left with a themed main menu, which uGUI + a palette SO already delivers. Don't pre-declare the verdict above the experiment that could kill it.
- **Real cheap wins exist without migrating.** The Fader NRE ([BUGS #14](../BUGS.md), `Fader.cs`) and the HUD coupling are both fixable in uGUI today. Colorblind palettes are a uGUI palette-SO + per-player icon (already proposed in [04 §6](./04-feature-depth.md)). The migration bundles real fixes with a speculative tech change — unbundle them.

**Verdict:** descope first — solve the HUD coupling + Fader NRE in uGUI now (banks the 4-player win cheaply), run the input-routing spike in isolation, and only adopt UITK if the spike passes *and* 02 independently chooses it. Do not pre-commit to Option B above the gating experiment.
