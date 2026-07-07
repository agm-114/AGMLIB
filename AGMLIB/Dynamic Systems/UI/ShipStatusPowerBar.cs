using System.Globalization;

[DisallowMultipleComponent]
public class ShipStatusPowerBar : MonoBehaviour
{
    public enum PowerStatusVisualMode
    {
        IconMask,
        TriplePercentBars,
        SharedScaleGenerationDemandBars,
    }

    public static bool DebugAutoAttachToAllShips = true;

    [Header("Resource")]
    public string ResourceName = "Power";

    [Header("Status Icon")]
    public bool ShowIconWhenNormal = true;
    public PowerStatusVisualMode VisualMode = PowerStatusVisualMode.SharedScaleGenerationDemandBars;
    public bool DrawFillMask = true;
    public Color MaskColor = new(0f, 0f, 0f, 0.85f);
    public Color GraphBackgroundColor = new(0f, 0f, 0f, 0.8f);
    public Color NeedFillColor = new(0.2f, 1f, 0.35f, 1f);
    public Color ProductionFillColor = new(0.3f, 0.8f, 1f, 1f);
    public Color DemandFillColor = new(1f, 0.15f, 0.1f, 1f);
    public float UpdateInterval = 0.1f;

    [Header("Tooltip")]
    public bool ReplaceTooltip = true;
    public string TooltipTitle = "Power Status";

    private ShipController _shipController;
    public static ShipStatusPowerBar EnsureAttachedTo(ShipController ship)
    {
        if (ship == null)
        {
            return null;
        }

        ShipStatusPowerBar powerBar = ship.GetComponentInChildren<ShipStatusPowerBar>(includeInactive: true);
        if (powerBar == null && DebugAutoAttachToAllShips)
        {
            powerBar = ship.gameObject.AddComponent<ShipStatusPowerBar>();
        }

        return powerBar;
    }

    private void Awake()
    {
        _shipController = GetComponent<ShipController>()
            ?? GetComponentInParent<ShipController>()
            ?? GetComponentInChildren<ShipController>();
    }

    private void OnDisable()
    {
        ShipStatusPowerBarBinding[] bindings = FindObjectsOfType<ShipStatusPowerBarBinding>();
        foreach (ShipStatusPowerBarBinding binding in bindings)
        {
            binding.ClearSource(this);
        }
    }

    public void LinkStatusIcons(ShipStatusIconGroup statusIcons)
    {
        if (_shipController == null || statusIcons == null)
        {
            return;
        }

        QuantityStatusIcon powerIcon = Common.GetVal<QuantityStatusIcon>(statusIcons, "_powerQuantityIcon");
        if (powerIcon == null)
        {
            return;
        }

        ShipStatusPowerBarBinding binding = powerIcon.GetComponent<ShipStatusPowerBarBinding>()
            ?? powerIcon.gameObject.AddComponent<ShipStatusPowerBarBinding>();
        binding.SetSource(this, powerIcon);
    }

    public IReadOnlyResourcePool GetResourcePool()
    {
        if (_shipController?.Ship?.Resources == null)
        {
            return null;
        }

        return _shipController.Ship.Resources.FirstOrDefault(pool => pool.ResourceName == ResourceName);
    }

    public float GetDemandFill(IReadOnlyResourcePool pool)
    {
        return Mathf.Clamp01(GetDemandRatio(pool));
    }

    public float GetPeakProductionFill(IReadOnlyResourcePool pool)
    {
        return GetPeakFill(pool.TotalAvailable, pool.PeakQuantity);
    }

    public float GetPeakDemandFill(IReadOnlyResourcePool pool)
    {
        return GetPeakFill(pool.AmountConsumed, pool.PeakDemand);
    }

    public float GetSharedScaleGenerationFill(IReadOnlyResourcePool pool)
    {
        return GetSharedScaleFill(pool.TotalAvailable, pool);
    }

    public float GetSharedScaleDemandFill(IReadOnlyResourcePool pool)
    {
        return GetSharedScaleFill(pool.AmountConsumed, pool);
    }

    public int GetSharedScaleMax(IReadOnlyResourcePool pool)
    {
        return Mathf.Max(pool.TotalAvailable, pool.AmountConsumed, pool.PeakDemand);
    }

    public float GetDemandRatio(IReadOnlyResourcePool pool)
    {
        if (pool.AmountConsumed <= 0)
        {
            return pool.TotalAvailable > 0 ? 1f : 0f;
        }

        return (float)pool.TotalAvailable / pool.AmountConsumed;
    }

    public Color GetStatusColor(IReadOnlyResourcePool pool)
    {
        if (pool.TotalAvailable <= 0)
        {
            return GameColors.Red;
        }

        if (pool.AmountConsumed > pool.TotalAvailable)
        {
            return GameColors.Yellow;
        }

        return GameColors.Green;
    }

    public string BuildTooltipText()
    {
        IReadOnlyResourcePool pool = GetResourcePool();
        if (pool == null)
        {
            return $"{TooltipTitle}: No {ResourceName} resource found";
        }

        int margin = pool.TotalAvailable - pool.AmountConsumed;
        float demandRatio = GetDemandRatio(pool);
        float peakProductionFill = GetPeakProductionFill(pool);
        float peakDemandFill = GetPeakDemandFill(pool);
        float generationScaleFill = GetSharedScaleGenerationFill(pool);
        float demandScaleFill = GetSharedScaleDemandFill(pool);
        int sharedScaleMax = GetSharedScaleMax(pool);
        string color = GetStatusTextColor(pool);
        string unit = string.IsNullOrWhiteSpace(pool.UnitAbbrev) ? ResourceName : pool.UnitAbbrev;

        return string.Format(
            CultureInfo.InvariantCulture,
            "<b>{0}</b>: <color={1}>{2:P0}</color>\nNeed Filled: {2:P0}\nGeneration vs Peak: {3:P0}\nDemand vs Peak: {4:P0}\nGeneration Bar: {5:P0}\nDemand Bar: {6:P0}\nShared Scale Max: {7:N0} {13}\nAvailable: {8:N0} {13}\nDemand: {9:N0} {13}\nMargin: {10:+#,0;-#,0;0} {13}\nPeak Production: {11:N0} {13}\nPeak Demand: {12:N0} {13}",
            TooltipTitle,
            color,
            demandRatio,
            peakProductionFill,
            peakDemandFill,
            generationScaleFill,
            demandScaleFill,
            sharedScaleMax,
            pool.TotalAvailable,
            pool.AmountConsumed,
            margin,
            pool.PeakQuantity,
            pool.PeakDemand,
            unit);
    }

    private string GetStatusTextColor(IReadOnlyResourcePool pool)
    {
        if (pool.TotalAvailable <= 0)
        {
            return GameColors.RedTextColor;
        }

        if (pool.AmountConsumed > pool.TotalAvailable)
        {
            return GameColors.YellowTextColor;
        }

        return GameColors.GreenTextColor;
    }

    private float GetPeakFill(int current, int peak)
    {
        int denominator = Mathf.Max(current, peak);
        if (denominator <= 0)
        {
            return 0f;
        }

        return Mathf.Clamp01((float)current / denominator);
    }

    private float GetSharedScaleFill(int amount, IReadOnlyResourcePool pool)
    {
        int denominator = GetSharedScaleMax(pool);
        if (denominator <= 0)
        {
            return 0f;
        }

        return Mathf.Clamp01((float)amount / denominator);
    }
}

public class ShipStatusPowerBarBinding : MonoBehaviour
{
    private ShipStatusPowerBar _source;
    private QuantityStatusIcon _powerIcon;
    private Image _iconImage;
    private Image _maskImage;
    private RectTransform _maskRect;
    private RectTransform _graphRoot;
    private Image[] _graphBackgrounds;
    private RectTransform[] _graphFillRects;
    private Image[] _graphFills;
    private int _graphBarCount;
    private TooltipTrigger _tooltip;
    private float _nextUpdateTime;

    public void SetSource(ShipStatusPowerBar source, QuantityStatusIcon powerIcon)
    {
        _source = source;
        _powerIcon = powerIcon;
        _iconImage = Common.GetVal<Image>(_powerIcon, "_iconImage");
        _tooltip = Common.GetVal<TooltipTrigger>(_powerIcon, "_tooltip", typeof(StatusIcon));
        EnsureTooltipCallback();
        EnsureMask();
        UpdateVisibleStatus();
    }

    public void ClearSource(ShipStatusPowerBar source)
    {
        if (_source != source)
        {
            return;
        }

        _source = null;
        SetGraphVisible(false);
        SetIconImageVisible(true);
        SetMaskFill(1f);
    }

    public void ClearAnySource()
    {
        _source = null;
        SetGraphVisible(false);
        SetIconImageVisible(true);
        SetMaskFill(1f);
    }

    private void OnDisable()
    {
        SetGraphVisible(false);
        SetIconImageVisible(true);
        SetMaskFill(1f);
    }

    private void Update()
    {
        if (_source == null || Time.unscaledTime < _nextUpdateTime)
        {
            return;
        }

        _nextUpdateTime = Time.unscaledTime + Mathf.Max(0.02f, _source.UpdateInterval);
        UpdateVisibleStatus();
    }

    private void UpdateVisibleStatus()
    {
        if (_source == null || _powerIcon == null)
        {
            return;
        }

        IReadOnlyResourcePool pool = _source.GetResourcePool();
        if (pool == null)
        {
            SetMaskFill(1f);
            return;
        }

        if (_source.ShowIconWhenNormal)
        {
            _powerIcon.Show();
        }

        if (_source.VisualMode == ShipStatusPowerBar.PowerStatusVisualMode.TriplePercentBars)
        {
            SetIconImageVisible(false);
            EnsureBarGraph(3, "AGMLIB Power Triple Bar Graph");
            UpdateTripleBarGraph(pool);
        }
        else if (_source.VisualMode == ShipStatusPowerBar.PowerStatusVisualMode.SharedScaleGenerationDemandBars)
        {
            SetIconImageVisible(false);
            EnsureBarGraph(2, "AGMLIB Power Generation Demand Graph");
            UpdateSharedScaleGenerationDemandGraph(pool);
        }
        else
        {
            SetGraphVisible(false);
            SetIconImageVisible(true);
        }

        if (_iconImage != null && _source.VisualMode == ShipStatusPowerBar.PowerStatusVisualMode.IconMask)
        {
            _iconImage.color = _source.GetStatusColor(pool);
        }

        EnsureTooltipCallback();
        if (_source.VisualMode != ShipStatusPowerBar.PowerStatusVisualMode.IconMask)
        {
            SetMaskFill(1f);
        }
        else
        {
            EnsureMask();
            SetMaskFill(_source.GetDemandFill(pool));
        }
    }

    private void EnsureTooltipCallback()
    {
        if (_source == null || !_source.ReplaceTooltip || _tooltip == null)
        {
            return;
        }

        _tooltip.SetGetTextCallback(_source.BuildTooltipText);
        _tooltip.enabled = true;
    }

    private void EnsureMask()
    {
        if (_source == null || !_source.DrawFillMask || _powerIcon == null || _maskImage != null)
        {
            return;
        }

        RectTransform iconRect = _powerIcon.transform as RectTransform;
        if (iconRect == null)
        {
            return;
        }

        GameObject maskObject = new("AGMLIB Power Status Mask", typeof(RectTransform), typeof(Image));
        _maskRect = maskObject.transform as RectTransform;
        _maskRect.SetParent(iconRect, false);
        _maskRect.anchorMin = new Vector2(1f, 0f);
        _maskRect.anchorMax = new Vector2(1f, 1f);
        _maskRect.offsetMin = Vector2.zero;
        _maskRect.offsetMax = Vector2.zero;
        _maskRect.SetAsLastSibling();

        _maskImage = maskObject.GetComponent<Image>();
        _maskImage.color = _source.MaskColor;
        _maskImage.raycastTarget = false;
    }

    private void SetMaskFill(float fill)
    {
        if (_maskRect == null)
        {
            return;
        }

        float clampedFill = Mathf.Clamp01(fill);
        _maskRect.anchorMin = new Vector2(clampedFill, 0f);
        _maskRect.anchorMax = new Vector2(1f, 1f);
        _maskRect.offsetMin = Vector2.zero;
        _maskRect.offsetMax = Vector2.zero;
        _maskRect.gameObject.SetActive(_source != null && _source.DrawFillMask && clampedFill < 0.999f);
    }

    private void EnsureBarGraph(int barCount, string objectName)
    {
        if (_source == null || _powerIcon == null)
        {
            return;
        }

        if (_graphRoot != null && _graphBarCount != barCount)
        {
            Destroy(_graphRoot.gameObject);
            _graphRoot = null;
            _graphBackgrounds = null;
            _graphFillRects = null;
            _graphFills = null;
            _graphBarCount = 0;
        }

        if (_graphRoot != null)
        {
            return;
        }

        RectTransform iconRect = _powerIcon.transform as RectTransform;
        if (iconRect == null)
        {
            return;
        }

        GameObject rootObject = new(objectName, typeof(RectTransform));
        _graphRoot = rootObject.transform as RectTransform;
        _graphRoot.SetParent(iconRect, false);
        _graphRoot.anchorMin = Vector2.zero;
        _graphRoot.anchorMax = Vector2.one;
        _graphRoot.offsetMin = Vector2.zero;
        _graphRoot.offsetMax = Vector2.zero;
        _graphRoot.SetAsLastSibling();

        _graphBarCount = barCount;
        _graphBackgrounds = new Image[barCount];
        _graphFillRects = new RectTransform[barCount];
        _graphFills = new Image[barCount];

        for (int i = 0; i < barCount; i++)
        {
            CreateBar(i, barCount);
        }
    }

    private void CreateBar(int index, int barCount)
    {
        float gap = 0.08f;
        float totalGap = gap * (barCount + 1);
        float barWidth = (1f - totalGap) / barCount;
        float xMin = gap + index * (barWidth + gap);
        float xMax = xMin + barWidth;

        GameObject backgroundObject = new($"AGMLIB Power Bar {index + 1} Background", typeof(RectTransform), typeof(Image));
        RectTransform backgroundRect = backgroundObject.transform as RectTransform;
        backgroundRect.SetParent(_graphRoot, false);
        backgroundRect.anchorMin = new Vector2(xMin, 0.08f);
        backgroundRect.anchorMax = new Vector2(xMax, 0.92f);
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;

        _graphBackgrounds[index] = backgroundObject.GetComponent<Image>();
        _graphBackgrounds[index].color = _source.GraphBackgroundColor;
        _graphBackgrounds[index].raycastTarget = false;

        GameObject fillObject = new($"AGMLIB Power Bar {index + 1} Fill", typeof(RectTransform), typeof(Image));
        _graphFillRects[index] = fillObject.transform as RectTransform;
        _graphFillRects[index].SetParent(backgroundRect, false);
        _graphFillRects[index].anchorMin = Vector2.zero;
        _graphFillRects[index].anchorMax = Vector2.one;
        _graphFillRects[index].offsetMin = Vector2.zero;
        _graphFillRects[index].offsetMax = Vector2.zero;

        _graphFills[index] = fillObject.GetComponent<Image>();
        _graphFills[index].raycastTarget = false;
    }

    private void UpdateTripleBarGraph(IReadOnlyResourcePool pool)
    {
        if (_source == null || _graphRoot == null || _graphFillRects == null)
        {
            return;
        }

        SetGraphVisible(true);
        SetGraphFill(0, _source.GetDemandFill(pool), _source.NeedFillColor);
        SetGraphFill(1, _source.GetPeakProductionFill(pool), _source.ProductionFillColor);
        SetGraphFill(2, _source.GetPeakDemandFill(pool), _source.DemandFillColor);
    }

    private void UpdateSharedScaleGenerationDemandGraph(IReadOnlyResourcePool pool)
    {
        if (_source == null || _graphRoot == null || _graphFillRects == null)
        {
            return;
        }

        SetGraphVisible(true);
        SetGraphFill(0, _source.GetSharedScaleGenerationFill(pool), _source.NeedFillColor);
        SetGraphFill(1, _source.GetSharedScaleDemandFill(pool), _source.DemandFillColor);
    }

    private void SetGraphFill(int index, float fill, Color color)
    {
        if (_graphFillRects == null || index < 0 || index >= _graphFillRects.Length || _graphFillRects[index] == null)
        {
            return;
        }

        float clampedFill = Mathf.Clamp01(fill);
        _graphFillRects[index].anchorMin = Vector2.zero;
        _graphFillRects[index].anchorMax = new Vector2(1f, clampedFill);
        _graphFillRects[index].offsetMin = Vector2.zero;
        _graphFillRects[index].offsetMax = Vector2.zero;

        if (_graphFills != null && _graphFills[index] != null)
        {
            _graphFills[index].color = color;
        }
    }

    private void SetGraphVisible(bool visible)
    {
        if (_graphRoot != null)
        {
            _graphRoot.gameObject.SetActive(visible);
        }
    }

    private void SetIconImageVisible(bool visible)
    {
        if (_iconImage != null)
        {
            _iconImage.enabled = true;
            _iconImage.raycastTarget = true;

            if (!visible)
            {
                Color color = _iconImage.color;
                color.a = 0f;
                _iconImage.color = color;
            }
            else
            {
                Color color = _iconImage.color;
                color.a = 1f;
                _iconImage.color = color;
            }
        }
    }
}

[HarmonyPatch(typeof(ShipStatusIconGroup), nameof(ShipStatusIconGroup.SetShip))]
class ShipStatusIconGroupSetShipPowerStatusBar
{
    static void Postfix(ShipStatusIconGroup __instance, ShipController ship)
    {
        if (ship == null)
        {
            QuantityStatusIcon powerIcon = Common.GetVal<QuantityStatusIcon>(__instance, "_powerQuantityIcon");
            powerIcon?.GetComponent<ShipStatusPowerBarBinding>()?.ClearAnySource();
            return;
        }

        ShipStatusPowerBar powerBar = ShipStatusPowerBar.EnsureAttachedTo(ship);
        powerBar?.LinkStatusIcons(__instance);
    }
}

[HarmonyPatch(typeof(ShipController), nameof(ShipController.Initialize))]
class ShipControllerInitializePowerStatusBar
{
    static void Postfix(ShipController __instance)
    {
        ShipStatusPowerBar.EnsureAttachedTo(__instance);
    }
}
