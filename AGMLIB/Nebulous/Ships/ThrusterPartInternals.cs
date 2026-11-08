using Ships;

public static partial class NativeInternalsExtensions
{
    public static ThrusterPartInternals Internals(this ThrusterPart target) => new(target);
}

public readonly struct ThrusterPartInternals(ThrusterPart target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<ThrusterPart, bool> EffectPlaying =
            AccessTools.FieldRefAccess<ThrusterPart, bool>("_effectPlaying");
    }

    public ref bool EffectPlaying => ref Refs.EffectPlaying(target);
}