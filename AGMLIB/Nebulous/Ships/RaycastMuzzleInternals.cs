using Ships;

public static partial class NativeInternalsExtensions
{
    public static RaycastMuzzleInternals Internals(this RaycastMuzzle target) => new(target);
}

public readonly struct RaycastMuzzleInternals(RaycastMuzzle target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<RaycastMuzzle, float> BeamLength =
            AccessTools.FieldRefAccess<RaycastMuzzle, float>("_beamLength");
        internal static readonly AccessTools.FieldRef<RaycastMuzzle, BeamMuzzleEffects> Effects =
            AccessTools.FieldRefAccess<RaycastMuzzle, BeamMuzzleEffects>("_effects");
    }

    public ref float BeamLength => ref Refs.BeamLength(target);
    public ref BeamMuzzleEffects Effects => ref Refs.Effects(target);
}