using Game.UI.Chessboard;

public static partial class NativeInternalsExtensions
{
    public static MissileDetailOverlayInternals Internals(this MissileDetailOverlay target) => new(target);
}

public readonly struct MissileDetailOverlayInternals(MissileDetailOverlay target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<MissileDetailOverlay, ISelectableMissile> Missile =
            AccessTools.FieldRefAccess<MissileDetailOverlay, ISelectableMissile>("_missile");
    }

    public ref ISelectableMissile Missile => ref Refs.Missile(target);
}