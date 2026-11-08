using FleetEditor;

public static partial class NativeInternalsExtensions
{
    public static ShipEditorPaneInternals Internals(this ShipEditorPane target) => new(target);
}

public readonly struct ShipEditorPaneInternals(ShipEditorPane target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<ShipEditorPane, ModalListSelectDetailed> OpenPalette =
            AccessTools.FieldRefAccess<ShipEditorPane, ModalListSelectDetailed>("_openPalette");
        internal static readonly AccessTools.FieldRef<ShipEditorPane, List<SocketItem>> Sockets =
            AccessTools.FieldRefAccess<ShipEditorPane, List<SocketItem>>("_sockets");
    }

    public ref ModalListSelectDetailed OpenPalette => ref Refs.OpenPalette(target);
    public ref List<SocketItem> Sockets => ref Refs.Sockets(target);
}