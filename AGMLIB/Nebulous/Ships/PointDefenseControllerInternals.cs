using Ships;

public static class PointDefenseControllerInternals
{
    private static class Refs
    {
        internal static readonly Func<object, IWeapon> TurretWeapon =
            NativePrivateFields.Getter<IWeapon>(typeof(PointDefenseController), "PDTurret", "Wep");
        internal static readonly Func<object, ITrack> TargetTrack =
            NativePrivateFields.Getter<ITrack>(typeof(PointDefenseController), "PDTarget", "Track");
    }

    public static IWeapon GetTurretWeapon(object turret) => Refs.TurretWeapon(turret);
    public static ITrack GetTargetTrack(object target) => Refs.TargetTrack(target);
}
