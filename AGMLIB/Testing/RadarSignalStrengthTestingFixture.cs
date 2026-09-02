#if DEBUG && AGMLIB_LOCAL_TEST_FIXTURES
using System;
using System.Collections;
using Game.Units;
using UnityEngine;

namespace Lib.Testing;

[TestingComponentFactory(Order = 110)]
public sealed class RadarSignalStrengthTestingComponentFactory : ITestingComponentFactory
{
    private const string EnabledEnvironmentVariable = "AGMLIB_RADAR_SIGNAL_STRENGTH_TEST";
    private const string SourceSaveKey = "Stock/Reinforced Thruster Nozzles";
    private const string TestingSaveKey = "agmlib-testing/Radar Detection Readout";

    public void CreateTestingComponents(TestingComponentContext context)
    {
        if (!string.Equals(
                Environment.GetEnvironmentVariable(EnabledEnvironmentVariable),
                "1",
                StringComparison.Ordinal))
        {
            return;
        }

        context.Create(SourceSaveKey, TestingSaveKey, builder =>
        {
            builder
                .SetDisplayName("[TEST] Radar Detection Readout")
                .SetCategory("Testing")
                .SetDescription(
                    "Temporary local test fixture. Install it on a ship, enter the Testing Range, "
                    + "then hover the TRACKED status icon to see each hostile radar's signal margin and track quality.")
                .SetPointCost(0)
                .UnlockForAllFactions();

            Common.SetVal(builder.Component, "_size", Vector3Int.one);
            Common.SetVal(builder.Component, "_mass", 0f);
            Common.SetVal(builder.Component, "_bindToTag", "");
            builder.Component.Modifiers = [];

            builder.AddToRoot<RadarSignalStrengthComponent>();
            builder.AddToRoot<RadarSignalStrengthFixtureTelemetry>();
        });
    }
}

internal sealed class RadarSignalStrengthFixtureTelemetry : MonoBehaviour
{
    private IEnumerator Start()
    {
        RadarSignalStrengthComponent? component = GetComponent<RadarSignalStrengthComponent>();
        ShipController? ship = GetComponentInParent<ShipController>();
        if (component == null || ship == null)
        {
            Debug.LogWarning("[AGMLIB Test] event=radar-readout-fixture-skipped reason=missing-runtime-owner");
            yield break;
        }

        Debug.Log($"[AGMLIB Test] event=radar-readout-fixture-active ship={ship.name}");

        const float timeoutSeconds = 120f;
        float deadline = Time.unscaledTime + timeoutSeconds;
        while (component != null && Time.unscaledTime < deadline)
        {
            string tooltip = component.BuildTooltipText();
            if (tooltip.Contains("\nSignal: "))
            {
                long initialRevision = component.Revision;
                Debug.Log(
                    $"[AGMLIB Test] event=radar-readout-reading ship={ship.name} "
                    + $"revision={initialRevision} tooltip={tooltip.Replace('\n', '|')}");

                yield return new WaitForSecondsRealtime(5f);
                long refreshedRevision = component.Revision;
                string refreshedTooltip = component.BuildTooltipText();
                string eventName = refreshedRevision > initialRevision
                    ? "radar-readout-refresh-verified"
                    : "radar-readout-refresh-stalled";
                Debug.Log(
                    $"[AGMLIB Test] event={eventName} ship={ship.name} "
                    + $"initialRevision={initialRevision} refreshedRevision={refreshedRevision} "
                    + $"tooltip={refreshedTooltip.Replace('\n', '|')}");
                yield break;
            }

            yield return new WaitForSecondsRealtime(1f);
        }

        Debug.LogWarning($"[AGMLIB Test] event=radar-readout-reading-timeout ship={ship.name} timeoutSeconds={timeoutSeconds}");
    }
}
#endif
