public class TurretedInternalActiveSensorComponent : BaseActiveSensorComponent
{
    [SerializeField]
    private TurretController _turretControl;

    protected override SensorCategory _sensorCategory => SensorCategory.GeneralSearch;
    protected override void SocketSet()
    {
        base.SocketSet();
        _turretControl.SetLimits(base.Socket.TraverseLimits, base.Socket.ForwardTraverseLimits);
    }
    public override bool EmissionsVisibleFromPosition(Vector3 sensorPosition) => _turretControl.TargetWithinLimits(sensorPosition);
    public override bool CanTrainOnTarget(Vector3 targetPos) => _turretControl.TargetWithinLimits(targetPos);
    protected override bool CanJammerHitApertureInternal(Vector3 jammingDirection) => _turretControl.TargetWithinLimits(base.transform.root.TransformDirection(-jammingDirection));
    protected override bool CanSeeSignature(IActiveSignature sig)
    {
        if (!_turretControl.TargetWithinLimits(sig.Position))
        {
            return false;
        }
        return base.CanSeeSignature(sig);
    }

}