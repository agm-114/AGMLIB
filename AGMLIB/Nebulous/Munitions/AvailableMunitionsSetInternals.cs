using Munitions;

public static partial class NativeInternalsExtensions
{
    public static AvailableMunitionsSetInternals Internals(this AvailableMunitionsSet target) => new(target);
}

public readonly struct AvailableMunitionsSetInternals(AvailableMunitionsSet target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<AvailableMunitionsSet, List<IMunition>> AllMunitions =
            AccessTools.FieldRefAccess<AvailableMunitionsSet, List<IMunition>>("_allMunitions");
        internal static readonly AccessTools.FieldRef<AvailableMunitionsSet, Dictionary<Guid, IMunition>> AllMunitionsByNetKey =
            AccessTools.FieldRefAccess<AvailableMunitionsSet, Dictionary<Guid, IMunition>>("_allMunitionsByNetKey");
    }

    public ref List<IMunition> AllMunitions => ref Refs.AllMunitions(target);
    public ref Dictionary<Guid, IMunition> AllMunitionsByNetKey => ref Refs.AllMunitionsByNetKey(target);
}