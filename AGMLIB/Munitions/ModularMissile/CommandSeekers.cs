using Game.UI.Chessboard;
using Munitions.ModularMissiles;
using Munitions.ModularMissiles.Descriptors;
using Munitions.ModularMissiles.Descriptors.Seekers;
using Munitions.ModularMissiles.Runtime.Seekers;
using Shapes;
using UnityEngine.Rendering.HighDefinition;
using static PositionSeekerDescriptor;
using static Utility.GameColors;




public class AdvancedModularMissile : ModularMissile
{
    public override bool SupportsVisualTargeting => true;

}
public struct DebugLine
{
    public Vector3 Start;
    public Vector3 End;
    public Color Color;
    public float Thickness = 0.5f; // Default to 0.5f in logic if 0

    public DebugLine()
    {
    }
}

public interface IDebugableMissileSeeker
{
    List<DebugLine> GetDebugLines();
}
[CreateAssetMenu(fileName = "New Position Seeker", menuName = "Nebulous/Missiles/Seekers/Position")]
public class PositionSeekerDescriptor : CommandSeekerDescriptor
{
    public override bool SupportsPositionTargeting => true;
    public override bool RequiresCommunicator => false;
    public ColorName Color => ColorName.Orange;
    public enum SensorUpdateMode
    {
        None,
        FirstSource,
        BestSource,
        WorstSource,
        Average
    }
    public enum SeekerMode
    {
        TrueCenter,
        SensorCenter,
        LaunchPlatform,
        None,
    }

    [Header("Launch Settings")]
    public SeekerMode PrimarySource = SeekerMode.SensorCenter;
    public bool PrimaryUseBounds = true;
    public bool PrimaryUseSphereNoise = false;
    public SeekerMode SecondarySource = SeekerMode.TrueCenter;
    public bool SecondaryUseBounds = true;
    public bool SecondaryUseSphereNoise = false;
    public SensorUpdateMode SourceSelection = SensorUpdateMode.Average;
    public float RandomScale = 1f;

    [Header("Inflight Settings")]
    public SensorUpdateMode SeekerUpdateMode = SensorUpdateMode.BestSource;

    public override string GetSummarySegment()
    {
        if (SeekerUpdateMode == SensorUpdateMode.None)
            return "<color=" + GameColors.GetTextColor(Color) + ">GOLIS</color>";
        return "<color=" + GameColors.GetTextColor(Color) + ">GOT</color>";
        
    }
    public override string GetDetailSummarySegment()
    {
        if (SeekerUpdateMode == SensorUpdateMode.None)
            return "GOTO LOCATION IN SPACE";
        return "GOTO TARGET";
        
    }

    public override string GetFunctionalDescriptionSegment()
    {
        if (SeekerUpdateMode == SensorUpdateMode.None)
            return "predicts target postion based on target kinematics before launch";
        return "predicts target postion based on last good target data";
        
    }

    public override void FinalSetup(ModularMissile missile)
    {

        Communicator component = missile.GetComponent<Communicator>();
        if (component == null)
        {
            component = missile.GameObj.AddComponent<Communicator>();
        }
        missile.AddRuntimeBehaviour<RuntimePostionSeeker>(this);
    }

    public override bool DuplicatesFunctionality(BaseSeekerDescriptor other)
    {
        return other is PositionSeekerDescriptor;
    }
}
public class TimedDestroyer : MonoBehaviour
{
    public float duration = 1.0f;

    void Start()
    {
        // Unity's built-in way to queue a destruction
        Destroy(gameObject, duration);
    }
}
public class RuntimePostionSeeker : RuntimeCommandSeeker, IDebugableMissileSeeker
{
    private ISensorProvider SensorProvider => Missile?.LaunchedFrom?.Sensors;
    private ITrack? _cachedTrack;
    private ITrack? TargetedTrack
    {
        get
        {
            if (_cachedTrack == null && _trackTargetID.HasValue && SensorProvider != null)
            {
                _cachedTrack = SensorProvider.GetSensorTrack(_trackTargetID.Value);
            }

            return _cachedTrack;
        }
    }
    private Vector3? _startposition = null; //=> _trackTargetInitialPos ?? Vector3.zero;
    private Vector3 _primarypos;
    private Vector3 _secondarypos;
    private Vector3 _startvelocity = Vector3.zero;
    private Vector3 _startaccel = Vector3.zero;
    private Vector3 PredictedPosition => (_startposition ?? _trackTargetInitialPos ?? Vector3.zero) + _startvelocity * _age;
    private Vector3 KnownPosition => TargetedTrack?.KnownPosition ?? Vector3.zero;
    private Vector3 TruePosition => TargetedTrack?.TruePosition ?? Vector3.zero;

    public Transform Transform => base.Missile.transform;

    private float _age = 0f;

    public override Vector3 LocalBeamDirection => Quaternion.RotateTowards(Quaternion.identity, Quaternion.LookRotation(Transform.InverseTransformDirection(Transform.position.To(PredictedPosition).normalized)), 90) * Vector3.forward; //;// 
    public Vector3 TrueBeamDirection => Quaternion.RotateTowards(Quaternion.identity, Quaternion.LookRotation(Transform.InverseTransformDirection(Transform.position.To(KnownPosition).normalized)), 90) * Vector3.forward; //;//

    //KnownPosition - base.Missile.transform.position;

    [SerializeField]
    PositionSeekerDescriptor _comdesc;

    public override void OnAdded(ModularMissile missile, MissileComponentDescriptor descriptor)
    {
        Common.Trace($"Position Seeker Added to Missile. {descriptor.GetType().Name}");
        _comdesc = descriptor as PositionSeekerDescriptor;
        base.OnAdded(missile, descriptor);

    }

    public override ConeDescriptor? GetAimingCone()
    {
        return new ConeDescriptor(base.Missile.transform.position.To(KnownPosition).magnitude, Mathf.Min(Vector3.Angle(LocalBeamDirection, TrueBeamDirection), 45f), 5f);//

    }

    private void SetupTrack()
    {

    }
    public Vector3 GetPostionWithNoise(Vector3 initialPosition, PositionSeekerDescriptor.SeekerMode possource,  bool useBounds, bool useSphereNoise)
    {
        Vector3 vector3 = GetPostion(initialPosition, possource);
        if (useBounds)
        {
            vector3 += TargetedTrack.Trackable.Rotation * (TargetedTrack?.Trackable.RandomPointInBounds() ?? Vector3.zero);
        }
        if (useSphereNoise)
        {
            vector3 += UnityEngine.Random.insideUnitSphere * 10f * _comdesc.RandomScale;
        }
        return vector3;
    }

    public Vector3 GetPostion(Vector3 initialPosition, PositionSeekerDescriptor.SeekerMode possource)
    {
        switch (possource)
        {
            case PositionSeekerDescriptor.SeekerMode.TrueCenter:
                return TruePosition;
            case PositionSeekerDescriptor.SeekerMode.SensorCenter:
                return KnownPosition;
            case PositionSeekerDescriptor.SeekerMode.LaunchPlatform:
                return initialPosition;
            case PositionSeekerDescriptor.SeekerMode.None:
                return initialPosition;
                break;
            default:
                return initialPosition;
        }
    }

    public override void SetTrackTarget(TrackIdentifier target, Vector3 initialPosition)
    {
        _cachedTrack = null;


        if (base.Missile == null || base.Missile.LaunchedFrom == null || _trackTargetID == null)
        {
            base.SetTrackTarget(target, initialPosition);
        }
        _startvelocity = (TargetedTrack?.AbsoluteVelocity ?? Vector3.zero);// + Vector3.up * 3;
        
        _startaccel = TargetedTrack?.Acceleration ?? Vector3.zero;

        Vector3 primaryPostion = GetPostionWithNoise(initialPosition, _comdesc.PrimarySource, _comdesc.PrimaryUseBounds, _comdesc.PrimaryUseSphereNoise);


        if (_comdesc.SecondarySource != PositionSeekerDescriptor.SeekerMode.None && _comdesc.SourceSelection != SensorUpdateMode.None)
        {
            Vector3 secondaryPosition = GetPostionWithNoise(initialPosition, _comdesc.SecondarySource, _comdesc.SecondaryUseBounds, _comdesc.SecondaryUseSphereNoise);
            _primarypos = primaryPostion;
            _secondarypos = secondaryPosition;
            switch (_comdesc.SourceSelection)
            {
                case SensorUpdateMode.FirstSource:
                    break;
                case SensorUpdateMode.BestSource:
                    primaryPostion = TruePosition.Closest(primaryPostion, secondaryPosition);
                    break;
                case SensorUpdateMode.WorstSource:
                    primaryPostion = TruePosition.Furthest(primaryPostion, secondaryPosition);
                    break;
                case SensorUpdateMode.Average:
                    primaryPostion = (primaryPostion + secondaryPosition) / 2f;
                    break;
                case SensorUpdateMode.None:
                    break;
                default:
                    break;
            }
        }


        _startposition = primaryPostion;
        base.SetTrackTarget(target, primaryPostion);
        SetupTrack();
    }

    public override void OnLaunched(ILaunchingPlatform platform, bool forceHotLaunch, bool immediateSearching)
    {
        base.OnLaunched(platform, forceHotLaunch, immediateSearching);
        _age = 0;
    }

    public void FixedUpdate()
    {
        _age += Time.fixedDeltaTime;


        if (_comdesc == null || _comdesc.SeekerUpdateMode == SensorUpdateMode.None)
            return;

        List<RuntimeMissileSeeker> _validationSeekers = Common.GetVal<List<RuntimeMissileSeeker>>(Missile, "_validationSeekers") ?? new();
        List<RuntimeMissileSeeker> _targetingSeekers = Common.GetVal<List<RuntimeMissileSeeker>>(Missile, "_targetingSeekers") ?? new();

        List<Vector3> _velocities = new List<Vector3>();
        List<Vector3> _accelerations = new List<Vector3>();
        List<Vector3> _positions = new List<Vector3>();

        foreach (RuntimeMissileSeeker targetingSeeker in _targetingSeekers)
        {
            if (targetingSeeker is RuntimePostionSeeker)
                continue;

            SeekerSearchResult seekerSearchResult = targetingSeeker.SearchForTarget(_validationSeekers, out Vector3 position, out Vector3 velocity, out Vector3 acceleration);
            if (seekerSearchResult != SeekerSearchResult.Found)
            {
                continue;

            }



            if (_comdesc.SeekerUpdateMode == SensorUpdateMode.FirstSource)
            {
                _startvelocity = velocity;
                _startaccel = acceleration;
                _startposition = position;
                _age = 0;
                return;
            }
            else if (_comdesc.SeekerUpdateMode == SensorUpdateMode.BestSource)
            {
                _startvelocity = (TargetedTrack?.Trackable?.Velocity ?? Vector3.zero).Closest(_startvelocity, velocity);
                _startaccel = (TargetedTrack?.Acceleration ?? Vector3.zero).Closest(_startaccel, acceleration);
                _startposition = TruePosition.Closest(_startaccel, position);
                _age = 0;
            }
            else if (_comdesc.SeekerUpdateMode == SensorUpdateMode.WorstSource)
            {
                if (_age != 0)
                {
                    _startvelocity = velocity;
                    _startaccel = acceleration;
                    _startposition = position;
                    _age = 0;
                }
                _startvelocity = (TargetedTrack?.Trackable?.Velocity ?? Vector3.zero).Furthest(_startvelocity, velocity);
                _startaccel = (TargetedTrack?.Acceleration ?? Vector3.zero).Furthest(_startaccel, acceleration);
                _startposition = TruePosition.Furthest(_startaccel, position);
                _age = 0;
            }
            else if (_comdesc.SeekerUpdateMode == SensorUpdateMode.Average)
            {
                _velocities.Add(velocity);
                _accelerations.Add(acceleration);
                _positions.Add(position);
            }
            // Check if C is closer to B than A

            // Check if C is closer to B than A

        }

        if (_comdesc.SeekerUpdateMode == SensorUpdateMode.Average && _positions.Count > 0)
        { 
            _startvelocity = Vector3.zero;
            _startaccel = Vector3.zero;
            _startposition = Vector3.zero;
            foreach (Vector3 vel in _velocities)
                _startvelocity += vel;
            foreach (Vector3 accel in _accelerations)
                _startaccel += accel;
            foreach (Vector3 pos in _positions)
                _startposition += pos;
            _startvelocity /= _velocities.Count;
            _startaccel /= _accelerations.Count;
            _startposition /= _positions.Count;
            _age = 0;
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


    private List<DebugLine> _currentDebugLines = new List<DebugLine>();
    protected override SeekerSearchResult SearchForTargetInternal(IReadOnlyList<RuntimeMissileSeeker> validators, out Vector3 position, out Vector3 velocity, out Vector3 acceleration)
    {
        //Debug.LogError(PredictedPosition);
        position = PredictedPosition;
        velocity = _startvelocity;
        acceleration = _startaccel;
        _currentDebugLines.Clear();
        if (PredictedPosition == Vector3.zero)
            return SeekerSearchResult.NotFound;
        _currentDebugLines.Add(new DebugLine
        {
            Start = Transform.position,
            End = PredictedPosition,
            Color = Color.cyan,
        });
        _currentDebugLines.Add(new DebugLine
        {
            Start = _startposition ?? Vector3.zero,
            End = PredictedPosition,
            Color = Color.red,
        });
        /*
        _currentDebugLines.Add(new DebugLine
        {
            Start = _primarypos,
            End = KnownPosition,
            Color = Color.yellow,
        });
        _currentDebugLines.Add(new DebugLine
        {
            Start = _secondarypos,
            End = KnownPosition,
            Color = Color.red,
        });
        */
        return SeekerSearchResult.Found;
    }

    public List<DebugLine> GetDebugLines()
    {
        return _currentDebugLines;
    }
}

[HarmonyPatch(typeof(MissileDetailOverlay), "DrawShapes")]
class MissileDetailOverlayDrawShapesPatch
{
    // We use ___missile to access the private field of the same name
    static void Postfix(MissileDetailOverlay __instance, Camera cam)
    {
        ISelectableMissile _missile = Common.GetVal<ISelectableMissile>(__instance, "_missile");
        // Basic safety check: same logic as original DrawShapes
        if (_missile == null || !_missile.Alive) return;

        // Iterate through seekers just like the original code
        foreach (var seeker in _missile.Seekers)
        {
            // Check if this seeker implements the debug interface
            if (seeker is IDebugableMissileSeeker debugSeeker)
            {
                List<DebugLine> lines = debugSeeker.GetDebugLines();
                if (lines == null || lines.Count == 0) continue;

                // Open a Draw Command using the same injection point as the original overlay
                using (Draw.Command(cam, CustomPassInjectionPoint.AfterPostProcess))
                {
                    // Set global states for this command block
                    Draw.ThicknessSpace = ThicknessSpace.Noots;
                    Draw.DetailLevel = DetailLevel.Extreme;
                    Draw.LineGeometry = LineGeometry.Volumetric3D;
                    Draw.Matrix = Matrix4x4.identity; // We are using world space coords

                    foreach (var line in lines)
                    {
                        float finalThickness = line.Thickness > 0 ? line.Thickness : 0.5f;
                        Draw.Thickness = finalThickness;
                        Draw.Color = line.Color;

                        Draw.Line(line.Start, line.End);
                    }
                }
            }
        }
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


    protected override SeekerSearchResult SearchForTargetInternal(IReadOnlyList<RuntimeMissileSeeker> validators, out Vector3 position, out Vector3 velocity, out Vector3 acceleration)
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


