using UnityEngine;
using Munitions.ModularMissiles.Descriptors.Controls;
using System.Collections.Generic;
using System;
using System.Reflection;
using Factions;
using System.Text.RegularExpressions;
using System.Linq;
using Game.Sensors;
using HarmonyLib;
using Ships;

[CreateAssetMenu(fileName = "New Scriptable Filter", menuName = "Nebulous/New Filter")]
public class ScriptableFilter : ScriptableObject, IFilterIndexed
{
    [SerializeField]
    protected List<string> _whitelist = new();
    [SerializeField]
    protected bool _whitelisteverything = false;
    [SerializeField]
    protected List<string> _blacklist = new();
    [SerializeField]
    protected bool _blacklisteverything = false;
    [SerializeField]
    protected int _index = 0;
    [SerializeField]
    protected bool _allindexes = false;
    public List<string> Whitelist => _whitelist;
    public bool Whitelisteverything => _whitelisteverything;
    public List<string> Blacklist => _blacklist;
    public bool Blacklisteverything => _blacklisteverything;
    public int Index => _index;
    public bool AllIndexes => _allindexes;
    [SerializeField]
    protected bool _allowIllegal = false;
    public bool AllowIllegal => _allowIllegal;
    [SerializeField]
    protected bool _bypassFactionRestrictions = false;
    public bool BypassFactionRestrictions => _bypassFactionRestrictions;
}

[CreateAssetMenu(fileName = "New Faction", menuName = "Nebulous/Modular Faction Description")]
public class ModularFactionDescription : FactionDescription, IModular
{
    [SerializeField]
    protected List<ScriptableObject> _modules = new();
    List<ScriptableObject> IModular.Modules => _modules;

    [SerializeField]
    private string[] _sharedFactionAllblacklist = new string[0];

    [SerializeField]
    private string[] _sharedEquipmentListblacklist = new string[0];

    [SerializeField]
    private string[] _sharedEquipmentPatternsblacklist = new string[0];

    public string[] SharedFactionAll => Common.GetVal<string[]>(this, "_sharedFactionAll");


    public string[] SharedEquipmentList => Common.GetVal<string[]>(this, "_sharedFactionAll");


    public string[] SharedEquipmentPatterns => Common.GetVal<string[]>(this, "_sharedFactionAll");

    [SerializeField]
    [HideInInspector]
    private string _saveKey;

    [SerializeField]
    [HideInInspector]
    private ulong _modId;

    private HashSet<string> _sharedFactionSet;

    private HashSet<string> _sharedEquipmentSet;

    private HashSet<string> _sharedFactionSetblacklist;

    private HashSet<string> _sharedEquipmentSetblacklist;

    private Regex[] _shareEquipmentRegex;

    private Regex[] _shareEquipmentRegexblacklist;


    public bool FullCheckSharedEquipment(string checkKey, string primaryFactionKey, bool includeFactionAll = true)
    {
        if (_sharedFactionSet == null)
        {
            _sharedFactionSet = new HashSet<string>(SharedFactionAll);
        }

        if (_sharedEquipmentSet == null)
        {
            _sharedEquipmentSet = new HashSet<string>(SharedEquipmentList);
        }

        if (_shareEquipmentRegex == null)
        {
            _shareEquipmentRegex = new Regex[SharedEquipmentPatterns.Length];
            for (int i = 0; i < SharedEquipmentPatterns.Length; i++)
            {
                _shareEquipmentRegex[i] = new Regex(SharedEquipmentPatterns[i]);
            }
        }

        if (_sharedFactionSetblacklist == null)
        {
            _sharedFactionSetblacklist = new HashSet<string>(_sharedFactionAllblacklist);
        }

        if (_sharedEquipmentSetblacklist == null)
        {
            _sharedEquipmentSetblacklist = new HashSet<string>(SharedEquipmentList);
        }

        if (_shareEquipmentRegexblacklist == null)
        {
            _shareEquipmentRegexblacklist = new Regex[_sharedEquipmentPatternsblacklist.Length];
            for (int i = 0; i < _sharedEquipmentPatternsblacklist.Length; i++)
            {
                _shareEquipmentRegexblacklist[i] = new Regex(_sharedEquipmentPatternsblacklist[i]);
            }
        }

        if (_sharedEquipmentSetblacklist.Contains(checkKey))
            return false;
        else if (_sharedEquipmentSet.Contains(checkKey))
            return true;
        else if (_shareEquipmentRegexblacklist.Any((Regex x) => x.IsMatch(checkKey)))
            return false;
        else if (_shareEquipmentRegex.Any((Regex x) => x.IsMatch(checkKey)))
            return true;
        else if (_sharedFactionSet.Contains(primaryFactionKey))
            return false;
        return (!string.IsNullOrEmpty(primaryFactionKey) && includeFactionAll && _sharedFactionSet.Contains(primaryFactionKey));
    }
}

[HarmonyPatch(typeof(FactionDescription), nameof(FactionDescription.CheckSharedEquipment))]
class FactionDescriptionCheckSharedEquipment
{
    static void Postfix(FactionDescription __instance, string checkKey, string primaryFactionKey, bool includeFactionAll, ref bool __result)
    {
        if (__instance is ModularFactionDescription FactionDescription)
            __result = FactionDescription.FullCheckSharedEquipment(checkKey, primaryFactionKey, includeFactionAll);
    }
}