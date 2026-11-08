using FleetEditor;

public static partial class NativeInternalsExtensions
{
    public static PaletteItemInternals Internals(this PaletteItem target) => new(target);
}

public readonly struct PaletteItemInternals(PaletteItem target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<PaletteItem, int> CompoundingClassCount =
            AccessTools.FieldRefAccess<PaletteItem, int>("_compoundingClassCount");
        internal static readonly AccessTools.FieldRef<PaletteItem, TextMeshProUGUI> Count =
            AccessTools.FieldRefAccess<PaletteItem, TextMeshProUGUI>("_count");
        internal static readonly AccessTools.FieldRef<PaletteItem, int> CurrentCount =
            AccessTools.FieldRefAccess<PaletteItem, int>("_currentCount");
    }

    public ref int CompoundingClassCount => ref Refs.CompoundingClassCount(target);
    public ref TextMeshProUGUI Count => ref Refs.Count(target);
    public ref int CurrentCount => ref Refs.CurrentCount(target);
}