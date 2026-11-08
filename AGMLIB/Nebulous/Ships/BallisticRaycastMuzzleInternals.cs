using Ships;

public static partial class NativeInternalsExtensions
{
    public static BallisticRaycastMuzzleInternals Internals(this BallisticRaycastMuzzle target) => new(target);
}

public readonly struct BallisticRaycastMuzzleInternals(BallisticRaycastMuzzle target)
{
    private static class Refs
    {
        internal static readonly Func<object, float> BulletLifetime =
            NativePrivateFields.Getter<float>(typeof(BallisticRaycastMuzzle), "RaycastBullet", "Lifetime");
        internal static readonly Func<object, Vector3> BulletPosition =
            NativePrivateFields.Getter<Vector3>(typeof(BallisticRaycastMuzzle), "RaycastBullet", "Position");
        internal static readonly AccessTools.FieldRef<BallisticRaycastMuzzle, IEnumerable> ActiveBullets =
            AccessTools.FieldRefAccess<BallisticRaycastMuzzle, IEnumerable>("_activeBullets");
    }

    public IEnumerable ActiveBullets => Refs.ActiveBullets(target);
    public static float GetBulletLifetime(object bullet) => Refs.BulletLifetime(bullet);
    public static Vector3 GetBulletPosition(object bullet) => Refs.BulletPosition(bullet);
}
