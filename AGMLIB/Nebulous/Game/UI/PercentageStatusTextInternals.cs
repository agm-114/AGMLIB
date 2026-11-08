using Game.UI;

public static partial class NativeInternalsExtensions
{
    public static PercentageStatusTextInternals Internals(this PercentageStatusText target) => new(target);
}

public readonly struct PercentageStatusTextInternals(PercentageStatusText target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<PercentageStatusText, TextMeshProUGUI> Text =
            AccessTools.FieldRefAccess<PercentageStatusText, TextMeshProUGUI>("_text");
    }

    public ref TextMeshProUGUI Text => ref Refs.Text(target);
}