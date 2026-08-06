using FleetEditor;
using HarmonyLib;
using TMPro;

public static partial class NativeInternalsExtensions
{
    public static ResourceItemInternals Internals(this ResourceItem item) => new(item);
}

public readonly struct ResourceItemInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<ResourceItem, TextMeshProUGUI> SummaryText =
            AccessTools.FieldRefAccess<ResourceItem, TextMeshProUGUI>("_summaryText");
    }

    private readonly ResourceItem _item;

    internal ResourceItemInternals(ResourceItem item)
    {
        _item = item ?? throw new ArgumentNullException(nameof(item));
    }

    public ref TextMeshProUGUI SummaryText => ref Refs.SummaryText(_item);
}
