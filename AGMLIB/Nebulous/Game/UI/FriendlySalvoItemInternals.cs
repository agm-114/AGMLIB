public static partial class NativeInternalsExtensions
{
    public static FriendlySalvoItemInternals Internals(this FriendlySalvoItem item) => new(item);
}

public readonly struct FriendlySalvoItemInternals(FriendlySalvoItem item)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<FriendlySalvoItem, Button> Button =
            AccessTools.FieldRefAccess<FriendlySalvoItem, Button>("_button");
    }

    public Button Button => Refs.Button(item);
}
