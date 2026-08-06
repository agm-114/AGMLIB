using HarmonyLib;
using Ships;

public static partial class NativeInternalsExtensions
{
    public static ShipInternals Internals(this Ship ship) => new(ship);
}

public readonly struct ShipInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<
            Ship,
            Dictionary<string, ResourcePool>
        > ResourcePools = AccessTools.FieldRefAccess<
            Ship,
            Dictionary<string, ResourcePool>
        >("_resources");
    }

    private readonly Ship _ship;

    internal ShipInternals(Ship ship)
    {
        _ship = ship ?? throw new ArgumentNullException(nameof(ship));
    }

    public ref Dictionary<string, ResourcePool> ResourcePools =>
        ref Refs.ResourcePools(_ship);
}
