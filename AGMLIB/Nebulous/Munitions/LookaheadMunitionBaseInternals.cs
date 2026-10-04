using Game.Reports;
using HarmonyLib;
using Mirror;
using Munitions;

public static partial class NativeInternalsExtensions
{
    public static LookaheadMunitionBaseInternals Internals(this LookaheadMunitionBase munition) => new(munition);
}

public readonly struct LookaheadMunitionBaseInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<LookaheadMunitionBase, IImbuedObjectSource> LocalLaunchedFrom =
            AccessTools.FieldRefAccess<LookaheadMunitionBase, IImbuedObjectSource>("_localLaunchedFrom");
        internal static readonly Func<LookaheadMunitionBase, IWeaponStatReportReceiver> ReportTo =
            AccessTools.MethodDelegate<Func<LookaheadMunitionBase, IWeaponStatReportReceiver>>(
                AccessTools.PropertyGetter(typeof(LookaheadMunitionBase), "_reportTo")
                ?? throw new MissingMemberException(typeof(LookaheadMunitionBase).FullName, "_reportTo"));
        internal static readonly Action<LookaheadMunitionBase, HitResult, int, int, Vector3, Quaternion, NetworkIdentity> RollImpact =
            AccessTools.MethodDelegate<Action<LookaheadMunitionBase, HitResult, int, int, Vector3, Quaternion, NetworkIdentity>>(
                AccessTools.Method(typeof(LookaheadMunitionBase), "RpcRollImpactEffect",
                    [typeof(HitResult), typeof(int), typeof(int), typeof(Vector3), typeof(Quaternion), typeof(NetworkIdentity)])
                ?? throw new MissingMethodException(typeof(LookaheadMunitionBase).FullName, "RpcRollImpactEffect"));
    }

    private readonly LookaheadMunitionBase _munition;
    internal LookaheadMunitionBaseInternals(LookaheadMunitionBase munition) =>
        _munition = munition ?? throw new ArgumentNullException(nameof(munition));
    public ref IImbuedObjectSource LocalLaunchedFrom => ref Refs.LocalLaunchedFrom(_munition);
    public IWeaponStatReportReceiver ReportTo => Refs.ReportTo(_munition);
    public void RpcRollImpactEffect(HitResult result, int effectSet, int surfaceSetIndex,
        Vector3 position, Quaternion rotation, NetworkIdentity attachTo) =>
        Refs.RollImpact(_munition, result, effectSet, surfaceSetIndex, position, rotation, attachTo);
}
