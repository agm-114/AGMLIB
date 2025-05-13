public class InternalSectorActiveSensorComponent : BaseActiveSensorComponent
{
    [SerializeField]
    private Sector _coverage;

    protected override SensorCategory _sensorCategory => SensorCategory.GeneralSearch;

    public override bool EmissionsVisibleFromPosition(Vector3 sensorPosition)
    {
        Vector3 toSensor = base._provider.ProviderPosition.To(sensorPosition);
        Vector3 localDirection = base.transform.root.InverseTransformDirection(toSensor.normalized);
        if ((_coverage & localDirection.ClassifySector()) == 0)
        {
            return false;
        }
        return base.EmissionsVisibleFromPosition(sensorPosition);
    }

    protected override bool CanSeeSignature(IActiveSignature sig)
    {
        Vector3 toSig = base.transform.position.To(sig.Position);
        Vector3 localPosition = base.transform.root.InverseTransformDirection(toSig);
        if ((_coverage & localPosition.ClassifySector()) == 0)
        {
            return false;
        }
        return base.CanSeeSignature(sig);
    }

    protected override bool CanJammerHitApertureInternal(Vector3 jammingDirection)
    {
        Vector3 localDirection = base.transform.root.InverseTransformDirection(-jammingDirection);
        return (_coverage & localDirection.ClassifySector()) != 0;
    }

    public override bool CanTrainOnTarget(Vector3 targetPos)
    {
        Vector3 toSig = base._provider.ProviderPosition.To(targetPos);
        Vector3 toSigLocal = base.transform.InverseTransformDirection(toSig).normalized;
        return (_coverage & toSigLocal.ClassifySector()) != 0;
    }
}
