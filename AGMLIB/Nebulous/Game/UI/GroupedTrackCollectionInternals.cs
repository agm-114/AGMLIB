using Game.UI;

public static partial class NativeInternalsExtensions
{
    public static GroupedTrackCollectionInternals Internals(this GroupedTrackCollection target) => new(target);
}

public readonly struct GroupedTrackCollectionInternals(GroupedTrackCollection target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<GroupedTrackCollection, IBoardPiece> PrimaryPiece =
            AccessTools.FieldRefAccess<GroupedTrackCollection, IBoardPiece>("_primaryPiece");
        internal static readonly AccessTools.FieldRef<GroupedTrackCollection, List<IBoardPiece>> Pieces =
            AccessTools.FieldRefAccess<GroupedTrackCollection, List<IBoardPiece>>("_pieces");
    }

    public ref IBoardPiece PrimaryPiece => ref Refs.PrimaryPiece(target);
    public ref List<IBoardPiece> Pieces => ref Refs.Pieces(target);
}