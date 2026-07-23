public class ShipStatusPowerBarBinding : ShipStatusPowerBar.Binding
{
    private QuantityStatusIcon _powerIcon;
    private Image _iconImage;
    private Image _maskImage;
    private RectTransform _maskRect;

    private void OnEnable() => EnableBinding();

    private void OnDisable() => DisableBinding();

    private void OnDestroy() => DestroyBinding();

    public void SetSource(ShipStatusPowerBar source, QuantityStatusIcon powerIcon)
    {
        _powerIcon = powerIcon;
        _iconImage = Common.GetVal<Image>(_powerIcon, "_iconImage");
        Tooltip = Common.GetVal<TooltipTrigger>(_powerIcon, "_tooltip", typeof(StatusIcon));
        EnsureMask();
        SetPowerSource(source);
    }

    protected override void ClearVisualState()
    {
        base.ClearVisualState();
        SetIconImageVisible(true);
        SetMaskFill(1f);
    }

    protected override bool ShouldShowGraph()
    {
        return Source != null && Source.TargetsStatusIcon && Source.IconPowerBarsEnabled && _powerIcon != null;
    }

    protected override void BeforeVisualUpdate(IReadOnlyResourcePool pool)
    {
        if (_powerIcon == null || Source == null || !Source.TargetsStatusIcon)
        {
            return;
        }

        if (Source.ShowIconWhenNormal)
        {
            _powerIcon.Show();
        }

        bool showingBars = Source.IconPowerBarsEnabled && (Source.GenerationBarEnabled || Source.DemandBarEnabled);
        SetIconImageVisible(!showingBars);

        if (_iconImage != null && (showingBars || Source.IconMaskEnabled))
        {
            _iconImage.color = Source.GetStatusColor(pool);
        }

        if (Source.IconMaskEnabled)
        {
            EnsureMask();
            SetMaskFill(Source.GetDemandFill(pool));
        }
        else
        {
            SetMaskFill(1f);
        }
    }

    protected override RectTransform CreateRoot()
    {
        if (_powerIcon == null)
        {
            return null;
        }

        RectTransform iconRect = _powerIcon.transform as RectTransform;
        if (iconRect == null)
        {
            return null;
        }

        GameObject rootObject = new("AGMLIB Power Icon Bars", typeof(RectTransform));
        RectTransform root = rootObject.transform as RectTransform;
        root.SetParent(iconRect, false);
        root.anchorMin = Vector2.zero;
        root.anchorMax = Vector2.one;
        root.offsetMin = Vector2.zero;
        root.offsetMax = Vector2.zero;
        root.SetAsLastSibling();
        return root;
    }

    protected override void UpdateGraphLayout(int barCount)
    {
        LayoutBars(barCount);
    }

    protected override void LayoutBar(RectTransform backgroundRect, int index, int barCount)
    {
        float gap = 0.08f;
        float totalGap = gap * (barCount + 1);
        float barWidth = (1f - totalGap) / barCount;
        float xMin = gap + index * (barWidth + gap);
        float xMax = xMin + barWidth;

        backgroundRect.anchorMin = new Vector2(xMin, 0.08f);
        backgroundRect.anchorMax = new Vector2(xMax, 0.92f);
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;
    }

    protected override void ApplyFill(RectTransform fillRect, float fill)
    {
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = new Vector2(1f, fill);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
    }

    private void EnsureMask()
    {
        if (Source == null || !Source.IconMaskEnabled || _powerIcon == null || _maskImage != null)
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
        _maskImage.color = Source.MaskColor;
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
        _maskRect.gameObject.SetActive(Source != null && Source.IconMaskEnabled && clampedFill < 0.999f);
    }

    private void SetIconImageVisible(bool visible)
    {
        if (_iconImage == null)
        {
            return;
        }

        _iconImage.enabled = true;
        _iconImage.raycastTarget = true;

        Color color = _iconImage.color;
        color.a = visible ? 1f : 0f;
        _iconImage.color = color;
    }
}
