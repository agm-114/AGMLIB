using Mirror;
using Munitions.ModularMissiles.Runtime;
using UnityEngine.SceneManagement;

namespace Lib.Diagnostics;

internal class SalvoDebugRecorder : MonoBehaviour
{
    internal const int MaxSalvos = 32;
    internal const int MaxMissiles = 64;
    private const int MaxRetainedMissiles = 256;
    private const int MaxRetainedRays = 1024;
    private class Flight(ActiveMissileSalvo salvo, MissileTrace trace)
    {
        public ActiveMissileSalvo Salvo = salvo;
        public MissileTrace Trace = trace;
    }

    public static SalvoDebugRecorder? Instance { get; private set; }
    public bool Recording { get; private set; }
    public readonly List<SalvoTrace> History = [];
    public int DroppedMissiles { get; private set; }
    private readonly Dictionary<IMissile, Flight> _flights = [];
    private readonly Dictionary<ActiveMissileSalvo, SalvoTrace> _reports = [];
    private SalvoDebugWindow _window = null!;
    private float _nextSample;
    private int _retainedMissiles;
    private int _retainedRays;

    public static void Initialize()
    {
        if (Instance == null && Environment.GetEnvironmentVariable("AGMLIB_SALVO_DEBUG") == "1")
            new GameObject("AGMLIB Salvo Debugger").AddComponent<SalvoDebugRecorder>();
    }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _window = gameObject.AddComponent<SalvoDebugWindow>();
        Recording = true;
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
        if (Instance == this)
            Instance = null;
    }

    private void OnSceneChanged(Scene previous, Scene next) => Clear();

    private void Update()
    {
        if (!Recording || !NetworkServer.active || Time.time < _nextSample)
            return;
        _nextSample = Time.time + 0.1f;
        foreach (var flight in _flights)
            flight.Value.Trace.Sample(Time.time, flight.Key.Position);
    }

    private void OnGUI()
    {
        Event input = Event.current;
        if (input.type == EventType.KeyDown && input.keyCode == KeyCode.F8 && input.control)
        {
            _window.Visible = !_window.Visible;
            input.Use();
        }
    }

    public void SetRecording(bool enabled)
    {
        if (enabled == Recording)
            return;
        Clear();
        Recording = enabled;
    }

    public void Clear()
    {
        _flights.Clear();
        _reports.Clear();
        History.Clear();
        DroppedMissiles = 0;
        _nextSample = 0;
        _retainedMissiles = _retainedRays = 0;
        _window.ClearSelection();
    }

    public void Register(ActiveMissileSalvo salvo, IMissile missile)
    {
        if (!Recording || !NetworkServer.active || salvo.LaunchedFrom?.OwnedBy?.IsLocalPlayer != true || _flights.ContainsKey(missile))
            return;
        _reports.TryGetValue(salvo, out SalvoTrace report);
        if (report?.Missiles.Count == MaxMissiles)
        {
            DroppedMissiles++;
            return;
        }
        while ((report == null && History.Count >= MaxSalvos) || _retainedMissiles >= MaxRetainedMissiles)
            if (!EvictFinished(report))
            {
                DroppedMissiles++;
                return;
            }
        if (report == null)
        {
            string launcher = salvo.LaunchedFrom is Component component ? component.name : "Local launcher";
            report = new SalvoTrace(salvo.SalvoID, launcher);
            _reports.Add(salvo, report);
            History.Add(report);
        }
        MissileTrace trace = new(missile.ID, missile.ShortMissileName);
        trace.Sample(Time.time, missile.Position, true);
        report.Missiles.Add(trace);
        _retainedMissiles++;
        _flights.Add(missile, new Flight(salvo, trace));
        if (report.Missiles.Count == 1)
            _window.Select([report]);
    }

    private bool EvictFinished(SalvoTrace? except = null)
    {
        foreach (SalvoTrace report in History)
        {
            if (report == except || !report.Finished)
                continue;
            ActiveMissileSalvo owner = _reports.First(entry => entry.Value == report).Key;
            if (owner.Active)
                continue;
            _reports.Remove(owner);
            History.Remove(report);
            _retainedMissiles -= report.Missiles.Count;
            _retainedRays -= report.Missiles.Sum(item => item.Rays.Count);
            return true;
        }
        return false;
    }

    public void Terminate(ActiveMissileSalvo salvo, IMissile missile, ActiveMissileSalvo.MissileStatus status)
    {
        if (!_flights.TryGetValue(missile, out Flight flight) || flight.Salvo != salvo)
            return;
        flight.Trace.Sample(Time.time, missile.Position, true);
        flight.Trace.Status = status;
        _flights.Remove(missile);
    }

    public void Open(IActiveMissileSalvo salvo)
    {
        if (!Recording || salvo.Active)
            return;
        IEnumerable<ActiveMissileSalvo> salvos = salvo switch
        {
            ActiveMissileSalvo single => [single],
            MissileSalvoGroup group => group.Internals().AllSalvos,
            _ => []
        };
        List<SalvoTrace> reports = [];
        foreach (ActiveMissileSalvo item in salvos)
            if (_reports.TryGetValue(item, out SalvoTrace report) && History.Contains(report))
                reports.Add(report);
        _window.Select(reports);
    }

    public static BeamRayCapture? BeginRay(RuntimeMissileWarhead runtime, BeamWarheadDescriptor descriptor,
        Ray ray, float length, Collider? collider, IDamageable? target, string kind = "Beam ray")
    {
        SalvoDebugRecorder? recorder = Instance;
        if (recorder == null || !recorder.Recording || !runtime.isServer ||
            !recorder._flights.TryGetValue(runtime.Missile, out Flight flight))
            return BeamRayCapture.SuppressNested();
        MissileTrace missile = flight.Trace;
        missile.Sample(Time.time, ray.origin, true);
        if (missile.Rays.Count >= MissileTrace.MaxRays)
        {
            missile.DroppedRays++;
            return BeamRayCapture.SuppressNested();
        }
        while (recorder._retainedRays >= MaxRetainedRays)
            if (!recorder.EvictFinished(recorder._reports[flight.Salvo]))
            {
                missile.DroppedRays++;
                return BeamRayCapture.SuppressNested();
            }
        BeamRayTrace trace = descriptor.CreateTrace(ray, length);
        trace.Time = Time.time;
        trace.MissileId = missile.MissileId;
        trace.RayIndex = missile.Rays.Count + 1;
        trace.Kind = kind;
        trace.Collider = collider != null ? collider.name : "None";
        trace.Hit = collider != null;
        trace.HasTargetBounds = collider != null;
        if (collider != null)
            trace.TargetBounds = collider.bounds;
        trace.Target = target?.GameObj != null ? target.GameObj.name : collider != null ? "Scenery / non-damageable" : "Miss";
        missile.Rays.Add(trace);
        recorder._retainedRays++;
        return new BeamRayCapture(trace);
    }
}

// Damage calls are synchronous on Unity's main thread. A stack restores attribution after nested damage.
internal class BeamRayCapture : IDisposable
{
    [ThreadStatic] private static BeamRayCapture? _current;
    private readonly BeamRayCapture? _previous;
    private readonly BeamRayTrace? _trace;
    public static BeamRayTrace? Current => _current?._trace;
    private IDamageDealer? _dealer;
    private DamageTraceStep? _internalRay;
    public static DamageTraceStep? CurrentInternalRay => _current?._internalRay;

    public static void RecordInternalRay(DamageTraceStep step)
    {
        if (_current?._trace is not BeamRayTrace trace)
            return;
        _current._internalRay = trace.Steps.Count < BeamRayTrace.MaxSteps ? step : null;
        trace.AddStep(step);
    }

    public static bool Owns(IDamageCharacteristic character) => ReferenceEquals(_current?._dealer, character) && character != null;

    public BeamRayCapture(BeamRayTrace? trace)
    {
        _trace = trace;
        _previous = _current;
        _current = this;
    }

    public void Complete(HitResult result, float damage = 0, bool destroyed = false)
    {
        if (_trace is not BeamRayTrace trace)
            return;
        trace.Result = result;
        trace.ReportedDamage = damage;
        trace.Destroyed = destroyed;
        trace.Completed = true;
    }

    public void SetDealer(IDamageDealer dealer)
    {
        if (_trace is not BeamRayTrace trace)
            return;
        _dealer = dealer;
        trace.Damager = dealer.GetType().Name;
        trace.ComponentDamage = dealer.ComponentDamage;
        trace.ArmorPenetration = dealer.ArmorPenetration;
    }

    public static BeamRayCapture? SuppressNested() => _current == null ? null : new BeamRayCapture(null);

    public static BeamRayCapture? SuspendUnlessOwned(IDamageDealer dealer)
    {
        if (_current == null || Owns(dealer))
            return null;
        BeamRayCapture previous = _current;
        _current = null;
        return previous;
    }

    public static void Restore(BeamRayCapture? previous)
    {
        if (previous != null)
            _current = previous;
    }

    public void Dispose() => _current = _previous;
}
