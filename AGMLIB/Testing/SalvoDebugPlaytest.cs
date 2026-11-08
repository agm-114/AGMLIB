#if DEBUG && AGMLIB_LOCAL_TEST_FIXTURES
using Lib.Diagnostics;
using global::Munitions.ModularMissiles;
using UnityEngine.SceneManagement;

namespace Lib.Testing;

internal class SalvoDebugPlaytest : MonoBehaviour
{
    private IEnumerator Start()
    {
        DontDestroyOnLoad(gameObject);
        float deadline = Time.realtimeSinceStartup + 120;
        while (!BundleManager.IsInitialized)
        {
            Require(Time.realtimeSinceStartup < deadline, "bundle-timeout");
            yield return null;
        }
        Fleet.FleetSummary fleet = Fleet.GetSummaryForFleet(new FilePath(
            Environment.GetEnvironmentVariable("AGMLIB_SALVO_DEBUG_PLAYTEST_FLEET")));
        Require(fleet != null && !fleet.NeedsUpgrade, "invalid-fleet");
        CrossScenePipe.SetData(new QuickFleetTestData
        {
            PlayerFleet = fleet,
            TargetFleet = fleet,
            BraindeadTarget = true,
            Map = BundleManager.Instance.GetMap("Stock-Maps/ShootingGalleryInfo")
        });
        SceneManager.LoadScene((int)SceneRegistry.SkirmishLoad);
        SkirmishGameManager? manager;
        deadline = Time.realtimeSinceStartup + 180;
        while ((manager = FindObjectOfType<SkirmishGameManager>()) == null || !manager.IsGameActive)
        {
            Require(Time.realtimeSinceStartup < deadline, "range-timeout");
            yield return null;
        }
        ShipController launcher = manager.SkirmishLocalPlayer.PlayerFleet.FleetShipControllers.First();
        ShipController target = FindObjectsOfType<ShipController>().First(ship => ship.OwnedBy != launcher.OwnedBy);
        ModularMissile pattern = manager.SkirmishLocalPlayer.PlayerFleet.AvailableMunitions.GetMunition(
            Environment.GetEnvironmentVariable("AGMLIB_SALVO_DEBUG_PLAYTEST_KEY")) as ModularMissile
            ?? throw new InvalidOperationException("Playtest missile was not found.");
        int salvoId = ActiveMissileSalvo.NextSalvoID(false);
        for (int i = 0; i < 3; i++)
        {
            Vector3 origin = target.transform.position + Vector3.forward * 400 + Vector3.right * (i * 12);
            ModularMissile missile = NetworkObjectPooler.Instance.GetNextOrNew<ModularMissile>(pattern.gameObject,
                origin, Quaternion.LookRotation(target.transform.position - origin));
            missile.SetProgramPath(launcher, [target.transform.position]);
            missile.Launch(launcher, salvoId, true, true, true);
            yield return new WaitForSeconds(0.3f);
        }
        SalvoDebugRecorder recorder = SalvoDebugRecorder.Instance!;
        SalvoTrace? report;
        deadline = Time.realtimeSinceStartup + 90;
        while ((report = recorder.History.FirstOrDefault(item => item.SalvoId == salvoId)) == null || !report.Finished)
        {
            Require(Time.realtimeSinceStartup < deadline, "missile-timeout");
            yield return null;
        }
        SalvoDebugWindow window = recorder.GetComponent<SalvoDebugWindow>();
        window.Select([report]);
        List<DamageTraceStep> internalRays = report.Missiles.SelectMany(item => item.Rays)
            .SelectMany(ray => ray.Steps).Where(step => step.HasSegment).ToList();
        Require(internalRays.Any(step => step.DamageEvaluated && step.ReportedDamage > 0), "missing-internal-damage-outcomes");
        if (report.Missiles.SelectMany(item => item.Rays).FirstOrDefault() is { } ray)
        {
            window.Pin(ray);
            window.FocusImpact();
        }
        Debug.Log($"[AGMLIB Test] event=salvo-debug-playtest-ready mode=testing-range missiles={report.Missiles.Count} rays={report.Missiles.Sum(item => item.Rays.Count)} capture={recorder.Recording} internalGreen={internalRays.Count(step => step.Outcome == RayDamageOutcome.Damage)} internalAmber={internalRays.Count(step => step.Outcome == RayDamageOutcome.NoDamage)} internalGray={internalRays.Count(step => step.Outcome == RayDamageOutcome.Miss)}");
        Destroy(gameObject);
    }

    private static void Require(bool condition, string reason)
    {
        if (!condition)
            throw new InvalidOperationException($"Salvo interactive range failed: {reason}");
    }
}
#endif
