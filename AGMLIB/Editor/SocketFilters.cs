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
using Bundles;

using Shapes;
using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Debug = UnityEngine.Debug;
using UnityEngine.UI;

/*
    <Reference Include="Unity.RenderPipelines.HighDefinition.Runtime">
      <HintPath>libs\Unity.RenderPipelines.HighDefinition.Runtime.dll</HintPath>
    </Reference>
 */

public interface IFilter
{
    public List<string> Whitelist { get;}
    public bool Whitelisteverything { get; }
    public List<string> Blacklist { get;}
    public bool Blacklisteverything { get;}
}
public interface IFilterIndexed : IFilter
{
    public int Index {  get; }
    public bool AllIndexes { get; }

    public bool AllowIllegal {  get; }
    public bool BypassFactionRestrictions {  get; }
}

public enum RenderShape
{
    Default,
    Invisible,
    Sphere,
    Cone,
    Torus
}

public class SocketFilters : MonoBehaviour, IFilter
{
    public string CustomType = "";
    public bool AllowNullComponent = true;
    public bool ShowPalette = true;
    public bool ShowInShipPane = true;
    public bool AllowHighlight = true;
    public bool AllowIllegal = true;
    //public bool EditorOutline = false;
    public RenderShape Shape = RenderShape.Default;
    public float Radius  = 1f;
    public float Thickness = 1f;
    [SerializeField]
    protected List<string> whitelist = new();
    [SerializeField]
    protected bool whitelisteverything = false;
    [SerializeField]
    protected List<string> blacklist = new();
    [SerializeField]
    protected bool blacklisteverything = false;
    public List<string> Whitelist => whitelist;
    public bool Whitelisteverything => whitelisteverything;
    public List<string> Blacklist =>     blacklist;
    public bool Blacklisteverything => blacklisteverything;
     

    // Start is called before the first frame update
    [HideInInspector]
    public Vector3Int Size;
    [HideInInspector]
    public bool Resized = false;

    public static bool CheckLegal(HullSocket socket, HullComponent hullComponent)
    {
        SocketFilters componentFilters = hullComponent.GetComponent<SocketFilters>() ?? new SocketFilters();
        SocketFilters socketFilters = socket.GetComponent<SocketFilters>() ?? new SocketFilters();
        if (!socketFilters.AllowNullComponent && hullComponent == null)
            return false;
        else if ((!socketFilters.AllowIllegal || !componentFilters.AllowIllegal) && !hullComponent.TestSocketFit(socket.Size))
            return false;
        else if (socketFilters.Whitelist.Contains(hullComponent.SaveKey) || componentFilters.Whitelist.Contains(socket.Key))
            return true;
        else if (socketFilters.Blacklist.Contains(hullComponent.SaveKey) || componentFilters.Blacklist.Contains(socket.Key))
            return false;
        else if (socketFilters.Whitelist.Contains(hullComponent.GetType().ToString()))
            return true;
        else if (socketFilters.Blacklist.Contains(hullComponent.GetType().ToString()))  
            return false;
        else if (socketFilters.Whitelist.Contains(hullComponent.Category))
            return true;
        else if (socketFilters.Blacklist.Contains(hullComponent.Category))
            return false;
        else if (socketFilters.Whitelist.Contains(Common.GetVal<string>(hullComponent, "_factionKey") ?? "NOFACTIONKEY")) 
            return true;
        else if (socketFilters.Blacklist.Contains(Common.GetVal<string>(hullComponent, "_factionKey") ?? "NOFACTIONKEY"))
            return false;
        else if (socketFilters.Whitelist.Contains(hullComponent.SourceModId?.ToString() ?? "Vanilla_Neb"))
            return true;
        else if (socketFilters.Blacklist.Contains(hullComponent.SourceModId?.ToString() ?? "Vanilla_Neb"))
            return false;
        else if (socketFilters.Whitelist.Contains(componentFilters.CustomType) || componentFilters.Whitelist.Contains(socketFilters.CustomType))
            return true;
        else if (socketFilters.Blacklist.Contains(componentFilters.CustomType) || componentFilters.Blacklist.Contains(socketFilters.CustomType))
            return false;
        else if (socketFilters.Whitelist.Contains(hullComponent.Type.ToString()) || componentFilters.Whitelist.Contains(socket.Type.ToString()))
            return true;
        else if (socketFilters.Blacklist.Contains(hullComponent.Type.ToString()) || componentFilters.Blacklist.Contains(socket.Type.ToString()))
            return false;
#pragma warning disable IDE0046 // Convert to conditional expression
        else if (socketFilters.Whitelisteverything || componentFilters.Whitelisteverything)
            return true;
        else if (socketFilters.Blacklisteverything || componentFilters.Blacklisteverything)
            return false;
        //Debug.LogError("Check Fit ");
        else return hullComponent.TestSocketFit(socket.Size);
#pragma warning restore IDE0046 // Convert to conditional expression
    }
}

//[HarmonyPatch(typeof(SocketOutlineManager), nameof(SocketOutlineManager.SetSockets))]
class SocketOutlineManagerSetSockets
{
    static void Prefix(SocketOutlineManager __instance, Camera cam)
    {
    }
}

[HarmonyPatch(typeof(SocketOutlineManager), nameof(SocketOutlineManager.DrawShapes))]
class SocketOutlineManagerDrawShapes
{
    static bool Prefix(SocketOutlineManager __instance, Camera cam, 
        Camera ____camera, 
        Color ____emptyColor, Color ____filledColor, Color ____selectedColor, Color ____arcHardLimit, Color ____arcSoftLimit,
        float ____arcThickness, float ____arcRadiusBuffer, float ____arcDashScale, float ____arcDashSpacing, float ____arcLift,
        bool ____visible, HullSocket[] ____sockets, HullSocket ____hoveredSocket, HullSocket ____selectedSocket

    )
    {
         Camera _camera = ____camera;

         Color _emptyColor = ____emptyColor;

         Color _filledColor = ____filledColor;
         Color _selectedColor = ____selectedColor;
         float _arcThickness = ____arcThickness;
         float _arcRadiusBuffer = ____arcRadiusBuffer;

         Color _arcHardLimit = ____arcHardLimit;
         Color _arcSoftLimit = ____arcSoftLimit;

        float _arcDashScale = ____arcDashScale;

         float _arcDashSpacing = ____arcDashSpacing;

         float _arcLift = ____arcLift;

        bool _visible = ____visible;

        HullSocket[] _sockets = ____sockets;

        HullSocket _hoveredSocket = ____hoveredSocket;

        HullSocket _selectedSocket = ____selectedSocket;

        if (!_visible || _sockets == null)
        {
            return false;
        }
        foreach (HullSocket socket in _sockets.OrderByDescending((HullSocket x) => Vector3.Distance(x.transform.position, _camera.transform.position)))
        {
            using (Draw.Command(cam, CustomPassInjectionPoint.AfterPostProcess))
            {

                Draw.SizeSpace = ThicknessSpace.Meters;
                Draw.ThicknessSpace = ThicknessSpace.Meters;
                Draw.RadiusSpace = ThicknessSpace.Meters;
                Draw.ZTest = CompareFunction.Always;
                Draw.Matrix = socket.transform.localToWorldMatrix;

                Draw.Color = socket == _selectedSocket
                    ? _selectedColor
                    : socket == _hoveredSocket ? _selectedColor : (socket.Component != null ? _filledColor : _emptyColor);
                SocketFilters socketFilters = socket.GetComponent<SocketFilters>() ?? new SocketFilters();
                Draw.Radius = socketFilters.Radius;
                switch (socketFilters.Shape)
                {
                    case RenderShape.Default:
                        Draw.Cuboid(Vector3.zero, Quaternion.identity, (Vector3)socket.Size.Dimensions * 0.25f);
                        break;
                    case RenderShape.Sphere:
                        Draw.Sphere(Vector3.zero);
                        break;
                    case RenderShape.Torus:
                        Draw.Torus(Vector3.zero, socketFilters.Radius, socketFilters.Thickness);
                        break;
                    case RenderShape.Cone:
                        Draw.Cone(Vector3.zero, socketFilters.Radius, socketFilters.Thickness);
                        break;
                }
            }
        }
        for (int i = 0; i < _sockets.Length; i++)
        {
            HullSocket socket2 = _sockets[i];
            if (socket2.Type != 0)
            {
                continue;
            }
            using (Draw.Command(cam, CustomPassInjectionPoint.AfterPostProcess))
            {
                Draw.LineGeometry = LineGeometry.Volumetric3D;
                Draw.ThicknessSpace = ThicknessSpace.Noots;
                Draw.Thickness = _arcThickness;
                Draw.UseDashes = true;
                Draw.DashSpace = DashSpace.Meters;
                Draw.DashSize = _arcDashScale;
                Draw.DashSpacing = _arcDashSpacing;
                Draw.LineEndCaps = LineEndCap.Round;
                Draw.Matrix = socket2.transform.localToWorldMatrix;
                Vector3Int socketSize = socket2.Size.Dimensions;
                float arcRadius = (float)Mathf.Max(socketSize.x, socketSize.z) * 0.25f * _arcRadiusBuffer;
                Vector3 attach = socket2.AttachPoint / 10f * _arcLift;
                if (socket2.Component != null && socket2.Component.UseTraversalArcs)
                {
                    if (socket2.TraverseLimits.HasValue)
                    {
                        Draw.Color = socket2.TraverseLimits.Value.LimitFiringOnly ? _arcSoftLimit : _arcHardLimit;
                        Draw.Arc(attach, Quaternion.Euler(0f, -90f, 0f) * Quaternion.LookRotation(Vector3.up), arcRadius, (0f - socket2.TraverseLimits.Value.LeftAngle) * ((float)Math.PI / 180f), socket2.TraverseLimits.Value.RightAngle * ((float)Math.PI / 180f));
                    }
                    else
                    {
                        Draw.Color = _arcSoftLimit;
                        Draw.Ring(attach, Vector3.up, arcRadius, _arcThickness);
                    }
                }
            }
        }
        return false;
    }
}

[HarmonyPatch(typeof(ShipEditorPane), nameof(ShipEditorPane.SetShip))]
class ShipEditorPaneSetShip
{
    static void Postfix(ShipEditorPane __instance, EditorShipController ship, RectTransform ____scrollPaneContent, EditorShipController ____currentShip, GameObject ____socketGroupingPrefab, GameObject ____socketItemPrefab)
    {
        foreach (SocketItem child in ____scrollPaneContent.GetComponentsInChildren<SocketItem>().Where(socketitem => socketitem.Socket != null))
        {

            HullSocket socket = child.GetComponent<SocketItem>()?.Socket;
            SocketFilters filter = socket?.GetComponent<SocketFilters>();
            if (!(filter?.ShowInShipPane ?? true))//|| socket.Type == HullSocketType.Compartment  || socket.name.Contains("Electronics") || socket.name.Contains("Secondary")
                UnityEngine.Object.Destroy(child.gameObject);

        }
        List<String> types = new() {};

        IReadOnlyCollection<HullSocket> allSockets = ____currentShip.Ship.Hull.AllSockets;
        foreach(HullSocket socket in allSockets)
        {
            SocketFilters filter = socket.GetComponent<SocketFilters>() ?? new();
            if(filter.CustomType.Length > 0 && !types.Contains(filter.CustomType))
                types.Add(filter.CustomType);
        }
        foreach (string value in types)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate(____socketGroupingPrefab, ____scrollPaneContent);
            gameObject.transform.SetAsFirstSibling();
            Accordion component = gameObject.GetComponent<Accordion>();
            HeaderItem componentInChildren = gameObject.GetComponentInChildren<HeaderItem>();
            componentInChildren.SetText(value);
            foreach (HullSocket item3 in allSockets)
            {
                SocketFilters filter = item3.GetComponent<SocketFilters>() ?? new();
                if (filter.CustomType == value)
                {
                    GameObject gameObject2 = UnityEngine.Object.Instantiate(____socketItemPrefab, component.Content);
                    SocketItem item = gameObject2.GetComponent<SocketItem>();
                    item.SetSocket(____currentShip, __instance, item3);
                    Button component2 = gameObject2.GetComponent<Button>();
                    component2.onClick.AddListener(delegate
                    {
                        __instance.OpenPalette(item.Socket);
                    });
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
        //Debug.LogError("Highlight Postfix");
        SocketFilters socketFilters = socket?.GetComponent<SocketFilters>() ?? new();
        return socket == null || socketFilters.AllowHighlight;
    }
}

[HarmonyPatch(typeof(ShipEditorPane), nameof(ShipEditorPane.OpenPalette))]
class ShipEditorPanePatch
{
    static bool Prefix(ShipEditorPane __instance, HullSocket socket)
    {
        SimpleDiscount.UISocket = socket;
        SocketFilters socketFilters = socket.GetComponent<SocketFilters>();
        return socketFilters == null || socketFilters.ShowPalette;
    }

    static void Postfix(ShipEditorPane __instance, HullSocket socket)
    {
        SimpleDiscount.UISocket = socket;
        //Debug.LogError("Ship Editor Pane Postfix");
        SocketFilters socketFilters = socket.GetComponent<SocketFilters>() ?? new SocketFilters();
        //if(socketFilters == null)
        //ComponentPalette _palette = (ComponentPalette)GetPrivateField(__instance, "_palette"); ;

        ModalListSelectDetailed _openPalette = Common.GetVal<ModalListSelectDetailed>(__instance, "_openPalette");
        if(_openPalette == null)
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

[HarmonyPatch(typeof(Ship), nameof(Ship.LoadFromSave))]
class LoadPatch
{
    static void Postfix(Ship __instance)
    {
        HullSocket[] sockets = __instance.GetComponentsInChildren<HullSocket>();
        foreach (HullSocket socket in sockets)
        {
            SocketFilters socketFilters = socket.GetComponent<SocketFilters>();
            if (socketFilters!= null && socket.Component == null && !socketFilters.AllowNullComponent)
            {
                socket.SetComponent(null);
            }
        }
    }
}

[HarmonyPatch(typeof(HullSocket), nameof(HullSocket.SetComponent))]
class HullSocketPatch 
{
    static bool Prefix(HullSocket __instance, ref HullComponent componentPrefab)
    {
        HullSocket socket = __instance;
        //Debug.LogError(componentPrefab.Category);
        
        //Debug.LogError("Set Component Prefix");
        SocketFilters socketFilters = socket.GetComponent<SocketFilters>() ?? new();
        //if(componentPrefab != null)
        //    Debug.LogError("General Install " + componentPrefab.SaveKey);

        if (componentPrefab == null)
        {
            if(socketFilters.AllowNullComponent)
                return true;
            componentPrefab = BundleManager.Instance.GetHullComponent(socket.DefaultComponent);
        }

        if (socket.GetComponent<SocketFilters>() == null && componentPrefab.GetComponent<SocketFilters>() == null)
            return true;

        HullComponent _component = socket.Component;
        if (_component != null)
        {
            _component.SetSocket(null);
            CheckDepedends(socket, true, true);
            UnityEngine.Object.Destroy(_component.gameObject);
            //SetPrivateField(_component, "_size", _component);
            socket.UpdateColliderActive();
        }
        /*
        if (!CheckDepedends(socket))
        {
            //Debug.LogError("Missing Component");
            //return false;
        }
        */

        if(!SocketFilters.CheckLegal(socket, componentPrefab))
            componentPrefab = BundleManager.Instance.GetHullComponent(socket.DefaultComponent);
        else if(!componentPrefab.TestSocketFit(socket.Size))
        {
            //Debug.LogError("Resizing Socket");
            socketFilters.Size = socket.Size.Dimensions;
            socketFilters.Resized = true;
            Common.SetVal(socket, "_size", componentPrefab.Size);
            return true;
        }

        return true;
    }

    static void Postfix(HullSocket __instance)
    {
        HullSocket socket = __instance;
        //Debug.LogError("Set Component Postfix");

        CheckDepedends(socket, true);
        SocketFilters socketFilters = socket.GetComponent<SocketFilters>() ?? new SocketFilters();
        if (socketFilters == null)
            return;
        if (socketFilters.Resized && socketFilters.Size != null)
        {
            Common.SetVal(__instance, "_size", socketFilters.Size);
            socketFilters.Resized = false;
        }
        /*if(socket.Component == null)
            Debug.LogError("Component Null");
        else
            Debug.LogError(socket.Component.ComponentName);*/
        if (socket.Component == null && !socketFilters.AllowNullComponent)
        {
            //Debug.LogError("Comp Removed");
            HullComponent componentPrefab = BundleManager.Instance.GetHullComponent(socket.DefaultComponent);
            socket.SetComponent(componentPrefab);
        }
    }
    
    static bool CheckDepedends(HullSocket socket, bool write = false, bool remove = false)
    {
        ComponentDependencies componentDependencies = socket?.Component?.GetComponent<ComponentDependencies>();
        if (componentDependencies == null) return true;

        bool returnval = true;
        HullSocket[] sockets = socket.gameObject.transform.parent.GetComponentsInChildren<HullSocket>();
        foreach (KeyValuePair<string, string> componentDependency in componentDependencies.Dependendents)
        {
            HullSocket testSocket = sockets.Where(testSocket => componentDependency.Key == testSocket.Key).First();
            //Debug.LogError("D " + componentDependency.Key + " " + componentDependency.Value);
            if (remove)
                testSocket.SetComponent(null);
            else if( write && (componentDependencies.HardInstallDepedenents || (componentDependency.Value != testSocket?.Component?.SaveKey && componentDependencies.InstallDepedenents)))
            {
                testSocket.SetComponent(BundleManager.Instance.GetHullComponent(componentDependency.Value));
            }
        }

        

        foreach (KeyValuePair<string, string> componentDependency in componentDependencies.Requirements)
        {
            HullSocket testSocket = sockets.Where(testSocket => componentDependency.Key == testSocket.Key).First();
            //Debug.LogError("R " + componentDependency.Key + " " + componentDependency.Value);
            if (remove)
                testSocket.SetComponent(null);
            else if (componentDependency.Value != testSocket?.Component?.SaveKey || componentDependencies.HardInstallDepedenents)
            {
                if (componentDependencies.InstallRequirements && write)
                {
                    //Debug.LogError("Add");
                    testSocket.SetComponent(BundleManager.Instance.GetHullComponent(componentDependency.Value));

                }
                else
                    returnval = false;
            }
        }

        //return true;
        return componentDependencies.InstallRequirements || returnval;

    }
}

