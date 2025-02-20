using System.Text.RegularExpressions;

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
    public IList<string> Whitelist => _whitelist;
    public bool Whitelisteverything => _whitelisteverything;
    public IList<string> Blacklist => _blacklist;
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
    static bool oneoff = true;

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
    bool setup = true;

    public bool FullCheckSharedEquipment(string checkKey, string primaryFactionKey, bool includeFactionAll = true)
    {
        if (oneoff)
        {
            //Debug.LogError("Click Me");
            oneoff = false;
        }

        //Debug.Log("Checking " + checkKey + " " + primaryFactionKey);
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
        {
            //Debug.Log("equipmentset blacklist");
            return false;
        }
        else if (_sharedEquipmentSet.Contains(checkKey))
        {
            //Debug.Log("shared witelist");

            return true;

        }
        else if (_shareEquipmentRegexblacklist.Any((Regex x) => x.IsMatch(checkKey)))
        {
            //Debug.Log("equipmentregex blacklist");

            return false;
        }

        else if (_shareEquipmentRegex.Any((Regex x) => x.IsMatch(checkKey)))
        {
            //Debug.Log("equipmentregex whitelist");

            return true;

        }
        if(primaryFactionKey == this.SaveKey)
        {
            //Debug.Log("primarysavekey whitelist");

            return true;
        }

        //foreach (string faction in _sharedFactionSet)
        //    Debug.Log("faction: " + faction);

        //Debug.Log("null: " + !string.IsNullOrEmpty(primaryFactionKey) +  " faction null: " + includeFactionAll + " contains: " + _sharedFactionSet.Contains(primaryFactionKey));
        return (!string.IsNullOrEmpty(primaryFactionKey) && includeFactionAll && _sharedFactionSet.Contains(primaryFactionKey));
    }
}

[HarmonyPatch(typeof(FactionDescription), nameof(FactionDescription.CheckSharedEquipment))]
class FactionDescriptionCheckSharedEquipment
{
    static bool oneoff = true;
    static void Postfix(FactionDescription __instance, string checkKey, string primaryFactionKey, bool includeFactionAll, ref bool __result)
    {
        if (__instance is not ModularFactionDescription FactionDescription)
            return;
        bool oldresult = __result;
        __result = FactionDescription.FullCheckSharedEquipment(checkKey, primaryFactionKey, includeFactionAll);
        bool delta = oldresult != __result;
        //Debug.Log("delta: " + delta + " checkKey:" + checkKey + " old: " + oldresult + " new: " + __result + " factionkey: " + primaryFactionKey);
    }
}

[HarmonyPatch(typeof(LookaheadMunition), nameof(LookaheadMunition.UseableByFaction))]
class LookaheadMunitionUseableByFaction
{
    static bool oneoff = true;
    static void Postfix(LookaheadMunition __instance, FactionDescription faction, ref bool __result)
    {
        if (faction is not ModularFactionDescription FactionDescription)
            return;

        
        bool oldresult = __result;
        __result = FactionDescription.FullCheckSharedEquipment(__instance.SaveKey, __instance.FactionKey, true);
        bool delta = oldresult != __result;
        //Debug.Log("delta: " + delta + " checkKey:" + __instance.SaveKey + " old: " + oldresult + " new: " + __result + " factionkey: " + __instance.FactionKey);
    }
}

[HarmonyPatch(typeof(AvailableMunitionsSet))]
public class AvailableMunitionsSetConstructor
{
    [HarmonyPostfix]
    [HarmonyPatch(MethodType.Constructor, new Type[] { typeof(Fleet) })]
    static void PostfixConstructor(ref AvailableMunitionsSet __instance, Fleet fleet)
    {
        FactionDescription faction = fleet.Faction;
        if (faction is not ModularFactionDescription FactionDescription)
        {
            return;

        }
        List<IMunition> _allMunitions = Common.GetVal<List<IMunition>>(__instance, "_allMunitions");
        Dictionary<Guid, IMunition> _allMunitionsByNetKey = Common.GetVal<Dictionary<Guid, IMunition>>(__instance, "_allMunitionsByNetKey");
        foreach (IMunition munition in BundleManager.Instance.AllMunitions)
        {
            
            //Debug.Log(munition.MunitionName + " " + munition.GetType());

            if (FactionDescription.FullCheckSharedEquipment(munition.SaveKey, munition.FactionKey, true))
            {
                //Debug.Log("Constructor True");
            }
            else
            {
                //Debug.Log("Constructor False");

                _allMunitions.Remove(munition);
                _allMunitionsByNetKey.Remove(munition.NetworkSpawnKey);
            }
        }
    }
}

//[HarmonyPatch(typeof(LightweightMunitionBase), "IFactionLocked.UseableByFaction")]
class LightweightMunitionBaseUseableByFaction
{
    static bool oneoff = true;
    static void Postfix(LightweightMunitionBase __instance, FactionDescription faction, ref bool __result)
    {

        if (faction is not ModularFactionDescription FactionDescription)
            return;
        //Debug.Log("primaryFactionKey");
        bool oldresult = __result;
        __result = FactionDescription.FullCheckSharedEquipment(__instance.SaveKey, ((IMunition)__instance).FactionKey, true);
        bool delta = oldresult != __result;
        //Debug.Log("delta: " + delta + " checkKey:" + __instance.SaveKey + " old: " + oldresult + " new: " + __result + " factionkey: " + ((IMunition)__instance).FactionKey);
    }
}

//[HarmonyPatch(typeof(IFactionLocked), nameof(IFactionLocked.UseableByFaction))]
class IFactionLockedUseableByFaction
{
    static void Postfix(IFactionLocked __instance, FactionDescription faction, ref bool __result)
    {

        if (faction is not ModularFactionDescription FactionDescription || __instance is not LookaheadMunition ammo)
            return;
        bool oldresult = __result;
        __result = FactionDescription.FullCheckSharedEquipment(ammo.SaveKey, ammo.FactionKey, true);
        bool delta = oldresult != __result;
        //Debug.Log("delta: " + delta + " checkKey:" + ammo.SaveKey + " old: " + oldresult + " new: " + __result + " factionkey: " + ammo.FactionKey);
    }
}