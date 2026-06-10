# Events

The **ScriptableObject event bus** — the primary decoupling mechanism between modules. Prefer raising/listening to an existing event SO over a hard cross-module reference.

## Key files

### Parameterized (current)
- `ParameterizedEvents/BaseGameEvent.cs` — abstract `ScriptableObject` with a C# `Action<T>` (`EventListeners`) + `Raise(T)`.
- `ParameterizedEvents/BaseGameEventListener.cs` — MonoBehaviour that subscribes a `UnityEvent<T>` to a `BaseGameEvent<T>`.
- Concrete instances — `Int/` (`IntEvent`), `Void/` (`VoidEvent`), `SpawnBlokHealth/` (`SpawnBlokReportingEvent` + reporter + listener).
- `ITrackableEventObject.cs` — marker for trackable event payloads.

### Legacy (UnityEvent-based, predates the above)
- `GameEvent/` `LegacyGameEvent` + listener, `GameEventWithGameObject/`, `GameEventWithVector3/`.

## Conventions

- Event **assets** live in `Assets/ScriptableObjects/Events/`, not here.
- `BaseGameEvent.EventListeners` is initialized to `delegate {}` so `Raise` never null-checks.

## Used by

- Nearly every context. This context depends on almost nothing.

## Language

> Stub — sharpen via grill-with-docs.

**Game Event (SO)**: A ScriptableObject channel; raisers call `Raise`, listeners subscribe. Decouples sender from receiver.

**Listener**: A MonoBehaviour that forwards a `Game Event` to a `UnityEvent` wired in the inspector.

**Legacy vs Parameterized**: Prefer the parameterized `BaseGameEvent<T>` family; `Legacy*` is kept for older wiring.
