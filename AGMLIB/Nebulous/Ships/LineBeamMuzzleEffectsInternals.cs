using Ships;

public static partial class NativeInternalsExtensions
{
    public static LineBeamMuzzleEffectsInternals Internals(this LineBeamMuzzleEffects target) => new(target);
}

public readonly struct LineBeamMuzzleEffectsInternals(LineBeamMuzzleEffects target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<LineBeamMuzzleEffects, LineRenderer> Beam =
            AccessTools.FieldRefAccess<LineBeamMuzzleEffects, LineRenderer>("_beam");
    }

    public ref LineRenderer Beam => ref Refs.Beam(target);
}