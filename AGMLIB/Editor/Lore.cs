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
using static System.Net.Mime.MediaTypeNames;
using Image = UnityEngine.UI.Image;

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

    public String Prefixstring => _prefixstring?.ToString() ?? "";
    public String Postdescriptionstring => _postdescriptionstring?.ToString() ?? "";
    public String Poststatsstring => _poststatsstring?.ToString() ?? "";
    public String Postresourcesstring => _postresourcesstring?.ToString() ?? "";
    public String Postbuffstring => _postbuffstring?.ToString() ?? "";
    public String Postlorestring => _postlorestring?.ToString() ?? "";
    public Sprite LoreIcon => _loreicon;
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


[HarmonyPatch(typeof(HullListItem), nameof(HullListItem.GetDetails))]
class HullListItemGetDetails
{
    static void Postfix(PaletteItem __instance, BaseHull ____hull, out string title, out string subtitle, out Sprite image, out string details)
    {

        //ref string title, ref string subtitle, ref Sprite image, ref string details
        BaseHull _hull = ____hull;
        Lore lore = _hull.GetComponentInChildren<Lore>() ?? new();
        title = _hull.ClassName;
        subtitle = "(" + _hull.HullClassification + " Class)";
        image = _hull.HullScreenshot;
        details = lore.Prefixstring;
        details += _hull.LongDescription + "\n\n" + _hull.EditorFormatHullStats(showBreakdown: false);
        details += lore.Postdescriptionstring;
        string buffs = _hull.EditorFormatHullBuffs();
        if (buffs != null)
        {
            details = details + "\n\n<b>Modifiers:</b>\n" + buffs;
            
        }
        details += lore.Postbuffstring;
        if (!string.IsNullOrEmpty(_hull.FlavorText))
        {
            details = details + "\n<i><color=#FFEF9E>" + _hull.FlavorText + "</color></i>";
        }
        details += lore.Postlorestring;

    }
}
