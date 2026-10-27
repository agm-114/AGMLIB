#if DEBUG && AGMLIB_LOCAL_TEST_FIXTURES
using Lib.Generic_Gameplay;
using UnityEngine.SceneManagement;

namespace Lib.Testing;

[TestingComponentFactory(Order = 131)]
public class BlastDamageTestingFactory : ITestingComponentFactory
{
    public void CreateTestingComponents(TestingComponentContext context)
    {
        if (Environment.GetEnvironmentVariable("AGMLIB_BLAST_DAMAGE_TEST") == "1")
            new GameObject("Blast Damage Tests").AddComponent<BlastDamageTests>();
    }
}

internal class BlastDamagePart : ISubDamageable
{
    public Vector3 Position => Vector3.zero;
    public bool Structural => false;
    public float CurrentHealth { get; private set; } = 100;
    public bool IsDestroyed => CurrentHealth <= 0;
    public bool DoDamage(float damage, IDamageCharacteristic character, MunitionHitInfo hitInfo)
    {
        CurrentHealth -= damage;
        return IsDestroyed;
    }
}

internal class BlastDamageTarget : MonoBehaviour, IDamageable
{
    public int Hits;
    public float Brush;
    public GameObject GameObj => gameObject;
    public float BoundingRadius => 1;
    public float TargetSize => 1;
    public Vector3 LocalDirection(Vector3 world) => transform.InverseTransformDirection(world);
    public float ApplyDamageReduction(float damage) => damage;
    public HitResult DoDamage(MunitionHitInfo hit, IDamageDealer dealer, out float damage, out bool destroyed)
    {
        Hits++;
        Brush = dealer.DamageBrushSize;
        damage = 0;
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
    public Vector3 RandomPointInBounds() => Vector3.zero;
    public RaycastHit? FindRandomExteriorPoint(Vector3 point, Vector3 direction, float radius) => null;
    public object GetColliderGroup(Collider collider) => collider;
}

internal class BlastDamageTests : MonoBehaviour
{
    private static readonly Vector3 Origin = new(100000, 100000, 100000);
    private static readonly Func<ScalingAOEExplosionEffectModule, IEnumerator> Explode =
        AccessTools.MethodDelegate<Func<ScalingAOEExplosionEffectModule, IEnumerator>>(
            AccessTools.Method(typeof(ScalingAOEExplosionEffectModule), "CoroutineExplode"));
    private readonly List<GameObject> _objects = [];
    private int _checks;
    private int _failures;

    private IEnumerator Start()
    {
        DontDestroyOnLoad(gameObject);
        Run("independent-armor-and-component-curves", () =>
        {
            ScalingAOEExplosionEffectModule blast = Blast();
            ConfigureScaling(blast);
            Require(Mathf.Approximately(((IDamageCharacteristic)blast).ArmorPenetration, 15), "wrong-armor-scaling");
            Set(blast, "_scaleDamageOverArea", false);
            Require(Mathf.Approximately(((IDamageCharacteristic)blast).ArmorPenetration, 30), "wrong-unattenuated-armor-scaling");
            Set(blast, "_scaleDamageOverArea", true);
            BlastDamagePart part = new();
            BlastDamageTarget target = Target(Vector3.forward);
            MunitionHitInfo hit = MunitionHitInfo.Take();
            try
            {
                ((IDamageDealer)blast).DamageComponents(target, [part], hit, HitResult.Penetrated, out float damage);
                Require(Mathf.Approximately(damage, 5) && Mathf.Approximately(part.CurrentHealth, 95), "wrong-component-scaling");
            }
            finally { hit.Dispose(); }
        });
        Run("destroyed-collider-does-not-abort-blast", () =>
        {
            ScalingAOEExplosionEffectModule blast = Blast();
            BlastDamageTarget removed = Target(Vector3.forward);
            BlastDamageTarget survivor = Target(Vector3.back);
            IEnumerator explosion = Explode(blast);
            try
            {
                Require(explosion.MoveNext(), "no-collection-frame");
                DestroyImmediate(removed.GetComponent<Collider>());
                Require(!explosion.MoveNext() && survivor.Hits == 1, "survivor-was-not-damaged");
            }
            finally { (explosion as IDisposable)?.Dispose(); }
        });
        CheckBrush("moving-target-outside-radius", Vector3.forward * 20, 0);
        CheckBrush("target-at-exact-radius", Vector3.forward * 10.5f, 0);
        CheckBrush("stationary-target-inside-radius", Vector3.forward, Mathf.Sqrt(100 - 0.25f));
        Debug.Log($"[AGMLIB Test] event=blast-damage-adapters result={(_failures == 0 ? "success" : "failure")} checks={_checks} failures={_failures}");
        string? fleetPath = Environment.GetEnvironmentVariable("AGMLIB_BLAST_DAMAGE_FLEET");
        if (!string.IsNullOrWhiteSpace(fleetPath))
            yield return CheckShipDamage(fleetPath);
        Debug.Log($"[AGMLIB Test] event=blast-damage-summary result={(_failures == 0 ? "success" : "failure")} checks={_checks} failures={_failures}");
        Application.Quit();
    }

    private void CheckBrush(string name, Vector3 finalPosition, float expected)
    {
        Run(name, () =>
        {
            ScalingAOEExplosionEffectModule blast = Blast();
            BlastDamageTarget target = Target(Vector3.forward);
            IEnumerator explosion = Explode(blast);
            try
            {
                Require(explosion.MoveNext(), "no-collection-frame");
                target.transform.position = Origin + finalPosition;
                Physics.SyncTransforms();
                Require(!explosion.MoveNext() && target.Hits == 1 && Mathf.Approximately(target.Brush, expected),
                    $"wrong-blast-footprint brush={target.Brush} expected={expected}");
            }
            finally { (explosion as IDisposable)?.Dispose(); }
        });
    }

    private IEnumerator CheckShipDamage(string fleetPath)
    {
        float deadline = Time.realtimeSinceStartup + 120;
        while (!BundleManager.IsInitialized)
        {
            Require(Time.realtimeSinceStartup < deadline, "bundle-loading-timeout");
            yield return null;
        }
        yield return new WaitForSecondsRealtime(2);
        Require(SceneManager.GetActiveScene().buildIndex == 0, "main-menu-required");
        Fleet.FleetSummary fleet = Fleet.GetSummaryForFleet(new FilePath(fleetPath));
        Require(fleet != null && !fleet.NeedsUpgrade && !fleet.HasMixedFactionHulls, "invalid-test-fleet");
        CrossScenePipe.SetData(new FleetEditor.QuickFleetTestData
        {
            PlayerFleet = fleet,
            TargetFleet = fleet,
            BraindeadTarget = true,
            Map = BundleManager.Instance.GetMap("Stock-Maps/ShootingGalleryInfo")
        });
        SceneManager.LoadScene(6);
        deadline = Time.realtimeSinceStartup + 180;
        Game.SkirmishGameManager? manager;
        while ((manager = FindObjectOfType<Game.SkirmishGameManager>()) == null || !manager.IsGameActive)
        {
            Require(Time.realtimeSinceStartup < deadline, "range-loading-timeout");
            yield return null;
        }
        Require(manager.IsHost, "server-required");
        ShipController ship = manager.SkirmishLocalPlayer.PlayerFleet.FleetShipControllers.First();
        IDamageable target = ship;
        RaycastHit exterior = FindArmorHit(ship);
        MunitionHitInfo hit = MunitionsHelpers.RaycastHitToMunitionHit(exterior, -exterior.normal, ship.gameObject);
        try
        {
            hit.HitUV = target.SampleUV((MeshCollider)exterior.collider, hit.LocalPoint, hit.LocalNormal);
            Require(hit.HitUV.HasValue, "no-armor-uv");
            ScalingAOEExplosionEffectModule blast = Blast();
            ConfigureScaling(blast);
            int armorIndex = ship.Ship.Hull.GetDynamicArmorIndex(((MeshCollider)exterior.collider).sharedMesh);
            float beforeArmor = ArmorRemaining(ship, armorIndex, hit);
            Require(beforeArmor > 0, "armor-already-empty");
            // The old component curve leaves penetration below this armor threshold;
            // the independent armor curve places it above the threshold.
            Set(blast, "_armorPenetration", beforeArmor / 1.25f);
            HitResult armorResult = target.DoArmorDamageOnly(hit, blast);
            float afterArmor = ArmorRemaining(ship, armorIndex, hit);
            deadline = Time.realtimeSinceStartup + 5;
            // Native GPU armor painting/readback completes asynchronously.
            while (afterArmor >= beforeArmor && Time.realtimeSinceStartup < deadline)
            {
                yield return null;
                afterArmor = ArmorRemaining(ship, armorIndex, hit);
            }
            HullComponent component = ship.Ship.Hull.AllComponents.First(part => !part.IsDestroyed && part.CurrentHealth > 10);
            float beforeHealth = component.CurrentHealth;
            ((IDamageDealer)blast).DamageComponents(target, [component], hit, armorResult, out float damage);
            ship.MarkAsDamaged();
            deadline = Time.realtimeSinceStartup + 5;
            // Public component HP changes after native damage-frame replication.
            while (component.CurrentHealth >= beforeHealth && Time.realtimeSinceStartup < deadline)
                yield return null;
            Run("real-ship-armor-and-component-damage", () =>
            {
                Require(armorResult >= HitResult.Penetrated, "armor-threshold-not-penetrated");
                Require(afterArmor < beforeArmor, "armor-did-not-decrease");
                Require(component.CurrentHealth < beforeHealth && damage > 0, "component-health-did-not-decrease");
                Debug.Log($"[AGMLIB Test] event=blast-damage-real-ship penetration={((IDamageCharacteristic)blast).ArmorPenetration} armorResult={armorResult} armorBefore={beforeArmor} armorAfter={afterArmor} healthBefore={beforeHealth} healthAfter={component.CurrentHealth} damage={damage}");
            });
        }
        finally { hit.Dispose(); }
    }

    private static float ArmorRemaining(ShipController ship, int index, MunitionHitInfo hit)
    {
        ship.Ship.Hull.WouldArmorHitPenetrate(index, hit.HitUV!.Value, hit.Normal, -hit.HitNormal, 0, false,
            out float remaining, out _, out _);
        return remaining;
    }

    private static RaycastHit FindArmorHit(ShipController ship)
    {
        foreach (MeshCollider collider in ship.GetComponentsInChildren<MeshCollider>())
        {
            if (!collider.enabled || collider.isTrigger || ship.Ship.Hull.GetDynamicArmorIndex(collider.sharedMesh) < 0)
                continue;
            foreach (Vector3 direction in new[] { Vector3.forward, Vector3.back, Vector3.left, Vector3.right, Vector3.up, Vector3.down })
            {
                float distance = collider.bounds.extents.magnitude + 5;
                if (collider.Raycast(new Ray(collider.bounds.center + direction * distance, -direction), out RaycastHit hit, distance * 2))
                    return hit;
            }
        }
        throw new InvalidOperationException("No live armor mesh could be raycast.");
    }

    private static void ConfigureScaling(ScalingAOEExplosionEffectModule blast)
    {
        blast.SetWeight(2);
        Set(blast, "_armorPenetrationWeightScaling", AnimationCurve.Constant(0, 10, 3));
        Set(blast, "_componentDamageWeightScaling", AnimationCurve.Constant(0, 10, 2));
        Set(blast, "_armorPenetrationRangeScaling", AnimationCurve.Constant(0, 1, 0.5f));
        Set(blast, "_componentDamageRangeScaling", AnimationCurve.Constant(0, 1, 0.25f));
        Set(blast, "_scaleDamageOverArea", true);
    }

    private ScalingAOEExplosionEffectModule Blast()
    {
        ScalingAOEExplosionEffectModule blast = Root("Blast", Origin).AddComponent<ScalingAOEExplosionEffectModule>();
        Set(blast, "_blastRadius", 10f);
        return blast;
    }

    private BlastDamageTarget Target(Vector3 offset)
    {
        GameObject root = Root("Blast Target", Origin + offset);
        root.AddComponent<BoxCollider>();
        BlastDamageTarget target = root.AddComponent<BlastDamageTarget>();
        Physics.SyncTransforms();
        return target;
    }

    private GameObject Root(string name, Vector3 position)
    {
        GameObject root = new(name);
        root.transform.position = position;
        _objects.Add(root);
        return root;
    }

    private void Run(string name, Action check)
    {
        _checks++;
        try
        {
            check();
            Debug.Log($"[AGMLIB Test] event=blast-damage-check name={name} result=success");
        }
        catch (Exception exception)
        {
            _failures++;
            Debug.LogError($"[AGMLIB Test] event=blast-damage-check name={name} result=failure detail={exception}");
        }
        finally
        {
            _objects.ForEach(UnityEngine.Object.DestroyImmediate);
            _objects.Clear();
            Physics.SyncTransforms();
        }
    }

    private static void Set<T>(ScalingAOEExplosionEffectModule blast, string field, T value) =>
        AccessTools.Field(typeof(ScalingAOEExplosionEffectModule), field).SetValue(blast, value);

    private static void Require(bool condition, string reason)
    {
        if (!condition)
            throw new InvalidOperationException($"Blast damage fixture failed: {reason}");
    }
}
#endif
