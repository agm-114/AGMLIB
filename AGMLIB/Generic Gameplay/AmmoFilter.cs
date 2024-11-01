
public class AmmoFilter : MonoBehaviour, ICoreFilter
{
    public bool AllowMissiles = true;
    public List<string> WhiteList = new();
    public List<string> Blacklist = new();
    public bool Default = false;
    IList<string> ICoreFilter.Whitelist => WhiteList;
    IList<string> ICoreFilter.Blacklist => Blacklist;

    public bool IsAmmoCompatible(IMunition ammo, bool debugmode = false)
    {

        if (ammo == null)
        {
            return true;
        }
        if(debugmode && false)  
        {
            //Debug.LogError(String.Join(", ", Whitelist));
            //Debug.LogError(ammo.Tags.Class);
            //Debug.LogError(ammo.Tags.Subclass);
        }

        //Debug.LogError(ammo.MunitionName);
        //Debug.LogError("Parse int: " + ammo?.Tags.Subclass.Substring(1));
        //Debug.LogError("Value: " + int.Parse(ammo?.Tags.Subclass.Substring(1)));

        if (WhiteList.Contains(ammo?.MunitionName))
            return true;
        else if (Blacklist.Contains(ammo?.MunitionName))
            return false;
        else if (WhiteList.Contains(ammo?.SaveKey))
            return true;
        else if (Blacklist.Contains(ammo?.SaveKey))
            return false;
        else if (WhiteList.Contains(ammo?.Type.ToString()))
            return true;
        else if (Blacklist.Contains(ammo?.Type.ToString()))
            return false;
        else if (WhiteList.Contains(ammo?.SimMethod.ToString()))
            return true;
        else if (Blacklist.Contains(ammo?.SimMethod.ToString()))
            return false;
        else if (WhiteList.Contains(ammo?.Role.ToString()))
            return true;
        else if (Blacklist.Contains(ammo?.Role.ToString()))
            return false;
        else if (WhiteList.Contains(ammo?.UtilityRole.ToString()))
            return true;
        else if (Blacklist.Contains(ammo?.UtilityRole.ToString()))
            return false;
        else if (WhiteList.Contains(ammo?.FactionKey))
            return true;
        else if (Blacklist.Contains(ammo?.FactionKey))
            return false;
        else if(WhiteList.Contains(ammo?.Tags.Subclass))
            return true;
        else if (Blacklist.Contains(ammo?.Tags.Subclass))
            return false;
        else if (WhiteList.Contains(ammo?.Tags.Class))
            return true;
        else if (Blacklist.Contains(ammo?.Tags.Class))
            return false;
        else 
            return Default;
    }
    public class AmmoCompatiblity : AmmoFilter
    {

    }
    public static bool IsAmmoCompatible(IMunition ammo, HullComponent component, out bool value, bool debugmode = false)
    {
        value = true;
        AmmoFilter filter = component?.GetComponentInChildren<AmmoFilter>();

        if (filter == null)
            return false;

        value = filter.IsAmmoCompatible(ammo, debugmode);
        return true;
    }
}

   

[HarmonyPatch(typeof(BaseCellLauncherComponent), nameof(BaseCellLauncherComponent.IsAmmoCompatible))]
class BaseCellLauncherComponentIsAmmoCompatible
{
    public static void Postfix(BaseCellLauncherComponent __instance, IMunition ammo, ref bool __result)
    {

        if (AmmoFilter.IsAmmoCompatible(ammo, __instance, out bool value, true))
            __result =  value;
    }
}

[HarmonyPatch(typeof(BaseTubeLauncherComponent), nameof(BaseTubeLauncherComponent.IsAmmoCompatible))]
class BaseTubeLauncherComponentIsAmmoCompatible
{

    public static void Postfix(FixedTubeLauncherComponent __instance, IMunition ammo, ref bool __result)
    {
        if (AmmoFilter.IsAmmoCompatible(ammo, __instance, out bool value))
            __result = value;
    }
}

[HarmonyPatch(typeof(WeaponComponent), nameof(WeaponComponent.IsAmmoCompatible))]
class WeaponComponentIsAmmoCompatible
{
    static void Postfix(WeaponComponent __instance, IMunition ammo, ref bool __result)
    {
        if (AmmoFilter.IsAmmoCompatible(ammo, __instance, out bool value))
            __result = value;
    }
}

[HarmonyPatch(typeof(BulkMagazineComponent), nameof(BulkMagazineComponent.RestrictionCheck))]
class BulkMagazineComponentRestrictionCheck
{
    static void Postfix(BulkMagazineComponent __instance, IMunition ammoType, ref bool __result)
    {
        if (AmmoFilter.IsAmmoCompatible(ammoType, __instance, out bool value))
            __result = value;
    }
}