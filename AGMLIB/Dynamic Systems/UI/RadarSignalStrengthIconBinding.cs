using Game.UI;
using UI;
using Utility.Localization;

internal sealed class RadarSignalStrengthIconBinding : MonoBehaviour
{
    private const float RefreshPeriodSeconds = 0.2f;

    private RadarSignalStrengthComponent? _source;
    private TooltipTrigger? _tooltip;
    private long _renderedRevision = -1;
    private float _nextRefreshTime;

    internal void SetSource(RadarSignalStrengthComponent source, TrackedStatusIcon trackedIcon)
    {
        ClearSource();
        _source = source;
        _tooltip = Common.GetVal<TooltipTrigger>(trackedIcon, "_tooltip", typeof(StatusIcon));
        _tooltip?.SetGetTextCallback(BuildTooltipText);
        _renderedRevision = -1;
        _nextRefreshTime = 0f;
    }

    internal void ClearSource()
    {
        _tooltip?.SetGetTextCallback(null);
        _tooltip = null;
        _source = null;
        _renderedRevision = -1;
        _nextRefreshTime = 0f;
    }

    private string BuildTooltipText()
    {
        if (_source == null)
        {
            return "Radar Detection";
        }

        string text = _source.BuildTooltipText();
        _renderedRevision = _source.Revision;
        _nextRefreshTime = Time.unscaledTime + RefreshPeriodSeconds;
        return text;
    }

    internal void RefreshOpenTooltip()
    {
        if (_source == null
            || _tooltip == null
            || _source.Revision == _renderedRevision
            || Time.unscaledTime < _nextRefreshTime)
        {
            return;
        }

        RefreshTooltipText(BuildTooltipText().Localize());
    }

    private static void RefreshTooltipText(string text)
    {
        Tooltip? tooltip = Tooltip.Instance;
        if (tooltip == null)
        {
            return;
        }

        TooltipInternals internals = tooltip.Internals();
        TextMeshProUGUI tooltipText = internals.TooltipText;
        tooltipText.text = text;
        tooltipText.overflowMode = TextOverflowModes.Overflow;
        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltip.transform as RectTransform);
        tooltipText.ForceMeshUpdate();

        int lineCount = tooltipText.textInfo.lineCount;
        if (lineCount > internals.MaxLines)
        {
            lineCount = Mathf.CeilToInt(lineCount / 2f);
            internals.OverflowText.gameObject.SetActive(value: true);
        }
        else
        {
            internals.OverflowText.gameObject.SetActive(value: false);
        }

        tooltipText.overflowMode = TextOverflowModes.Linked;
        internals.TextLayout.minHeight = lineCount * tooltipText.fontSize;
        tooltipText.ForceMeshUpdate();
        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltip.transform as RectTransform);
    }

    private void OnDestroy() => ClearSource();
}

[HarmonyPatch(typeof(TooltipTrigger), "Update")]
internal static class RadarSignalStrengthOpenTooltipRefreshPatch
{
    private static void Postfix(TooltipTrigger __instance)
    {
        if (__instance.Internals().IsOpen)
        {
            __instance.GetComponent<RadarSignalStrengthIconBinding>()?.RefreshOpenTooltip();
        }
    }
}

[HarmonyPatch(typeof(ShipStatusIconGroup), nameof(ShipStatusIconGroup.SetShip))]
internal static class ShipStatusIconGroupSetShipRadarSignalStrengthPatch
{
    private static void Postfix(ShipStatusIconGroup __instance, ShipController ship)
    {
        TrackedStatusIcon trackedIcon = Common.GetVal<TrackedStatusIcon>(__instance, "_trackedIcon");
        if (trackedIcon == null)
        {
            return;
        }

        RadarSignalStrengthIconBinding? binding = trackedIcon.GetComponent<RadarSignalStrengthIconBinding>();
        RadarSignalStrengthComponent? source = ship?
            .GetComponentsInChildren<RadarSignalStrengthComponent>(includeInactive: true)
            .FirstOrDefault(component => component.isActiveAndEnabled);
        if (source == null)
        {
            binding?.ClearSource();
            if (ship?.Trackable is SensorTrackableObject trackable)
            {
                trackedIcon.SetStatus(trackable.Status, initial: true);
            }

            return;
        }

        binding ??= trackedIcon.gameObject.AddComponent<RadarSignalStrengthIconBinding>();
        binding.SetSource(source, trackedIcon);
    }
}
