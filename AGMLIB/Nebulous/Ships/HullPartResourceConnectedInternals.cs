using NativePriority = Ships.Priority;
using HarmonyLib;
using Ships;

public static partial class NativeInternalsExtensions
{
    public static HullPartResourceConnectedInternals Internals(this HullPartResourceConnected component) => new(component);
}

public readonly struct HullPartResourceConnectedInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<HullPartResourceConnected, ResourceValue[]> RequiredResourceValues =
            AccessTools.FieldRefAccess<HullPartResourceConnected, ResourceValue[]>("_requiredResources");
        internal static readonly AccessTools.FieldRef<HullPartResourceConnected, NativePriority> ResourceDemandPriority =
            AccessTools.FieldRefAccess<HullPartResourceConnected, NativePriority>("_resourceDemandPriority");
    }

    private readonly HullPartResourceConnected _component;
    internal HullPartResourceConnectedInternals(HullPartResourceConnected component)
    {
        _component = component ?? throw new ArgumentNullException(nameof(component));
    }

    public ref ResourceValue[] RequiredResourceValues => ref Refs.RequiredResourceValues(_component);
    public ref NativePriority ResourceDemandPriority => ref Refs.ResourceDemandPriority(_component);
}