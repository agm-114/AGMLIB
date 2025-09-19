using Munitions.ModularMissiles;
using Munitions.ModularMissiles.Descriptors.Support;
using Munitions.ModularMissiles.Descriptors.Warheads;
using Munitions.ModularMissiles.Runtime;
public interface IFuse
{
    //public Vector3 FuseDimensions { get; }
}

[CreateAssetMenu(fileName = "New Missile Fuse", menuName = "Nebulous/Missiles/Support/Generic Fuse")]
public class FuseSupportDescriptor : BaseSupportDescriptor, IFuse
{

    public MissileSocketType SocketType = MissileSocketType.Support;
    public Vector3 FuseDimensions = Vector3.one;
    public override MissileSocketType FitsSocketType => SocketType;

    private ContactClassification _targetType = ContactClassification.Unknown;


    public override void FinalSetup(ModularMissile missile)
    {
        base.FinalSetup(missile);
        missile.SpawnProximityFuze(FuseDimensions);//Fusedimensions * 10 * (AoeRadius / 10) 100 
    }

}

[CreateAssetMenu(fileName = "New Missile Fuse", menuName = "Nebulous/Missiles/Support/Warhead Fuse")]
public class FuseWarheadDescriptor : BaseWarheadDescriptor
{
    public MissileSocketType SocketType = MissileSocketType.Support;
    public Vector3 FuseDimensions = Vector3.one;
    public override MissileSocketType FitsSocketType => SocketType;

    private ContactClassification _targetType = ContactClassification.Unknown;

    public WeaponEffectSet Effects;

    public override void FinalSetup(ModularMissile missile)
    {
        base.FinalSetup(missile);
        missile.SpawnProximityFuze(FuseDimensions);//Fusedimensions * 10 * (AoeRadius / 10) 100 
    }
    public override void SetTrackTarget(ModularMissile missile, TrackIdentifier trackId)
    {
        base.SetTrackTarget(missile, trackId);
        if (missile.LaunchedFrom != null && missile.LaunchedFrom.Sensors != null)
        {
            ITrack sensorTrack = missile.LaunchedFrom.Sensors.GetSensorTrack(trackId);
            if (sensorTrack != null)
            {
                _targetType = sensorTrack.Trackable.ContactType;
            }
        }
    }

    public override void ResetWarhead()
    {
        base.ResetWarhead();
        _targetType = ContactClassification.Unknown;
    }

    public override bool ShouldFuzeOnTarget(MunitionHitInfo hitInfo, bool trigger)
    {
        if (!trigger)
        {
            return true;
        }

        if (_targetType != 0)
        {
            ISensorTrackable component = hitInfo.HitObject.transform.root.GetComponent<ISensorTrackable>();
            if (component != null && (component.ContactType == ContactClassification.SmallCraft || component.ContactType == ContactClassification.Missile) && _targetType != component.ContactType)
            {
                return false;
            }
        }

        return true;
    }

    public float BlastRadius => 0;
    public override float ArmorPenetration => 0;
    public override float ComponentDamage => 0;
    public override float TotalComponentDamagePotential => ComponentDamage;
    protected override bool _showFractionalDamageInStats => true;

    public override WeaponEffectSet GetEffectSet(int setIndex)
    {
        return Effects;
    }

    public override string GetSummarySegment()
    {
        return "FUSE";
    }

    public override string GetDetailSummarySegment()
    {
        return "Proximity Fuze";
    }

    public override void GetWarheadStatsBlock(ref List<(string, string)> rows)
    {
        base.GetWarheadStatsBlock(ref rows);
        rows.Add(("Blast Radius", $"{BlastRadius * 10f:N0} m"));
    }

    public override HitResult CollisionDetonate(RuntimeMissileWarhead runtime, IDamageable hitObject, MunitionHitInfo hitInfo)
    {
        return HitResult.Stopped;
    }

    public override HitResult TriggerDetonate(RuntimeMissileWarhead runtime, IDamageable hitObject, MunitionHitInfo hitInfo, out bool noVfx)
    {
        noVfx = false;
        return HitResult.Stopped;
    }

}