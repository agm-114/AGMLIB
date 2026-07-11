using System.Collections.Generic;

#pragma warning disable CA1050 // Match the repo's existing global Unity script style.
#pragma warning disable CA1812 // Harmony patch classes are discovered reflectively.

[HarmonyPatch(typeof(ShipController), nameof(ShipController.Initialize))]
internal class ShipControllerInitializeDroneTester
{
    private static void Postfix(ShipController __instance)
    {
        //DroneTester.EnsureAttachedTo(__instance);
    }
}

public class DroneTester : ShipState
{
    [SerializeField]
    private int _droneCount = 24;

    [SerializeField]
    private float _formationRadius = 120f;

    [SerializeField]
    private Vector3 _droneScale = new(4f, 1.5f, 1.5f);

    [SerializeField]
    private float _orbitDegreesPerSecond = 12f;

    [SerializeField]
    private float _spawnIntervalSeconds = 0.08f;

    [SerializeField]
    private float _initialClimbSeconds = 0.45f;

    [SerializeField]
    private float _initialClimbDistance = 25f;

    [SerializeField]
    private float _maxSpeed = 180f;

    [SerializeField]
    private float _acceleration = 280f;

    [SerializeField]
    private float _settleDistance = 2f;

    [SerializeField]
    private float _naturalDrift = 4f;

    private readonly List<Transform> _drones = new();
    private readonly List<DroneFlight> _flights = new();
    private readonly List<Vector3> _formationOffsets = new();
    private Transform? _droneRoot;
    private bool _wasBattleshortActive;
    private DroneTesterMode _mode = DroneTesterMode.Idle;
    private float _modeTime;

    public static DroneTester? EnsureAttachedTo(ShipController ship)
    {
        if (ship == null)
        {
            return null;
        }

        DroneTester tester = ship.GetComponent<DroneTester>() ?? ship.gameObject.AddComponent<DroneTester>();
        tester.EnsureDrones();
        return tester;
    }

    public override void Awake()
    {
        base.Awake();
        EnsureDrones();
    }

    private void LateUpdate()
    {
        EnsureDrones();

        bool battleshortActive = BattleshortState == ConditionalState.Enabled;
        if (battleshortActive && !_wasBattleshortActive)
        {
            StartDeployment();
        }
        else if (!battleshortActive && _wasBattleshortActive)
        {
            StartRecall();
        }

        UpdateDroneFlights(battleshortActive);
        _wasBattleshortActive = battleshortActive;
    }

    private void EnsureDrones()
    {
        if (_droneRoot == null)
        {
            GameObject root = new("Drone Tester Drones");
            root.transform.SetParent(transform, false);
            _droneRoot = root.transform;
        }

        while (_drones.Count < _droneCount)
        {
            AddDrone(_drones.Count);
        }

        RebuildFormationOffsets();
    }

    private void AddDrone(int index)
    {
        GameObject drone = GameObject.CreatePrimitive(PrimitiveType.Cube);
        drone.name = "Drone Tester Box " + index;
        drone.transform.SetParent(_droneRoot, false);
        drone.transform.localScale = _droneScale;
        drone.SetActive(false);

        foreach (Collider collider in drone.GetComponentsInChildren<Collider>())
        {
            UnityEngine.Object.Destroy(collider);
        }

        _drones.Add(drone.transform);
        _flights.Add(new DroneFlight(drone.transform));
    }

    private void RebuildFormationOffsets()
    {
        int droneCount = Mathf.Max(1, _droneCount);
        if (_formationOffsets.Count == droneCount)
        {
            return;
        }

        _formationOffsets.Clear();
        float goldenAngle = Mathf.PI * (3f - Mathf.Sqrt(5f));
        for (int i = 0; i < droneCount; i++)
        {
            float y = droneCount == 1 ? 0f : 1f - (i / (droneCount - 1f)) * 2f;
            float radius = Mathf.Sqrt(1f - y * y);
            float theta = goldenAngle * i;
            Vector3 direction = new(Mathf.Cos(theta) * radius, y, Mathf.Sin(theta) * radius);
            _formationOffsets.Add(direction * _formationRadius);
        }
    }

    private void StartDeployment()
    {
        _mode = DroneTesterMode.Deploying;
        _modeTime = 0f;
        for (int i = 0; i < _flights.Count; i++)
        {
            DroneFlight flight = _flights[i];
            flight.LocalPosition = Vector3.zero;
            flight.LocalVelocity = Vector3.zero;
            flight.StartTime = i * _spawnIntervalSeconds;
            flight.Active = false;
            flight.Transform.localPosition = Vector3.zero;
            flight.Transform.localRotation = Quaternion.LookRotation(Vector3.up, Vector3.forward);
            flight.Transform.gameObject.SetActive(false);
        }
    }

    private void StartRecall()
    {
        _mode = DroneTesterMode.Returning;
        _modeTime = 0f;
        for (int i = 0; i < _flights.Count; i++)
        {
            DroneFlight flight = _flights[i];
            flight.LocalPosition = flight.Transform.localPosition;
            flight.LocalVelocity *= 0.35f;
            flight.StartTime = i * _spawnIntervalSeconds;
            flight.Active = true;
            flight.Transform.gameObject.SetActive(true);
        }
    }

    private void UpdateDroneFlights(bool battleshortActive)
    {
        Quaternion orbit = Quaternion.AngleAxis(Time.time * _orbitDegreesPerSecond, Vector3.up);
        _modeTime += Time.deltaTime;

        if (_mode == DroneTesterMode.Idle)
        {
            return;
        }

        bool allSettled = true;
        for (int i = 0; i < _flights.Count && i < _formationOffsets.Count; i++)
        {
            Vector3 formationPosition = orbit * _formationOffsets[i];
            DroneFlight flight = _flights[i];
            float age = _modeTime - flight.StartTime;

            if (age < 0f)
            {
                allSettled = false;
                if (_mode == DroneTesterMode.Returning)
                {
                    FlyDrone(flight, formationPosition, Time.deltaTime);
                }

                continue;
            }

            if (_mode == DroneTesterMode.Returning && !flight.Active)
            {
                continue;
            }

            if (!flight.Active)
            {
                flight.Active = true;
                flight.Transform.gameObject.SetActive(true);
            }

            Vector3 target = _mode switch
            {
                DroneTesterMode.Deploying => DeploymentTarget(i, age, formationPosition),
                DroneTesterMode.Formed => formationPosition + NaturalOffset(i),
                DroneTesterMode.Returning => Vector3.zero,
                _ => Vector3.zero
            };

            FlyDrone(flight, target, Time.deltaTime);

            float distance = Vector3.Distance(flight.LocalPosition, target);
            bool settled = distance <= _settleDistance && flight.LocalVelocity.magnitude <= _settleDistance * 2f;
            if (_mode == DroneTesterMode.Returning && settled)
            {
                flight.Transform.gameObject.SetActive(false);
                flight.Active = false;
            }
            else
            {
                allSettled = false;
            }
        }

        if (_mode == DroneTesterMode.Deploying && battleshortActive && allSettled)
        {
            _mode = DroneTesterMode.Formed;
        }
        else if (_mode == DroneTesterMode.Returning && allSettled)
        {
            _mode = DroneTesterMode.Idle;
        }
    }

    private Vector3 DeploymentTarget(int index, float age, Vector3 formationPosition)
    {
        if (age <= _initialClimbSeconds)
        {
            float climb = _initialClimbSeconds <= 0f ? 1f : Mathf.Clamp01(age / _initialClimbSeconds);
            return Vector3.up * (_initialClimbDistance * Mathf.SmoothStep(0f, 1f, climb));
        }

        float maneuverAge = age - _initialClimbSeconds;
        float maneuverWindow = Mathf.Max(0.1f, Vector3.Distance(Vector3.up * _initialClimbDistance, formationPosition) / Mathf.Max(1f, _maxSpeed));
        float maneuver = Mathf.Clamp01(maneuverAge / maneuverWindow);
        Vector3 climbPoint = Vector3.up * _initialClimbDistance;
        return Vector3.Lerp(climbPoint, formationPosition + NaturalOffset(index), Mathf.SmoothStep(0f, 1f, maneuver));
    }

    private Vector3 NaturalOffset(int index)
    {
        float phase = Time.time * 1.7f + index * 0.61f;
        return new Vector3(Mathf.Sin(phase * 1.3f), Mathf.Sin(phase * 0.7f), Mathf.Cos(phase)) * _naturalDrift;
    }

    private void FlyDrone(DroneFlight flight, Vector3 target, float deltaTime)
    {
        Vector3 toTarget = target - flight.LocalPosition;
        float distance = toTarget.magnitude;
        Vector3 desiredVelocity = distance <= 0.001f
            ? Vector3.zero
            : toTarget.normalized * Mathf.Min(_maxSpeed, Mathf.Sqrt(distance * _acceleration));
        Vector3 steering = Vector3.ClampMagnitude(desiredVelocity - flight.LocalVelocity, _acceleration * deltaTime);
        flight.LocalVelocity += steering;
        flight.LocalVelocity *= Mathf.Clamp01(1f - deltaTime * 0.45f);
        flight.LocalPosition += flight.LocalVelocity * deltaTime;

        flight.Transform.localPosition = flight.LocalPosition;
        if (flight.LocalVelocity.sqrMagnitude > 0.01f)
        {
            flight.Transform.localRotation = Quaternion.LookRotation(flight.LocalVelocity.normalized, Vector3.up);
        }
    }

    private enum DroneTesterMode
    {
        Idle,
        Deploying,
        Formed,
        Returning
    }

    private sealed class DroneFlight
    {
        public DroneFlight(Transform transform)
        {
            Transform = transform;
        }

        public Transform Transform { get; }
        public Vector3 LocalPosition { get; set; }
        public Vector3 LocalVelocity { get; set; }
        public float StartTime { get; set; }
        public bool Active { get; set; }
    }
}

#pragma warning restore CA1812
#pragma warning restore CA1050
