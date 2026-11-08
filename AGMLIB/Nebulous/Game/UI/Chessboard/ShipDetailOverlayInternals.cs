using Game.UI.Chessboard;

public static partial class NativeInternalsExtensions
{
    public static ShipDetailOverlayInternals Internals(this ShipDetailOverlay target) => new(target);
}

public readonly struct ShipDetailOverlayInternals(ShipDetailOverlay target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<ShipDetailOverlay, bool> Active =
            AccessTools.FieldRefAccess<ShipDetailOverlay, bool>("_active");
    }

    public ref bool Active => ref Refs.Active(target);
}