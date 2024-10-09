using Image = UnityEngine.UI.Image;
public class Lore : MonoBehaviour
{
    [SerializeField]
    protected Sprite _loreicon;

    public TMP_FontAsset Font;

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

    public TextMeshProUGUI _detailTitle;
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

[HarmonyPatch(typeof(ModalListSelectDetailed), "SelectItemInternal")]
class ModalListSelectDetailedSelectItemInternal
{
    static void Postfix(ModalListSelectDetailed __instance, SelectableListItem selected, TextMeshProUGUI ____detailText)
    {
        if(selected is PaletteItem listItem)
        {

            TMP_FontAsset font = listItem?.Component?.GetComponentInChildren<Lore>()?.Font ?? null;
            if (font != null) 
                ____detailText.font = font;

        }

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
        List<(string, string)> rows = new List<(string, string)>();
        _component.GetFormattedStats(rows, full: true);
        string text2 = rows.StatRowsToTextBlock();
        string formattedResources = _component.GetFormattedResources();
        string formattedBuffs = _component.GetFormattedBuffs();
        if (!string.IsNullOrEmpty(text2))
        {
            text = text + "\n<b>Stats:</b>\n" + text2;
        }
        text += lore.Poststatsstring;

        if (!string.IsNullOrEmpty(formattedResources))
        {
            text = text + "\n<b>Resources:</b>\n" + formattedResources;
        }
        text += lore.Postresourcesstring;

        if (!string.IsNullOrEmpty(formattedBuffs))
        {
            text = text + "\n<b>Modifiers:</b>\n" + formattedBuffs;
        }
        text += lore.Postbuffstring;

        if (!string.IsNullOrEmpty(_component.FlavorText))
        {
            text = text + "\n<i><color=" + GameColors.FlavorTextColor + ">" + _component.FlavorText + "</color></i>";
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
        BaseHull _hull = ____hull;
        Lore lore = _hull.GetComponentInChildren<Lore>() ?? new();
        title = _hull.ClassName;
        subtitle = "(" + _hull.HullClassification + " Class)";
        image = _hull.HullScreenshot;
        _hull.EditorFormatHullStats(out var hull, out var sigs, showBreakdown: false);
        details = lore.Prefixstring;
        details += _hull.LongDescription + "\n\n" + hull.StatRowsToTextBlock() + "\n\n" + sigs.StatRowsToTextBlock();
        details += lore.Postdescriptionstring;
        string text = _hull.EditorFormatHullBuffs();
        if (text != null)
        {
            details = details + "\n\n<b>Modifiers:</b>\n" + text;
        }
        details += lore.Postbuffstring;
        if (!string.IsNullOrEmpty(_hull.FlavorText))
        {
            details = details + "\n<i><color=#FFEF9E>" + _hull.FlavorText + "</color></i>";
        }
        details += lore.Postlorestring;

    }
}
