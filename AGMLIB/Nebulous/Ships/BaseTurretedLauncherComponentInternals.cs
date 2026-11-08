using Ships;

public static partial class NativeInternalsExtensions
{
    public static BaseTurretedLauncherComponentInternals Internals(this BaseTurretedLauncherComponent target) => new(target);
}

public readonly struct BaseTurretedLauncherComponentInternals(BaseTurretedLauncherComponent target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<BaseTurretedLauncherComponent, Vector3?> LastAimPoint =
            AccessTools.FieldRefAccess<BaseTurretedLauncherComponent, Vector3?>("_lastAimPoint");
        internal static readonly AccessTools.FieldRef<BaseTurretedLauncherComponent, float> MissileFlightSpeed =
            AccessTools.FieldRefAccess<BaseTurretedLauncherComponent, float>("_missileFlightSpeed");
        internal static readonly AccessTools.FieldRef<BaseTurretedLauncherComponent, StatValue> StatElevationRate =
            AccessTools.FieldRefAccess<BaseTurretedLauncherComponent, StatValue>("_statElevationRate");
        internal static readonly AccessTools.FieldRef<BaseTurretedLauncherComponent, StatValue> StatTraverseRate =
            AccessTools.FieldRefAccess<BaseTurretedLauncherComponent, StatValue>("_statTraverseRate");
        internal static readonly AccessTools.FieldRef<BaseTurretedLauncherComponent, Vector3> TrackOffset =
            AccessTools.FieldRefAccess<BaseTurretedLauncherComponent, Vector3>("_trackOffset");
        internal static readonly AccessTools.FieldRef<BaseTurretedLauncherComponent, Vector3?> TrainOnPosition =
            AccessTools.FieldRefAccess<BaseTurretedLauncherComponent, Vector3?>("_trainOnPosition");
        internal static readonly AccessTools.FieldRef<BaseTurretedLauncherComponent, ITrack> TrainOnTrack =
            AccessTools.FieldRefAccess<BaseTurretedLauncherComponent, ITrack>("_trainOnTrack");
        internal static readonly AccessTools.FieldRef<BaseTurretedLauncherComponent, TurretController> TurretControl =
            AccessTools.FieldRefAccess<BaseTurretedLauncherComponent, TurretController>("_turretControl");
    }

    public ref Vector3? LastAimPoint => ref Refs.LastAimPoint(target);
    public ref float MissileFlightSpeed => ref Refs.MissileFlightSpeed(target);
    public ref StatValue StatElevationRate => ref Refs.StatElevationRate(target);
    public ref StatValue StatTraverseRate => ref Refs.StatTraverseRate(target);
    public ref Vector3 TrackOffset => ref Refs.TrackOffset(target);
    public ref Vector3? TrainOnPosition => ref Refs.TrainOnPosition(target);
    public ref ITrack TrainOnTrack => ref Refs.TrainOnTrack(target);
    public ref TurretController TurretControl => ref Refs.TurretControl(target);
}