using Factions;

public static partial class NativeInternalsExtensions
{
    public static FactionDescriptionInternals Internals(this FactionDescription target) => new(target);
}

public readonly struct FactionDescriptionInternals(FactionDescription target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<FactionDescription, string[]> SharedEquipmentList =
            AccessTools.FieldRefAccess<FactionDescription, string[]>("_sharedEquipmentList");
        internal static readonly AccessTools.FieldRef<FactionDescription, string[]> SharedEquipmentPatterns =
            AccessTools.FieldRefAccess<FactionDescription, string[]>("_sharedEquipmentPatterns");
        internal static readonly AccessTools.FieldRef<FactionDescription, string[]> SharedFactionAll =
            AccessTools.FieldRefAccess<FactionDescription, string[]>("_sharedFactionAll");
    }

    public ref string[] SharedEquipmentList => ref Refs.SharedEquipmentList(target);
    public ref string[] SharedEquipmentPatterns => ref Refs.SharedEquipmentPatterns(target);
    public ref string[] SharedFactionAll => ref Refs.SharedFactionAll(target);
}