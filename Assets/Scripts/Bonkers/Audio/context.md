# Audio

ScriptableObject-based audio events so designers can author randomized one-shots without code.

## Key files

- `Runtime/AudioEvents/AudioEvent.cs` — abstract SO audio event (`Play(AudioSource)`).
- `Runtime/AudioEvents/SimpleAudioEvent.cs` — randomized clip + volume/pitch range.
- `Runtime/AudioComponents/BlokAudio.cs` — plays blok bonk/break audio events.

## Depends on / Used by

- Uses **Helpers** (`RangedFloat`/`MinMaxRange`); used by **BlokControl**, **Combat**.

## Language

> Stub.

**Audio Event (SO)**: A reusable, randomizable sound definition played against an `AudioSource`.
