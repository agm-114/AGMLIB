using Ships;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FleetEditor;
using Munitions;
using System.Linq;
using System.Reflection;
using Utility;
using HarmonyLib;
using System.Diagnostics;

using UI;
using Debug = UnityEngine.Debug;
using static UnityEngine.ParticleSystem;
using Bundles;
using static UnityEngine.EventSystems.EventTrigger;
using System.Runtime.CompilerServices;
using Game;
using Ships.Serialization;
using Game.Units;
using Mirror;
using System.Xml.Linq;
using Factions;
using Procedural.Naming;
using Steamworks;
using System;
using UnityEngine.UI;
using Munitions.ModularMissiles;
using FleetEditor.MissileEditor;
using Munitions.ModularMissiles.Descriptors.Seekers;
using Munitions.ModularMissiles.Descriptors;
using Munitions.ModularMissiles.Descriptors.Controls;
using Game.Orders.Tasks;
using static UI.SequentialButton;
using UnityEngine.Serialization;

public class IndexedSocketFilter : MonoBehaviour, IFilterIndexed
{
    [SerializeField]
    protected List<string> _whitelist = new();
    [SerializeField]
    protected bool _whitelisteverything = false;
    [SerializeField]
    protected List<string> _blacklist = new();
    [SerializeField]
    protected bool _blacklisteverything = false;
    public List<string> Whitelist => _whitelist;
    public bool Whitelisteverything => _whitelisteverything;
    public List<string> Blacklist => _blacklist;
    public bool Blacklisteverything => _blacklisteverything;

    [SerializeField]
    protected int _index = 0;
    [SerializeField]
    protected bool _allindexes = false;
    public int Index => _index;
    public bool AllIndexes => _allindexes;
    public static IFilterIndexed FindIndexedFilter(List<IFilterIndexed> filters, int index = -1)
    {
        if (filters == null || filters.Count <= 0)
            return null;
        List <IFilterIndexed>  testlist = filters.FindAll(filter => filter.Index == index);
        if (testlist.Count > 0)
            return testlist.First();
        testlist = filters.FindAll(filter => filter.AllIndexes);
        if (testlist.Count > 0)
            return testlist.First();
        return null;
    }

    public static IFilterIndexed FindIndexedFilter(IEnumerable<IFilterIndexed> filters, int index = -1)
    {
        return FindIndexedFilter(filters.ToList(), index);
    }
}

[HarmonyPatch(typeof(MissileComponentPalette), nameof(MissileComponentPalette.SetEditingSocket))]
class MissileComponentPaletteSetEditingSocket
{
    static void Postfix(MissileComponentPalette __instance, MissileSocket socket, List<MissilePaletteItem> ____allComponents, ModularMissile ____editingMissile)
    {
        if (____editingMissile == null || ____editingMissile.Sockets.Count <= 0)
            return;
        int index = ____editingMissile.Sockets.ToList().FindIndex(testsocket => testsocket == socket);
        IFilterIndexed socketfilter = IndexedSocketFilter.FindIndexedFilter(____editingMissile.GetComponentsInChildren<IFilterIndexed>(), index) ?? new ScriptableFilter();
        foreach (MissilePaletteItem item in ____allComponents)
        {
            List<IFilterIndexed> filters = (item.Component as IModular)?.Modules?.Cast<IFilterIndexed>()?.ToList();
            IFilterIndexed componentfilters = IndexedSocketFilter.FindIndexedFilter(filters, index) ?? new ScriptableFilter();
            if (socketfilter.Blacklist.Contains(item.Component.SaveKey) || componentfilters.Blacklist.Contains(____editingMissile.SaveKey))
                item.gameObject.SetActive(value: false);
            else if (socketfilter.Whitelist.Contains(item.Component.SaveKey) || componentfilters.Whitelist.Contains(____editingMissile.SaveKey))
                item.gameObject.SetActive(value: true);
            else if (socketfilter.Blacklisteverything || componentfilters.Blacklisteverything)
                item.gameObject.SetActive(value: false);
        }
    }
}

[HarmonyPatch(typeof(MissileSettingsPane), "OpenSettingsPanel")]
class MissileSettingsPaneOpenSettingsPanel
{
    static void Postfix(MissileSettingsPane __instance, MissileComponentDescriptor component, ref bool __result, List<IMissileSettingsPane> ____settingsPanes)// 
    {
        if (!__result || component == null || component is not ILimited isettings)
            return;
        IMissileSettingsPane missileSettingsPane = ____settingsPanes.Where(missileSettingsPane => missileSettingsPane.Active).First();
        MissileCruiseAvionicsSettings cruisepane = missileSettingsPane as MissileCruiseAvionicsSettings;
        MissileDirectAvionicsSettings directpane = missileSettingsPane as MissileDirectAvionicsSettings;
        //isettings = new ModularCruiseGuidanceDescriptor();
        if (cruisepane == null && directpane == null)
            return;
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
/*

SequenceOption _disabledOption = new SequenceOption
{
    Text = "HOT",
    TextColor = GameColors.ColorName.Red
};
Common.SetVal(avionicssettings2, "_disabledOption", _disabledOption);
//_launchButton.SetDisabledText("HOT");
_launchButton.SetEnabled(enabled: false);
*/
//, MissileComponentDescriptor component, SequentialButton ____launchButton
//____launchButton.SetEnabled(enabled: false);
//SequentialButton _launchButton = Common.GetVal<SequentialButton>(avionicssettings, "_launchButton");
/*
SequenceOption[] _options = Common.GetVal<SequenceOption[]>(avionicssettings, "_options");
if (_options == null)
{
    Debug.LogError("Null Option ");

}
else
{
    SequenceOption _disabledOption = _options[1];
    Common.SetVal(avionicssettings, "_disabledOption", _disabledOption);
}*/


//_launchButton.SetOptionWithoutNotify(1, mixed: false);
//_launchButton.SetEnabled(false);
//_launchButton.SetDisabledText("HOT");
//_launchButton.SetEnabled(enabled: false);
