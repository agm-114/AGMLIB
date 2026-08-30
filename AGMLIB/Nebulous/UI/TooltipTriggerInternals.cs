using HarmonyLib;
using UI;

public static partial class NativeInternalsExtensions
{
    public static TooltipTriggerInternals Internals(this TooltipTrigger tooltip) => new(tooltip);
}

public readonly struct TooltipTriggerInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<TooltipTrigger, bool> IsOpen =
            AccessTools.FieldRefAccess<TooltipTrigger, bool>("_isOpen");
    }

    private readonly TooltipTrigger _tooltip;

    internal TooltipTriggerInternals(TooltipTrigger tooltip)
    {
        _tooltip = tooltip ?? throw new ArgumentNullException(nameof(tooltip));
    }

    public ref bool IsOpen => ref Refs.IsOpen(_tooltip);
}
