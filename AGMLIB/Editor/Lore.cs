using Ships;
using System.Collections.Generic;
using UnityEngine;
using FleetEditor;
using System.Linq;
using HarmonyLib;
using Bundles;
using UnityEngine.UI;
using Utility;
using System;

public class Lore : MonoBehaviour
{
    [SerializeField]
    protected Sprite _loreicon;

    [SerializeField]
    protected StringFormatter _prefixstring = new();
    [SerializeField]
    protected StringFormatter _postdescriptionstring = new();
    [SerializeField]
    protected StringFormatter _poststatsstring = new();
    [SerializeField]
    protected StringFormatter _postresourcesstring = new();
    [SerializeField]
    protected StringFormatter _postbuffstring = new();
    [SerializeField]
    protected StringFormatter _postlorestring = new();

    public String Prefixstring { get => _prefixstring.ToString(); }
    public String Postdescriptionstring { get => _postdescriptionstring.ToString(); }
    public String Poststatsstring { get => _poststatsstring.ToString(); }
    public String Postresourcesstring { get => _postresourcesstring.ToString(); }
    public String Postbuffstring { get => _postbuffstring.ToString(); }
    public String Postlorestring { get => _postlorestring.ToString(); }
    public Sprite LoreIcon { get => _loreicon; }
}

[HarmonyPatch(typeof(PaletteItem), nameof(PaletteItem.SetComponent))]
class ComponentPaletteCreateItemPatch
{
    static void Postfix(HullComponent __instance, HullComponent component, Image ____modBadge)
    {
        if (!component?.SourceModId.HasValue ?? true)
            return;
        Sprite _loreicon = component.GetComponentInChildren<Lore>()?.LoreIcon ?? null;
        if(_loreicon != null)
            ____modBadge.sprite = _loreicon;
        //____modBadge.sprite = BundleManager.Instance.AllFactions.ToList()[2].SmallLogo;
    }





}

[HarmonyPatch(typeof(PaletteItem), "GetDetailText")]
class ComponentPaletteGetDetailTextPatch
{
    static void Postfix(PaletteItem __instance, HullComponent ____component, ref string __result)
    {
        HullComponent _component = ____component;
        Lore lore = _component.GetComponentInChildren<Lore>();
        if (_component == null || lore == null)
            return;
        string text = string.Empty;
        text += lore.Prefixstring;
        text += _component.GetFormattedDesc();
        text += lore.Postdescriptionstring;
        string stats = _component.GetFormattedStats(full: true);
        if (!string.IsNullOrEmpty(stats))
        {
            text = text + "\n<b>Stats:</b>\n" + stats;
        }
        text += lore.Poststatsstring;
        string resources = _component.GetFormattedResources();
        if (!string.IsNullOrEmpty(resources))
        {
            text = text + "\n<b>Resources:</b>\n" + resources;
        }
        text += lore.Postresourcesstring;
        string buffs = _component.GetFormattedBuffs();
        if (!string.IsNullOrEmpty(buffs))
        {
            text = text + "\n<b>Modifiers:</b>\n" + buffs;
        }
        text += lore.Postbuffstring;
        if (!string.IsNullOrEmpty(_component.FlavorText))
        {
            
            text = text + "\n<i><color=" + GameColors.FlavorTextColor + ">" + _component.FlavorText + "</color></i>"; //GameColors.GetTextColor(GameColors.ColorName.Red)
        }
        text += lore.Postlorestring;
        //text = text + "\n\n\n\n" + "<rotate=\"45\"> " + test + " </rotate>";
        __result = text;
    }


    


}
