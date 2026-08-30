using HarmonyLib;
using TMPro;
using UI;
using UnityEngine.UI;

public static partial class NativeInternalsExtensions
{
    public static TooltipInternals Internals(this Tooltip tooltip) => new(tooltip);
}

public readonly struct TooltipInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<Tooltip, TextMeshProUGUI> TooltipText =
            AccessTools.FieldRefAccess<Tooltip, TextMeshProUGUI>("_tooltipText");

        internal static readonly AccessTools.FieldRef<Tooltip, TextMeshProUGUI> OverflowText =
            AccessTools.FieldRefAccess<Tooltip, TextMeshProUGUI>("_overflowText");

        internal static readonly AccessTools.FieldRef<Tooltip, LayoutElement> TextLayout =
            AccessTools.FieldRefAccess<Tooltip, LayoutElement>("_textLayout");

        internal static readonly AccessTools.FieldRef<Tooltip, int> MaxLines =
            AccessTools.FieldRefAccess<Tooltip, int>("_maxLines");
    }

    private readonly Tooltip _tooltip;

    internal TooltipInternals(Tooltip tooltip)
    {
        _tooltip = tooltip ?? throw new ArgumentNullException(nameof(tooltip));
    }

    public TextMeshProUGUI TooltipText => Refs.TooltipText(_tooltip);

    public TextMeshProUGUI OverflowText => Refs.OverflowText(_tooltip);

    public LayoutElement TextLayout => Refs.TextLayout(_tooltip);

    public int MaxLines => Refs.MaxLines(_tooltip);
}
