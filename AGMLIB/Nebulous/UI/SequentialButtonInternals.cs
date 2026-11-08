using UI;

public static partial class NativeInternalsExtensions
{
    public static SequentialButtonInternals Internals(this SequentialButton target) => new(target);
}

public readonly struct SequentialButtonInternals(SequentialButton target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<SequentialButton, SequentialButton.SequentialButtonEvent> OnValueChanged =
            AccessTools.FieldRefAccess<SequentialButton, SequentialButton.SequentialButtonEvent>("_onValueChanged");
        internal static readonly AccessTools.FieldRef<SequentialButton, SequentialButton.SequenceOption[]> Options =
            AccessTools.FieldRefAccess<SequentialButton, SequentialButton.SequenceOption[]>("_options");
        internal static readonly AccessTools.FieldRef<SequentialButton, string> OriginalTooltip =
            AccessTools.FieldRefAccess<SequentialButton, string>("_originalTooltip");
        internal static readonly AccessTools.FieldRef<SequentialButton, TooltipTrigger> Tooltip =
            AccessTools.FieldRefAccess<SequentialButton, TooltipTrigger>("_tooltip");
    }

    public ref SequentialButton.SequentialButtonEvent OnValueChanged => ref Refs.OnValueChanged(target);
    public ref SequentialButton.SequenceOption[] Options => ref Refs.Options(target);
    public ref string OriginalTooltip => ref Refs.OriginalTooltip(target);
    public ref TooltipTrigger Tooltip => ref Refs.Tooltip(target);
}