using QFSW.QC.Demo;
using System.Drawing;
using UnityEngine;
using Image = UnityEngine.UI.Image;

    public class Lore : MonoBehaviour
    {
        [SerializeField] protected Sprite? _loreicon;
        
        [SerializeField] protected StringFormatter _prefixstring = new();
        [SerializeField] protected StringFormatter _postdescriptionstring = new();
        [SerializeField] protected StringFormatter _poststatsstring = new();
        [SerializeField] protected StringFormatter _postresourcesstring = new();
        [SerializeField] protected StringFormatter _postbuffstring = new();
        [SerializeField] protected StringFormatter _postlorestring = new();
        [SerializeField] protected StringFormatter _postdetailtext = new();

        public string Prefixstring => _prefixstring?.ToString() ?? "";
        public string Postdescriptionstring => _postdescriptionstring?.ToString() ?? "";
        public string Poststatsstring => _poststatsstring?.ToString() ?? "";
        public string Postresourcesstring => _postresourcesstring?.ToString() ?? "";
        public string Postbuffstring => _postbuffstring?.ToString() ?? "";
        public string Postlorestring => _postlorestring?.ToString() ?? "";

        public string Postdetailtext => _postdetailtext?.ToString() ?? "";
        public Sprite? LoreIcon => _loreicon;
        public TMP_FontAsset? LoreFont => _postdetailtext?.Font;
        public TextMeshProUGUI? _detailTitle;

        public static TextMeshProUGUI GetExtraLoreTMP(TextMeshProUGUI detailText)
        {
            GameObject returnval = detailText.transform.parent.transform.Find("ExtraLore")?.gameObject ?? Instantiate(detailText.gameObject, detailText.transform.parent.transform);
            returnval.name = "ExtraLore";



            return returnval.GetComponent<TextMeshProUGUI>(); 
        }
    }

    [HarmonyPatch(typeof(PaletteItem), nameof(PaletteItem.SetComponent))]
    static class ComponentPaletteCreateItemPatch
    {
        static void Postfix(PaletteItem __instance, HullComponent component, Image ____modBadge)
        {
        //foreach (HullComponent pcomp in BundleManager.Instance.AllComponents)
        //    Debug.Log(pcomp.ComponentName + " bndl2 " + pcomp.SaveKey);

            if (component == null)
                return;
            //Debug.Log("pltset " + component.name + "  " + component.SaveKey + " " + component.Type + " " + component.Category + " " + component);

            if (!component.SourceModId.HasValue)
                    return;


            component.gameObject.GetComponentInChildren<Lore>(true);
            component.GetComponentInChildren<Lore>(true);
            Sprite? _loreicon = component.GetComponentInChildren<Lore>(true)?.LoreIcon ?? null;
            if (_loreicon != null)
                ____modBadge.sprite = _loreicon;
            //____modBadge.sprite = BundleManager.Instance.AllFactions.ToList()[2].SmallLogo;
        }
    }

    [HarmonyPatch(typeof(ModalListSelectDetailed), "SelectItemInternal")]
    static class ModalListSelectDetailedSelectItemInternal
    {
        static void Postfix(ModalListSelectDetailed __instance, SelectableListItem selected, TextMeshProUGUI ____detailText)
        {
            MonoBehaviour loreobject = null;
            if (selected is PaletteItem listItem)
                loreobject = listItem.Component;
            else if (selected is HullListItem hullitem)
                loreobject = hullitem.Hull;
            else
                return;
            //Transform root = ____detailText.transform.parent.transform;
            TextMeshProUGUI extralore = Lore.GetExtraLoreTMP(____detailText);
            extralore.text = Common.Cat;


            if (loreobject?.GetComponentInChildren<Lore>() is not Lore lore)
                    return;
            extralore.text = lore.Postdetailtext;
            //Debug.LogError("lor " + lore.Postdetailtext);


            if (lore.LoreFont is TMP_FontAsset font)
                extralore.font = font;
            else
                extralore.font = ____detailText.font;


            //____detailText.font = font;
        }
    }

    [HarmonyPatch(typeof(PaletteItem), "GetDetailText")]
    static class ComponentPaletteGetDetailTextPatch
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
            List<(string, string)> rows = [];
            _component.GetFormattedStats(rows, full: true);
            string text2 = rows.StatRowsToTextBlock();
            string formattedResources = _component.GetFormattedResources();
            string formattedBuffs = _component.GetFormattedBuffs();
            if (!string.IsNullOrEmpty(text2))
                text = text + "\n<b>Stats:</b>\n" + text2;
            text += lore.Poststatsstring;

            if (!string.IsNullOrEmpty(formattedResources))
                text = text + "\n<b>Resources:</b>\n" + formattedResources;
            text += lore.Postresourcesstring;

            if (!string.IsNullOrEmpty(formattedBuffs))
                text = text + "\n<b>Modifiers:</b>\n" + formattedBuffs;
            text += lore.Postbuffstring;

            if (!string.IsNullOrEmpty(_component.FlavorText))
                text = text + "\n<i><color=" + GameColors.FlavorTextColor + ">" + _component.FlavorText + "</color></i>";
            text += lore.Postlorestring;
            //text = text + "\n\n\n\n" + "<rotate=\"45\"> " + test + " </rotate>";
            __result = text;
        }
    }

    [HarmonyPatch(typeof(HullListItem), nameof(HullListItem.GetDetails))]
    static class HullListItemGetDetails
    {
        static void Postfix(PaletteItem __instance, BaseHull ____hull, out string title, out string subtitle, out Sprite image, out string details)
        {
            BaseHull _hull = ____hull;
            Lore lore = _hull.GetComponentInChildren<Lore>() ?? new();
            title = _hull.ClassName;
            subtitle = "(" + _hull.HullClassification + " Class)";
            image = _hull.HullScreenshot;
            _hull.EditorFormatHullStats(out List<(string, string)>? hull, out List<(string, string)>? sigs, showBreakdown: false);
            details = lore.Prefixstring;
            details += _hull.LongDescription + "\n\n" + hull.StatRowsToTextBlock() + "\n\n" + sigs.StatRowsToTextBlock();
            details += lore.Postdescriptionstring;
            string text = _hull.EditorFormatHullBuffs();
            if (text != null)
                details = details + "\n\n<b>Modifiers:</b>\n" + text;
            details += lore.Postbuffstring;
            if (!string.IsNullOrEmpty(_hull.FlavorText))
                details = details + "\n<i><color=#FFEF9E>" + _hull.FlavorText + "</color></i>";
            details += lore.Postlorestring;

        }
    }