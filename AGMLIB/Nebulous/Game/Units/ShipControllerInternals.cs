using Game.Orders.Tasks;

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
            AccessTools.MethodDelegate<Action<ShipController, HullPart>>(AccessTools.Method(typeof(ShipController), "HandleDrivesWorkingChanged"));
        internal static readonly Action<ShipController, HullPart> PowerplantsWorkingChanged =
            AccessTools.MethodDelegate<Action<ShipController, HullPart>>(AccessTools.Method(typeof(ShipController), "HandlePowerplantsWorkingChanged"));
        internal static readonly Action<ShipController, HullPart> CommandWorkingChanged =
            AccessTools.MethodDelegate<Action<ShipController, HullPart>>(
                AccessTools.Method(typeof(ShipController), "HandleCommandWorkingChanged", [typeof(HullPart)])
                ?? throw new MissingMethodException(typeof(ShipController).FullName, "HandleCommandWorkingChanged"));
        internal static readonly AccessTools.FieldRef<ShipController, List<CommandComponent>> AllCommand =
            AccessTools.FieldRefAccess<ShipController, List<CommandComponent>>("_allCommand");
        internal static readonly AccessTools.FieldRef<ShipController, List<IWeapon>> AllWeapons =
            AccessTools.FieldRefAccess<ShipController, List<IWeapon>>("_allWeapons");
        internal static readonly AccessTools.FieldRef<ShipController, bool> BattleShortEnabled =
            AccessTools.FieldRefAccess<ShipController, bool>("_battleShortEnabled");
        internal static readonly AccessTools.FieldRef<ShipController, NavigationTask> NavOrder =
            AccessTools.FieldRefAccess<ShipController, NavigationTask>("_navOrder");
        internal static readonly AccessTools.FieldRef<ShipController, PointDefenseController> PdwController =
            AccessTools.FieldRefAccess<ShipController, PointDefenseController>("_pdwController");
        internal static readonly AccessTools.FieldRef<ShipController, Rigidbody> Rigidbody =
            AccessTools.FieldRefAccess<ShipController, Rigidbody>("_rigidbody");
        internal static readonly AccessTools.FieldRef<ShipController, List<ISensorComponent>> Sensors =
            AccessTools.FieldRefAccess<ShipController, List<ISensorComponent>>("_sensors");
        internal static readonly AccessTools.FieldRef<ShipController, WeaponsControlStatus> WeaponsControl =
            AccessTools.FieldRefAccess<ShipController, WeaponsControlStatus>("_weaponsControl");
    }

    public ref List<DriveComponent> AllDrives => ref Refs.AllDrives(ship);
    public ref List<PowerplantComponent> AllPowerplants => ref Refs.AllPowerplants(ship);

    public void RefreshWorkingState()
    {
        Refs.PowerplantsWorkingChanged(ship, null!);
        Refs.DrivesWorkingChanged(ship, null!);
    }

    public ref List<CommandComponent> AllCommand => ref Refs.AllCommand(ship);
    public ref List<IWeapon> AllWeapons => ref Refs.AllWeapons(ship);
    public ref bool BattleShortEnabled => ref Refs.BattleShortEnabled(ship);
    public ref NavigationTask NavOrder => ref Refs.NavOrder(ship);
    public ref PointDefenseController PdwController => ref Refs.PdwController(ship);
    public ref Rigidbody Rigidbody => ref Refs.Rigidbody(ship);
    public ref List<ISensorComponent> Sensors => ref Refs.Sensors(ship);
    public ref WeaponsControlStatus WeaponsControl => ref Refs.WeaponsControl(ship);

    public void RefreshCommandState() => Refs.CommandWorkingChanged(ship, null!);
}