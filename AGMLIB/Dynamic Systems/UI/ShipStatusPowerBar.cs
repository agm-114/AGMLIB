using System.Globalization;

public class ShipStatusPowerBar : MonoBehaviour
{
    public enum DisplaySurface
    {
        StatusIcon,
        StatusBoard,
    }

    public static bool DebugAutoAttachToAllShips = true;

    [Header("Display")]
    public DisplaySurface DisplayTarget = DisplaySurface.StatusBoard;

    [Header("Resource")]
    public string ResourceName = "Power";

    [Header("Status Icon")]
    public bool ShowIconWhenNormal = true;
    public bool IconPowerBarsEnabled = false;
    public bool IconMaskEnabled = false;
    public Color MaskColor = new(0f, 0f, 0f, 0.85f);
    public Color GraphBackgroundColor = new(0f, 0f, 0f, 0.8f);
    public GameColors.ColorName GenerationColorName = GameColors.ColorName.Green;
    public Color DemandFillColor = new(1f, 0.15f, 0.1f, 1f);
    public float UpdateInterval = 0.1f;

    [Header("Power Bars")]
    public bool GenerationBarEnabled = true;
    public bool DemandBarEnabled = true;
    public bool IncludePeakPowerInScale = true;

    [Header("Status Board")]
    public bool StatusBoardPowerBarsEnabled = true;
    public float BoardBarHeight = 5f;
    public float BoardBarGap = 2f;
    public float BoardBarPadding = 4f;
    public Vector2 BoardBarMargin = new(3f, 3f);
    public Vector2 BoardBarNudge = new(-8f, 6f);
    public Vector2 BoardBarSize = new(0f, 18f);
    public Vector2 StatusBoardMaxSize = new(700f, 350f);
    public Color BoardGraphBackgroundColor = new(0.25f, 0.25f, 0.25f, 0.85f);

    [Header("Tooltip")]
    public bool ReplaceTooltip = true;
    public string TooltipTitle = "Power Status";

    private ShipController _shipController;
    public static ShipStatusPowerBar EnsureAttachedTo(ShipController ship, DisplaySurface displayTarget)
    {
        if (ship == null)
            return null;

        ShipStatusPowerBar powerBar = ship.GetComponentsInChildren<ShipStatusPowerBar>(includeInactive: true)
            .FirstOrDefault(bar => bar.DisplayTarget == displayTarget);
        if (powerBar == null && DebugAutoAttachToAllShips)
        {
            powerBar = ship.gameObject.AddComponent<ShipStatusPowerBar>();
            powerBar.ApplyDisplayDefaults(displayTarget);
        }

        return powerBar;
    }

    public static void EnsureDebugAttachedTo(ShipController ship)
    {
        EnsureAttachedTo(ship, DisplaySurface.StatusIcon);
        EnsureAttachedTo(ship, DisplaySurface.StatusBoard);
    }

    public bool TargetsStatusIcon => DisplayTarget == DisplaySurface.StatusIcon;

    public bool TargetsStatusBoard => DisplayTarget == DisplaySurface.StatusBoard;

    private void ApplyDisplayDefaults(DisplaySurface displayTarget)
    {
        DisplayTarget = displayTarget;
        IconPowerBarsEnabled = displayTarget == DisplaySurface.StatusIcon && IconPowerBarsEnabled;
        IconMaskEnabled = displayTarget == DisplaySurface.StatusIcon && IconMaskEnabled;
        StatusBoardPowerBarsEnabled = displayTarget == DisplaySurface.StatusBoard;
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

        ShipStatusPowerStatusBoardBinding[] boardBindings = FindObjectsOfType<ShipStatusPowerStatusBoardBinding>();
        foreach (ShipStatusPowerStatusBoardBinding binding in boardBindings)
        {
            binding.ClearSource(this);
        }
    }

    public void LinkStatusIcons(ShipStatusIconGroup statusIcons)
    {
        if (!TargetsStatusIcon || _shipController == null || statusIcons == null)
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
            return null;
        return _shipController.Ship.Resources.FirstOrDefault(pool => pool.ResourceName == ResourceName);
    }

    public float GetDemandFill(IReadOnlyResourcePool pool) => Mathf.Clamp01(GetDemandRatio(pool));
    public float GetPeakProductionFill(IReadOnlyResourcePool pool) => GetPeakFill(pool.TotalAvailable, pool.PeakQuantity);
    public float GetPeakDemandFill(IReadOnlyResourcePool pool) => GetPeakFill(pool.AmountConsumed, pool.PeakDemand);

    public float GetSharedScaleGenerationFill(IReadOnlyResourcePool pool) => GetSharedScaleFill(pool.TotalAvailable, GetSharedScaleMax(pool));

    public float GetSharedScaleDemandFill(IReadOnlyResourcePool pool) => GetSharedScaleFill(pool.AmountConsumed, GetSharedScaleMax(pool));
    public int GetSharedScaleMax(IReadOnlyResourcePool pool)
    {
        int max = Mathf.Max(pool.TotalAvailable, pool.AmountConsumed);
        if (IncludePeakPowerInScale)
            max = Mathf.Max(max, pool.PeakQuantity, pool.PeakDemand);

        return max;
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

    public Color GetGenerationFillColor()
    {
        return GameColors.GetColor(GenerationColorName);
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
            "<b>{0}</b>: <color={1}>{2:P0}</color>\nNeed Filled: {2:P0}\nGeneration vs Peak: {3:P0}\nDemand vs Peak: {4:P0}\nGeneration Bar: {5:P0}\nDemand Bar: {6:P0}\nBar Scale Max: {7:N0} {14}\nPeak Included In Scale: {8}\nAvailable: {9:N0} {14}\nDemand: {10:N0} {14}\nMargin: {11:+#,0;-#,0;0} {14}\nPeak Production: {12:N0} {14}\nPeak Demand: {13:N0} {14}",
            TooltipTitle,
            color,
            demandRatio,
            peakProductionFill,
            peakDemandFill,
            generationScaleFill,
            demandScaleFill,
            sharedScaleMax,
            IncludePeakPowerInScale ? "Yes" : "No",
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

    private float GetSharedScaleFill(int amount, int denominator)
    {
        if (denominator <= 0)
        {
            return 0f;
        }

        return Mathf.Clamp01((float)amount / denominator);
    }

    public int EnabledBarCount
    {
        get
        {
            int count = 0;
            if (GenerationBarEnabled)
            {
                count++;
            }

            if (DemandBarEnabled)
            {
                count++;
            }

            return count;
        }
    }

    public void SetTooltipCallback(TooltipTrigger tooltip)
    {
        if (!ReplaceTooltip || tooltip == null)
        {
            return;
        }

        tooltip.SetGetTextCallback(BuildTooltipText);
        tooltip.enabled = true;
    }

    public abstract class Binding : MonoBehaviour
    {
        protected ShipStatusPowerBar Source { get; private set; }
        protected RectTransform Root { get; private set; }
        protected TooltipTrigger Tooltip { get; set; }

        private RectTransform[] _barBackgroundRects;
        private RectTransform[] _fillRects;
        private Image[] _fills;
        private int _barCount;
        private float _nextUpdateTime;

        protected void SetPowerSource(ShipStatusPowerBar source)
        {
            Source = source;
            EnsureTooltipCallback();
            UpdateVisibleStatus();
        }

        public void ClearSource(ShipStatusPowerBar source)
        {
            if (Source != source)
            {
                return;
            }

            Source = null;
            ClearVisualState();
        }

        public void ClearAnySource()
        {
            Source = null;
            ClearVisualState();
        }

        protected virtual void OnDisable()
        {
            ClearVisualState();
        }

        private void Update()
        {
            if (Source == null || Time.unscaledTime < _nextUpdateTime)
            {
                return;
            }

            _nextUpdateTime = Time.unscaledTime + Mathf.Max(0.02f, Source.UpdateInterval);
            UpdateVisibleStatus();
        }

        protected void UpdateVisibleStatus()
        {
            if (Source == null)
            {
                return;
            }

            IReadOnlyResourcePool pool = Source.GetResourcePool();
            if (pool == null)
            {
                ClearVisualState();
                return;
            }

            int barCount = Source.EnabledBarCount;
            if (barCount <= 0 || !ShouldShowGraph())
            {
                BeforeVisualUpdate(pool);
                EnsureTooltipCallback();
                SetVisible(false);
                return;
            }

            BeforeVisualUpdate(pool);
            EnsureGraph(barCount);
            if (Root == null || _fillRects == null)
            {
                return;
            }

            UpdateGraphLayout(barCount);
            EnsureTooltipCallback();
            SetVisible(true);
            UpdatePowerBars(pool);
        }

        protected virtual void ClearVisualState()
        {
            SetVisible(false);
        }

        protected virtual bool ShouldShowGraph()
        {
            return true;
        }

        protected virtual void BeforeVisualUpdate(IReadOnlyResourcePool pool)
        {
        }

        protected abstract RectTransform CreateRoot();

        protected abstract void UpdateGraphLayout(int barCount);

        protected abstract void LayoutBar(RectTransform backgroundRect, int index, int barCount);

        protected abstract void ApplyFill(RectTransform fillRect, float fill);

        protected virtual string BarNamePrefix => "AGMLIB Power Bar";

        protected virtual Color BarBackgroundColor => Source.GraphBackgroundColor;

        private void EnsureGraph(int barCount)
        {
            if (Root != null && _barCount != barCount)
            {
                Destroy(Root.gameObject);
                Root = null;
                _barBackgroundRects = null;
                _fillRects = null;
                _fills = null;
                _barCount = 0;
            }

            if (Root != null)
            {
                return;
            }

            Root = CreateRoot();
            if (Root == null)
            {
                return;
            }

            _barCount = barCount;
            _barBackgroundRects = new RectTransform[barCount];
            _fillRects = new RectTransform[barCount];
            _fills = new Image[barCount];

            for (int i = 0; i < barCount; i++)
            {
                CreateBar(i);
            }
        }

        private void CreateBar(int index)
        {
            GameObject backgroundObject = new($"{BarNamePrefix} {index + 1} Background", typeof(RectTransform), typeof(Image));
            RectTransform backgroundRect = backgroundObject.transform as RectTransform;
            backgroundRect.SetParent(Root, false);
            _barBackgroundRects[index] = backgroundRect;

            Image background = backgroundObject.GetComponent<Image>();
            background.color = BarBackgroundColor;
            background.raycastTarget = false;

            GameObject fillObject = new($"{BarNamePrefix} {index + 1} Fill", typeof(RectTransform), typeof(Image));
            RectTransform fillRect = fillObject.transform as RectTransform;
            fillRect.SetParent(backgroundRect, false);
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            _fillRects[index] = fillRect;

            _fills[index] = fillObject.GetComponent<Image>();
            _fills[index].raycastTarget = false;
        }

        private void UpdatePowerBars(IReadOnlyResourcePool pool)
        {
            int index = 0;
            if (Source.GenerationBarEnabled)
            {
                SetBar(index, Source.GetSharedScaleGenerationFill(pool), Source.GetGenerationFillColor());
                index++;
            }

            if (Source.DemandBarEnabled)
            {
                SetBar(index, Source.GetSharedScaleDemandFill(pool), Source.DemandFillColor);
            }
        }

        private void SetBar(int index, float fill, Color color)
        {
            if (_fillRects == null || index < 0 || index >= _fillRects.Length || _fillRects[index] == null)
            {
                return;
            }

            ApplyFill(_fillRects[index], Mathf.Clamp01(fill));

            if (_fills != null && _fills[index] != null)
            {
                _fills[index].color = color;
            }
        }

        protected void LayoutBars(int barCount)
        {
            if (_barBackgroundRects == null)
            {
                return;
            }

            for (int i = 0; i < barCount; i++)
            {
                if (_barBackgroundRects[i] != null)
                {
                    LayoutBar(_barBackgroundRects[i], i, barCount);
                }
            }
        }

        protected void EnsureTooltipCallback() => Source?.SetTooltipCallback(Tooltip);

        protected void SetVisible(bool visible) => Root?.gameObject?.SetActive(visible);
    }
}

