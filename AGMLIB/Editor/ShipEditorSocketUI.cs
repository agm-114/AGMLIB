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
[HarmonyPatch(typeof(ShipEditorPane), nameof(ShipEditorPane.SetShip))]
class ShipEditorPaneSetShip
{
    static void Prefix(ShipEditorPane __instance, EditorShipController ship, RectTransform ____scrollPaneContent, EditorShipController ____currentShip, GameObject ____socketGroupingPrefab, GameObject ____socketItemPrefab)
    {
        Common.LogPatch();
        foreach (Accordion child2 in ____scrollPaneContent.GetComponentsInChildren<Accordion>())
        {
            UnityEngine.Object.Destroy(child2?.gameObject);
        }
    }

    static void Postfix(ShipEditorPane __instance, EditorShipController ship, RectTransform ____scrollPaneContent, EditorShipController ____currentShip, GameObject ____socketGroupingPrefab, GameObject ____socketItemPrefab, SocketOutlineManager ____socketOutliner)
    {
        Common.LogPatch();

        

        IReadOnlyCollection<HullSocket> allSockets = ____currentShip.Ship.Hull.AllSockets;
        //Debug.LogError("Setting Ship");
        foreach (SocketItem socketitem in ____scrollPaneContent.GetComponentsInChildren<SocketItem>().Where(socketitem => socketitem.Socket != null))
        {
            HullSocket socket = socketitem.Socket;


            SocketEditorUISettings? socketEditorUISettings = socket.GetComponent<SocketEditorUISettings>();

            SocketEditorChildSettings childsettings = socket.GetComponent<SocketEditorChildSettings>();



            if (childsettings != null && childsettings.VisualChildren.Count > 0)
            {

                GameObject socketgroupgo2 = UnityEngine.Object.Instantiate(____socketGroupingPrefab, socketitem.gameObject.transform);
                Accordion accordion2 = socketgroupgo2.GetComponent<Accordion>();
                HeaderItem accordionName = socketgroupgo2.GetComponentInChildren<HeaderItem>();
                SocketGroupDropdown dropdown = accordionName.gameObject.AddComponent<SocketGroupDropdown>();
                dropdown.OutlineManager = ____socketOutliner;
                dropdown.Sockets = childsettings.VisualChildren;
                dropdown.Editor = __instance;
                dropdown.ParentSocket = socket;

                Image image = socketgroupgo2.GetComponentInChildren<Image>();
                image.color = childsettings.BackgroundColor;
                //socketgroupgo2.active = socket.Component != null;
                TextMeshProUGUI accordianText = Common.GetVal<TextMeshProUGUI>(accordionName, "_text");
                accordianText.color = childsettings.TextColor;

                void HandleComponentChange(HullComponent component)
                {
                    if (socketgroupgo2 == null)
                    {
                        return;
                    }

                    socketgroupgo2.SetActive(component != null || !childsettings.ComponentBasedChildVisiblity);
                    string Name = childsettings.FilterLookup?.GetGropupName(component?.SaveKey ?? "") ?? component?.name ?? "Error";
                    accordionName.SetText(Name);
                    image.color = childsettings.FilterLookup?.GetGroupBackgroundColor (component?.SaveKey ?? "") ?? childsettings.BackgroundColor;
                    accordianText.color = childsettings.FilterLookup?.GetGroupTextColor(component?.SaveKey ?? "") ?? childsettings.TextColor;
                    foreach (HullSocket visualChild in childsettings.VisualChildren)
                    {

                        SocketFilters.EnsureChildSetup(visualChild, component);
                        if (component != null && !SocketFilters.CheckLegal(visualChild, visualChild.Component))
                        {
                            //Debug.LogError("Childfilter: Illegal component installed on socket: " + visualChild.name);
                            visualChild.SetComponent(null);
                        }
                        else
                        {
                            //Debug.LogError("Childfilter: legal component installed on socket: " + visualChild.name);

                        }

                        
                    }
                }

                HullSocket.InstalledComponentChanged componentChangeHandler = HandleComponentChange;
                SocketGroupComponentChangeBinding binding = socketgroupgo2.AddComponent<SocketGroupComponentChangeBinding>();
                binding.Socket = socket;
                binding.Handler = componentChangeHandler;
                socket.OnInstalledComponentChanged += componentChangeHandler;
                HandleComponentChange(socket.Component);

                foreach (HullSocket visualChild in childsettings.VisualChildren)
                {
                    GameObject socketitemgo2 = UnityEngine.Object.Instantiate(____socketItemPrefab, accordion2.Content);
                    SocketItem item2 = socketitemgo2.GetComponent<SocketItem>();
                    item2.SetSocket(____currentShip, __instance, visualChild);
                }
            }


            //Debug.LogError($"checking socket {socket?.ShortName} setts: {basesettings != null} type: {basesettings?.CustomType ?? "None"}  show: {basesettings?.ShowInShipPane ?? true}");



            //we should only be destroying the default socket ui elements for custom types if they indicate they should be hidden
            if (string.IsNullOrEmpty(socket.DisplayType()) && (socketEditorUISettings?.ShowInShipPane ?? true) )
            {
                continue;

            }
            //Debug.LogError("removing socket " + socket?.ShortName);
            List<SocketItem> _sockets = Common.GetVal<List<SocketItem>>(__instance, "_sockets");
            _sockets.Remove(socketitem);
            UnityEngine.Object.Destroy(socketitem?.gameObject);

        }
        HashSet<string> types = [];

        foreach (HullSocket socket in allSockets)
        {
            string DisplayType = socket.DisplayType();
            if (DisplayType.Length > 0)
                types.Add(DisplayType);
        }
        foreach (string type in types)
        {
            GameObject socketgroupgo = UnityEngine.Object.Instantiate(____socketGroupingPrefab, ____scrollPaneContent);
            socketgroupgo.transform.SetAsFirstSibling();
            Accordion accordion = socketgroupgo.GetComponent<Accordion>();
            HeaderItem componentInChildren = socketgroupgo.GetComponentInChildren<HeaderItem>();
            componentInChildren.SetText(type);
            foreach (HullSocket socket in allSockets)
            {
                if (socket.DisplayType() == type && (socket.GetComponent<SocketEditorUISettings>()?.ShowInShipPane ?? true))
                {
                    GameObject socketitemgo = UnityEngine.Object.Instantiate(____socketItemPrefab, accordion.Content);


                    SocketItem item = socketitemgo.GetComponent<SocketItem>();
                    item.SetSocket(____currentShip, __instance, socket);
                    Button socketbutton = socketitemgo.GetComponent<Button>();
                    socketbutton?.onClick?.AddListener(delegate
                    {
                        __instance.OpenPalette(item.Socket);
                    });
                    //break;
                }
            }
        }
    }
}

[HarmonyPatch(typeof(FleetCompositionSubmodeController), nameof(FleetCompositionSubmodeController.SetHighlightedSocket))]
class FleetCompositionSubmodeControllerSetHighlightedSocketPatch
{
    static bool Prefix(FleetCompositionSubmodeController __instance, HullSocket socket, bool fromUI)
    {
        //Common.LogPatch();
        //Debug.LogError("Highlight Postfix");
        SocketEditorUISettings socketFilters = socket?.GetComponent<SocketEditorUISettings>() ?? new();
        return socket == null || socketFilters.AllowHighlight;
    }
}

[HarmonyPatch(typeof(ShipEditorPane), nameof(ShipEditorPane.OpenPalette))]
class ShipEditorPanePatch
{
    static bool Prefix(ShipEditorPane __instance, HullSocket socket, ComponentPalette ____palette)
    {
        //
        Common.LogPatch();

        List<SelectableListItem> validComponents = ____palette.GetItemsForSocket(socket);
        foreach (SelectableListItem item in validComponents)
        {
            //if (item.Data is not HullComponent component)
            //    continue;
            //Debug.Log("plt " +  component.name + "  " + component.SaveKey + " " + component.Type + " " + component.Category);

        }

        //
        SimpleDiscount.UISocket = socket;
        SocketEditorUISettings editorsettings = socket.GetComponent<SocketEditorUISettings>();
        return editorsettings == null || editorsettings.ShowPalette;
    }

    static void Postfix(ShipEditorPane __instance, HullSocket socket)
    {
        SimpleDiscount.UISocket = socket;
        //Debug.Log("Ship Editor Pane Postfix: "  + socket.Key);
        SocketFilters socketFilters = socket.GetComponent<SocketFilters>() ?? new SocketFilters();
        //if(socketFilters == null)
        //ComponentPalette _palette = (ComponentPalette)GetPrivateField(__instance, "_palette"); ;

        ModalListSelectDetailed _openPalette = Common.GetVal<ModalListSelectDetailed>(__instance, "_openPalette");
        if (_openPalette == null)
            return;

        _openPalette.RemoveFilters();
        _openPalette.AddFilter("Show Only Filtered", delegate (object data)
        {
            //Debug.LogError("Running Fillter Delegate");
            return data == null
                ? socketFilters.AllowNullComponent
                : data is HullComponent hullComponent && SocketFilters.CheckLegal(socket, hullComponent);
        }, initialState: true);
        //Debug.LogError("Finished Delegates");
    }
}


