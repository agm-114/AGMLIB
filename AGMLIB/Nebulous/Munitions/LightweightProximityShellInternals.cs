using Munitions;

public static partial class NativeInternalsExtensions
{
    public static LightweightProximityShellInternals Internals(this LightweightProximityShell target) => new(target);
}

public readonly struct LightweightProximityShellInternals(LightweightProximityShell target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<LightweightProximityShell, float> TriggerRadius =
            AccessTools.FieldRefAccess<LightweightProximityShell, float>("_triggerRadius");
    }

    public ref float TriggerRadius => ref Refs.TriggerRadius(target);
}