using FleetEditor.MissileEditor;
using Munitions.ModularMissiles;
using Munitions.ModularMissiles.Descriptors;
using System.Net;
using Munitions.ModularMissiles.Runtime;

/*
[HarmonyPatch(typeof(ModularMissile), "InstallSocketModuleInternal")]
class MissileComponentDescriptorInstallSocketModuleInternal
{
    public static MissileSocketType? SocketType = null;
    public static String FactionKey = null;
    static void Prefix(ModularMissile __instance, MissileSocket socket, MissileComponentDescriptor component, FactionDescription ____fleetFaction)
    {
        Common.LogPatch();
        MissileComponentDescriptor Component = component;
        if (component == null || socket == null)
            return;
        int index = __instance.Sockets.ToList().FindIndex(testsocket => testsocket == socket);
        List<IFilterIndexed> filters = (Component as IModular)?.Modules?.Cast<IFilterIndexed>()?.ToList();

        IFilterIndexed socketfilter = Modular.FindIndexedFilter(__instance.GetComponentsInChildren<IFilterIndexed>(), index) ?? Modular.Default;
        IFilterIndexed componentfilters = Modular.FindIndexedFilter(filters, index) ?? Modular.Default;
        
        if (socket.ComponentPermitted(component, ____fleetFaction))
            return;
        if (socketfilter.Whitelist.Contains(Component.SaveKey) || componentfilters.Whitelist.Contains(__instance.BaseMissileDesignation) || socketfilter.Whitelisteverything || componentfilters.Whitelisteverything)
        {
            Debug.LogError("Whitelisted");
            foreach(string whitelist in socket.RestrictToModules)
            {
                Debug.LogError($"Built in whitelist {whitelist}");
            }

            if (!Component.UseableByFaction(____fleetFaction))
            {
                Debug.LogError("Faction Issue");
                if (socketfilter.BypassFactionRestrictions && socketfilter.Whitelist.Contains(Component.SaveKey))
                {
                    FactionKey = Component.FactionKey;
                    Common.SetVal(component, "_factionKey", null);
                    Debug.LogError("Patching Faction");
                }

                else if (componentfilters.BypassFactionRestrictions && componentfilters.Whitelist.Contains(Component.SaveKey))
                {
                    FactionKey = Component.FactionKey;
                    Common.SetVal(component, "_factionKey", null);
                    Debug.LogError("Patching Faction");

                }
            }
            bool compatible = !((socket.SocketType & Component.FitsSocketType) == 0);

            if (!compatible)
            {
                Debug.LogError("Compatiblity Issue");
                if (socketfilter.AllowIllegal && socketfilter.Whitelist.Contains(Component.SaveKey))
                {
                    SocketType = socket.SocketType;
                    socket.SocketType = Component.FitsSocketType;
                    Debug.LogError("Patching Type");
                }
                else if (componentfilters.AllowIllegal && componentfilters.Whitelist.Contains(Component.SaveKey))
                {
                    SocketType = socket.SocketType;
                    socket.SocketType = Component.FitsSocketType;
                    Debug.LogError("Patching Type");

                }
            }
            if (!socket.ComponentPermitted(component, ____fleetFaction))
            {
                Debug.LogError("Patching issue");
            }
        }
    }

    static void Postfix(ModularMissile __instance, MissileSocket socket, MissileComponentDescriptor component)
    {
        Common.LogPatch();
        Debug.LogError("Cleanup");
        if (SocketType.HasValue)
        {
            socket.SocketType = SocketType.Value;
        }
        if (FactionKey != null)
        {
            Common.SetVal(component, "_factionKey", FactionKey);
        }
        FactionKey = null;
        SocketType = null;

    }
}


[HarmonyPatch(typeof(MissileComponentPalette), nameof(MissileComponentPalette.SetEditingSocket))]
class MissileComponentPaletteSetEditingSocket
{
    static void Postfix(MissileComponentPalette __instance, MissileSocket socket, List<MissilePaletteItem> ____allComponents, ModularMissile ____editingMissile, FactionDescription ____fleetFaction)
    {
        Common.LogPatch();
        if (socket == null || ____editingMissile == null || ____editingMissile.Sockets.Count <= 0)
            return;
        
        int index = ____editingMissile.Sockets.ToList().FindIndex(testsocket => testsocket == socket);
        IFilterIndexed socketfilter = Modular.FindIndexedFilter(____editingMissile.GetComponentsInChildren<IFilterIndexed>(), index) ?? Modular.Default;
        foreach (MissilePaletteItem item in ____allComponents.Where(item => item.Component != null))
        {
            MissileComponentDescriptor Component = item?.Component;
            GameObject buttonGO = item.gameObject;
            List<IFilterIndexed> filters = (Component as IModular)?.Modules?.Cast<IFilterIndexed>()?.ToList();

            IFilterIndexed componentfilters = Modular.FindIndexedFilter(filters, index) ?? Modular.Default;
            Debug.LogError("Comp Savekey: " + item.Component.SaveKey);
            if (socketfilter.Blacklist.Contains(Component.SaveKey) || componentfilters.Blacklist.Contains(____editingMissile.BaseMissileDesignation))
            {
                Debug.LogError("BlackListed");
                buttonGO.SetActive(value: false);
                continue;
            }
            else if (socketfilter.Whitelist.Contains(Component.SaveKey) || componentfilters.Whitelist.Contains(____editingMissile.BaseMissileDesignation))
            {
                bool compatible = !((socket.SocketType & Component.FitsSocketType) == 0);

                if (Component.UseableByFaction(____fleetFaction) && compatible)
                {
                    buttonGO.SetActive(value: true);
                    Debug.LogError("Generally Compatible");
                }

                if (!Component.UseableByFaction(____fleetFaction))
                {
                    Debug.LogError("Faction Issue");

                    if (socketfilter.BypassFactionRestrictions && socketfilter.Whitelist.Contains(Component.SaveKey))
                        buttonGO.SetActive(value: true);
                    else if (componentfilters.BypassFactionRestrictions && componentfilters.Whitelist.Contains(Component.SaveKey))
                        buttonGO.SetActive(value: true);
                }
                if (!compatible)
                {
                    Debug.LogError("compatible Issue");

                    if (socketfilter.AllowIllegal && socketfilter.Whitelist.Contains(Component.SaveKey))
                        buttonGO.SetActive(value: true);
                    else if (componentfilters.AllowIllegal && componentfilters.Whitelist.Contains(Component.SaveKey))
                        buttonGO.SetActive(value: true);

                }
                continue;
            }
            else if (socketfilter.Blacklisteverything || componentfilters.Blacklisteverything)
            {
                Debug.LogError("Never Active");
                buttonGO.SetActive(value: false);

            }
            else if (socketfilter.Whitelisteverything || componentfilters.Whitelisteverything)
            {
                Debug.LogError("Always Active");
                buttonGO.SetActive(value: true);
            }
        }
    }
}
*/
[HarmonyPatch(typeof(MissileSocket), nameof(MissileSocket.ComponentPermitted))]
class MissileSocketComponentPermittedPatch
{
    static void Postfix(MissileSocket __instance, MissileComponentDescriptor component, FactionDescription fleetFaction, ref bool __result)
    {
        if (component == null)
            return;

        ModularMissile missile = __instance.Missile;
        if (missile == null)
            return;

        // Find the index of this socket within the missile
        int index = missile.Sockets.ToList().IndexOf(__instance);
        if (index == -1)
            return;

        // Resolve custom filters
        List<IFilterIndexed> filters = (component as IModular)?.Modules?.Cast<IFilterIndexed>()?.ToList();
        IFilterIndexed socketFilter = Modular.FindIndexedFilter(missile.GetComponentsInChildren<IFilterIndexed>(), index) ?? Modular.Default;
        IFilterIndexed componentFilter = Modular.FindIndexedFilter(filters, index) ?? Modular.Default;

        if (socketFilter.Blacklist.Contains(component.SaveKey) ||
            componentFilter.Blacklist.Contains(missile.BaseMissileDesignation))
        {
            __result = false;
            return;
        }

        bool CheckLegal()
        {
            if (fleetFaction != null && !component.UseableByFaction(fleetFaction))
            {
                bool bypassFaction = (socketFilter.BypassFactionRestrictions && socketFilter.Whitelist.Contains(component.SaveKey)) ||
                                     (componentFilter.BypassFactionRestrictions && componentFilter.Whitelist.Contains(component.SaveKey));
                if (!bypassFaction)
                {
                    return false;
                }
            }

            // Evaluate Socket Type Compatibility Bypasses
            if ((__instance.SocketType & component.FitsSocketType) == 0)
            {
                bool allowIllegal = (socketFilter.AllowIllegal && socketFilter.Whitelist.Contains(component.SaveKey)) ||
                                    (componentFilter.AllowIllegal && componentFilter.Whitelist.Contains(component.SaveKey));
                if (!allowIllegal)
                {
                    return false;

                }
            }
            return true;
        }
        if (socketFilter.Whitelist.Contains(component.SaveKey) || componentFilter.Whitelist.Contains(missile.BaseMissileDesignation))
        {
            __result = CheckLegal();
            return;
        }

        if (socketFilter.Blacklisteverything || componentFilter.Blacklisteverything)
        {
            __result = false;
            return;
        }


        if (socketFilter.Whitelisteverything || componentFilter.Whitelisteverything)
        {
            __result = CheckLegal();
            return;
        }


        // If no custom rules matched, we don't alter __result.
        // It stays whatever the base game method returned natively.
    }
}


[HarmonyPatch(typeof(MissileSettingsPane), "OpenSettingsPanel")]
class MissileSettingsPaneOpenSettingsPanel
{
    static void Prefix(MissileSettingsPane __instance, MissileComponentDescriptor component, List<IMissileSettingsPane> ____settingsPanes)
    {
        Common.LogPatch();
        List<IMissileSettingsPane> _settingsPanes = ____settingsPanes;
        foreach (IMissileSettingsPane settingsPane in _settingsPanes.Where(settingPane => settingPane.Name == (component?.SettingsPanel ?? "")))
        {
            MissileCruiseAvionicsSettings cruisepane = settingsPane as MissileCruiseAvionicsSettings;
            MissileDirectAvionicsSettings directpane = settingsPane as MissileDirectAvionicsSettings;

            if (cruisepane == null && directpane == null)
                continue;
            ModularDirectGuidanceDescriptor buttonlist = new ModularDirectGuidanceDescriptor();
            foreach (KeyValuePair<string, int> lockoptions in buttonlist.RestrictedOptions)
            {
                SequentialButton _launchButton = Common.GetVal<SequentialButton>(settingsPane, lockoptions.Key);
                _launchButton?.SetOverride(null);
                _launchButton?.SetEnabled(enabled: true);
            }
        }
        if (component == null)
        {
            //Debug.LogError("Null Component");

            return;
        }
        //Debug.LogError("Prefix " + component.SaveKey + component.SettingsPanel);
    }
    static void Postfix(MissileSettingsPane __instance, MissileComponentDescriptor component, ref bool __result, List<IMissileSettingsPane> ____settingsPanes)// 
    {
        Common.LogPatch();
        if (component == null)
            return;
        //Debug.LogError("Postfix " + component.SaveKey + __result);

        if (!__result || component == null)
            return;
        if (component is not ILimited isettings)
            return;
        IMissileSettingsPane missileSettingsPane = ____settingsPanes.Where(missileSettingsPane => missileSettingsPane.Active).First();
        MissileCruiseAvionicsSettings cruisepane = missileSettingsPane as MissileCruiseAvionicsSettings;
        MissileDirectAvionicsSettings directpane = missileSettingsPane as MissileDirectAvionicsSettings;
        //isettings = new ModularCruiseGuidanceDescriptor();
        if (cruisepane == null && directpane == null)
            return;
        //Debug.LogError("Restricted Missile");
        foreach (KeyValuePair<string, int> lockoptions in isettings.RestrictedOptions)
        {
            if (lockoptions.Value < 0)
                continue;
            SequentialButton _launchButton = Common.GetVal<SequentialButton>(missileSettingsPane, lockoptions.Key);
            _launchButton.SetOverride(lockoptions.Value);
            _launchButton.SetEnabled(enabled: true);
            switch (lockoptions.Key)
            {
                case "_roleButton":
                    cruisepane?.ButtonSetRole(lockoptions.Value);
                    directpane?.ButtonSetRole(lockoptions.Value);
                    break;
                case "_launchButton":
                    cruisepane?.ButtonSetLaunch(lockoptions.Value);
                    directpane?.ButtonSetLaunch(lockoptions.Value);
                    break;
                case "_targetLostBehavior":
                    cruisepane?.ButtonSetTargetLost(lockoptions.Value);
                    directpane?.ButtonSetTargetLost(lockoptions.Value);
                    break;
                case "_terminalManeuvers":
                    cruisepane?.ButtonSetManeuvers(lockoptions.Value);
                    directpane?.ButtonSetManeuvers(lockoptions.Value);
                    break;
                case "_approachButton":
                    directpane?.ButtonSetAAC(lockoptions.Value);
                    break;
                default:
                    break;
            }
        }
    }
}

[HarmonyPatch(typeof(RuntimeMissileWarhead), "ShouldFuzeOnTarget")]
class RuntimeMissileWarhead_ShouldFuzeOnTargetPatch
{
    static bool Prefix(RuntimeMissileWarhead __instance, MunitionHitInfo hitInfo, bool trigger, ref bool __result)
    {
        try
        {
            //Debug.LogError("RuntimeMissileWarhead-ShouldFuzeOnTarget");
            // Get the SensorTrack of the hit target so we can get the owner
            ISensorTrackable component = hitInfo.HitObject.transform.root.GetComponent<ISensorTrackable>();
            IPlayer owner = __instance.Missile.OwnedBy;
            // Get the IFF relationship
            IFF iffStatus = component.GetIFF(owner);
            //Debug.LogError("IFF Status: " + iffStatus);

            // Get the Proxy Fuse of the missile
            BoxCollider collider = Traverse.Create(__instance.Missile).Field("_proxFuze").GetValue<BoxCollider>();

            // Make sure the proxy fuse exists before performing the bypass check
            if (collider != null && collider.size.z > 0)
            {
                // IFF returning IFF.NONE results in Fuse triggering.  IFF.NONE return only seems to happen for the first check between a submunition and it's launcher.
                // Thus just skip this Fuze Target
                if (iffStatus == IFF.None)
                {
                    return false;
                }
            }

            return true;
        }
        catch (Exception e)
        {
            return true;
        }
    }
}
