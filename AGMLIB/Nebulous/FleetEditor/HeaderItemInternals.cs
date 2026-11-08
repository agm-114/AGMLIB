using FleetEditor;

public static partial class NativeInternalsExtensions
{
    public static HeaderItemInternals Internals(this HeaderItem target) => new(target);
}

public readonly struct HeaderItemInternals(HeaderItem target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<HeaderItem, TextMeshProUGUI> Text =
            AccessTools.FieldRefAccess<HeaderItem, TextMeshProUGUI>("_text");
    }

    public ref TextMeshProUGUI Text => ref Refs.Text(target);
}