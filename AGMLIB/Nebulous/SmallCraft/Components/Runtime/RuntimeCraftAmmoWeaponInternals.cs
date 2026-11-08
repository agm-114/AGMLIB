using SmallCraft.Components.Runtime;

public static partial class NativeInternalsExtensions
{
    public static RuntimeCraftAmmoWeaponInternals Internals(this RuntimeCraftAmmoWeapon target) => new(target);
}

public readonly struct RuntimeCraftAmmoWeaponInternals(RuntimeCraftAmmoWeapon target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<RuntimeCraftAmmoWeapon, ITrack> Target =
            AccessTools.FieldRefAccess<RuntimeCraftAmmoWeapon, ITrack>("_target");
    }

    public ref ITrack Target => ref Refs.Target(target);
}