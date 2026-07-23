# Native internals accessors

Use native-internals accessors when AGMLIB needs repeated, strongly typed access to a non-public
field on a NEBULOUS runtime type. They replace scattered `Common.GetVal` calls and local Harmony
field delegates with one verified compatibility boundary.

## Directory layout

Mirror the declaring type's namespace beneath `AGMLIB/Nebulous`, and keep one accessor file
per native type:

```text
Nebulous/
  Munitions/ModularMissiles/ModularMissileInternals.cs
  Ships/WeaponComponentInternals.cs
```

A type in `Munitions.ModularMissiles` belongs under `Munitions/ModularMissiles`; a type in `Ships`
belongs under `Ships`. Place a global-namespace native type directly in `Nebulous`.

## API pattern

Each file contributes an overload to the partial `NativeInternalsExtensions` class and returns a
readonly accessor struct:

```csharp
public static partial class NativeInternalsExtensions
{
    public static ModularMissileInternals Internals(this ModularMissile missile) => new(missile);
}

public readonly struct ModularMissileInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<ModularMissile, int> CurrentStageNumber =
            AccessTools.FieldRefAccess<ModularMissile, int>("_currentStageNumber");
    }

    private readonly ModularMissile _missile;

    internal ModularMissileInternals(ModularMissile missile)
    {
        _missile = missile ?? throw new ArgumentNullException(nameof(missile));
    }

    public ref int CurrentStageNumber => ref Refs.CurrentStageNumber(_missile);
}
```

The ref-return property supports both reads and writes without allocating:

```csharp
int stageNumber = missile.Internals().CurrentStageNumber;
missile.Internals().CurrentStageNumber = 1;
```

Accessor structs may expose a narrow typed convenience such as `CurrentStage` or
`TryGetCurrentStage`, but gameplay behavior stays in the owning feature.

## Rules

- Verify the exact declaring type, private member name, and member type against the currently loaded
  game assembly before adding a binding.
- Cache `AccessTools.FieldRef` once; do not perform field-name reflection in a frame or physics loop.
- Keep cached delegate names compact inside a nested `Refs` class; use `Stages`, not `StagesField`.
- Expose concrete types and ref-return properties so reads and writes use the same API.
- Keep raw native field names private to the accessor file.
- Fail clearly when a required binding changes after a game update. Provide a `Try...` helper only
  when an invalid runtime lifecycle state is expected and the caller should fail closed.
- Use `Common.GetVal` only for infrequent or genuinely dynamic reflection where a typed accessor
  cannot be defined.
