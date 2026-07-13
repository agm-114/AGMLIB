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

public class ComponentDependencies : MonoBehaviour
{

    public bool InstallDepedenents = true;
    public bool InstallRequirements = true;
    public bool HardInstallDepedenents = true;
    public bool HardInstallRequirements = true;
    public bool RemoveDepedenents = true;
    public bool RemoveRequirements = true;

    public List<String> DependendentSocketKeys = new();
    public List<String> DependendentComponentKeys = new();
    public List<String> RequirementSocketKeys = new();
    public List<String> RequirementComponentKeys = new();

    public IEnumerable<KeyValuePair<String, String>> Dependendents => DependendentSocketKeys.Zip(DependendentComponentKeys, (key, value) => new KeyValuePair<String, String>(key, value));
    public IEnumerable<KeyValuePair<String, String>> Requirements => RequirementSocketKeys.Zip(RequirementComponentKeys, (key, value) => new KeyValuePair<String, String>(key, value));

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
    public ShipEditorPane Editor;
    public HullSocket ParentSocket;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Editor.SetSocketHovered(null, true);
        SocketOutlineManagerDrawShapes.SetHoveredGroup(this, OutlineManager, Sockets);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SocketOutlineManagerDrawShapes.ClearHoveredGroup(this);
        Editor.SetSocketHovered(ParentSocket, true);
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
