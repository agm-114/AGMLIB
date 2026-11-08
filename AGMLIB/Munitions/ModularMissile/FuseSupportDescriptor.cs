using Munitions.ModularMissiles;
using Munitions.ModularMissiles.Descriptors;
using Munitions.ModularMissiles.Descriptors.Support;
using Munitions.ModularMissiles.Descriptors.Warheads;
using Munitions.ModularMissiles.Runtime;
using System.Xml;

public interface IFuse
{
    //public Vector3 FuseDimensions { get; }
}

[CreateAssetMenu(fileName = "New Missile Fuse", menuName = "Nebulous/Missiles/Support/Generic Fuse")]
public class FuseSupportDescriptor : BaseSupportDescriptor, IFuse
{
    public MissileSocketType SocketType = MissileSocketType.Support;
    public Vector3 FuseDimensions = Vector3.one;
    public bool OnlyFuseOnTarget = false;
    public bool RequireFoundSeeker = false;
    public bool FilterTargetClassification = true;
    public override MissileSocketType FitsSocketType => SocketType;

    public override void FinalSetup(ModularMissile missile)
    {
        base.FinalSetup(missile);
        missile.AddRuntimeBehaviour<RuntimeMissileFuse>(this);
        missile.SpawnProximityFuze(FuseDimensions);
    }
}

[CreateAssetMenu(fileName = "New Missile Fuse", menuName = "Nebulous/Missiles/Support/Warhead Fuse")]
public class FuseWarheadDescriptor : BaseWarheadDescriptor, IFuse
{
    public MissileSocketType SocketType = MissileSocketType.Support;
    public Vector3 FuseDimensions = Vector3.one;
    public bool OnlyFuseOnTarget = false;
    public bool RequireFoundSeeker = false;
    public bool FilterTargetClassification = true;
    public WeaponEffectSet Effects;

    public override MissileSocketType FitsSocketType => SocketType;

    public override void FinalSetup(ModularMissile missile)
    {
        base.FinalSetup(missile);
        missile.AddRuntimeBehaviour<RuntimeMissileFuse>(this);
        missile.SpawnProximityFuze(FuseDimensions);
    }

    public float BlastRadius => 0;
    public override float ArmorPenetration => 0;
    public override float ComponentDamage => 0;
    public override float TotalComponentDamagePotential => ComponentDamage;
    protected override bool _showFractionalDamageInStats => true;
    public override WeaponEffectSet GetEffectSet(int setIndex) => Effects;
    public override string GetSummarySegment() => "FUSE";
    public override string GetDetailSummarySegment() => "Proximity Fuze";
    public override void GetWarheadStatsBlock(ref List<(string, string)> rows)
    {
        base.GetWarheadStatsBlock(ref rows);
        rows.Add(("Blast Radius", $"{BlastRadius * 10f:N0} m"));
    }
    public override HitResult CollisionDetonate(RuntimeMissileWarhead runtime, IDamageable hitObject, MunitionHitInfo hitInfo) => HitResult.Stopped;
    public override HitResult TriggerDetonate(RuntimeMissileWarhead runtime, IDamageable hitObject, MunitionHitInfo hitInfo, out bool noVfx)
    {
        noVfx = false;
        return HitResult.Stopped;
    }
}

public class RuntimeMissileFuse : RuntimeMissileBehaviour
{
    [SerializeField]
    private bool _onlyFuseOnTarget;
    [SerializeField]
    private bool _requireFoundSeeker;
    [SerializeField]
    private bool _filterTargetClassification = true;

    private TrackIdentifier? _launchTargetId;

    public override void OnAdded(ModularMissile missile, MissileComponentDescriptor descriptor)
    {
        base.OnAdded(missile, descriptor);
        (_onlyFuseOnTarget, _requireFoundSeeker, _filterTargetClassification) = descriptor switch
        {
            FuseSupportDescriptor support => (support.OnlyFuseOnTarget, support.RequireFoundSeeker, support.FilterTargetClassification),
            FuseWarheadDescriptor warhead => (warhead.OnlyFuseOnTarget, warhead.RequireFoundSeeker, warhead.FilterTargetClassification),
            _ => throw new ArgumentException("Expected a fuse descriptor.", nameof(descriptor))
        };
    }

    internal void SetLaunchTarget(TrackIdentifier trackId) => _launchTargetId ??= trackId;

    private ISensorTrackable? Target => Missile.CurrentTarget ??
        (_launchTargetId is { } trackId ? Missile.LaunchedFrom?.Sensors?.GetSensorTrack(trackId)?.Trackable : null);

    internal bool CanFuzeOnTarget(MunitionHitInfo hitInfo, bool trigger)
    {
        if (!trigger)
            return true;

        // Native guidance publishes a selected seeker for Found/Jammed and clears it for NotFound.
        if (_requireFoundSeeker && (Missile.CurrentTargetSeeker == null || Missile.SeekersJammed))
            return false;

        ISensorTrackable? target = Target;
        ISensorTrackable? component = hitInfo.HitObject?.transform.root.GetComponent<ISensorTrackable>();
        if (_onlyFuseOnTarget)
            return target != null && target == component;

        return !_filterTargetClassification || target == null || component == null || target.ContactType == ContactClassification.Unknown ||
            target.ContactType == component.ContactType ||
            component.ContactType is not (ContactClassification.SmallCraft or ContactClassification.Missile);
    }

    public override void OnCloned() { base.OnCloned(); _launchTargetId = null; }
    public override void OnUnpooled() { base.OnUnpooled(); _launchTargetId = null; }
    public override void OnRepooled() { base.OnRepooled(); _launchTargetId = null; }

    protected override void WriteSaveStateInternal(XmlElement self) => _launchTargetId.AppendToDocument(self, "LaunchTarget");

    protected override void LoadSaveStateInternal(XmlElement self) =>
        _launchTargetId = self.ReadFromDocumentParent<TrackIdentifier?>("LaunchTarget");
}

[HarmonyPatch(typeof(ModularMissile), nameof(ModularMissile.ProcessCollision))]
internal static class RuntimeMissileFuseCheckPatch
{
    private static bool Prefix(ModularMissile __instance, MunitionHitInfo hitInfo, bool trigger)
    {
        if (!__instance.isServer || !trigger)
            return true;

        foreach (RuntimeMissileFuse fuse in __instance.GetComponents<RuntimeMissileFuse>())
        {
            if (!fuse.CanFuzeOnTarget(hitInfo, trigger))
                return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(ModularMissile), "__DoRpcSetProgramTrack")]
internal static class RuntimeMissileFuseLaunchTargetPatch
{
    private static void Postfix(ModularMissile __instance, TrackIdentifier trackId)
    {
        foreach (RuntimeMissileFuse fuse in __instance.GetComponents<RuntimeMissileFuse>())
            fuse.SetLaunchTarget(trackId);
    }
}
