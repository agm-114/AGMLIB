using SmallCraft.Components.Runtime;
using SmallCraft.Components;

public static partial class NativeInternalsExtensions
{
    public static CraftGunWeaponInternals Internals(this CraftGunWeapon target) => new(target);
}

public readonly struct CraftGunWeaponInternals(CraftGunWeapon target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<CraftGunWeapon, RuntimeCraftAmmoWeapon> Runtime =
            AccessTools.FieldRefAccess<CraftGunWeapon, RuntimeCraftAmmoWeapon>("_runtime");
    }

    public ref RuntimeCraftAmmoWeapon Runtime => ref Refs.Runtime(target);
}