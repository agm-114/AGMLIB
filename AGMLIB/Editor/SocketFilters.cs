using FleetEditor;
using Lib;
using Lib.Dynamic_Systems.Area;
using Munitions.ModularMissiles;
using Shapes;
using SmallCraft;
using Steamworks.Ugc;
using System.ComponentModel;
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
public interface ICoreFilter
{
    public IList<string> Whitelist { get; }

    public IList<string> Blacklist { get; }
}

public interface ISimpleFilter
{
    public bool CheckIFF(IPlayer parent, IPlayer remote);
    public bool CheckShip(ShipController parent, ShipController remote);
    public bool CheckMissile(ShipController parent, ModularMissile remote);
    public bool CheckComponent(HullPartResourceConnected hullComponent);
    public bool IsAmmoCompatible(IMunition ammo, bool debugmode = false);
}

public abstract class BaseFilter : MonoBehaviour, ISimpleFilter
{
    virtual public bool CheckComponent(HullPartResourceConnected hullComponent) => throw new NotImplementedException();
    virtual public bool CheckIFF(IPlayer parent, IPlayer remote) => throw new NotImplementedException();
    virtual public bool CheckMissile(ShipController parent, ModularMissile remote) => throw new NotImplementedException();
    virtual public bool CheckShip(ShipController parent, ShipController remote) => throw new NotImplementedException();
    virtual public bool IsAmmoCompatible(IMunition ammo, bool debugmode = false) => throw new NotImplementedException();
}


public class AmmoTagFilter : BaseFilter
{
    public List<MunitionTags>  WhiteListTags= new();

    public override bool IsAmmoCompatible(IMunition ammo, bool debugmode = false)
    {
        for (int i = 0; i < WhiteListTags.Count; i++)
        {
            //Common.Hint(WhiteListTags[i].Class + " " + WhiteListTags[i].Subclass);
        }

        //Common.Hint(ammo?.Tags.Class + " " + ammo?.Tags.Subclass);
        return WhiteListTags.Any(x => x.Equals(ammo.Tags));
    }
}

public interface IFilter : ICoreFilter
{
    public bool Whitelisteverything { get; }

    public bool Blacklisteverything { get; }
}
public interface IFilterIndexed : IFilter
{
    public int Index { get; }
    public bool AllIndexes { get; }

    public bool AllowIllegal { get; }
    public bool BypassFactionRestrictions { get; }
}

public enum RenderShape
{
    Default,
    Invisible,
    Sphere,
    Cone,
    Torus
}

public class SimpleAndFilter : BaseFilter, ISimpleFilter
{
    [SerializeField] protected List<BaseFilter> _filters = [];

    public bool CheckComponent(HullComponent hullComponent)
    {
        foreach (ISimpleFilter filter in _filters)
        {
            if (!filter.CheckComponent(hullComponent))
                return false;
        }
        return true;
    }


}

public class SimpleOwnerFilter : BaseFilter, ISimpleFilter
{
    [SerializeField] protected bool Self = false;
    [SerializeField] protected bool Mine = true;
    [SerializeField] protected bool Friendly = false;
    [SerializeField] protected bool Enemy = true;
    [SerializeField] protected bool None = true;
        

    override public bool CheckMissile(ShipController parent, ModularMissile target) => ValidIFFTarget(parent.OwnedBy, target?.OwnedBy);
    public bool ValidIFFTarget(IPlayer owner, IPlayer toPlayer)
    {

        Game.IFF IFFstatus = IFFExtensions.GetIFF(owner, toPlayer);
        if (!Friendly && IFFstatus != Game.IFF.Enemy)
            return false;
        else if (!Enemy && IFFstatus == Game.IFF.Enemy)
            return false;
        return true;
    }
    override public bool CheckShip(ShipController parent, ShipController remote)
    {
        if (parent == remote)
            return Self;
        return ValidIFFTarget(parent.OwnedBy, remote?.OwnedBy);
    }

}



public class SimpleFilter : BaseFilter, ISimpleFilter
{
    [SerializeField] protected List<string> _whitelist = [];
    [SerializeField] protected List<string> _blacklist = [];
    [SerializeField] protected bool _defaultvalue = false;
    public IList<string> Whitelist => _whitelist;
    public IList<string> Blacklist => _blacklist;
    public ISimpleFilter CopyFilter(ICoreFilter filter, bool newdefault = true)
    {
        _whitelist = filter.Whitelist.ToList();
        _blacklist = filter.Blacklist.ToList();
        _defaultvalue = newdefault;

        return this;
    }

    override public bool CheckComponent(HullPartResourceConnected hullComponent)
    {
        bool valid = Filters.CheckComponent(hullComponent, _whitelist, _blacklist, _defaultvalue);
        foreach (string whitelist in Whitelist)
        {
            //Common.Trace("w" + whitelist);
        }
        //Common.Trace("checking weapon" + hullComponent.SaveKey + hullComponent.Category + valid);

        return valid;
    }

    override public bool IsAmmoCompatible(IMunition ammo, bool debugmode = false)
    {

        if (ammo == null)
        {
            return true;
        }
        if (debugmode && false)
        {
            //Debug.LogError(String.Join(", ", Whitelist));
            //Debug.LogError(ammo.Tags.Class);
            //Debug.LogError(ammo.Tags.Subclass);
        }

        //Debug.LogError(ammo.MunitionName);
        //Debug.LogError("Parse int: " + ammo?.Tags.Subclass.Substring(1));
        //Debug.LogError("Value: " + int.Parse(ammo?.Tags.Subclass.Substring(1)));

        if (Whitelist.Contains(ammo?.MunitionName))
            return true;
        else if (Blacklist.Contains(ammo?.MunitionName))
            return false;
        else if (Whitelist.Contains(ammo?.SaveKey))
            return true;
        else if (Blacklist.Contains(ammo?.SaveKey))
            return false;
        else if (Whitelist.Contains(ammo?.Type.ToString()))
            return true;
        else if (Blacklist.Contains(ammo?.Type.ToString()))
            return false;
        else if (Whitelist.Contains(ammo?.SimMethod.ToString()))
            return true;
        else if (Blacklist.Contains(ammo?.SimMethod.ToString()))
            return false;
        else if (Whitelist.Contains(ammo?.Role.ToString()))
            return true;
        else if (Blacklist.Contains(ammo?.Role.ToString()))
            return false;
        else if (Whitelist.Contains(ammo?.UtilityRole.ToString()))
            return true;
        else if (Blacklist.Contains(ammo?.UtilityRole.ToString()))
            return false;
        else if (Whitelist.Contains(ammo?.FactionKey))
            return true;
        else if (Blacklist.Contains(ammo?.FactionKey))
            return false;
        else if (Whitelist.Contains(ammo?.Tags.Subclass))
            return true;
        else if (Blacklist.Contains(ammo?.Tags.Subclass))
            return false;
        else if (Whitelist.Contains(ammo?.Tags.Class))
            return true;
        else if (Blacklist.Contains(ammo?.Tags.Class))
            return false;
        else
            return _defaultvalue;
    }
}

public static class SocketHelpers
{
    public static string DisplayType(this HullSocket socket)
    {
        string displayName = socket.GetComponent<SocketEditorUISettings>()?.CustomType ?? "";
        if (!string.IsNullOrEmpty(displayName))
            return displayName;
        displayName = socket.GetComponent<BasicSocketEditorUISettings>()?.CustomType ?? "";
        if (!string.IsNullOrEmpty(displayName))
            return displayName;

        return "";
    }
}

public class BasicSocketEditorUISettings : MonoBehaviour
{
    public string CustomType = "";
    //THIS needs to go into SocketEditorUISettings

}


public class SocketEditorUISettings : BasicSocketEditorUISettings
{
    public bool ShowInShipPane = true;//THIS needs to go into SocketEditorUISettings
    public bool ShowPalette = true;
    public bool AllowHighlight = true;
    public ColorName InstalledColor = ColorName.Green;
    public Color DrawColor = Color.clear;
    public RenderShape Shape= RenderShape.Default;
    public float Radius = 1f;
    public float Thickness = 1f;
}



public abstract class BaseSocketLookup : MonoBehaviour
{
    public abstract SocketFilters GetFilter(string key);
    public abstract string GetGropupName(string key);
    public abstract Color GetGroupBackgroundColor(string key);
    public abstract Color GetGroupTextColor(string key);
}

public class KeyBasedFilterLookup : BaseSocketLookup
{
    public SocketFilters DefaultChildFilter = null;
    public string DefaultName = "Default";
    public Color DefaultBackgroundColor = new Color(0.9f, 0.9f, 0.9f, 1f);
    public Color DefaultTextColor = Color.white;
    public List<string> Keys = [];
    public List<SocketFilters> Filters = [];
    public List<string> Names = [];
    public List<Color> BackgroundColors = [];
    public List<Color> TextColors = [];
    public Dictionary<string, SocketFilters> FilterLookup => Keys.Zip(Filters, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
    public Dictionary<string, Color> BackgroundColorLookup => Keys.Zip(BackgroundColors, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
    public Dictionary<string, Color> TextColorLookup => Keys.Zip(TextColors, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);

    public override SocketFilters GetFilter(string key)
    {
        if (FilterLookup.ContainsKey(key)) return FilterLookup[key];
        return DefaultChildFilter;
    }


    public override string GetGropupName(string key)
    {
        if (Keys.Contains(key))
        {
            int index = Keys.IndexOf(key);
            if (index >= 0 && index >= Names.Count)
                return Filters[index].gameObject.name;
            return Names[index];
        }
        return DefaultName;
    }

    public override Color GetGroupBackgroundColor(string key)
    {
        if (Keys.Contains(key)) 
        {
            int index = Keys.IndexOf(key);
            if (index >= 0 && index >= BackgroundColors.Count)
                return DefaultBackgroundColor;
            return BackgroundColors[index];
        }
        return DefaultBackgroundColor;
    }
    public override Color GetGroupTextColor(string key)
    {
        if (Keys.Contains(key))
        {
            int index = Keys.IndexOf(key);
            if (index >= 0 && index >= TextColors.Count)
                return DefaultTextColor;
            return TextColors[index];
        }
        return DefaultTextColor;
    }
}

public class SocketEditorParentSettings : MonoBehaviour
{
    public SocketEditorChildSettings Parent;
}

public class SocketEditorChildSettings : MonoBehaviour
{
    public List<HullSocket> VisualChildren = new();
    public bool ComponentBasedChildVisiblity = false;
    public bool ComponentBasedChildStrip = false;
    public Color BackgroundColor = new Color(0.9f, 0.9f, 0.9f, 1f);
    public Color TextColor = Color.white;
    public BaseSocketLookup FilterLookup;

    public void Setup()
    {
        foreach(HullSocket visualChild in VisualChildren)
        {
            //Debug.LogError("Adding parent basesettings to child: " + visualChild.name);
            visualChild.gameObject.AddComponent<SocketEditorParentSettings>().Parent = this;
        }
    }

}

public class SocketGroupComponentChangeBinding : MonoBehaviour
{
    public HullSocket Socket;
    public HullSocket.InstalledComponentChanged Handler;

    void OnDestroy()
    {
        if (Socket != null && Handler != null)
        {
            Socket.OnInstalledComponentChanged -= Handler;
        }
    }
}

public class SocketGroupDropdown : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SocketOutlineManager OutlineManager;
    public IReadOnlyCollection<HullSocket> Sockets;

    public void OnPointerEnter(PointerEventData eventData)
    {
        SocketOutlineManagerDrawShapes.SetHoveredGroup(this, OutlineManager, Sockets);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SocketOutlineManagerDrawShapes.ClearHoveredGroup(this);
    }

    void OnDestroy()
    {
        SocketOutlineManagerDrawShapes.ClearHoveredGroup(this);
    }
}

#pragma warning disable CA1708 // Identifiers should differ by more than case
public class SocketFilters : BasicSocketEditorUISettings, IFilter
#pragma warning restore CA1708 // Identifiers should differ by more than case
{
    public bool AllowNullComponent = true;
    public bool AllowIllegalSize = false;
    public void Copy(SocketFilters filter)
    {
        //Debug.LogError(filter.whitelist.Count);
        whitelist = filter.whitelist;
        whitelisteverything = filter.whitelisteverything;
        blacklist = filter.blacklist;
        blacklisteverything = filter .blacklisteverything;
    }

    [SerializeField]
    protected List<string> whitelist = [];
    [SerializeField]
    protected bool whitelisteverything = false;
    [SerializeField]
    protected List<string> blacklist = [];
    [SerializeField]
    protected bool blacklisteverything = false;
    public IList<string> Whitelist => whitelist;
    public bool Whitelisteverything => whitelisteverything;
    public IList<string> Blacklist => blacklist;
    public bool Blacklisteverything => blacklisteverything;
    // Start is called before the first frame update
    [HideInInspector]
    public Vector3Int Size;
    [HideInInspector]
    public bool Resized = false;

    
    public static void EnsureChildSetup(HullSocket socket, HullComponent hullComponent)
    {
        SocketEditorChildSettings? childsettings = socket?.GetComponent<SocketEditorChildSettings>();
        SocketEditorParentSettings? parentsettings = socket?.GetComponent<SocketEditorParentSettings>();
        

        if (childsettings != null && childsettings.VisualChildren.Count > 0)
        {

            string componentkey = hullComponent?.SaveKey ?? "";
            SocketFilters? filter = childsettings.FilterLookup?.GetFilter(componentkey) ?? null;
            foreach (HullSocket visualChild in childsettings.VisualChildren)
            {

                if (filter != null)
                {
                    SocketFilters childfilter = visualChild.gameObject.GetOrAddComponent<SocketFilters>();
                    childfilter.Copy(filter);
                }


            }
        }
        if (parentsettings != null)
        {
            HullSocket parentSocket = parentsettings.Parent.gameObject.GetComponent<HullSocket>();
            SocketFilters? filter = parentsettings.Parent.FilterLookup?.GetFilter(parentSocket.Component?.SaveKey ?? "") ?? null;
            SocketFilters socketFilters2 = socket.gameObject.GetOrAddComponent<SocketFilters>();
            socketFilters2.Copy(filter);
        }

    }

    public static bool CheckLegal(HullSocket socket, HullComponent hullComponent)
    {
        //if (hullComponent?.name != null)
        //    Debug.LogError("Socketfilter Checking legality of component " + hullComponent?.name + " on socket " + socket?.name);
  




        SocketFilters socketFilters = socket?.GetComponent<SocketFilters>() ?? new SocketFilters();

        if (hullComponent == null)
            return socketFilters.AllowNullComponent;
        if (hullComponent.SaveKey == socket.DefaultComponent)
            return true;
        SocketFilters componentFilters = hullComponent.GetComponent<SocketFilters>() ?? new SocketFilters();
        //Debug.Log("Filtering: " +  hullComponent.SaveKey);
        //bool legal = hullComponent.TestSocketFit(socket.Size) && socket.MyHull.CanMountEquipment(hullComponent);
        //Debug.Log("Legal: " + legal + "Socket Allow Illegal: " + socketFilters.AllowIllegalSize + "Component Allow Illegal: " + componentFilters.AllowIllegalSize);
        if (!socketFilters.AllowNullComponent && hullComponent == null)
            return false;
        //else if (!socketFilters.AllowIllegalSize && !componentFilters.AllowIllegalSize && !(hullComponent.TestSocketFit(socket.Size) || socket.MyHull.CanMountEquipment(hullComponent)))
        else if (!hullComponent.TestSocketFit(socket.Size) && (!socketFilters.AllowIllegalSize || !componentFilters.AllowIllegalSize) )
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
        else if (socketFilters.Whitelisteverything || componentFilters.Whitelisteverything)
            return true;
        else if (socketFilters.Blacklisteverything || componentFilters.Blacklisteverything)
            return false;
        //Debug.LogError("Check Fit ");
        else return hullComponent.TestSocketFit(socket.Size);
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
    static Random random = new Random();
    static SocketGroupDropdown hoveredGroupOwner;
    static SocketOutlineManager hoveredGroupManager;
    static IReadOnlyCollection<HullSocket> hoveredGroupSockets;

    public static void SetHoveredGroup(SocketGroupDropdown owner, SocketOutlineManager manager, IReadOnlyCollection<HullSocket> sockets)
    {
        hoveredGroupOwner = owner;
        hoveredGroupManager = manager;
        hoveredGroupSockets = sockets;
    }

    public static void ClearHoveredGroup(SocketGroupDropdown owner)
    {
        if (!ReferenceEquals(hoveredGroupOwner, owner))
        {
            return;
        }

        hoveredGroupOwner = null;
        hoveredGroupManager = null;
        hoveredGroupSockets = null;
    }

    static bool Prefix(SocketOutlineManager __instance, Camera cam,
        Camera ____camera,
        Color ____emptyColor, Color ____filledColor, Color ____selectedColor, Color ____arcHardLimit, Color ____arcSoftLimit,
        float ____arcThickness, float ____arcRadiusBuffer, float ____arcDashScale, float ____arcDashSpacing, float ____arcLift,
        bool ____visible, HullSocket[] ____sockets, HullSocket ____hoveredSocket, HullSocket ____selectedSocket

    )
    {
        //Common.LogPatch();

        //Debug.LogError("Help me I am trapped in a scripting factory ");
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
            return false;
        foreach (HullSocket socket in _sockets.OrderByDescending((x) => Vector3.Distance(x.transform.position, _camera.transform.position)))
        {
            using (Draw.Command(cam, CustomPassInjectionPoint.AfterPostProcess))
            {

                Draw.SizeSpace = ThicknessSpace.Meters;
                Draw.ThicknessSpace = ThicknessSpace.Meters;
                Draw.RadiusSpace = ThicknessSpace.Meters;
                Draw.ZTest = CompareFunction.Always;
                Draw.Matrix = socket.transform.localToWorldMatrix;
                List<Color> numbers = new List<Color> { Color.green, Color.blue, Color.white, Color.black, Color.yellow, Color.cyan, Color.magenta };


                //Draw.BackgroundColor = numbers[random.Next(numbers.Count)];
                bool groupHoverActive = __instance == hoveredGroupManager && hoveredGroupSockets != null;
                bool groupHovered = groupHoverActive && hoveredGroupSockets.Contains(socket);
                bool singleSocketHovered = !groupHoverActive && socket == _hoveredSocket;
                Draw.Color = socket == _selectedSocket || groupHovered || singleSocketHovered
                        ? _selectedColor
                        : socket.Component != null ? _filledColor : _emptyColor;

                //Draw.BackgroundColor = numbers[random.Next(numbers.Count)];

                //Testing

                //End testing


                SocketEditorUISettings socketFilters = socket.GetComponent<SocketEditorUISettings>() ?? new SocketEditorUISettings();
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
                    case RenderShape.Invisible:
                        break;
                    default:
                        break;
                }
                if (socketFilters.DrawColor != Color.clear)
                {
                    Color GetDebugColor(HullSocket socket)
                    {
                        return socket.Type switch
                        {
                            HullSocketType.Surface => Color.red,
                            HullSocketType.Compartment => Color.yellow,
                            HullSocketType.Module => Color.cyan,
                            _ => Color.white,
                        };
                    }
                    Color newcolor = GetDebugColor(socket);
                    newcolor.a = Draw.Color.a;
                    Draw.Color = newcolor;
                }


            }
        }
        for (int i = 0; i < _sockets.Length; i++)
        {
            HullSocket socket = _sockets[i];
            if (socket.Type != 0)
                continue;
            if (socket.Component != null && socket.Component.UseTraversalArcs)
            {
                void DrawArc(Quaternion rot, float angleStart, float angleEnd, bool circle = false)
                {
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
                        Draw.Matrix = socket.transform.localToWorldMatrix;
                        Vector3Int socketSize = socket.Size.Dimensions;
                        float arcRadius = Mathf.Max(socketSize.x, socketSize.z) * 0.25f * _arcRadiusBuffer;
                        Vector3 attach = socket.AttachPoint / 10f * _arcLift;
                        Draw.Color = _arcSoftLimit;
                        if (circle)
                        {
                            Draw.Ring(attach, Vector3.up, arcRadius, _arcThickness);
                            return;
                        }

                        Draw.Color = (socket?.TraverseLimits?.LimitFiringOnly ?? false) ? _arcSoftLimit : _arcHardLimit;

                        Draw.Arc(attach, rot * Quaternion.LookRotation(Vector3.up), arcRadius, angleStart * ((float)Math.PI / 180f), angleEnd * ((float)Math.PI / 180f));
                    }
                }
                //x = 90 makes elevation
                if (socket.TraverseLimits is not TraversalLimits _rearLimits)
                {
                    DrawArc(Quaternion.identity, 0, 0, true);
                    continue;
                }
                if (socket.gameObject.transform.GetComponent<CustomTraversalLimits>()?.PublicForwardLimits is not TraversalLimits _forwardLimits || socket.gameObject.transform.GetComponent<CustomTraversalLimits>().Ignore)
                {
                    DrawArc(Quaternion.Euler(0f, -90f, 0f), 0f - _rearLimits.LeftAngle, _rearLimits.RightAngle);
                    continue;
                }
                //Debug.LogError("Rear arc");
                //Debug.LogError(_rearLimits.LeftAngle + " " + _rearLimits.RightAngle);
                //Debug.LogError("Front arc");
                //Debug.LogError(_forwardLimits.LeftAngle + " " + _forwardLimits.RightAngle);
                //Draw.Arc(attach, Quaternion.Euler(0f, 0f, 0f) * Quaternion.LookRotation(Vector3.up), arcRadius, (0f - socket2.TraverseLimits.Value.LeftAngle) * ((float)Math.PI / 180f), (socket2.TraverseLimits.Value.RightAngle - 90f) * ((float)Math.PI / 180f));

                //Draw.Arc(attach, Quaternion.Euler(0f, 0f, 0f) * Quaternion.LookRotation(Vector3.up), arcRadius, (90f - _forwardLimits.RightAngle) * ((float)Math.PI / 180f), (socket2.TraverseLimits.Value.RightAngle - 90f) * ((float)Math.PI / 180f));
                DrawArc(Quaternion.Euler(0f, 0f, 0f), -90f + _forwardLimits.RightAngle, socket.TraverseLimits.Value.RightAngle - 90f);
                DrawArc(Quaternion.Euler(0f, 0f, 180f), -90f + _forwardLimits.LeftAngle, socket.TraverseLimits.Value.LeftAngle - 90f);

            }
        }
        return false;
    }
}

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


[HarmonyPatch(typeof(SettingsMagazineLoadout), nameof(SettingsMagazineLoadout.AddMagazine))]
class SettingsMagazineLoadoutAddMagazine
{
    static void Prefix(SettingsMagazineLoadout __instance)
    {
        //
        Common.LogPatch();


    }

    static void Postfix(SettingsMagazineLoadout __instance)
    {
        Common.LogPatch();
        
        ModalListSelectDetailed select = (ModalListSelectDetailed)MenuController.Instance.GetTopMenu();
        if (select == null)
            return;

        IMagazineProvider _provider = Common.GetVal<IMagazineProvider>(__instance, "_provider");
        BaseHull _hull = Common.GetVal<BaseHull>(__instance, "_hull");
        /*
        _openPalette.AddFilter("Show Only Filtered", delegate (object data)
        {
            IMunition ammo2 = data as IMunition;
            return true;
        }, initialState: true);
        */
        select.RemoveFilters();
        //Debug.LogError("Finished Delegates");
 

        bool anyTemplates = false;
        foreach (MissileTemplate template in _hull.MyShip.Fleet.AvailableMunitions.AllMissileTemplates)
        {
            if (!template.InstanceInFleet && _provider.RestrictionCheck(template.MissileBody))
            {
                anyTemplates = true;
                break;
            }
        }
        if (_provider.CanFeedExternally)
        {
            List<IWeapon> weapons = _hull.CollectComponents<IWeapon>();
            List<Spacecraft> craft = _hull.MyShip.UniqueCraftTypesOnShip();
            List<AmmoConsumingFalloffEffect> ammoeffects = _hull.gameObject.GetComponentsInChildren<AmmoConsumingFalloffEffect>().ToList();
            select.AddFilter("$UI_FLTED_SELECTAMMO_CURRENTONLY", delegate (object data)
            {
                IMunition ammo2 = data as IMunition;
                if (ammo2 != null)
                {
                    return 
                        weapons.Any((IWeapon x) => x.NeedsExternalAmmoFeed && x.IsAmmoCompatible(ammo2)) || 
                        craft.Any((Spacecraft x) => x.AnyLoadoutUsesAmmoType(ammo2)) ||
                        ammoeffects.Any(effect => effect.ValidAmmo.Any(x => x.IsAmmoCompatible(ammo2)));
                        ;
                }
                MissileTemplate template2 = data as MissileTemplate;
                return template2 == null || 
                weapons.Any((IWeapon x) => x.NeedsExternalAmmoFeed && x.IsAmmoCompatible(template2.MissileBody));
            }, initialState: true);
            //select.AddFilter("Actually Good", delegate (object data) { return false; }, true);
        }
        if (anyTemplates)
        {
            select.AddFilter("$UI_FLTED_SELECTAMMO_ADDEDONLY", (object data) => !(data is MissileTemplate), initialState: true);
        }
    }
}

[HarmonyPatch(typeof(Ship), nameof(Ship.LoadFromSave))]
class LoadPatch
{
    static void Postfix(Ship __instance)
    {
        Common.LogPatch();
        HullSocket[] sockets = __instance.GetComponentsInChildren<HullSocket>();
        foreach (HullSocket socket in sockets)
        {
            SocketFilters socketFilters = socket.GetComponent<SocketFilters>();
            if (socketFilters != null && socket.Component == null && !socketFilters.AllowNullComponent)
                socket.SetComponent(null);
        }
    }
}

[HarmonyPatch(typeof(Ship), nameof(Ship.RebuildAmmoFeeder))]
class ShipRebuildAmmoFeeder
{
    static void Postfix(Ship __instance)
    {
        //Debug.LogError("Rebuilding Ammo Feeder");

        foreach (HullSocket socket in __instance.Hull.AllSockets)
        {
            SocketFilters.EnsureChildSetup(socket, socket.Component);
            //if (socket.Component != null)
                //Debug.LogError("Socket Build" + socket.name + " has component " + socket.Component.name);

        }
        foreach (HullSocket socket in __instance.Hull.AllSockets)
        {
            if (socket.Component != null && !SocketFilters.CheckLegal(socket, socket.Component))
            {
                //Debug.LogError("Validator: Illegal component installed on socket: " + socket.name);
                socket.SetComponent(null);
            }
            else if (socket.Component != null)
            {
                //Debug.LogError("Validator: legal component installed on socket: " + socket.name);
            }
        }

    }
}

[HarmonyPatch(typeof(SocketItem), "SetComponent")]
class SocketItemSetComponent
{
    static void Prefix(SocketItem __instance, HullComponent component)
    {
        SocketEditorUISettings? settings = component?.gameObject?.GetComponent<SocketEditorUISettings>();
        settings = settings ?? __instance.Socket.gameObject.GetComponent<SocketEditorUISettings>();
        if (settings == null)
            return;
        Common.SetVal(__instance, "_installedColor", settings.InstalledColor);
    }

    static void Postfix(SocketItem __instance, HullComponent component)
    {
        Common.SetVal(__instance, "_installedColor", ColorName.DarkBlue);
    }

}

[HarmonyPatch(typeof(HullSocket), nameof(HullSocket.SetComponent))]
class HullSocketPatch
{
    static bool HandleNullSocket(HullSocket socket, ref HullComponent componentPrefab)
    {
        if (componentPrefab != null)
        {
            return false;
        }
        SocketFilters socketFilters = socket.GetComponent<SocketFilters>() ?? new();
        if (socketFilters.AllowNullComponent)
        {
            //Common.Trace("Null Socket OK");
            return false;
        }

        //Common.Trace("Handle Null Socket");

        componentPrefab = BundleManager.Instance.GetHullComponent(socket.DefaultComponent);
        if (componentPrefab == null)
        {
            return true;
            Common.Hint(socket, $"{socket.Key} Does not have a valid default component {socket.DefaultComponent} and doesn't allow null component");
        }
        return false;
    }

    static bool Prefix(HullSocket __instance, ref HullComponent componentPrefab)
    {
        Common.LogPatch();
        HullSocket socket = __instance;
        SocketFilters socketFilters = socket.GetComponent<SocketFilters>() ?? new();
        
        //Common.Trace($"Socket Filter Key: {socket.Key} Allow Null {socketFilters.AllowNullComponent} comp key {componentPrefab?.SaveKey}");

        if (socket.GetComponent<SocketFilters>() == null && componentPrefab?.GetComponent<SocketFilters>() == null)
            return true;

        if (socket.Component is HullComponent _component && false)
        {
            _component.SetSocket(null);
            CheckDepedends(socket, true, true);
            _component.enabled = false;
            UnityEngine.Object.Destroy(_component.gameObject);
            Common.SetVal(socket, "_component", null);
            //SetPrivateField(_component, "_size", _component);
            socket.UpdateColliderActive();
        }
        //return true;
        //Common.Trace(componentPrefab.Category);

        //Common.Trace("Set Component Prefix");
        //if(componentPrefab != null)
        //    Common.Trace("General Install " + componentPrefab.SaveKey);

        if (HandleNullSocket(socket, ref componentPrefab))
            return true;


        if (!CheckDepedends(socket))
        {
            //Common.Trace("Bad Depedends Check");
            //return false;
        }


        if (!SocketFilters.CheckLegal(socket, componentPrefab))
        {
            //Common.Trace("Illegal socket");
            componentPrefab = null;
            if (HandleNullSocket(socket, ref componentPrefab))
                return true;

        }
        else if (!componentPrefab?.TestSocketFit(socket.Size) ?? false)
        {

            //Common.Trace("poor socket");
            socketFilters.Size = socket.Size.Dimensions;
            socketFilters.Resized = true;
            Common.SetVal(socket, "_size", componentPrefab.Size);
            return true;
        }

        return true;
    }

    static void Postfix(HullSocket __instance)
    {
        Common.LogPatch();
        HullSocket socket = __instance;

        CheckDepedends(socket, true);
        SocketFilters socketFilters = socket.GetComponent<SocketFilters>() ?? new SocketFilters();

        if (socketFilters.Resized && socketFilters.Size != null)
        {
            //Common.Trace("Reseting size");
            Common.SetVal(__instance, "_size", socketFilters.Size);
            socketFilters.Resized = false;
        }
        /*
        if(socket.Component == null)
            Common.Trace("Component Null");
        else
            Common.Trace(socket.Component.ComponentName);
        */
        if (socket.Component == null && !socketFilters.AllowNullComponent)
        {
            //Common.Trace("Comp Removed");
            HullComponent componentPrefab = null;
            if (HandleNullSocket(socket, ref componentPrefab))
                return;
            socket.SetComponent(componentPrefab);
        }
        Finalize(__instance);

    }
    public static void Finalize(HullSocket __instance)
    {
        //Needed for dynamic reduction
        //__instance.GetComponentInParent<Ship>().EditorRecalcCrewAndResources();
    }

    static bool CheckDepedends(HullSocket socket, bool write = false, bool remove = false)
    {
        if (socket == null)
            return true;
        ComponentDependencies? componentDependencies = socket.Component?.GetComponent<ComponentDependencies>();
        if (componentDependencies == null) return true;
        HullSocket[] sockets = socket.gameObject.transform.parent.GetComponentsInChildren<HullSocket>();

        bool returnval = true;
        foreach (KeyValuePair<string, string> componentDependency in componentDependencies.Dependendents)
        {
            HullSocket testSocket = sockets.Where(testSocket => componentDependency.Key == testSocket.Key).First();
            //Common.Trace("D " + componentDependency.Key + " " + componentDependency.Value);
            if (remove)
            {
                testSocket.SetComponent(null);

            }
            else if (write && (componentDependencies.HardInstallDepedenents || (componentDependency.Value != testSocket?.Component?.SaveKey && componentDependencies.InstallDepedenents)))
            {
                testSocket?.SetComponent(BundleManager.Instance.GetHullComponent(componentDependency.Value));
            }
        }

        foreach (KeyValuePair<string, string> componentDependency in componentDependencies.Requirements)
        {
            HullSocket testSocket = sockets.Where(testSocket => componentDependency.Key == testSocket.Key).First();
            //Common.Trace("R " + componentDependency.Key + " " + componentDependency.Value);
            if (remove)
                testSocket.SetComponent(null);
            else if (componentDependency.Value != testSocket?.Component?.SaveKey || componentDependencies.HardInstallDepedenents)
            {
                if (componentDependencies.InstallRequirements && write)
                    //Debug.LogError("Add");
                    testSocket?.SetComponent(BundleManager.Instance.GetHullComponent(componentDependency.Value));
                else
                    returnval = false;
            }
        }

        //return true;
        return componentDependencies.InstallRequirements || returnval;

    }
}
