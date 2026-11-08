using Game.EWar;

public static partial class NativeInternalsExtensions
{
    public static ActiveEWarEffectInternals Internals(this ActiveEWarEffect target) => new(target);
}

public readonly struct ActiveEWarEffectInternals(ActiveEWarEffect target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<ActiveEWarEffect, SignatureType> SigType =
            AccessTools.FieldRefAccess<ActiveEWarEffect, SignatureType>("_sigType");
    }

    public ref SignatureType SigType => ref Refs.SigType(target);
}