using Munitions;

public static partial class NativeInternalsExtensions
{
    public static LightweightClusterShellInternals Internals(this LightweightClusterShell target) => new(target);
}

public readonly struct LightweightClusterShellInternals(LightweightClusterShell target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<LightweightClusterShell, float> LookaheadSphereRadius =
            AccessTools.FieldRefAccess<LightweightClusterShell, float>("_lookaheadSphereRadius");
    }

    public ref float LookaheadSphereRadius => ref Refs.LookaheadSphereRadius(target);
}