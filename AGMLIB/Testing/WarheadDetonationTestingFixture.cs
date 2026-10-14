#if DEBUG && AGMLIB_LOCAL_TEST_FIXTURES
using Lib.Munitions.Damagers;
using Lib.Generic_Gameplay;
using Munitions.ModularMissiles;
using Munitions.ModularMissiles.Runtime;

namespace Lib.Testing;

[TestingComponentFactory(Order = 130)]
public class WarheadDetonationTestingFactory : ITestingComponentFactory
{
    public void CreateTestingComponents(TestingComponentContext context)
    {
        if (Environment.GetEnvironmentVariable("AGMLIB_WARHEAD_DETONATION_TEST") == "1")
            new GameObject("Warhead Detonation Tests").AddComponent<WarheadDetonationAdapterTests>();
        if (Environment.GetEnvironmentVariable("AGMLIB_WARHEAD_NETWORK_TEST") is "host" or "client")
            new GameObject("Warhead Network Tests").AddComponent<WarheadNetworkTests>();
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AGMLIB_WARHEAD_FLIGHT_FLEET")))
            new GameObject("Warhead Native Flight Tests").AddComponent<WarheadFlightTests>();
    }
}

internal class WarheadAdapterMissile : ModularMissile
{
    public void SetAlive(bool alive) => _isDead = !alive;
    protected override void Awake() { }
    protected override void FixedUpdate() { }
    protected override void OnDestroy() { }
}

internal class WarheadAdapterRuntime : RuntimeMissileWarhead
{
    private void FixedUpdate() { }
}

internal class WarheadEffectProbe : MonoBehaviour, IEffectModule
{
    public int Plays;
    public virtual void Play() => Plays++;
    public void Stop() { }
    public void SetEffectParameter(string name, float value) { }
}

internal class WarheadAdapterBeam : LineBeamMuzzleEffects
{
    public int Starts;
    public float Length => _beamLength;
    public bool Hit;
    public Vector3 Endpoint;
    protected override void Awake() { }
    protected override void Update() { }
    protected override void HandleBeamLengthChanged() { }
    public override void StartEffect() => Starts++;
    public override void PositionHitEffect(bool on, Vector3 position, Vector3? normal)
    {
        Hit = on;
        Endpoint = position;
    }
}

internal class WarheadAdapterTarget : MonoBehaviour, IDamageable
{
    public int Hits;
    public int RandomPoints;
    public Vector3 LocalAimPoint;
    public Vector3 LastLocalHit;
    public GameObject GameObj => gameObject;
    public float BoundingRadius => 1f;
    public float TargetSize => 1f;
    public Vector3 LocalDirection(Vector3 world) => transform.InverseTransformDirection(world);
    public float ApplyDamageReduction(float damage) => damage;
    public HitResult DoDamage(MunitionHitInfo hit, IDamageDealer dealer, out float damage, out bool destroyed)
    {
        if (hit.HitObject != gameObject || dealer == null)
            throw new InvalidOperationException("Wrong beam damage target or missing dealer.");
        Hits++;
        LastLocalHit = hit.LocalPoint;
        damage = 1f;
        destroyed = false;
        return HitResult.Penetrated;
    }
    public HitResult DoArmorDamageOnly(MunitionHitInfo hit, IDamageDealer dealer) => HitResult.None;
    public Vector2? SampleUV(MeshCollider collider, Vector3 point, Vector3 normal) => Vector2.zero;
    public RaycastHit? FindExitPoint(MunitionHitInfo hit, out bool local)
    {
        local = false;
        return null;
    }
    public Vector3 RandomPointInBounds()
    {
        RandomPoints++;
        return LocalAimPoint;
    }
    public RaycastHit? FindRandomExteriorPoint(Vector3 point, Vector3 direction, float radius) => null;
    public object GetColliderGroup(Collider collider) => collider;
}

internal class WarheadDetonationAdapterTests : MonoBehaviour
{
    private readonly List<UnityEngine.Object> _objects = [];
    private int _checks;
    private int _failures;

    private IEnumerator Start()
    {
        if (ObjectPooler.Instance == null)
            Root("Adapter Effect Pool", Vector3.zero).AddComponent<ObjectPooler>();
        BeamWarheadDescriptor descriptor = Keep(ScriptableObject.CreateInstance<BeamWarheadDescriptor>());
        descriptor.BeamCount = 25;
        descriptor.LaunchAngle = 1;
        descriptor.BeamLength = 20;
        descriptor.DamagerSettings = Keep(ScriptableObject.CreateInstance<SingleRayDamagerSettings>());
        GameObject root = Root("Adapter Missile", new Vector3(100000, 100000, 100000));
        root.SetActive(false);
        root.AddComponent<Mirror.NetworkIdentity>();
        WarheadAdapterMissile missile = root.AddComponent<WarheadAdapterMissile>();
        WarheadAdapterRuntime runtime = root.AddComponent<WarheadAdapterRuntime>();
        runtime.OnAdded(missile, descriptor);
        root.SetActive(true);
        WarheadAdapterTarget initial = Root("Fuse Target", root.transform.position + Vector3.right * 10)
            .AddComponent<WarheadAdapterTarget>();
        MunitionHitInfo hit = MunitionHitInfo.Take();
        hit.HitObject = initial.gameObject;
        hit.Point = initial.transform.position;
        try
        {
            Run("configured-ray-count", () => Require(descriptor.EffectiveBeamCount == 25));
            Run("missing-damager-settings-fallback", () =>
            {
                BeamWarheadDescriptor fallback = Keep(Instantiate(descriptor));
                fallback.DamagerSettings = null;
                Require(fallback.ComponentDamage > 0 && fallback.DamagerSettings != null);
                Keep(fallback.DamagerSettings!);
            });
            Run("independent-count-angle-and-damage-scaling", () =>
            {
                BeamWarheadDescriptor scaled = Keep(Instantiate(descriptor));
                scaled.BeamCount = 7;
                scaled.LaunchAngle = 2;
                scaled.BeamCountScaling = new(new Keyframe(0, 1), new Keyframe(2, 3));
                scaled.BeamAngleScaling = new(new Keyframe(0, 1), new Keyframe(2, 5));
                scaled.BeamComponentDamageSizeScaling = new(new Keyframe(0, 10), new Keyframe(2, 30));
                scaled.BeamComponentDamageRangeScaling = new(new Keyframe(0, 1), new Keyframe(10, 0.5f));
                IDamageCharacteristic ranged = scaled.DamagerSettings!.MakeDamageCharacteristic(scaled, 10);
                Require(scaled.EffectiveBeamCount == 14 && scaled.EffectiveLaunchAngle == 6
                    && scaled.ComponentDamage == 20 && scaled.TotalComponentDamagePotential == 280
                    && ranged.ComponentDamage == 10);
            });
            Run("impact-target-aim", () =>
            {
                descriptor.SelectRandomPointInTarget = false;
                Vector3 direction = descriptor.RandomConeRay(runtime, initial, hit);
                Require(initial.RandomPoints == 1 && Vector3.Angle(direction, Vector3.right) < 0.01f);
            });
            Run("targetless-forward-cone", () =>
                Require(Vector3.Angle(descriptor.RandomConeRay(runtime, initial, null!), Vector3.forward) <= 1.42f));
            Run("impact-target-overrides-omnidirectional-fallback", () =>
            {
                descriptor.Omnidirectional = true;
                Require(Vector3.Angle(descriptor.RandomConeRay(runtime, initial, hit), Vector3.right) < 0.01f);
                descriptor.Omnidirectional = false;
            });
            Run("target-aim-without-hit-info", () =>
            {
                descriptor.SelectRandomPointInTarget = true;
                Require(Vector3.Angle(descriptor.RandomConeRay(runtime, initial, null!), Vector3.right) < 0.01f);
            });
            Run("missing-damageable-target", () =>
                Require(descriptor.RandomConeRay(runtime, null!, hit).sqrMagnitude > 0.99f));
            Run("coincident-target-fallback", () =>
            {
                Vector3 position = initial.transform.position;
                initial.transform.position = runtime.transform.position;
                try
                {
                    Require(Vector3.Angle(descriptor.RandomConeRay(runtime, initial, hit), Vector3.forward) <= 1.42f);
                }
                finally
                {
                    initial.transform.position = position;
                }
            });
            Run("targetless-explosion", () => Require(descriptor.Explode(runtime, null!, null!) == HitResult.None));
            WarheadAdapterTarget actual = Root("Ray Target", root.transform.position + Vector3.forward * 10)
                .AddComponent<WarheadAdapterTarget>();
            actual.gameObject.AddComponent<BoxCollider>();
            Physics.SyncTransforms();
            Run("ray-damages-actual-target", () =>
            {
                descriptor.DoRay(runtime, initial, hit, Vector3.forward, new Ray(root.transform.position, Vector3.forward));
                Require(actual.Hits == 1 && initial.Hits == 0);
            });
            Run("ray-without-fuse-target", () =>
            {
                descriptor.DoRay(runtime, null!, null!, Vector3.forward, new Ray(root.transform.position, Vector3.forward));
                Require(actual.Hits == 2);
            });
            GameObject scenery = Root("Scenery", root.transform.position + Vector3.back * 10);
            scenery.AddComponent<BoxCollider>();
            Physics.SyncTransforms();
            Run("ray-hits-scenery", () => Require(descriptor.DoRay(runtime, null!, null!, Vector3.back,
                new Ray(root.transform.position, Vector3.back)) == HitResult.None));
            Run("no-ray-damage-on-client", () =>
            {
                int hits = actual.Hits;
                Require(!runtime.isServer);
                descriptor.TriggerDetonate(runtime, initial, hit, out _);
                Require(actual.Hits == hits && initial.Hits == 0);
            });
            Run("targetless-cone-fragments", () =>
            {
                descriptor.ConeFrag = true;
                Require(descriptor.Explode(runtime, null!, null!) == HitResult.Penetrated
                    && actual.Hits == 3 && initial.Hits == 0);
                descriptor.ConeFrag = false;
            });
            Run("child-collider-damage-frame", () =>
            {
                BoxCollider body = actual.GetComponent<BoxCollider>();
                body.enabled = false;
                actual.transform.rotation = Quaternion.Euler(0, 90, 0);
                GameObject child = Root("Child Hit Collider", actual.transform.position);
                child.transform.SetParent(actual.transform, false);
                child.transform.localPosition = Vector3.zero;
                child.AddComponent<BoxCollider>();
                Physics.SyncTransforms();
                int hits = actual.Hits;
                try
                {
                    descriptor.DoRay(runtime, initial, hit, Vector3.forward, new Ray(root.transform.position, Vector3.forward));
                    Require(actual.Hits == hits + 1 && Mathf.Abs(actual.LastLocalHit.x - 0.5f) < 0.01f
                        && Mathf.Abs(actual.LastLocalHit.z) < 0.01f);
                }
                finally
                {
                    DestroyImmediate(child);
                    actual.transform.rotation = Quaternion.identity;
                    body.enabled = true;
                    Physics.SyncTransforms();
                }
            });
            CheckPresentation(descriptor, runtime, actual);
            Run("cancel-delayed-fuse-on-dead-missile", () =>
            {
                IEnumerator delay = descriptor.CoroutineOnTriggerEnter(missile, null!, 1f);
                Require(delay.MoveNext() && !delay.MoveNext());
            });
            root.AddComponent<RuntimeTimeFuse>();
            root.SetActive(false);
            yield return null;
            Run("pooled-missile-clears-delayed-fuse", () => Require(root.GetComponent<RuntimeTimeFuse>() == null));
        }
        finally
        {
            hit.Dispose();
            _objects.ForEach(UnityEngine.Object.DestroyImmediate);
            _objects.Clear();
        }
        string? keys = Environment.GetEnvironmentVariable("AGMLIB_WARHEAD_DETONATION_DESCRIPTORS");
        if (!string.IsNullOrWhiteSpace(keys))
        {
            float deadline = Time.realtimeSinceStartup + 120f;
            while (!BundleManager.IsInitialized && Time.realtimeSinceStartup < deadline)
                yield return null;
            foreach (string key in keys.Split('|'))
            {
                Run("loaded-warhead-effect-authority", () =>
                {
                    BeamWarheadDescriptor warhead = BundleManager.Instance.GetMissileComponent(key) as BeamWarheadDescriptor
                        ?? throw new InvalidOperationException("Configured warhead is not loaded.");
                    Require(warhead.BeamPrefab == null || warhead.BeamPrefab.GetComponentsInChildren<IDamageDealer>(true).Length == 0);
                    Require(warhead.ExplosionPrefabs.All(prefab => prefab != null && prefab.GetComponent<Poolable>() != null));
                    Debug.Log($"[AGMLIB Test] event=warhead-prefab key={key} rays={warhead.EffectiveBeamCount} explosionPrefabs={warhead.ExplosionPrefabs.Count} beamPresentationOnly=true");
                });
                Run("loaded-warhead-target-aim", () => CheckLoadedTargetAim(key));
            }
        }
        Debug.Log($"[AGMLIB Test] event=warhead-detonation-adapters result={(_failures == 0 ? "success" : "failure")} checks={_checks} failures={_failures}");
        Destroy(gameObject);
    }

    private void CheckLoadedTargetAim(string key)
    {
        BeamWarheadDescriptor descriptor = BundleManager.Instance.GetMissileComponent(key) as BeamWarheadDescriptor
            ?? throw new InvalidOperationException("Configured warhead is not loaded.");
        float distance = Mathf.Min(100f, descriptor.BeamLength / 2f);
        Require(distance > 1f);
        GameObject root = Root("Standoff Missile", new Vector3(100000, 100000, 100000));
        root.SetActive(false);
        root.AddComponent<Mirror.NetworkIdentity>();
        WarheadAdapterMissile missile = root.AddComponent<WarheadAdapterMissile>();
        WarheadAdapterRuntime runtime = root.AddComponent<WarheadAdapterRuntime>();
        runtime.OnAdded(missile, descriptor);
        root.SetActive(true);
        GameObject parent = Root("Target Parent", root.transform.position + Vector3.right * (distance - 5f));
        parent.transform.rotation = Quaternion.Euler(0, 45, 0);
        WarheadAdapterTarget target = Root("Standoff Target", root.transform.position + Vector3.right * distance)
            .AddComponent<WarheadAdapterTarget>();
        target.transform.SetParent(parent.transform, true);
        target.LocalAimPoint = new Vector3(0.2f, 0.1f, 0.1f);
        target.gameObject.AddComponent<BoxCollider>();
        Physics.SyncTransforms();
        MunitionHitInfo hit = MunitionHitInfo.Take();
        hit.HitObject = parent;
        hit.Point = target.transform.position;
        try
        {
            const int rays = 100;
            for (int i = 0; i < rays; i++)
            {
                Vector3 direction = descriptor.RandomConeRay(runtime, target, hit);
                descriptor.DoRay(runtime, target, hit, direction, new Ray(root.transform.position, direction));
            }
            Debug.Log($"[AGMLIB Test] event=warhead-standoff-aim key={key} rays={rays} hits={target.Hits} distance={distance} omnidirectional={descriptor.Omnidirectional} selectRandomPoint={descriptor.SelectRandomPointInTarget}");
            Require(target.Hits == rays && target.RandomPoints == rays);
        }
        finally
        {
            hit.Dispose();
            _objects.ForEach(UnityEngine.Object.DestroyImmediate);
            _objects.Clear();
        }
    }

    private void CheckPresentation(BeamWarheadDescriptor descriptor, RuntimeMissileWarhead runtime, WarheadAdapterTarget target)
    {
        GameObject visualPrefab = Root("Explosion Probe", Vector3.zero);
        visualPrefab.SetActive(false);
        visualPrefab.AddComponent<WarheadEffectProbe>();
        visualPrefab.AddComponent<ModularEffect>();
        GameObject damagePrefab = Root("Damage Probe", Vector3.zero);
        damagePrefab.SetActive(false);
        damagePrefab.AddComponent<ScalingAOEExplosionEffectModule>();
        damagePrefab.AddComponent<ModularEffect>();
        descriptor.ExplosionPrefabs = [damagePrefab, visualPrefab];
        var receive = AccessTools.MethodDelegate<Action<LookaheadMunitionBase, HitResult, int, int, Vector3, Quaternion, Mirror.NetworkIdentity>>(
            AccessTools.Method(typeof(LookaheadMunitionBase), "UserCode_RpcRollImpactEffect"));
        Vector3 position = runtime.transform.position;
        Run("ordinary-impact-does-not-explode", () =>
        {
            receive(runtime.Missile, HitResult.Stopped, 0, -1, position, Quaternion.identity, null!);
            Require(FindObjectsOfType<WarheadEffectProbe>().Length == 0);
        });
        WarheadEffectProbe? explosion = null;
        Run("custom-explosion-client-presentation", () =>
        {
            receive(runtime.Missile, HitResult.None, -1, 0, position, Quaternion.identity, null!);
            explosion = FindObjectsOfType<WarheadEffectProbe>().Single();
            Require(explosion.Plays == 1 && explosion.transform.position == position);
            Require(FindObjectsOfType<ScalingAOEExplosionEffectModule>().Length == 0);
            Keep(explosion.gameObject);
        });
        Run("following-impact-does-not-duplicate-explosion", () =>
        {
            receive(runtime.Missile, HitResult.Stopped, 0, -1, position, Quaternion.identity, null!);
            Require(explosion != null && explosion.Plays == 1 && FindObjectsOfType<WarheadEffectProbe>().Length == 1);
        });
        Run("pooled-explosion-second-use", () =>
        {
            explosion!.GetComponent<Poolable>().RepoolSelf();
            receive(runtime.Missile, HitResult.None, -1, 0, position + Vector3.up, Quaternion.identity, null!);
            Require(FindObjectsOfType<WarheadEffectProbe>().Single() == explosion && explosion.Plays == 2
                && explosion.transform.position == position + Vector3.up);
        });
        GameObject beamPrefab = Root("Beam Probe", Vector3.zero);
        beamPrefab.SetActive(false);
        beamPrefab.AddComponent<WarheadAdapterBeam>();
        beamPrefab.AddComponent<ModularEffect>();
        descriptor.BeamPrefab = beamPrefab;
        Quaternion rotation = Quaternion.LookRotation(Vector3.right);
        const float length = 13.75f;
        int lengthBits = BitConverter.ToInt32(BitConverter.GetBytes(length), 0);
        WarheadAdapterBeam? beam = null;
        Run("client-beam-exact-endpoint-no-damage", () =>
        {
            int hits = target.Hits;
            receive(runtime.Missile, HitResult.Stopped, -2, lengthBits, position, rotation, null!);
            beam = FindObjectsOfType<WarheadAdapterBeam>().Single();
            Require(beam.Starts == 1 && beam.Length == length && beam.Hit && beam.Endpoint == position + Vector3.right * length
                && target.Hits == hits);
            Keep(beam.gameObject);
        });
        Run("pooled-beam-miss-second-use", () =>
        {
            beam!.GetComponent<Poolable>().RepoolSelf();
            receive(runtime.Missile, HitResult.None, -2, lengthBits, position, rotation, null!);
            Require(FindObjectsOfType<WarheadAdapterBeam>().Single() == beam && beam.Starts == 2 && !beam.Hit);
        });
    }

    private T Keep<T>(T value) where T : UnityEngine.Object
    {
        _objects.Add(value);
        return value;
    }
    private GameObject Root(string name, Vector3 position)
    {
        GameObject root = Keep(new GameObject(name));
        root.transform.position = position;
        return root;
    }
    private void Run(string name, Action test)
    {
        _checks++;
        try
        {
            test();
            Debug.Log($"[AGMLIB Test] event=warhead-check name={name} result=success");
        }
        catch (Exception error)
        {
            _failures++;
            Debug.Log($"[AGMLIB Test] event=warhead-check name={name} result=failure error={error}");
        }
    }
    private static void Require(bool condition)
    {
        if (!condition)
            throw new InvalidOperationException("Warhead regression assertion failed.");
    }
}
#endif
