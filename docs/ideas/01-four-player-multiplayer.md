# Four-Player Local Multiplayer

**Size:** L · **Confidence:** promising · **Depends on refactors:** #2 (SO runtime state), #6 (unify controllers) in `ARCHITECTURE-REFACTORS.md`.

Goal: go from the current 2-ish player setup to a clean 1–4 player local couch experience.

## Where we are today (grounding)
- `Control/PlayerConfigurationSystem.cs` already supports **three** join paths: WASD keyboard (`Keyboard`), arrow keyboard (`Keyboard2`), and gamepad (`Controller`), instantiating a prefab per scheme. So the join flow is *most* of the way to N players already.
- But there are 2-player assumptions baked in: `Player2Control.cs`, the `CheckKeyboard1Input` / `CheckKeyboard2Input` split, and likely fixed spawn points / camera framing / UI for two.
- `playerConfigs` is a list (good), but it lives on a ScriptableObject with persistence pitfalls (`BUGS.md` #7) — fix before scaling player count or you'll get ghost players.

## The shape of the work
1. **Generalize join → N.** Drop `Player1/2`-specific scripts in favor of one player prefab parameterized by a `PlayerConfiguration` (index, device, scheme, color). Two keyboard players + 2 gamepads = 4; or 4 gamepads. Cap at 4.
2. **Device/scheme allocation.** Today a single keyboard hosts two schemes. For 4-player, decide the matrix: max 2 keyboard players (WASD + arrows) + up to 4 gamepads, clamped to 4 total. Reject joins past 4 with feedback.
3. **Spawn points.** Replace fixed P1/P2 spawns with an ordered list of N spawn points (or derive from map size — ties into procedural generation, doc 03).
4. **Camera.** Options: (a) single shared camera that frames the whole arena (simplest, fits a grid arena); (b) dynamic zoom to bounding box of living players; (c) split-screen (probably overkill for a shared-grid game). Recommend (a)/(b).
5. **UI / HUD.** Per-player score, boost bar, color, and ready cards must scale to 4. The join screen needs 4 slots.
6. **Color/identity.** `SetPlayerColor` exists — extend to a palette of ≥4 distinct, colorblind-friendly colors; show on join cards and in-game.

## Open questions / spitballs
- **Team mode?** 2v2 once 4 players exist — shared score, friendly-fire toggle, team-colored bloks.
- **Drop-in / drop-out** mid-match? Or locked at level start (simpler)?
- **Player collision** — do players push each other? Can you bonk a blok into another player? (Great chaos potential; needs a rule for friendly damage.)
- **Asymmetry** — different characters (Grubber/Slimo/Turb) with different stats per player? Pick-screen during join.
- **Scaling difficulty** — enemy counts / blok spawn rate scale with player count.

## Risks
- Input System device pairing edge cases (controllers connecting/disconnecting mid-match).
- Performance with 4 players × effects × many bloks/enemies — watch the material-instance churn (`BUGS.md` #16) and pooling.

## Smallest first step
Fix `BUGS.md` #7, then make the player a single prefab driven by `PlayerConfiguration` and replace fixed spawns with a spawn-point list. Get 3 players working before chasing 4 + team mode.
