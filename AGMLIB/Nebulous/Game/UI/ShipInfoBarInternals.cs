using Game.UI;

public static partial class NativeInternalsExtensions
{
    public static ShipInfoBarInternals Internals(this ShipInfoBar target) => new(target);
}

public readonly struct ShipInfoBarInternals(ShipInfoBar target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<ShipInfoBar, ShipController> PrimaryShip =
            AccessTools.FieldRefAccess<ShipInfoBar, ShipController>("_primaryShip");
        internal static readonly AccessTools.FieldRef<ShipInfoBar, List<ShipController>> ShipGroup =
            AccessTools.FieldRefAccess<ShipInfoBar, List<ShipController>>("_shipGroup");
    }

    public ref ShipController PrimaryShip => ref Refs.PrimaryShip(target);
    public ref List<ShipController> ShipGroup => ref Refs.ShipGroup(target);
}