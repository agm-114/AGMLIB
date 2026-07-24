using HarmonyLib;
using Munitions;
using Ships;

public static partial class NativeInternalsExtensions
{
    public static MuzzleInternals Internals(this Muzzle muzzle) => new(muzzle);
}

public readonly struct MuzzleInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<Muzzle, IMagazine> AmmoSource =
            AccessTools.FieldRefAccess<Muzzle, IMagazine>("_ammoSource");
    }

    private readonly Muzzle _muzzle;

    internal MuzzleInternals(Muzzle muzzle)
    {
        _muzzle = muzzle ?? throw new ArgumentNullException(nameof(muzzle));
    }

    public ref IMagazine AmmoSource => ref Refs.AmmoSource(_muzzle);
}
