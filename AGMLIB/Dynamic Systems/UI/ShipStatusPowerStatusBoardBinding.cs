public class ShipStatusPowerStatusBoardBinding : ShipStatusPowerBar.Binding
{
    private RectTransform _layoutParent;

    public void SetSource(ShipStatusPowerBar source)
    {
        SetPowerSource(source);
    }

    protected override bool ShouldShowGraph()
    {
        return Source != null
            && Source.TargetsStatusBoard
            && Source.StatusBoardPowerBarsEnabled
            && IsCompactStatusDisplay();
    }

    protected override RectTransform CreateRoot()
    {
        RectTransform displayRect = transform as RectTransform;
        _layoutParent = GetStatusWindowRect();
        if (displayRect == null || _layoutParent == null || Source == null)
        {
            return null;
        }

        GameObject rootObject = new("AGMLIB Power Status Board Bars", typeof(RectTransform), typeof(Image), typeof(TooltipTrigger));
        RectTransform root = rootObject.transform as RectTransform;
        root.SetParent(_layoutParent, false);
        root.SetAsLastSibling();

        Image hitTarget = rootObject.GetComponent<Image>();
        hitTarget.color = Source.BoardGraphBackgroundColor;
        hitTarget.raycastTarget = true;

        Tooltip = rootObject.GetComponent<TooltipTrigger>();
        Tooltip.TriggerOnChildren = true;

        return root;
    }

    protected override void UpdateGraphLayout(int barCount)
    {
        if (Source == null || Root == null)
        {
            return;
        }

        RectTransform displayRect = transform as RectTransform;
        RectTransform smallBoardRect = transform.parent as RectTransform;
        _layoutParent = GetStatusWindowRect();
        if (displayRect == null || smallBoardRect == null || _layoutParent == null)
        {
            return;
        }

        if (Root.parent != _layoutParent)
        {
            Root.SetParent(_layoutParent, false);
            Root.SetAsLastSibling();
        }

        Vector2 margin = ScreenPixelsToLocal(Source.BoardBarMargin);
        Vector2 nudge = ScreenPixelsToLocal(Source.BoardBarNudge);
        float graphHeight = ScreenPixelsToLocal(Source.BoardBarSize.y);
        float width = Mathf.Max(0f, displayRect.rect.width - (margin.x * 2f));
        float x = smallBoardRect.anchoredPosition.x + margin.x + nudge.x;

        Root.anchorMin = Vector2.zero;
        Root.anchorMax = Vector2.zero;
        Root.pivot = Vector2.zero;
        Root.anchoredPosition = new Vector2(x, margin.y + nudge.y);
        Root.sizeDelta = new Vector2(width, graphHeight);

        LayoutBars(barCount);
    }

    protected override void LayoutBar(RectTransform backgroundRect, int index, int barCount)
    {
        float barHeight = Source.BoardBarHeight;
        float totalBarHeight = (barHeight * barCount) + (Source.BoardBarGap * Mathf.Max(0, barCount - 1));
        float yOffsetFromTop = Mathf.Max(0f, (Source.BoardBarSize.y - totalBarHeight) * 0.5f);
        yOffsetFromTop += index * (barHeight + Source.BoardBarGap);

        float padding = ScreenPixelsToLocal(Source.BoardBarPadding);
        float height = ScreenPixelsToLocal(barHeight);
        float yOffset = ScreenPixelsToLocal(yOffsetFromTop);

        backgroundRect.anchorMin = new Vector2(0f, 1f);
        backgroundRect.anchorMax = new Vector2(1f, 1f);
        backgroundRect.offsetMin = new Vector2(padding, -yOffset - height);
        backgroundRect.offsetMax = new Vector2(-padding, -yOffset);
    }

    protected override void ApplyFill(RectTransform fillRect, float fill)
    {
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = new Vector2(fill, 1f);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
    }

    protected override string BarNamePrefix => "AGMLIB Power Board Bar";

    protected override Color BarBackgroundColor => Color.clear;

    private bool IsCompactStatusDisplay()
    {
        if (Source == null)
        {
            return false;
        }

        RectTransform rect = transform as RectTransform;
        if (rect == null)
        {
            return false;
        }

        return rect.rect.width <= Source.StatusBoardMaxSize.x && rect.rect.height <= Source.StatusBoardMaxSize.y;
    }

    private RectTransform GetStatusWindowRect()
    {
        Transform current = transform;
        while (current != null)
        {
            if (current.name == "Ship Status")
            {
                return current as RectTransform;
            }

            current = current.parent;
        }

        return transform as RectTransform;
    }

    private Vector2 ScreenPixelsToLocal(Vector2 pixels)
    {
        float scale = GetUiScale();
        return pixels / scale;
    }

    private float ScreenPixelsToLocal(float pixels)
    {
        return pixels / GetUiScale();
    }

    private float GetUiScale()
    {
        RectTransform rect = transform as RectTransform;
        float scale = rect == null ? 1f : Mathf.Abs(rect.lossyScale.x);
        return scale <= 0.001f ? 1f : scale;
    }
}
