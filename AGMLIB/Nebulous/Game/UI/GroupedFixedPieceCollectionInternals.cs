using Game.UI;

public static partial class NativeInternalsExtensions
{
    public static GroupedFixedPieceCollectionInternals Internals(this GroupedFixedPieceCollection target) => new(target);
}

public readonly struct GroupedFixedPieceCollectionInternals(GroupedFixedPieceCollection target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<GroupedFixedPieceCollection, IBoardPiece> PrimaryPiece =
            AccessTools.FieldRefAccess<GroupedFixedPieceCollection, IBoardPiece>("_primaryPiece");
        internal static readonly AccessTools.FieldRef<GroupedFixedPieceCollection, List<IBoardPiece>> Pieces =
            AccessTools.FieldRefAccess<GroupedFixedPieceCollection, List<IBoardPiece>>("_pieces");
    }

    public ref IBoardPiece PrimaryPiece => ref Refs.PrimaryPiece(target);
    public ref List<IBoardPiece> Pieces => ref Refs.Pieces(target);
}