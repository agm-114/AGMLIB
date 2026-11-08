using FleetEditor;

public static partial class NativeInternalsExtensions
{
    public static SocketItemInternals Internals(this SocketItem target) => new(target);
}

public readonly struct SocketItemInternals(SocketItem target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<SocketItem, ShipEditorPane> Editor =
            AccessTools.FieldRefAccess<SocketItem, ShipEditorPane>("_editor");
        internal static readonly AccessTools.FieldRef<SocketItem, GameColors.ColorName> InstalledColor =
            AccessTools.FieldRefAccess<SocketItem, GameColors.ColorName>("_installedColor");
    }

    public ref ShipEditorPane Editor => ref Refs.Editor(target);
    public ref GameColors.ColorName InstalledColor => ref Refs.InstalledColor(target);
}