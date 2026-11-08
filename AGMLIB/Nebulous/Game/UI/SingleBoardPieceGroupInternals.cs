using Game.UI;

public static partial class NativeInternalsExtensions
{
    public static SingleBoardPieceGroupInternals Internals(this SingleBoardPieceGroup target) => new(target);
}

public readonly struct SingleBoardPieceGroupInternals(SingleBoardPieceGroup target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<SingleBoardPieceGroup, IBoardPiece> PrimaryPiece =
            AccessTools.FieldRefAccess<SingleBoardPieceGroup, IBoardPiece>("_primaryPiece");
    }

    public ref IBoardPiece PrimaryPiece => ref Refs.PrimaryPiece(target);
}