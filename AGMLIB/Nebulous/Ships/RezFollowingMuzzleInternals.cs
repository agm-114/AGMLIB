using Ships;

public static partial class NativeInternalsExtensions
{
    public static RezFollowingMuzzleInternals Internals(this RezFollowingMuzzle target) => new(target);
}

public readonly struct RezFollowingMuzzleInternals(RezFollowingMuzzle target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<RezFollowingMuzzle, GameObject> FollowingPrefab =
            AccessTools.FieldRefAccess<RezFollowingMuzzle, GameObject>("_followingPrefab");
    }

    public ref GameObject FollowingPrefab => ref Refs.FollowingPrefab(target);
}