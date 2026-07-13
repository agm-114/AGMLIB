using FleetEditor;
using Lib;
using Lib.Dynamic_Systems.Area;
using Munitions.ModularMissiles;
using Shapes;
using SmallCraft;
using Steamworks.Ugc;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.EventSystems;
using static Utility.GameColors;

//using static UnityEditorInternal.ReorderableList;#dll
using Random = System.Random;
/*
    <Reference Include="Unity.RenderPipelines.HighDefinition.Runtime">
      <HintPath>libs\Unity.RenderPipelines.HighDefinition.Runtime.dll</HintPath>
    </Reference>
 */
[HarmonyPatch(typeof(SettingsMagazineLoadout), nameof(SettingsMagazineLoadout.AddMagazine))]
class SettingsMagazineLoadoutAddMagazine
{
    static void Prefix(SettingsMagazineLoadout __instance)
    {
        //
        Common.LogPatch();


    }

    static void Postfix(SettingsMagazineLoadout __instance)
    {
        Common.LogPatch();
        
        ModalListSelectDetailed select = (ModalListSelectDetailed)MenuController.Instance.GetTopMenu();
        if (select == null)
            return;

        IMagazineProvider _provider = Common.GetVal<IMagazineProvider>(__instance, "_provider");
        BaseHull _hull = Common.GetVal<BaseHull>(__instance, "_hull");
        /*
        _openPalette.AddFilter("Show Only Filtered", delegate (object data)
        {
            IMunition ammo2 = data as IMunition;
            return true;
        }, initialState: true);
        */
        select.RemoveFilters();
        //Debug.LogError("Finished Delegates");
 

        bool anyTemplates = false;
        foreach (MissileTemplate template in _hull.MyShip.Fleet.AvailableMunitions.AllMissileTemplates)
        {
            if (!template.InstanceInFleet && _provider.RestrictionCheck(template.MissileBody))
            {
                anyTemplates = true;
                break;
            }
        }
        if (_provider.CanFeedExternally)
        {
            List<IWeapon> weapons = _hull.CollectComponents<IWeapon>();
            List<Spacecraft> craft = _hull.MyShip.UniqueCraftTypesOnShip();
            List<AmmoConsumingFalloffEffect> ammoeffects = _hull.gameObject.GetComponentsInChildren<AmmoConsumingFalloffEffect>().ToList();
            select.AddFilter("$UI_FLTED_SELECTAMMO_CURRENTONLY", delegate (object data)
            {
                IMunition ammo2 = data as IMunition;
                if (ammo2 != null)
                {
                    return 
                        weapons.Any((IWeapon x) => x.NeedsExternalAmmoFeed && x.IsAmmoCompatible(ammo2)) || 
                        craft.Any((Spacecraft x) => x.AnyLoadoutUsesAmmoType(ammo2)) ||
                        ammoeffects.Any(effect => effect.ValidAmmo.Any(x => x.IsAmmoCompatible(ammo2)));
                        ;
                }
                MissileTemplate template2 = data as MissileTemplate;
                return template2 == null || 
                weapons.Any((IWeapon x) => x.NeedsExternalAmmoFeed && x.IsAmmoCompatible(template2.MissileBody));
            }, initialState: true);
            //select.AddFilter("Actually Good", delegate (object data) { return false; }, true);
        }
        if (anyTemplates)
        {
            select.AddFilter("$UI_FLTED_SELECTAMMO_ADDEDONLY", (object data) => !(data is MissileTemplate), initialState: true);
        }
    }
}

[HarmonyPatch(typeof(Ship), nameof(Ship.LoadFromSave))]
class LoadPatch
{
    static void Postfix(Ship __instance)
    {
        Common.LogPatch();
        HullSocket[] sockets = __instance.GetComponentsInChildren<HullSocket>();
        foreach (HullSocket socket in sockets)
        {
            SocketFilters socketFilters = socket.GetComponent<SocketFilters>();
            if (socketFilters != null && socket.Component == null && !socketFilters.AllowNullComponent)
                socket.SetComponent(null);
        }
    }
}

[HarmonyPatch(typeof(Ship), nameof(Ship.RebuildAmmoFeeder))]
class ShipRebuildAmmoFeeder
{
    static void Postfix(Ship __instance)
    {
        //Debug.LogError("Rebuilding Ammo Feeder");

        foreach (HullSocket socket in __instance.Hull.AllSockets)
        {
            SocketFilters.EnsureChildSetup(socket, socket.Component);
            //if (socket.Component != null)
                //Debug.LogError("Socket Build" + socket.name + " has component " + socket.Component.name);

        }
        foreach (HullSocket socket in __instance.Hull.AllSockets)
        {
            if (socket.Component != null && !SocketFilters.CheckLegal(socket, socket.Component))
            {
                //Debug.LogError("Validator: Illegal component installed on socket: " + socket.name);
                socket.SetComponent(null);
            }
            else if (socket.Component != null)
            {
                //Debug.LogError("Validator: legal component installed on socket: " + socket.name);
            }
        }

    }
}

[HarmonyPatch(typeof(SocketItem), "SetComponent")]
class SocketItemSetComponent
{
    static void Prefix(SocketItem __instance, HullComponent component)
    {
        SocketEditorUISettings? settings = component?.gameObject?.GetComponent<SocketEditorUISettings>();
        settings = settings ?? __instance.Socket.gameObject.GetComponent<SocketEditorUISettings>();
        if (settings == null)
            return;
        Common.SetVal(__instance, "_installedColor", settings.InstalledColor);
    }

    static void Postfix(SocketItem __instance, HullComponent component)
    {
        Common.SetVal(__instance, "_installedColor", ColorName.DarkBlue);
    }

}

[HarmonyPatch(typeof(HullSocket), nameof(HullSocket.SetComponent))]
class HullSocketPatch
{
    static bool HandleNullSocket(HullSocket socket, ref HullComponent componentPrefab)
    {
        if (componentPrefab != null)
        {
            return false;
        }
        SocketFilters socketFilters = socket.GetComponent<SocketFilters>() ?? new();
        if (socketFilters.AllowNullComponent)
        {
            //Common.Trace("Null Socket OK");
            return false;
        }

        //Common.Trace("Handle Null Socket");

        componentPrefab = BundleManager.Instance.GetHullComponent(socket.DefaultComponent);
        if (componentPrefab == null)
        {
            return true;
            Common.Hint(socket, $"{socket.Key} Does not have a valid default component {socket.DefaultComponent} and doesn't allow null component");
        }
        return false;
    }

    static bool Prefix(HullSocket __instance, ref HullComponent componentPrefab)
    {
        Common.LogPatch();
        HullSocket socket = __instance;
        SocketFilters socketFilters = socket.GetComponent<SocketFilters>() ?? new();
        
        //Common.Trace($"Socket Filter Key: {socket.Key} Allow Null {socketFilters.AllowNullComponent} comp key {componentPrefab?.SaveKey}");

        if (socket.GetComponent<SocketFilters>() == null && componentPrefab?.GetComponent<SocketFilters>() == null)
            return true;

        if (socket.Component is HullComponent _component && false)
        {
            _component.SetSocket(null);
            CheckDepedends(socket, true, true);
            _component.enabled = false;
            UnityEngine.Object.Destroy(_component.gameObject);
            Common.SetVal(socket, "_component", null);
            //SetPrivateField(_component, "_size", _component);
            socket.UpdateColliderActive();
        }
        //return true;
        //Common.Trace(componentPrefab.Category);

        //Common.Trace("Set Component Prefix");
        //if(componentPrefab != null)
        //    Common.Trace("General Install " + componentPrefab.SaveKey);

        if (HandleNullSocket(socket, ref componentPrefab))
            return true;


        if (!CheckDepedends(socket))
        {
            //Common.Trace("Bad Depedends Check");
            //return false;
        }


        if (!SocketFilters.CheckLegal(socket, componentPrefab))
        {
            //Common.Trace("Illegal socket");
            componentPrefab = null;
            if (HandleNullSocket(socket, ref componentPrefab))
                return true;

        }
        else if (!componentPrefab?.TestSocketFit(socket.Size) ?? false)
        {

            //Common.Trace("poor socket");
            socketFilters.Size = socket.Size.Dimensions;
            socketFilters.Resized = true;
            Common.SetVal(socket, "_size", componentPrefab.Size);
            return true;
        }

        return true;
    }

    static void Postfix(HullSocket __instance)
    {
        Common.LogPatch();
        HullSocket socket = __instance;

        CheckDepedends(socket, true);
        SocketFilters socketFilters = socket.GetComponent<SocketFilters>() ?? new SocketFilters();

        if (socketFilters.Resized && socketFilters.Size != null)
        {
            //Common.Trace("Reseting size");
            Common.SetVal(__instance, "_size", socketFilters.Size);
            socketFilters.Resized = false;
        }
        /*
        if(socket.Component == null)
            Common.Trace("Component Null");
        else
            Common.Trace(socket.Component.ComponentName);
        */
        if (socket.Component == null && !socketFilters.AllowNullComponent)
        {
            //Common.Trace("Comp Removed");
            HullComponent componentPrefab = null;
            if (HandleNullSocket(socket, ref componentPrefab))
                return;
            socket.SetComponent(componentPrefab);
        }
        Finalize(__instance);

    }
    public static void Finalize(HullSocket __instance)
    {
        //Needed for dynamic reduction
        //__instance.GetComponentInParent<Ship>().EditorRecalcCrewAndResources();
    }

    static bool CheckDepedends(HullSocket socket, bool write = false, bool remove = false)
    {
        if (socket == null)
            return true;
        ComponentDependencies? componentDependencies = socket.Component?.GetComponent<ComponentDependencies>();
        if (componentDependencies == null) return true;
        HullSocket[] sockets = socket.gameObject.transform.parent.GetComponentsInChildren<HullSocket>();

        bool returnval = true;
        foreach (KeyValuePair<string, string> componentDependency in componentDependencies.Dependendents)
        {
            HullSocket testSocket = sockets.Where(testSocket => componentDependency.Key == testSocket.Key).First();
            //Common.Trace("D " + componentDependency.Key + " " + componentDependency.Value);
            if (remove)
            {
                testSocket.SetComponent(null);

            }
            else if (write && (componentDependencies.HardInstallDepedenents || (componentDependency.Value != testSocket?.Component?.SaveKey && componentDependencies.InstallDepedenents)))
            {
                testSocket?.SetComponent(BundleManager.Instance.GetHullComponent(componentDependency.Value));
            }
        }

        foreach (KeyValuePair<string, string> componentDependency in componentDependencies.Requirements)
        {
            HullSocket testSocket = sockets.Where(testSocket => componentDependency.Key == testSocket.Key).First();
            //Common.Trace("R " + componentDependency.Key + " " + componentDependency.Value);
            if (remove)
                testSocket.SetComponent(null);
            else if (componentDependency.Value != testSocket?.Component?.SaveKey || componentDependencies.HardInstallDepedenents)
            {
                if (componentDependencies.InstallRequirements && write)
                    //Debug.LogError("Add");
                    testSocket?.SetComponent(BundleManager.Instance.GetHullComponent(componentDependency.Value));
                else
                    returnval = false;
            }
        }

        //return true;
        return componentDependencies.InstallRequirements || returnval;

    }
}
