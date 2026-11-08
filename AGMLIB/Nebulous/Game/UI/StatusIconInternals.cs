using Game.UI;

public static partial class NativeInternalsExtensions
{
    public static StatusIconInternals Internals(this StatusIcon target) => new(target);
}

public readonly struct StatusIconInternals(StatusIcon target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<StatusIcon, TooltipTrigger> Tooltip =
            AccessTools.FieldRefAccess<StatusIcon, TooltipTrigger>("_tooltip");
    }

    public ref TooltipTrigger Tooltip => ref Refs.Tooltip(target);
}