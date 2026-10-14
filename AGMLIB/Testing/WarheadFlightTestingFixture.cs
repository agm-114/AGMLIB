#if DEBUG && AGMLIB_LOCAL_TEST_FIXTURES
using Munitions.ModularMissiles;
using Munitions.ModularMissiles.Runtime;
using UnityEngine.SceneManagement;

namespace Lib.Testing;

internal class WarheadFlightProbe : MonoBehaviour
{
    public int Collisions;
    public int TriggerCollisions;
    public int SceneryImpacts;
    public int Detonations;
    public int DamageEffects;
    public int PresentationEffects;
    public int BeamEvents;
    public int BeamHits;
    public int WrongDirections;
    public float DetonationRange;
    public Transform Target = null!;
}

internal class WarheadFlightTests : MonoBehaviour
{
    private IEnumerator Start()
    {
        DontDestroyOnLoad(gameObject);
        float deadline = Time.realtimeSinceStartup + 120f;
        while (!BundleManager.IsInitialized)
        {
            Require(Time.realtimeSinceStartup < deadline, "bundle-loading-timeout");
            yield return null;
        }
        yield return new WaitForSecondsRealtime(2f);
        Require(SceneManager.GetActiveScene().buildIndex == 0, "main-menu-required");
        Fleet.FleetSummary fleet = Fleet.GetSummaryForFleet(new FilePath(
            Environment.GetEnvironmentVariable("AGMLIB_WARHEAD_FLIGHT_FLEET")));
        Require(fleet != null && !fleet.NeedsUpgrade && !fleet.HasMixedFactionHulls, "invalid-test-fleet");
        CrossScenePipe.SetData(new FleetEditor.QuickFleetTestData
        {
            PlayerFleet = fleet,
            TargetFleet = fleet,
            BraindeadTarget = true,
            Map = BundleManager.Instance.GetMap("Stock-Maps/ShootingGalleryInfo")
        });
        SceneManager.LoadScene(6);
        deadline = Time.realtimeSinceStartup + 180f;
        Game.SkirmishGameManager? manager;
        while ((manager = FindObjectOfType<Game.SkirmishGameManager>()) == null || !manager.IsGameActive)
        {
            Require(Time.realtimeSinceStartup < deadline, "testing-range-timeout");
            yield return null;
        }
        Require(manager.IsHost && NetworkObjectPooler.Instance.IsHost, "server-required");
        ShipController launcher = manager.SkirmishLocalPlayer.PlayerFleet.FleetShipControllers.First();
        Vector3 origin = launcher.transform.position + Vector3.up * 10000;
        WarheadAdapterTarget target = new GameObject("Native Flight Damage Target").AddComponent<WarheadAdapterTarget>();
        target.transform.position = origin + Vector3.forward * 80;
        BoxCollider targetCollider = target.gameObject.AddComponent<BoxCollider>();
        targetCollider.size = Vector3.one;
        bool scenery = Environment.GetEnvironmentVariable("AGMLIB_WARHEAD_FLIGHT_SCENERY") == "1";
        if (scenery)
            DestroyImmediate(target);
        Physics.SyncTransforms();
        string[] keys = (Environment.GetEnvironmentVariable("AGMLIB_WARHEAD_FLIGHT_KEYS") ?? "").Split('|');
        int launches = 0;
        foreach (string key in keys)
        {
            ModularMissile pattern = manager.SkirmishLocalPlayer.PlayerFleet.AvailableMunitions.GetMunition(key) as ModularMissile
                ?? throw new InvalidOperationException("Test missile pattern was not registered.");
            for (int reuse = 0; reuse < 3; reuse++)
            {
                int hits = scenery ? 0 : target.Hits;
                ModularMissile missile = NetworkObjectPooler.Instance.GetNextOrNew<ModularMissile>(
                    pattern.gameObject, origin, Quaternion.identity);
                RuntimeMissileWarhead runtime = missile.GetComponent<RuntimeMissileWarhead>();
                Require(missile.GetComponents<RuntimeMissileWarhead>().Length == 1, "duplicate-runtime-warheads");
                Require(runtime != null && runtime.Missile == missile && runtime.Internals().Descriptor is BeamWarheadDescriptor,
                    "cloned-warhead-wiring");
                BeamWarheadDescriptor descriptor = (BeamWarheadDescriptor)runtime.Internals().Descriptor;
                WarheadFlightProbe probe = missile.gameObject.AddComponent<WarheadFlightProbe>();
                probe.Target = targetCollider.transform;
                bool proximity = missile.GetComponentsInChildren<Collider>(true)
                    .Any(collider => collider.isTrigger && collider.gameObject.layer == 22);
                missile.SetProgramPath(launcher, [targetCollider.transform.position]);
                missile.Launch(launcher, -1, true, true, true);
                Debug.Log($"[AGMLIB Test] event=warhead-native-launch key={key} reuse={reuse} netId={missile.netId} rays={descriptor.EffectiveBeamCount} weight={descriptor.WeightedSocketSize} finalStage={missile.FinalStage} phase={missile.FlightPhase}");
                deadline = Time.realtimeSinceStartup + 30f;
                while (probe.Detonations + probe.SceneryImpacts == 0)
                {
                    Require(Time.realtimeSinceStartup < deadline && !missile.IsDead, "flight-without-detonation");
                    yield return null;
                }
                yield return new WaitForSecondsRealtime(2f);
                int damagePrefabs = descriptor.ExplosionPrefabs.Count(prefab => prefab.GetComponentsInChildren<IDamageDealer>(true).Length > 0);
                int expectedDamageEffects = scenery ? 0 : damagePrefabs;
                int expectedBeams = scenery ? 0 : (int)Math.Ceiling(descriptor.EffectiveBeamCount);
                int expectedPresentation = descriptor.ExplosionPrefabs.Count - damagePrefabs + expectedBeams;
                Debug.Log($"[AGMLIB Test] event=warhead-native-effects scenery={scenery} damageEffects={probe.DamageEffects}/{expectedDamageEffects} presentationEffects={probe.PresentationEffects}/{expectedPresentation}");
                Require(probe.Collisions > 0 && probe.Detonations == (scenery ? 0 : 1)
                    && probe.SceneryImpacts == (scenery ? 1 : 0), "native-collision-count");
                Require(probe.TriggerCollisions == (proximity ? 1 : 0), "native-fuse-path");
                Require(probe.DamageEffects == expectedDamageEffects && probe.PresentationEffects == expectedPresentation,
                    "native-effect-count");
                Require(probe.BeamEvents == expectedBeams, "native-beam-delivery-count");
                Require(probe.BeamHits == probe.BeamEvents && probe.WrongDirections == 0, "native-beam-aim");
                Require(!proximity || probe.BeamEvents == 0 || probe.DetonationRange > 20f, "native-beam-standoff-distance");
                int damageCalls = scenery ? 0 : target.Hits - hits;
                Require(scenery || damageCalls > 0, "native-damage-count");
                Debug.Log($"[AGMLIB Test] event=warhead-native-impact result=success key={key} reuse={reuse} netId={missile.netId} collisions={probe.Collisions} triggers={probe.TriggerCollisions} detonations={probe.Detonations} damageEffects={probe.DamageEffects} presentationEffects={probe.PresentationEffects} beams={probe.BeamEvents} beamHits={probe.BeamHits} wrongDirections={probe.WrongDirections} distance={probe.DetonationRange} damageCalls={damageCalls}");
                launches++;
                Destroy(probe);
                deadline = Time.realtimeSinceStartup + 20f;
                while (missile.gameObject.activeSelf)
                {
                    Require(Time.realtimeSinceStartup < deadline, "native-repool-timeout");
                    yield return null;
                }
            }
        }
        Debug.Log($"[AGMLIB Test] event=warhead-native-summary result=success launches={launches} damageCalls={(scenery ? 0 : target.Hits)}");
        Application.Quit();
    }

    private static void Require(bool condition, string reason)
    {
        if (!condition)
            throw new InvalidOperationException($"Warhead native flight fixture failed: {reason}");
    }
}

[HarmonyPatch(typeof(ModularMissile), "GenericImpact")]
internal static class WarheadFlightSceneryObserver
{
    private static void Postfix(ModularMissile __instance)
    {
        if (__instance.GetComponent<WarheadFlightProbe>() is { } probe)
            probe.SceneryImpacts++;
    }
}

[HarmonyPatch(typeof(ModularMissile), nameof(ModularMissile.ProcessCollision))]
internal static class WarheadFlightCollisionObserver
{
    private static void Prefix(ModularMissile __instance, bool trigger)
    {
        if (__instance.GetComponent<WarheadFlightProbe>() is { } probe)
        {
            probe.Collisions++;
            if (trigger)
                probe.TriggerCollisions++;
        }
    }
}

[HarmonyPatch(typeof(AngleWarheadDescriptor), nameof(AngleWarheadDescriptor.TriggerDetonate))]
internal static class WarheadFlightDetonationObserver
{
    private static void Postfix(RuntimeMissileWarhead runtime)
    {
        if (runtime.Missile.GetComponent<WarheadFlightProbe>() is { } probe)
        {
            probe.Detonations++;
            probe.DetonationRange = Vector3.Distance(runtime.transform.position, probe.Target.position);
        }
    }
}

[HarmonyPatch(typeof(AngleWarheadDescriptor), nameof(AngleWarheadDescriptor.SpawnEffect))]
internal static class WarheadFlightEffectObserver
{
    private static void Postfix(Poolable __result, RuntimeMissileWarhead runtime)
    {
        if (__result == null || runtime.Missile.GetComponent<WarheadFlightProbe>() is not { } probe)
            return;
        if (__result.GetComponentsInChildren<IDamageDealer>(true).Length > 0)
            probe.DamageEffects++;
        else
            probe.PresentationEffects++;
    }
}

[HarmonyPatch(typeof(LookaheadMunitionBase), "UserCode_RpcRollImpactEffect")]
internal static class WarheadFlightBeamObserver
{
    private static void Postfix(LookaheadMunitionBase __instance, int effectSet, HitResult hitRes, Vector3 position, Quaternion rotation)
    {
        if (effectSet == -2 && __instance.GetComponent<WarheadFlightProbe>() is { } probe)
        {
            probe.BeamEvents++;
            if (hitRes != HitResult.None)
                probe.BeamHits++;
            if (Vector3.Angle(rotation * Vector3.forward, probe.Target.position - position) > 0.1f)
                probe.WrongDirections++;
        }
    }
}
#endif
