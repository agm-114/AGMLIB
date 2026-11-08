#if DEBUG && AGMLIB_LOCAL_TEST_FIXTURES
using Lib.Diagnostics;
using global::Munitions.ModularMissiles;
using UnityEngine.SceneManagement;

namespace Lib.Testing;

internal class SalvoDebugShipTests : MonoBehaviour
{
    private IEnumerator Start()
    {
        DontDestroyOnLoad(gameObject);
        float deadline = Time.realtimeSinceStartup + 90;
        while (!BundleManager.IsInitialized)
        {
            Require(Time.realtimeSinceStartup < deadline, "bundle-timeout");
            yield return null;
        }
        yield return new WaitForSecondsRealtime(2);
        Fleet.FleetSummary fleet = Fleet.GetSummaryForFleet(new FilePath(Environment.GetEnvironmentVariable("AGMLIB_SALVO_DEBUG_SHIP_FLEET")));
        Require(fleet != null && !fleet.NeedsUpgrade && !fleet.HasMixedFactionHulls, "invalid-fleet");
        CrossScenePipe.SetData(new QuickFleetTestData
        {
            PlayerFleet = fleet,
            TargetFleet = fleet,
            BraindeadTarget = true,
            Map = BundleManager.Instance.GetMap("Stock-Maps/ShootingGalleryInfo")
        });
        SceneManager.LoadScene(6);
        deadline = Time.realtimeSinceStartup + 120;
        SkirmishGameManager? manager;
        while ((manager = FindObjectOfType<SkirmishGameManager>()) == null || !manager.IsGameActive)
        {
            Require(Time.realtimeSinceStartup < deadline, "match-timeout");
            yield return null;
        }
        Require(manager.IsHost, "host-required");
        ShipController launcher = manager.SkirmishLocalPlayer.PlayerFleet.FleetShipControllers.First();
        ShipController target = FindObjectsOfType<ShipController>().First(ship => ship.OwnedBy != launcher.OwnedBy);
        float beforeHealth = AvailableHealth(target);
        ModularMissile pattern = manager.SkirmishLocalPlayer.PlayerFleet.AvailableMunitions.GetMunition(
            Environment.GetEnvironmentVariable("AGMLIB_SALVO_DEBUG_SHIP_KEY")) as ModularMissile
            ?? throw new InvalidOperationException("Ship test missile was not found.");
        ModularMissile missile = NetworkObjectPooler.Instance.GetNextOrNew<ModularMissile>(pattern.gameObject,
            target.transform.position + Vector3.forward * 400, Quaternion.LookRotation(Vector3.back));
        missile.SetProgramPath(launcher, [target.transform.position]);
        int salvoId = ActiveMissileSalvo.NextSalvoID(false);
        missile.Launch(launcher, salvoId, true, true, true);
        SalvoDebugRecorder recorder = SalvoDebugRecorder.Instance!;
        deadline = Time.realtimeSinceStartup + 90;
        SalvoTrace? report;
        while ((report = recorder.History.FirstOrDefault(item => item.SalvoId == salvoId)) == null || !report.Finished)
        {
            Require(Time.realtimeSinceStartup < deadline, "missile-timeout");
            yield return null;
        }
        yield return new WaitForSecondsRealtime(2);
        List<BeamRayTrace> rays = report.Missiles.SelectMany(item => item.Rays).ToList();
        List<DamageTraceStep> steps = rays.SelectMany(ray => ray.Steps).ToList();
        Require(report.Missiles.Single().Path.Count > 2 && rays.Count > 0, "missing-flight-or-rays");
        Require(rays.All(ray => ray.Completed), "incomplete-ray");
        Require(rays.Sum(ray => ray.ReportedDamage) > 0 && AvailableHealth(target) < beforeHealth, "no-real-health-loss");
        Require(steps.Any(step => step.Description.StartsWith("Armor test:")), "missing-armor-explanation");
        Require(steps.Any(step => step.Description.StartsWith("Ship damage reduction:")), "missing-dr-explanation");
        Require(steps.Any(step => step.HasSegment), "missing-internal-rays");
        Require(steps.Any(step => step.Description.StartsWith("Component ") || step.Description.StartsWith("Structure ")), "missing-allocation");
        SalvoDebugWindow window = recorder.GetComponent<SalvoDebugWindow>();
        window.Visible = false;
        FindObjectsOfType<FriendlySalvoItem>().First().Internals().Button.onClick.Invoke();
        Require(window.Visible, "finished-row-did-not-open");
        window.Pin(rays.First(ray => ray.ReportedDamage > 0));
        window.FocusImpact();
        string? path = window.ExportSelection();
        Require(path != null && File.Exists(path), "export-file-missing");
        SalvoDebugWindow.Export exported = SalvoTraceJson.Read<SalvoDebugWindow.Export>(File.ReadAllText(path!));
        Require(exported.Salvos.Single().Missiles.Sum(item => item.Rays.Count) == rays.Count &&
            Mathf.Approximately(exported.Salvos.Single().Damage, report.Damage), "export-data-mismatch");
        Debug.Log($"[AGMLIB Test] event=salvo-debug-ship-summary result=success rays={rays.Count} steps={steps.Count} damage={report.Damage:N3} healthLoss={beforeHealth - AvailableHealth(target):N3}");
        yield return new WaitForSecondsRealtime(3);
        SalvoDebugWorldView world = recorder.GetComponent<SalvoDebugWorldView>();
        LineRenderer[] lines = world.GetComponentsInChildren<LineRenderer>();
        Require(lines.Length >= rays.Count && lines.All(line => line.useWorldSpace && line.sharedMaterial.shader.isSupported), "invalid-world-renderers");
        Require(world.GetComponentsInChildren<Collider>(true).Length == 0, "debug-geometry-has-colliders");
        Camera camera = Camera.main;
        Require(camera != null && rays.Any(ray => new[] { 0.25f, 0.5f, 0.75f }.Any(along =>
        {
            Vector3 point = camera.WorldToScreenPoint(Vector3.Lerp(ray.Origin, ray.End, along));
            return point.z > camera.nearClipPlane && point.x >= 0 && point.x < Screen.width - 440 &&
                point.y >= 0 && point.y < Screen.height && world.Pick(new Vector2(point.x, Screen.height - point.y)) != null;
        })), "world-ray-hover-mapping");
        window.Visible = false;
        yield return null;
        yield return null;
        Require(lines.All(line => !line.gameObject.activeInHierarchy), "hidden-inspector-left-world-lines");
        window.Visible = true;
        yield return null;
        Debug.Log($"[AGMLIB Test] event=salvo-debug-world-summary result=success lines={lines.Length} hover=true hiddenCleanup=true");
        SalvoDebugTests.CaptureScreenshot();
        yield return new WaitForSecondsRealtime(5);
        Application.Quit();
    }

    private static float AvailableHealth(ShipController ship) => ship.GetComponentsInChildren<MonoBehaviour>()
        .OfType<ISubDamageable>().Sum(part => part.CurrentHealth);

    private static void Require(bool condition, string reason)
    {
        if (!condition)
            throw new InvalidOperationException($"Salvo real-ship test failed: {reason}");
    }
}
#endif
