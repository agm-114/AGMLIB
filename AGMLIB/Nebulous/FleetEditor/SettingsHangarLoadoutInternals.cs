public static partial class NativeInternalsExtensions
{
    public static SettingsHangarLoadoutInternals Internals(this SettingsHangarLoadout panel) => new(panel);
}

public readonly struct SettingsHangarLoadoutInternals(SettingsHangarLoadout panel)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<SettingsHangarLoadout, BulkCraftHangarComponent?> Hangar =
            AccessTools.FieldRefAccess<SettingsHangarLoadout, BulkCraftHangarComponent?>("_hangar");
        internal static readonly AccessTools.FieldRef<SettingsHangarLoadout, Image> CapacityGauge =
            AccessTools.FieldRefAccess<SettingsHangarLoadout, Image>("_gaugeImage");
        internal static readonly Action<SettingsHangarLoadout> RefreshQuantities =
            AccessTools.MethodDelegate<Action<SettingsHangarLoadout>>(AccessTools.Method(typeof(SettingsHangarLoadout), "UpdateQuantities"));
    }

    public BulkCraftHangarComponent? Hangar => Refs.Hangar(panel);
    public Image CapacityGauge => Refs.CapacityGauge(panel);
    public void RefreshQuantities() => Refs.RefreshQuantities(panel);
}
