using Munitions.ModularMissiles.Runtime.Seekers;
using Munitions.ModularMissiles.Descriptors.Seekers;
using Munitions.ModularMissiles;
using static Utility.GameColors;
using Munitions.ModularMissiles.Descriptors;

public class AdvancedModularMissile : ModularMissile
{
    public override bool SupportsVisualTargeting => true;

}

[CreateAssetMenu(fileName = "New Position Seeker", menuName = "Nebulous/Missiles/Seekers/Position")]
public class PositionSeekerDescriptor : CommandSeekerDescriptor
{
    public override bool SupportsPositionTargeting => true;
    public override bool RequiresCommunicator => false;
    public ColorName Color => ColorName.Orange;
    public bool MemoryMode = false;
    public bool MultiSensorMemoryMode = false;
    public override string GetSummarySegment()
    {
        if(MemoryMode)
            return "<color=" + GetTextColor(Color) + ">GOT</color>";
        return "<color=" + GetTextColor(Color) + ">GOLIS</color>";
    }
    public override string GetDetailSummarySegment()
    {
        if (MemoryMode)
            return "GOTO TARGET";
        return "GOTO LOCATION IN SPACE";
    }

    public override string GetFunctionalDescriptionSegment()
    {
        if (MemoryMode)
            return "predicts target postion based on last good target data";
        return "predicts target postion based on target kinematics before launch";
    }

    public override void FinalSetup(ModularMissile missile)
    {
        
        Communicator component = missile.GetComponent<Communicator>();
        if (component == null)
        {
            component = missile.GameObj.AddComponent<Communicator>();
            component.SpawnJammableTarget();
        }

        missile.AddRuntimeBehaviour<RuntimePostionSeeker>(this);
    }
}
public class RuntimePostionSeeker : RuntimeCommandSeeker
{
    private ISensorProvider _sensorProvider;
    private ITrack _targetedTrack;
    private Vector3? _startposition = null; //=> _trackTargetInitialPos ?? Vector3.zero;
    private Vector3 _startvelocity = Vector3.zero;
    private Vector3 _startaccel = Vector3.zero;
    private Vector3 PredictedPosition => (_startposition ?? _trackTargetInitialPos ?? Vector3.zero) + _startvelocity * _age;
    private Vector3 KnownPosition => _sensorProvider?.GetSensorTrack(base._trackTargetID.Value)?.KnownPosition ?? Vector3.zero;
    private Vector3 TruePosition => _sensorProvider?.GetSensorTrack(base._trackTargetID.Value)?.TruePosition ?? Vector3.zero;


    private float _age = 0f;

    public override Vector3 LocalBeamDirection => Quaternion.RotateTowards(Quaternion.identity, Quaternion.LookRotation(base.Missile.transform.InverseTransformDirection(base.Missile.transform.position.To(PredictedPosition).normalized)), 90) * Vector3.forward; //;// 
    public Vector3 TrueBeamDirection => Quaternion.RotateTowards(Quaternion.identity, Quaternion.LookRotation(base.Missile.transform.InverseTransformDirection(base.Missile.transform.position.To(KnownPosition).normalized)), 90) * Vector3.forward; //;//

    //KnownPosition - base.Missile.transform.position;


    PositionSeekerDescriptor _comdesc;

    public override void OnAdded(ModularMissile missile, MissileComponentDescriptor descriptor)
    {
        base.OnAdded(missile, descriptor);
        _comdesc = descriptor as PositionSeekerDescriptor;
    }

    public override ConeDescriptor? GetAimingCone()
    {
        return new ConeDescriptor(base.Missile.transform.position.To(KnownPosition).magnitude, Mathf.Min(Vector3.Angle(LocalBeamDirection, TrueBeamDirection), 45f), 5f);//

    }

    private void SetupTrack()
    {
        if (base.Missile == null || base.Missile.LaunchedFrom == null || _trackTargetID == null)
        {
            return;
        }
        _sensorProvider = base.Missile?.LaunchedFrom?.Sensors;
        _targetedTrack = _sensorProvider?.GetSensorTrack(base._trackTargetID.Value);
        _startvelocity = (_targetedTrack?.AbsoluteVelocity ?? Vector3.zero);// + Vector3.up * 3;
        _startaccel = _targetedTrack?.Acceleration ?? Vector3.zero;
        _startposition = _targetedTrack?.KnownPosition;
    }
    public override void SetTrackTarget(TrackIdentifier target, Vector3 initialPosition)
    {
        base.SetTrackTarget(target, initialPosition);
        SetupTrack();
    }

    public override void OnLaunched()
    {
        base.OnLaunched();
        _age = 0;
    }

    public void FixedUpdate()
    {
        _age += Time.fixedDeltaTime;


        if (_comdesc == null || !_comdesc.MemoryMode)
            return;

        List<RuntimeMissileSeeker> _validationSeekers = Common.GetVal<List<RuntimeMissileSeeker>>(Missile, "_validationSeekers") ??  new();
        List<RuntimeMissileSeeker> _targetingSeekers = Common.GetVal<List<RuntimeMissileSeeker>>(Missile, "_targetingSeekers")   ?? new();

        foreach (RuntimeMissileSeeker targetingSeeker in _targetingSeekers)
        {
            if (targetingSeeker is RuntimePostionSeeker)
                continue;

            RuntimeMissileSeeker.SeekerSearchResult seekerSearchResult = targetingSeeker.SearchForTarget(_validationSeekers, out Vector3 position, out Vector3 velocity, out Vector3 acceleration);
            if (seekerSearchResult == RuntimeMissileSeeker.SeekerSearchResult.Found)
            {
                Vector3 ClosestVector(Vector3 a, Vector3 b, Vector3 target)
                {
                    float distanceToA = Vector3.Distance(a, target);
                    float distanceToB = Vector3.Distance(b, target);

                    return distanceToA < distanceToB ? a : b;
                }

                if(!_comdesc.MultiSensorMemoryMode)
                {
                    _startvelocity = velocity;
                    _startaccel = acceleration;
                    _startposition = position;
                    _age = 0;
                    return;
                }

                // Check if C is closer to B than A

                // Check if C is closer to B than A
                _startvelocity = ClosestVector(_startvelocity, velocity, (_targetedTrack?.Trackable?.Velocity ?? Vector3.zero));
                _startaccel = ClosestVector(_startaccel, acceleration, (_targetedTrack?.Acceleration ?? Vector3.zero));
                _startposition = ClosestVector(_startaccel, position, (_targetedTrack?.Trackable?.Position ?? Vector3.zero));
                _age = 0;
                
            }
        }
        /*
        foreach (RuntimeMissileSeeker seeker in .gameObject.GetComponents<RuntimeMissileSeeker>())
        {

        }
        */
        
        //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //sphere.transform.position = CurrentPostion;
    }
    public override Vector3? InitialTargetPosition()
    {
        return base._trackTargetInitialPos;
    }
    protected override SeekerSearchResult SearchForTargetInternal(IReadOnlyCollection<RuntimeMissileSeeker> validators, out Vector3 position, out Vector3 velocity, out Vector3 acceleration)
    {
        //Debug.LogError(PredictedPosition);
        position = PredictedPosition;
        velocity = _startvelocity;
        acceleration = _startaccel;
        if (PredictedPosition == Vector3.zero)
            return SeekerSearchResult.NotFound;

        return SeekerSearchResult.Found;    
    }
}

[CreateAssetMenu(fileName = "New Position Seeker", menuName = "Nebulous/Missiles/Seekers/Range Based Command")]
public class RangedCommandSeekerDescriptor : CommandSeekerDescriptor
{
    public bool LimitedRange = false;
    public float CommsRange = float.MaxValue;
    public override bool RequiresCommunicator => false;
    public ColorName Color = ColorName.Orange;

    public override void FinalSetup(ModularMissile missile)
    {
        Communicator component = missile.GetComponent<Communicator>();
        if (component == null)
        {
            component = missile.GameObj.AddComponent<Communicator>();
            component.SpawnJammableTarget();
        }

        missile.AddRuntimeBehaviour<RuntimeRangedCommandSeeker>(this);
        
    }

    public override string GetSummarySegment()
    {
        return "<color=" + GameColors.YellowTextColor + ">CMD</color>";
    }

    public override string GetDetailSummarySegment()
    {
        return "COMMAND";
    }

    public override string GetFunctionalDescriptionSegment()
    {
        return "unjammable command guidance from its launching ship";
    }

}

public class RuntimeRangedCommandSeeker : RuntimeCommandSeeker
{
    RangedCommandSeekerDescriptor _comdesc;

    private ISensorProvider _sensorProvider => base.Missile?.LaunchedFrom?.Sensors;
    private ITrack _targetedTrack;


    public override bool CurrentlyJammed => false;
    public override void OnAdded(ModularMissile missile, MissileComponentDescriptor descriptor)
    {
        base.OnAdded(missile, descriptor);
        _comdesc = descriptor as RangedCommandSeekerDescriptor;
    }

    public override void SetTrackTarget(TrackIdentifier target, Vector3 initialPosition)
    {
        base.SetTrackTarget(target, initialPosition);
        UpdateTrack();
    }

    public override Vector3? InitialTargetPosition()
    {
        UpdateTrack();
        ITrack targetedTrack = _targetedTrack;
        return (targetedTrack != null) ? new Vector3?(targetedTrack.KnownPosition) : base._trackTargetInitialPos;
    }


    protected override SeekerSearchResult SearchForTargetInternal(IReadOnlyCollection<RuntimeMissileSeeker> validators, out Vector3 position, out Vector3 velocity, out Vector3 acceleration)
    {

        UpdateTrack();
        if (_targetedTrack != null && _targetedTrack.IsValid)
        {
            position = _targetedTrack.KnownPosition + _targetedTrack.Trackable.Rotation * Network_targetOffset;
            velocity = _targetedTrack.AbsoluteVelocity;
            acceleration = _targetedTrack.Acceleration;
            Vector3 rhs = base.transform.position.To(position);
            if (Vector3.Dot(base.transform.forward, rhs) < 0f && rhs.magnitude < _comdesc.MissTurnaroundDistance)
            {
                position = rhs.normalized * 200f;
                velocity = Vector3.zero;
            }

            return SeekerSearchResult.Found;
        }



        position = Vector3.zero;
        velocity = Vector3.zero;
        acceleration = Vector3.zero;
        return SeekerSearchResult.NotFound;
    }

    public override SeekerValidationResult ValidateTargetInternal(ISensorTrackable target)
    {

        UpdateTrack();
        if (_targetedTrack == null)
        {
            return SeekerValidationResult.NotEvaluated;
        }

        float num = Vector3.Distance(target.Position, _targetedTrack.KnownPosition);
        float num2 = Vector3.Distance(_targetedTrack.KnownPosition, _targetedTrack.Trackable.Position);
        if (num <= num2)
        {
            return SeekerValidationResult.Pass;
        }

        return SeekerValidationResult.Fail;
    }


    private void UpdateTrack()
    {
        if (_sensorProvider == null)
        {
            return;
        }

 
        if (_targetedTrack == null)
        {
            if (base._trackTargetID.HasValue)
            {
                _targetedTrack = _sensorProvider.GetSensorTrack(base._trackTargetID.Value);
            }

            if (base.isServer && _targetedTrack != null)
            {
                Network_targetOffset = (_targetedTrack.Trackable.RandomPointInBounds() ?? Vector3.zero) * 0.8f;
            }
        }
        else if (_targetedTrack.Mode is TrackingMode.Visual or TrackingMode.BearingOnly)
        {
            _targetedTrack = null;
        }
        else if (!_targetedTrack.IsValid)
        {
            if (_targetedTrack.IsSuperseded)
            {
                _targetedTrack = _targetedTrack.NewTrack;
            }
            else
            {
                _targetedTrack = null;
            }
        }
    }
}