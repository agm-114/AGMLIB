using Ships;

public static partial class NativeInternalsExtensions
{
    public static BaseTubeLauncherComponentInternals Internals(this BaseTubeLauncherComponent target) => new(target);
}

public readonly struct BaseTubeLauncherComponentInternals(BaseTubeLauncherComponent target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<BaseTubeLauncherComponent, int> CurrentShot =
            AccessTools.FieldRefAccess<BaseTubeLauncherComponent, int>("_currentShot");
        internal static readonly Func<BaseTubeLauncherComponent, float> GetCycleLength =
            AccessTools.MethodDelegate<Func<BaseTubeLauncherComponent, float>>(
                AccessTools.PropertyGetter(typeof(BaseTubeLauncherComponent), "_cycleLength")
                ?? throw new MissingMemberException(typeof(BaseTubeLauncherComponent).FullName, "_cycleLength"));
        internal static readonly Func<BaseTubeLauncherComponent, bool> GetIsCyclingNextShot =
            AccessTools.MethodDelegate<Func<BaseTubeLauncherComponent, bool>>(
                AccessTools.PropertyGetter(typeof(BaseTubeLauncherComponent), "_isCyclingNextShot")
                ?? throw new MissingMemberException(typeof(BaseTubeLauncherComponent).FullName, "_isCyclingNextShot"));
        internal static readonly Func<BaseTubeLauncherComponent, bool> GetIsReloading =
            AccessTools.MethodDelegate<Func<BaseTubeLauncherComponent, bool>>(
                AccessTools.PropertyGetter(typeof(BaseTubeLauncherComponent), "_isReloading")
                ?? throw new MissingMemberException(typeof(BaseTubeLauncherComponent).FullName, "_isReloading"));
        internal static readonly Action<BaseTubeLauncherComponent, bool> SetIsReloading =
            AccessTools.MethodDelegate<Action<BaseTubeLauncherComponent, bool>>(
                AccessTools.PropertySetter(typeof(BaseTubeLauncherComponent), "_isReloading")
                ?? throw new MissingMemberException(typeof(BaseTubeLauncherComponent).FullName, "_isReloading"));
        internal static readonly AccessTools.FieldRef<BaseTubeLauncherComponent, float> NextShotTime =
            AccessTools.FieldRefAccess<BaseTubeLauncherComponent, float>("_nextShotTime");
        internal static readonly AccessTools.FieldRef<BaseTubeLauncherComponent, LauncherProgrammingQueue> PendingLaunches =
            AccessTools.FieldRefAccess<BaseTubeLauncherComponent, LauncherProgrammingQueue>("_pendingLaunches");
    }

    public ref int CurrentShot => ref Refs.CurrentShot(target);
    public float CycleLength => Refs.GetCycleLength(target);
    public bool IsCyclingNextShot => Refs.GetIsCyclingNextShot(target);
    public bool IsReloading { get => Refs.GetIsReloading(target); set => Refs.SetIsReloading(target, value); }
    public ref float NextShotTime => ref Refs.NextShotTime(target);
    public ref LauncherProgrammingQueue PendingLaunches => ref Refs.PendingLaunches(target);
}