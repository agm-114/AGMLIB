using Game;

public static partial class NativeInternalsExtensions
{
    public static ShortDurationEffectInternals Internals(this ShortDurationEffect target) => new(target);
}

public readonly struct ShortDurationEffectInternals(ShortDurationEffect target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<ShortDurationEffect, float> Accumulator =
            AccessTools.FieldRefAccess<ShortDurationEffect, float>("_accumulator");
    }

    public ref float Accumulator => ref Refs.Accumulator(target);
}