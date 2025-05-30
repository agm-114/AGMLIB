
public class AmmoCompatiblity : MonoBehaviour
{

    [SerializeField] protected BaseFilter _filter;

    public bool IsAmmoCompatible(IMunition ammo, bool debugmode = false)
    {
        //return _filter.IsAmmoCompatible(ammo, debugmode);


        if (_filter == null)
            _filter = gameObject.GetComponent<SimpleFilter>();
        if (_filter == null)
        {
            Common.Hint(this, "has Ammo Filter that must be configured using simple filter");
            return false;
        }
        return _filter.IsAmmoCompatible(ammo, debugmode);

    }
    public static bool IsAmmoCompatible(IMunition ammo, HullComponent component, out bool value, bool debugmode = false)
    {

        value = true;
        AmmoCompatiblity filter = component?.GetComponentInChildren<AmmoCompatiblity>();

        if (filter == null)
            return false;
        debugmode = true;

        value = filter.IsAmmoCompatible(ammo, debugmode);
        return true;
    }
}
public class AmmoFilter : AmmoCompatiblity { public void Awake() { Common.Hint(this.gameObject, "Replace Ammo filter with AmmoCompatiblity"); } }



[HarmonyPatch(typeof(BaseCellLauncherComponent), nameof(BaseCellLauncherComponent.IsAmmoCompatible))]
class BaseCellLauncherComponentIsAmmoCompatible
{
    public static void Postfix(BaseCellLauncherComponent __instance, IMunition ammo, ref bool __result)
    {
        Common.LogPatch();
        if (AmmoCompatiblity.IsAmmoCompatible(ammo, __instance, out bool value ))
            __result = value;
    }
}

[HarmonyPatch(typeof(BaseTubeLauncherComponent), nameof(BaseTubeLauncherComponent.IsAmmoCompatible))]
class BaseTubeLauncherComponentIsAmmoCompatible
{

    public static void Postfix(FixedTubeLauncherComponent __instance, IMunition ammo, ref bool __result)
    {
        Common.LogPatch();
        if (AmmoCompatiblity.IsAmmoCompatible(ammo, __instance, out bool value))
            __result = value;
    }
}

[HarmonyPatch(typeof(WeaponComponent), nameof(WeaponComponent.IsAmmoCompatible))]
class WeaponComponentIsAmmoCompatible
{
    static void Postfix(WeaponComponent __instance, IMunition ammo, ref bool __result)
    {
        Common.LogPatch();
        if (AmmoCompatiblity.IsAmmoCompatible(ammo, __instance, out bool value))
            __result = value;
    }
}

[HarmonyPatch(typeof(BulkMagazineComponent), nameof(BulkMagazineComponent.RestrictionCheck))]
class BulkMagazineComponentRestrictionCheck
{
    static void Postfix(BulkMagazineComponent __instance, IMunition ammoType, ref bool __result)
    {
        Common.LogPatch();
        if (AmmoCompatiblity.IsAmmoCompatible(ammoType, __instance, out bool value))
            __result = value;
    }
}