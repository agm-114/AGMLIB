using Game.UI;

public static partial class NativeInternalsExtensions
{
    public static DCTeamPieceInternals Internals(this DCTeamPiece target) => new(target);
}

public readonly struct DCTeamPieceInternals(DCTeamPiece target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<DCTeamPiece, GameColors.ColorName> DeadColor =
            AccessTools.FieldRefAccess<DCTeamPiece, GameColors.ColorName>("_deadColor");
        internal static readonly AccessTools.FieldRef<DCTeamPiece, GameColors.ColorName> EnRouteColor =
            AccessTools.FieldRefAccess<DCTeamPiece, GameColors.ColorName>("_enRouteColor");
        internal static readonly AccessTools.FieldRef<DCTeamPiece, GameColors.ColorName> IdleColor =
            AccessTools.FieldRefAccess<DCTeamPiece, GameColors.ColorName>("_idleColor");
        internal static readonly AccessTools.FieldRef<DCTeamPiece, Image> ProgressBar =
            AccessTools.FieldRefAccess<DCTeamPiece, Image>("_progressBar");
        internal static readonly AccessTools.FieldRef<DCTeamPiece, TooltipTrigger> Tooltip =
            AccessTools.FieldRefAccess<DCTeamPiece, TooltipTrigger>("_tooltip");
        internal static readonly AccessTools.FieldRef<DCTeamPiece, GameColors.ColorName> WorkingColor =
            AccessTools.FieldRefAccess<DCTeamPiece, GameColors.ColorName>("_workingColor");
    }

    public ref GameColors.ColorName DeadColor => ref Refs.DeadColor(target);
    public ref GameColors.ColorName EnRouteColor => ref Refs.EnRouteColor(target);
    public ref GameColors.ColorName IdleColor => ref Refs.IdleColor(target);
    public ref Image ProgressBar => ref Refs.ProgressBar(target);
    public ref TooltipTrigger Tooltip => ref Refs.Tooltip(target);
    public ref GameColors.ColorName WorkingColor => ref Refs.WorkingColor(target);
}