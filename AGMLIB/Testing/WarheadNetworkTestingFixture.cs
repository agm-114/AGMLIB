#if DEBUG && AGMLIB_LOCAL_TEST_FIXTURES
using Lib.Munitions.Damagers;
using Mirror;
using Munitions.ModularMissiles.Runtime;

namespace Lib.Testing;

// A quarantined, loopback-only transport experiment. Both game processes create the
// same adapter component layout, then use the native impact and pooling RPCs.
internal class NetworkWarheadMissile : WarheadAdapterMissile
{
    public int DelayedCollisions;
    public bool ImpactScenery(MunitionHitInfo hit) => base.GenericImpact(hit, false);
    public override void ProcessCollision(MunitionHitInfo hit, bool trigger)
    {
        if (!trigger)
            throw new InvalidOperationException("Unexpected physical collision in delayed-fuse adapter.");
        DelayedCollisions++;
        GetComponent<RuntimeMissileWarhead>().TriggerDetonate(hit.HitObject.GetComponent<IDamageable>(), hit, out _);
    }
    protected override void OnUnpooled() => GetComponent<RuntimeMissileWarhead>().OnUnpooled();
    protected override void OnRepooled() { }
    protected override void OnRepoolDelayed(bool disabledImmediately) { }
    public override bool OnSerialize(NetworkWriter writer, bool initialState) => false;
    public override void OnDeserialize(NetworkReader reader, bool initialState) { }
}

internal class NetworkExplosionProbe : WarheadEffectProbe
{
    public static int Count;
    public override void Play()
    {
        base.Play();
        Count++;
    }
}

internal class NetworkBeamProbe : WarheadAdapterBeam
{
    public static int Count;
    public static int Hits;
    public static int WrongDirections;
    public override void StartEffect()
    {
        base.StartEffect();
        Count++;
    }
    public override void PositionHitEffect(bool on, Vector3 position, Vector3? normal)
    {
        base.PositionHitEffect(on, position, normal);
        if (on)
            Hits++;
        Vector3 expected = (((Count - 1) / WarheadNetworkTests.Rays) % 4) switch
        {
            2 => Vector3.right,
            3 => Vector3.back,
            _ => Vector3.forward
        };
        if (Vector3.Angle(transform.forward, expected) > 0.01f)
            WrongDirections++;
    }
}

internal class WarheadNetworkTests : MonoBehaviour, IObjectPool<NetworkPoolable>
{
    private static readonly Guid AssetId = new("6de4902c-144f-4fde-8435-d662ee6a4453");
    private static readonly Vector3 Position = new(100000, 100000, 100000);
    private const int Detonations = 40;
    private const int Explosions = Detonations + 1;
    internal const int Rays = 25;
    private BeamWarheadDescriptor _descriptor = null!;
    private WarheadAdapterTarget _target = null!;
    private Collider _targetCollider = null!;
    private bool _host;
    private int _repoolCount;

    private IEnumerator Start()
    {
        _host = Environment.GetEnvironmentVariable("AGMLIB_WARHEAD_NETWORK_TEST") == "host";
        Require(!NetworkServer.active && !NetworkClient.active, "session-already-active");
        if (ObjectPooler.Instance == null)
            new GameObject("Network Effect Pool").AddComponent<ObjectPooler>();
        Application.runInBackground = true;
        if (Transport.activeTransport != null)
            Transport.activeTransport.enabled = false;
        TelepathyTransport transport = gameObject.AddComponent<TelepathyTransport>();
        transport.port = 43186;
        Transport.activeTransport = transport;
        BuildDescriptor();
        NetworkClient.RegisterSpawnHandler(AssetId, (SpawnMessage message) => CreateMissile().gameObject, Destroy);
        NetworkClient.OnConnectedEvent = () =>
        {
            NetworkClient.connection.isAuthenticated = true;
            NetworkClient.Ready();
        };
        if (_host)
        {
            NetworkServer.OnConnectedEvent = connection => connection.isAuthenticated = true;
            NetworkServer.Listen(2);
            NetworkClient.ConnectHost();
            NetworkClient.ConnectLocalServer();
        }
        else
            NetworkClient.Connect("127.0.0.1");
        Debug.Log($"[AGMLIB Test] event=warhead-network-start role={Role} transport=loopback-tcp");
        float deadline = Time.realtimeSinceStartup + 90f;
        if (_host)
        {
            while (NetworkServer.connections.Values.Count(connection => connection.isReady) < 2)
            {
                Require(Time.realtimeSinceStartup < deadline, "peer-readiness-timeout");
                yield return null;
            }
            NetworkWarheadMissile? missile = CreateMissile();
            NetworkServer.Spawn(missile.gameObject, AssetId);
            missile.Unpool(Position, Quaternion.identity);
            missile.SetAlive(true);
            _descriptor.FuseDelay = 1f;
            missile.OnTriggerEnter(_targetCollider);
            Require(missile.GetComponent<RuntimeTimeFuse>() != null, "delayed-fuse-not-scheduled");
            missile.RepoolSelf();
            while (missile.gameObject.activeSelf)
            {
                Require(Time.realtimeSinceStartup < deadline, "cancelled-fuse-repool-timeout");
                yield return null;
            }
            yield return null;
            Require(missile.GetComponent<RuntimeTimeFuse>() == null && missile.DelayedCollisions == 0,
                "pooled-fuse-not-cancelled");
            Debug.Log("[AGMLIB Test] event=warhead-network-delayed-fuse result=success scenario=cancel-on-repool");
            _descriptor.FuseDelay = 0;
            for (int index = 0; index < Detonations; index++)
            {
                if (missile == null)
                {
                    missile = CreateMissile();
                    NetworkServer.Spawn(missile.gameObject, AssetId);
                }
                missile.Unpool(Position, Quaternion.identity);
                int scenario = index % 4;
                bool targeted = scenario >= 2;
                _descriptor.Omnidirectional = targeted;
                Vector3 targetDirection = scenario switch
                {
                    0 => Vector3.forward,
                    3 => Vector3.back,
                    _ => Vector3.right
                };
                _target.transform.position = Position + targetDirection * 100;
                Physics.SyncTransforms();
                RuntimeMissileWarhead runtime = missile.GetComponent<RuntimeMissileWarhead>();
                if (targeted)
                {
                    MunitionHitInfo hit = MunitionHitInfo.Take();
                    try
                    {
                        hit.HitObject = _target.gameObject;
                        hit.Point = _target.transform.position;
                        if (scenario == 3)
                            runtime.CollisionDetonate(_target, hit);
                        else if (index == 38)
                        {
                            missile.SetAlive(true);
                            _descriptor.FuseDelay = 0.05f;
                            missile.OnTriggerEnter(_targetCollider);
                            missile.OnTriggerEnter(_targetCollider);
                            Require(missile.DelayedCollisions == 0, "delayed-fuse-fired-immediately");
                            while (missile.DelayedCollisions == 0)
                            {
                                Require(Time.realtimeSinceStartup < deadline, "delayed-fuse-timeout");
                                yield return null;
                            }
                            yield return null;
                            Require(missile.DelayedCollisions == 1 && missile.GetComponent<RuntimeTimeFuse>() == null,
                                "delayed-fuse-repeat-or-marker-leak");
                            Debug.Log("[AGMLIB Test] event=warhead-network-delayed-fuse result=success scenario=duplicate-triggers");
                            _descriptor.FuseDelay = 0;
                        }
                        else
                            runtime.TriggerDetonate(_target, hit, out _);
                    }
                    finally
                    {
                        hit.Dispose();
                    }
                }
                else
                    runtime.TriggerDetonate(null!, null!, out _);
                // Physical impacts also emit an ordinary effect after the custom event.
                ((LookaheadMunitionBase)missile).Internals().RpcRollImpactEffect(HitResult.Stopped, 0, -1,
                    Position, Quaternion.identity, null!);
                if (index % 10 == 9)
                {
                    NetworkServer.Destroy(missile.gameObject);
                    missile = null;
                }
                else
                {
                    missile.RepoolSelf();
                    while (missile.gameObject.activeSelf)
                    {
                        Require(Time.realtimeSinceStartup < deadline, "repool-timeout");
                        yield return null;
                    }
                }
                yield return new WaitForSecondsRealtime(0.2f);
            }
            missile = CreateMissile();
            NetworkServer.Spawn(missile.gameObject, AssetId);
            missile.Unpool(Position, Quaternion.identity);
            GameObject scenery = new("Network Scenery");
            MunitionHitInfo sceneryHit = MunitionHitInfo.Take();
            try
            {
                sceneryHit.HitObject = scenery;
                sceneryHit.Point = sceneryHit.LocalPoint = Position;
                sceneryHit.Normal = sceneryHit.LocalNormal = Vector3.back;
                Require(missile.ImpactScenery(sceneryHit), "native-scenery-impact");
            }
            finally
            {
                sceneryHit.Dispose();
                Destroy(scenery);
            }
            while (missile.gameObject.activeSelf)
            {
                Require(Time.realtimeSinceStartup < deadline, "scenery-repool-timeout");
                yield return null;
            }
        }
        while (NetworkExplosionProbe.Count < Explosions || NetworkBeamProbe.Count < Detonations * Rays)
        {
            Require(Time.realtimeSinceStartup < deadline, "effect-delivery-timeout");
            yield return null;
        }
        yield return new WaitForSecondsRealtime(2f);
        Require(NetworkExplosionProbe.Count == Explosions && NetworkBeamProbe.Count == Detonations * Rays,
            "missing-or-duplicate-effects");
        Require(NetworkBeamProbe.Hits == Detonations * Rays * 3 / 4, "wrong-beam-hit-presentation");
        Require(NetworkBeamProbe.WrongDirections == 0, "wrong-beam-direction");
        Require(_target.Hits == (_host ? Detonations * Rays * 3 / 4 : 0), "damage-authority-or-count");
        Require(!_host || _repoolCount == 38, "pool-lifecycle-count");
        Debug.Log($"[AGMLIB Test] event=warhead-network-summary role={Role} result=success explosions={NetworkExplosionProbe.Count} beams={NetworkBeamProbe.Count} beamHits={NetworkBeamProbe.Hits} wrongDirections={NetworkBeamProbe.WrongDirections} damageCalls={_target.Hits} detonations={Detonations} repools={_repoolCount}");
        yield return new WaitForSecondsRealtime(2f);
        Application.Quit();
    }

    private void BuildDescriptor()
    {
        _descriptor = ScriptableObject.CreateInstance<BeamWarheadDescriptor>();
        _descriptor.BeamCount = Rays;
        _descriptor.LaunchAngle = 0;
        _descriptor.BeamLength = 200;
        _descriptor.DamagerSettings = ScriptableObject.CreateInstance<SingleRayDamagerSettings>();
        GameObject explosion = new("Network Explosion Prefab");
        explosion.SetActive(false);
        explosion.AddComponent<NetworkExplosionProbe>();
        explosion.AddComponent<ModularEffect>();
        _descriptor.ExplosionPrefabs = [explosion];
        GameObject beam = new("Network Beam Prefab");
        beam.SetActive(false);
        beam.AddComponent<NetworkBeamProbe>();
        beam.AddComponent<ModularEffect>();
        _descriptor.BeamPrefab = beam;
        _target = new GameObject("Network Ray Target").AddComponent<WarheadAdapterTarget>();
        _targetCollider = _target.gameObject.AddComponent<BoxCollider>();
        _target.transform.position = Position + Vector3.forward * 10;
    }

    private NetworkWarheadMissile CreateMissile()
    {
        GameObject root = new("Network Adapter Missile");
        root.SetActive(false);
        root.transform.position = Position;
        root.AddComponent<NetworkIdentity>();
        NetworkWarheadMissile missile = root.AddComponent<NetworkWarheadMissile>();
        missile.SetPool(this);
        root.AddComponent<WarheadAdapterRuntime>().OnAdded(missile, _descriptor);
        root.SetActive(true);
        return missile;
    }

    private string Role => _host ? "host" : "client";
    public void Repool(NetworkPoolable missile) => _repoolCount++;
    private void Require(bool condition, string reason)
    {
        if (!condition)
            throw new InvalidOperationException($"Warhead network fixture failed: role={Role} reason={reason} explosions={NetworkExplosionProbe.Count} beams={NetworkBeamProbe.Count} repools={_repoolCount}");
    }
}
#endif
