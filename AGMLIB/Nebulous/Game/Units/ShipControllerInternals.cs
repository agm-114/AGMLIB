public static partial class NativeInternalsExtensions
{
    public static ShipControllerInternals Internals(this ShipController ship) => new(ship);
}

public readonly struct ShipControllerInternals(ShipController ship)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<ShipController, List<DriveComponent>> AllDrives =
            AccessTools.FieldRefAccess<ShipController, List<DriveComponent>>("_allDrives");
        internal static readonly AccessTools.FieldRef<ShipController, List<PowerplantComponent>> AllPowerplants =
            AccessTools.FieldRefAccess<ShipController, List<PowerplantComponent>>("_allPowerplants");
        internal static readonly Action<ShipController, HullPart> DrivesWorkingChanged =
            AccessTools.MethodDelegate<Action<ShipController, HullPart>>(
                AccessTools.Method(typeof(ShipController), "HandleDrivesWorkingChanged"));
        internal static readonly Action<ShipController, HullPart> PowerplantsWorkingChanged =
            AccessTools.MethodDelegate<Action<ShipController, HullPart>>(
                AccessTools.Method(typeof(ShipController), "HandlePowerplantsWorkingChanged"));
    }

    public ref List<DriveComponent> AllDrives => ref Refs.AllDrives(ship);
    public ref List<PowerplantComponent> AllPowerplants => ref Refs.AllPowerplants(ship);

    public void RefreshWorkingState()
    {
        Refs.PowerplantsWorkingChanged(ship, null!);
        Refs.DrivesWorkingChanged(ship, null!);
    }
}
