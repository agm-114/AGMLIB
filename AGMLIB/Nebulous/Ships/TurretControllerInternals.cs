using Ships;

public static partial class NativeInternalsExtensions
{
    public static TurretControllerInternals Internals(this TurretController target) => new(target);
}

public readonly struct TurretControllerInternals(TurretController target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<TurretController, Transform> Barrel =
            AccessTools.FieldRefAccess<TurretController, Transform>("_barrel");
        internal static readonly AccessTools.FieldRef<TurretController, Transform> Body =
            AccessTools.FieldRefAccess<TurretController, Transform>("_body");
        internal static readonly AccessTools.FieldRef<TurretController, TraversalLimits?> ForwardLimits =
            AccessTools.FieldRefAccess<TurretController, TraversalLimits?>("_forwardLimits");
        internal static readonly AccessTools.FieldRef<TurretController, bool> InsideLimits =
            AccessTools.FieldRefAccess<TurretController, bool>("_insideLimits");
        internal static readonly AccessTools.FieldRef<TurretController, float> MaxElevation =
            AccessTools.FieldRefAccess<TurretController, float>("_maxElevation");
        internal static readonly AccessTools.FieldRef<TurretController, float> MinElevation =
            AccessTools.FieldRefAccess<TurretController, float>("_minElevation");
    }

    public ref Transform Barrel => ref Refs.Barrel(target);
    public ref Transform Body => ref Refs.Body(target);
    public ref TraversalLimits? ForwardLimits => ref Refs.ForwardLimits(target);
    public ref bool InsideLimits => ref Refs.InsideLimits(target);
    public ref float MaxElevation => ref Refs.MaxElevation(target);
    public ref float MinElevation => ref Refs.MinElevation(target);
}