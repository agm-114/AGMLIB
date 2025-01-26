using static Ships.BaseCellLauncherComponent;
using static Ships.BulkMagazineComponent;

public class MagazineLoader : MonoBehaviour
{
    bool PullFromMagazines = true;
}

[HarmonyPatch(typeof(BaseCellLauncherComponent))]//, nameof(BaseCellLauncherComponent.)
[HarmonyPatch("NeedsExternalAmmoFeed", MethodType.Getter)]
class CellLauncherComponentNeedsExternalAmmoFeed
{
    public static void Postfix(ref bool __result, BaseCellLauncherComponent __instance)
    {
        if (__result)
            return;
        if (__instance.GetComponentInChildren<MagazineLoader?>() != null)
            __result = true;
    }
}

[HarmonyPatch(typeof(SettingsMagazineLoadout), nameof(SettingsMagazineLoadout.Set))]
class SettingsMagazineLoadoutSet
{
    public static Dictionary<SettingsMagazineLoadout, IMagazineProvider> Mags = new();

    public static void Postfix(SettingsMagazineLoadout __instance, EditorShipController ship, HullSocket socket, HullComponent component)
    {
        IMagazineProvider _provider = (component as IConfigurableMagazineLoadout).GetMagazineProvider<IMagazineProvider>();
        if (!Mags.ContainsKey(__instance))
            Mags.Add(__instance, _provider);
        Mags[__instance] = _provider;
    }
}

[HarmonyPatch(typeof(SettingsMagazineLoadout), nameof(SettingsMagazineLoadout.UpdateQuantities))]
class SettingsMagazineLoadoutUpdateQuantities
{

    public static void Postfix(SettingsMagazineLoadout __instance)
    {
        return;
        //Debug.Log("Postfix");
        if (!SettingsMagazineLoadoutSet.Mags.ContainsKey(__instance))
        {
            //Debug.LogError("Postfix Key Issue");
            return;
        }
        IMagazineProvider _provider = SettingsMagazineLoadoutSet.Mags[__instance];
        IMagazineProvider base_provider = _provider;
        TextMeshProUGUI _capacityText = Common.GetVal<TextMeshProUGUI>(__instance, "_capacityText");
        TextMeshProUGUI _emptyLoadText = Common.GetVal<TextMeshProUGUI>(__instance, "_emptyLoadText");
        Image _gaugeImage = Common.GetVal<Image>(__instance, "_gaugeImage");
        Button _addMagazineButton = Common.GetVal<Button>(__instance, "_addMagazineButton");
        List<MagazineAmmoItem> _magazines = Common.GetVal<List<MagazineAmmoItem>>(__instance, "_magazines");

        if (base_provider == null)
        {
            //Debug.LogError("Postfix Null" );
            return;
        }
        //else
        //    Debug.LogError("Postfix Good");

        int maxCapacity = base_provider.MaxCapacity;
        int usedCapacity = base_provider.UsedCapacity;
        float remainingCapacity = base_provider.RemainingCapacity;
        _gaugeImage.fillAmount = ((float)maxCapacity - remainingCapacity) / (float)maxCapacity;
        _gaugeImage.color = ((_gaugeImage.fillAmount > 0.9f) ? GameColors.Yellow : GameColors.Green);
        _capacityText.text = $"Capacity: {(float)usedCapacity + 0.25f}/{maxCapacity} {base_provider.UnitName}";
        _emptyLoadText.gameObject.SetActive(base_provider.NoLoad);
        foreach (MagazineAmmoItem magazine in _magazines)
        {
            magazine.UpdateRemainingSpace();
        }

        _addMagazineButton.interactable = base_provider.UsedCapacity < base_provider.MaxCapacity;

    }
}

[HarmonyPatch(typeof(BaseCellLauncherComponent), nameof(BaseCellLauncherComponent.GetDesignWarnings))]
class BaseCellLauncherComponentGetDesignWarnings
{
    public static bool Prefix(BaseCellLauncherComponent __instance, List<string> warnings)
    {
        MagazineLoader loader = __instance.GetComponentInChildren<MagazineLoader>();
        if (loader == null)
            return true;
        BaseCellMissileMagazine Missiles = Common.GetVal<BaseCellMissileMagazine>(__instance, "_missiles");

        if (Missiles.AnyMissileDesignsInvalid())
        {
            warnings.Add("Launcher on " + __instance.Socket.name + " contains invalid munition designs.");
        }
        BaseHull _myHull = Common.GetVal<BaseHull>(__instance, "_myHull");
        if (_myHull.MyShip.AmmoFeed.GetAllCompatibleAmmoTypes(__instance).Count == 0)
        {
            warnings.Add("No ammunition for weapon: " + __instance.ComponentName + " on " + __instance.Socket.name);
        }
        return false;
    }
}

[HarmonyPatch(typeof(BaseHull), nameof(BaseHull.EditorAmmoTypeInUse))]
class EditorDoesAnyWeaponUseAmmoType
{
    public static void Postfix(BaseHull __instance, IMunition ammoType, bool externalFeed, ref bool __result)
    {

        if (__result == true)
            return;
        foreach (IWeapon weapon in __instance.CollectComponents<IWeapon>().Where(weapon => weapon.IsAmmoCompatible(ammoType)))
        {
            if ((weapon as MonoBehaviour)?.GetComponentInChildren<MagazineLoader>() != null)
            {
                __result = true;
                return;
            }
        }
    }
}

[HarmonyPatch(typeof(Ship), nameof(Ship.LoadFromSave))]
class ShipLoadFromSave
{
    public static void Prefix(Ship __instance, ref SerializedShip saved, BaseHull ____hull)
    {

        if (__instance.GetComponent<ShipController>().enabled == false)
        {
            //Debug.LogError("In Editor");
            return;
        }

        foreach (SerializedHullSocket item in saved.SocketMap)
        {
            //Debug.LogError(item.Key + "  " +  item.ComponentName + "  " + BundleManager.Instance.GetHullComponent(item.ComponentName).GetType().ToString());
            CellLauncherComponent launcher = BundleManager.Instance.GetHullComponent(item.ComponentName) as CellLauncherComponent;
            //if(launcher != null) 
            //   Debug.LogError("Launcher Detected");
            CellLauncherData socketdata = item.ComponentData as CellLauncherData;

            if (launcher?.GetComponentInChildren<MagazineLoader>() == null)
                continue;
            //Debug.LogError("Loader Detected");

            socketdata.MissileLoad.RemoveAll(a => true);
            foreach (SerializedHullSocket magitem in saved.SocketMap)
            {
                //BulkMagazineComponent templauncher = ____hull?.GetSocket(magitem.Key)?.Component as BulkMagazineComponent;
                if (magitem.ComponentData is not BulkMagazineData magdata)
                    continue;
                //Debug.LogError("Mag Found");
                foreach (Magazine.MagSaveData data in magdata?.Load?.Where(a => a.Quantity > 0 && launcher.IsAmmoCompatible(____hull.MyShip.Fleet.AvailableMunitions.GetMunition(a.MunitionKey))))
                {
                    if (socketdata.MissileLoad.Any(load => load.MunitionKey == data.MunitionKey))
                    {
                        Magazine.MagSaveData existingdata = socketdata.MissileLoad.Find(load => load.MunitionKey == data.MunitionKey);
                        socketdata.MissileLoad.Remove(data);
                        existingdata.Quantity += data.Quantity;
                        socketdata.MissileLoad.Add(data);
                        //Debug.LogError("Loading Extra "+ data.Quantity + " "+ existingdata.Quantity + " ");
                        //Debug.LogError(existingdata);
                    }
                    else
                    {
                        Magazine.MagSaveData newdata = new();
                        newdata.MunitionKey = data.MunitionKey;
                        newdata.Quantity = data.Quantity;
                        newdata.MagazineKey = data.MagazineKey;
                        socketdata.MissileLoad.Add(data);
                        //Debug.LogError("Loading " + data.MunitionKey);

                    }
                }
                magdata.Load.RemoveAll(a => launcher.IsAmmoCompatible(____hull.MyShip.Fleet.AvailableMunitions.GetMunition(a.MunitionKey)));

            }
        }
        //Debug.LogError("InEditor " + __instance.GetComponent<ShipController>().enabled);
        //Debug.LogError("found L");

        //__instance.GetComponentInChildren<FixedCellLauncherComponent>().Slurp();
        //__instance.GetComponentInChildren<FixedCellLauncherComponent>().OnAmmoQuantityChanged.Invoke()
    }

    public static void Postfix(Ship __instance)
    {

        //__instance.GetComponentInChildren<FixedCellLauncherComponent>().Slurp();
        //__instance.GetComponentInChildren<FixedCellLauncherComponent>().OnAmmoQuantityChanged.Invoke()
    }
}
