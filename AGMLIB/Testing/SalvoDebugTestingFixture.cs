using Munitions.ModularMissiles.Runtime;
#if DEBUG && AGMLIB_LOCAL_TEST_FIXTURES
using Lib.Diagnostics;
using Lib.Munitions.Damagers;
using Munitions.ModularMissiles;

namespace Lib.Testing;

[TestingComponentFactory(Order = 145)]
public class SalvoDebugTestingFactory : ITestingComponentFactory
{
    public void CreateTestingComponents(TestingComponentContext context)
    {
        if (Environment.GetEnvironmentVariable("AGMLIB_SALVO_DEBUG_TEST") == "1")
            new GameObject("Salvo Debug Tests").AddComponent<SalvoDebugTests>();
        if (Environment.GetEnvironmentVariable("AGMLIB_SALVO_DEBUG_LIVE_TEST") == "1")
            new GameObject("Salvo Debug Flight Assertions").AddComponent<SalvoDebugFlightAssertions>();
        if (Environment.GetEnvironmentVariable("AGMLIB_SALVO_DEBUG_SHIP_FLEET") is { Length: > 0 })
            new GameObject("Salvo Debug Ship Tests").AddComponent<SalvoDebugShipTests>();
        if (Environment.GetEnvironmentVariable("AGMLIB_SALVO_DEBUG_PLAYTEST_FLEET") is { Length: > 0 })
            new GameObject("Salvo Debug Interactive Range").AddComponent<SalvoDebugPlaytest>();
    }
}

internal class SalvoDebugTestPart : HullPart
{
    public override string UIName => "Test component";
    protected override void Awake() => ReinitHP();
    protected override void OnDestroy() { }
}

internal class SalvoDebugTests : MonoBehaviour
{
    private int _checks;
    private readonly List<UnityEngine.Object> _objects = [];

    private IEnumerator Start()
    {
        Require(BeamRayCapture.Current == null, "initial-context");
        MissileTrace missile = new(1, "TEST / synthetic trajectory");
        for (int i = 0; i < MissileTrace.MaxPoints * 4; i++)
            missile.Sample(i, Vector3.forward * i);
        missile.Sample(5000, Vector3.right, true);
        Require(missile.Path.Count <= MissileTrace.MaxPoints && missile.PathDecimations > 0 &&
            missile.Path[0].Position == Vector3.zero && missile.Path.Last().Position == Vector3.right, "bounded-path-endpoints");
        int count = missile.Path.Count;
        missile.Sample(5000, Vector3.right, true);
        Require(missile.Path.Count == count, "duplicate-terminal-sample");
        Require(Mathf.Approximately(SalvoTraceGeometry.DistanceToSegment(new Vector2(5, 2), Vector2.zero, Vector2.right * 10), 2), "line-hover");
        Require(Mathf.Approximately(SalvoTraceGeometry.DistanceToSegment(Vector2.right, Vector2.zero, Vector2.zero), 1), "zero-length-hover");
        BeamRayTrace bounded = new();
        for (int i = 0; i < BeamRayTrace.MaxSteps + 5; i++)
            bounded.AddStep(new DamageTraceStep("Bounded step"));
        Require(bounded.Steps.Count == BeamRayTrace.MaxSteps && bounded.DroppedSteps == 5, "bounded-steps");

        BeamWarheadDescriptor descriptor = Keep(ScriptableObject.CreateInstance<BeamWarheadDescriptor>());
        descriptor.DamagerSettings = Keep(ScriptableObject.CreateInstance<SingleRayDamagerSettings>());
        descriptor.BeamComponentDamageSizeScaling = AnimationCurve.Constant(0, 10, 120);
        descriptor.BeamComponentDamageRangeScaling = AnimationCurve.Linear(0, 1, 200, 0.5f);
        descriptor.BeamArmorPenetrationSizeScaling = AnimationCurve.Constant(0, 10, 60);
        descriptor.BeamArmorPenetrationRangeScaling = AnimationCurve.Constant(0, 1000, 0.8f);
        BeamRayTrace trace = descriptor.CreateTrace(new Ray(Vector3.zero, Vector3.forward), 100);
        Require(trace.Range == 100 && Mathf.Approximately(trace.SizeDamage * trace.RangeDamageMultiplier, 90) &&
            Mathf.Approximately(trace.SizePenetration * trace.RangePenetrationMultiplier, 48), "independent-curve-snapshots");
        IDamageDealer dealer = descriptor.DamagerSettings.MakeDamageDealer(descriptor, 100);
        using (BeamRayCapture outer = new(trace))
        {
            outer.SetDealer(dealer);
            Require(BeamRayCapture.Current == trace && BeamRayCapture.Owns(dealer), "dealer-attribution");
            using (BeamRayCapture inner = new(new BeamRayTrace()))
                Require(BeamRayCapture.Current != trace && !BeamRayCapture.Owns(dealer), "nested-attribution");
            Require(BeamRayCapture.Current == trace, "nested-context-restoration");
            using (BeamRayCapture? suppressed = BeamRayCapture.SuppressNested())
                Require(BeamRayCapture.Current == null && !BeamRayCapture.Owns(dealer), "uncaptured-nested-ray-excluded");
            Require(BeamRayCapture.Current == trace, "uncaptured-nested-ray-restoration");
            SalvoDebugTestPart recorded = Part();
            recorded.DoDamage(150, dealer);
            Require(trace.Steps.Any(step => step.Description.Contains("allocated 150") && step.Description.Contains("pending HP loss 100")),
                "native-component-allocation-and-pending-health");
            SalvoDebugTestPart unrelated = Part();
            int steps = trace.Steps.Count;
            unrelated.DoDamage(10, descriptor.DamagerSettings.MakeDamageDealer(descriptor));
            Require(trace.Steps.Count == steps, "unrelated-dealer-excluded");
            outer.Complete(HitResult.Penetrated, 100);
        }
        Require(BeamRayCapture.Current == null && trace.Completed && trace.ReportedDamage == 100, "scope-and-completion");
        SalvoDebugTestPart baseline = Part();
        baseline.DoDamage(150, dealer);
        Require(((ISubDamageable)baseline).CurrentHealth == 0, "same-native-damage-without-recording");
        Require(!trace.Describe().Contains("NaN"), "finite-description");
        BlastDamageTarget parent = Keep(new GameObject("Internal Damage Parent")).AddComponent<BlastDamageTarget>();
        BeamRayTrace internalTrace = new();
        using (BeamRayCapture capture = new(internalTrace))
        {
            IDamageDealer internalDealer = descriptor.DamagerSettings.MakeDamageDealer(descriptor, 100);
            capture.SetDealer(internalDealer);
            DamageTraceStep damaging = new("Internal hit") { HasSegment = true, Intersections = 1 };
            BeamRayCapture.RecordInternalRay(damaging);
            internalDealer.DamageComponents(parent, [Part()], null!, HitResult.Penetrated, out float damage);
            Require(damaging.DamageEvaluated && damaging.ReportedDamage == damage && damage > 0 && damaging.Outcome == RayDamageOutcome.Damage,
                "internal-native-damage-and-green-color");
            DamageTraceStep zero = new("Filtered hit") { HasSegment = true, Intersections = 1 };
            BeamRayCapture.RecordInternalRay(zero);
            internalDealer.DamageComponents(parent, [], null!, HitResult.Penetrated, out _);
            Require(zero.DamageEvaluated && zero.ReportedDamage == 0 && zero.Outcome == RayDamageOutcome.NoDamage, "internal-zero-damage-amber-color");
            Require(new DamageTraceStep("Miss") { HasSegment = true }.Outcome == RayDamageOutcome.Miss, "internal-miss-gray-color");
        }

        SalvoDebugRecorder recorder = SalvoDebugRecorder.Instance!;
        Require(recorder.Recording, "capture-enabled-after-opt-in");
        recorder.SetRecording(true);
        SalvoTrace report = new(1, "TEST / synthetic viewer fixture");
        missile = new MissileTrace(1, "TEST / synthetic trajectory") { Status = ActiveMissileSalvo.MissileStatus.Hit };
        for (int i = 0; i <= 60; i++)
            missile.Sample(i * 0.1f, new Vector3(i * 2, Mathf.Sin(i * 0.07f) * 10, i * 3));
        trace.Origin = missile.Path.Last().Position;
        trace.End = trace.Origin + new Vector3(5, 0, 20);
        trace.Target = "TEST target";
        trace.HasTargetBounds = true;
        trace.TargetBounds = new Bounds(trace.End, new Vector3(8, 10, 20));
        missile.Rays.Add(trace);
        missile.Rays.Add(new BeamRayTrace { Origin = trace.Origin, End = trace.End + Vector3.right * 30, Completed = true });
        report.Missiles.Add(missile);
        string json = SalvoTraceJson.Write(report);
        SalvoTrace restored = SalvoTraceJson.Read<SalvoTrace>(json);
        Require(restored.Missiles.Count == 1 && restored.Missiles[0].Rays.Count == 2 &&
            restored.Missiles[0].Rays[0].ReportedDamage == 100 && restored.Missiles[0].Path.Count > 0 &&
            restored.Missiles[0].Rays[0].TargetBounds.center == trace.End, "snapshot-json-roundtrip");
        float deadline = Time.realtimeSinceStartup + 60;
        while (!BundleManager.IsInitialized)
        {
            if (Time.realtimeSinceStartup >= deadline)
                throw new InvalidOperationException("Salvo debug fixture: bundle initialization timed out.");
            yield return null;
        }
        yield return new WaitForSecondsRealtime(2);
        recorder.History.Add(report);
        SalvoDebugWindow window = recorder.GetComponent<SalvoDebugWindow>();
        window.Select([report]);
        window.Pin(trace);
        window.Inspect(missile.Rays[1]);
        window.Inspect(null);
        Require(window.SelectedRay == missile.Rays[1], "selection-survives-pointer-leaving-ray");
        window.Pin(trace);
        Debug.Log($"[AGMLIB Test] event=salvo-debug-adapters result=success checks={_checks}");
        yield return new WaitForSecondsRealtime(5);
        CaptureScreenshot();
        foreach (UnityEngine.Object item in _objects)
            Destroy(item);
        // Leave a bounded interval for external rendering checks, then close the agent-launched game.
        yield return new WaitForSecondsRealtime(10);
        Application.Quit();
    }

    private SalvoDebugTestPart Part() => Keep(new GameObject("Salvo Debug Test Part")).AddComponent<SalvoDebugTestPart>();
    internal static void CaptureScreenshot()
    {
        string? screenshot = Environment.GetEnvironmentVariable("AGMLIB_SALVO_DEBUG_SCREENSHOT");
        if (string.IsNullOrEmpty(screenshot))
            return;
        // Optional Unity module: load only for this isolated rendering check.
        Type capture = Type.GetType("UnityEngine.ScreenCapture, UnityEngine.ScreenCaptureModule", true);
        AccessTools.MethodDelegate<Action<string, int>>(AccessTools.Method(capture, "CaptureScreenshot", [typeof(string), typeof(int)]))(screenshot, 1);
    }
    private T Keep<T>(T item) where T : UnityEngine.Object
    {
        _objects.Add(item);
        return item;
    }
    private void Require(bool condition, string name)
    {
        if (!condition)
            throw new InvalidOperationException($"Salvo debug fixture failed: {name}");
        _checks++;
    }
}

internal class SalvoDebugFlightAssertions : MonoBehaviour
{
    private readonly Dictionary<MissileTrace, (int Points, int Rays, float Damage)> _finished = [];
    private int _failures;
    private int _beamMissiles;

    private void Awake() => DontDestroyOnLoad(gameObject);

    private void LateUpdate()
    {
        SalvoDebugRecorder recorder = SalvoDebugRecorder.Instance!;
        foreach (MissileTrace missile in recorder.History.SelectMany(report => report.Missiles))
        {
            if (missile.Status == ActiveMissileSalvo.MissileStatus.InFlight || _finished.ContainsKey(missile))
                continue;
            WarheadFlightProbe probe = FindObjectsOfType<WarheadFlightProbe>()
                .FirstOrDefault(item => ((IMissile)item.GetComponent<ModularMissile>()).ID == missile.MissileId);
            int expectedRays = probe != null ? (int)Math.Ceiling(((BeamWarheadDescriptor)probe
                .GetComponent<RuntimeMissileWarhead>().Internals().Descriptor).EffectiveBeamCount) : -1;
            if (missile.Path.Count < 2 || missile.Rays.Count != expectedRays || missile.Rays.Any(ray => !ray.Completed || ray.ReportedDamage != 1f))
            {
                _failures++;
                Debug.LogError($"[AGMLIB Test] event=salvo-debug-flight result=failure points={missile.Path.Count} rays={missile.Rays.Count} expectedRays={expectedRays}");
            }
            _finished.Add(missile, (missile.Path.Count, missile.Rays.Count, missile.Rays.Sum(ray => ray.ReportedDamage)));
            Debug.Log($"[AGMLIB Test] event=salvo-debug-flight result=recorded points={missile.Path.Count} rays={missile.Rays.Count} damage={missile.Rays.Sum(ray => ray.ReportedDamage)}");
            // Exercise the real HUD button after completion, including its pooled binding.
            SalvoDebugWindow window = recorder.GetComponent<SalvoDebugWindow>();
            window.Visible = false;
            FriendlySalvoItem row = FindObjectsOfType<FriendlySalvoItem>().FirstOrDefault();
            row?.Internals().Button.onClick.Invoke();
            if (!window.Visible)
            {
                _failures++;
                Debug.LogError("[AGMLIB Test] event=salvo-debug-row result=failure");
            }
            if (missile.Rays.Count > 0)
            {
                window.Pin(missile.Rays[0]);
                _beamMissiles++;
                if (_beamMissiles == 1)
                    StartCoroutine(CaptureView());
            }
        }
        foreach (var entry in _finished)
            if (entry.Key.Path.Count != entry.Value.Points || entry.Key.Rays.Count != entry.Value.Rays ||
                entry.Key.Rays.Sum(ray => ray.ReportedDamage) != entry.Value.Damage)
            {
                _failures++;
                Debug.LogError("[AGMLIB Test] event=salvo-debug-pooled-snapshot result=failure");
                break;
            }
    }

    private static IEnumerator CaptureView()
    {
        yield return new WaitForSecondsRealtime(1);
        SalvoDebugTests.CaptureScreenshot();
    }

    private void OnApplicationQuit() => Debug.Log($"[AGMLIB Test] event=salvo-debug-flight-summary result={(_beamMissiles > 0 && _failures == 0 ? "success" : "failure")} missiles={_finished.Count} beamMissiles={_beamMissiles} failures={_failures}");
}
#endif
