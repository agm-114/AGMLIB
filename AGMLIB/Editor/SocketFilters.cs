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
using Debug = UnityEngine.Debug;
using static UnityEngine.ParticleSystem;
using Bundles;
using static UnityEngine.EventSystems.EventTrigger;
using System.Runtime.CompilerServices;
using UnityEngine.Serialization;

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
}

public class SocketFilters : MonoBehaviour, IFilter
{
    
    public bool disableremoval = false;
    public bool disabepalette = false;
    [SerializeField]
    [FormerlySerializedAs("whitelist")]
    protected List<string> whitelist = new();
    [SerializeField]
    [FormerlySerializedAs("whitelisteverything")]
    protected bool whitelisteverything = false;
    [SerializeField]
    [FormerlySerializedAs("blacklist")]
    protected List<string> blacklist = new();
    [SerializeField]
    [FormerlySerializedAs("blacklisteverything")]
    protected bool blacklisteverything = false;
    public List<string> Whitelist => whitelist;
    public bool Whitelisteverything => whitelisteverything;
    public List<string> Blacklist => blacklist;
    public bool Blacklisteverything => blacklisteverything;

    // Start is called before the first frame update
    [HideInInspector]
    public Vector3Int Size;
    [HideInInspector]
    public bool Resized = false;
}


[HarmonyPatch(typeof(ShipEditorPane), nameof(ShipEditorPane.OpenPalette))]
class ShipEditorPanePatch
{
    static bool Prefix(ShipEditorPane __instance, HullSocket socket)
    {
        SocketFilters socketFilters = socket.GetComponent<SocketFilters>();
        if (socketFilters != null)
            return !socketFilters.disabepalette;
        else return true;
    }

    static void Postfix(ShipEditorPane __instance, HullSocket socket)
    {
        //Debug.LogError("Ship Editor Pane Postfix");
        SocketFilters socketFilters = socket.GetComponent<SocketFilters>() ?? new SocketFilters();
        //if(socketFilters == null)
        //ComponentPalette _palette = (ComponentPalette)GetPrivateField(__instance, "_palette"); ;


        ModalListSelectDetailed _openPalette = Common.GetVal<ModalListSelectDetailed>(__instance, "_openPalette");
        if(_openPalette == null)
        {
            return;
            //Debug.LogError("WARN: No Open Palette");
        }


        //SocketOutlineManager

        //Debug.LogError("Remove Filter");
        _openPalette.RemoveFilters();

        //Debug.LogError("Add Filter");
        _openPalette.AddFilter("Show Only Filtered", delegate (object data)
        {
            //Debug.LogError("Running Fillter Delegate");
            if (data == null)
            {
                if (socketFilters.disableremoval)
                    return false;
                return true;
            }
            if (data is HullComponent hullComponent)
            {

                SocketFilters componentFilters = hullComponent.GetComponent<SocketFilters>() ?? new SocketFilters();
                //Debug.LogError("Filtering " + hullComponent.name + " on socket" + socket.Key);
                //Debug.LogError("Check Whitelists ");
                //Debug.LogError("Check Socketfilter whitelists ");
                //socketFilters.whitelist.Contains(hullComponent.SaveKey);
                //Debug.LogError("Check Comp Whitelists ");
                //componentFilters.whitelist.Contains(socket.Key);
                //Debug.LogError("Check both Whitelists ");
                //componentFilters.whitelist.Contains(socket.Key);
                if (socketFilters.Whitelist.Contains(hullComponent.SaveKey) || componentFilters.Whitelist.Contains(socket.Key))
                    //Debug.LogError("In Whitelists");
                    return true;
                //Debug.LogError("Check Blaclist ");
                else if (socketFilters.Blacklist.Contains(hullComponent.SaveKey) || componentFilters.Blacklist.Contains(socket.Key) || socketFilters.Blacklisteverything || componentFilters.Blacklisteverything)
                    return false;
                //Debug.LogError("Check Universal Blacklists ");
                else if (socketFilters.Whitelisteverything || componentFilters.Whitelisteverything)
                    return true;
                //Debug.LogError("Check Fit ");
                else if(hullComponent.TestSocketFit(socket.Size))
                    return true;
                else
                    return false;
                
            }
            else
                return false;
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
            if (socketFilters!= null && socket.Component == null && socketFilters.disableremoval)
            {
                HullComponent componentPrefab = BundleManager.Instance.GetHullComponent(socket.DefaultComponent);
                socket.SetComponent(componentPrefab);
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

        //Debug.LogError("Set Component Prefix");
        SocketFilters socketFilters = socket.GetComponent<SocketFilters>() ?? new();

        if (componentPrefab == null)
            return true;
        if (socket.GetComponent<SocketFilters>() == null && componentPrefab.GetComponent<SocketFilters>() == null)
            return true;

        HullComponent _component = socket.Component;
        if (_component != null)
        {

            _component.SetSocket(null);
            UnityEngine.Object.Destroy(_component.gameObject);
            //SetPrivateField(_component, "_size", _component);
            socket.UpdateColliderActive();
        }


        SocketFilters componentFilters = componentPrefab.GetComponent<SocketFilters>() ?? new();
        bool resizesocket = false;

        if (socketFilters.Whitelist.Contains(componentPrefab.SaveKey) || componentFilters.Whitelist.Contains(socket.Key))
            resizesocket = true;
        else if (socketFilters.Blacklist.Contains(componentPrefab.SaveKey) || componentFilters.Blacklist.Contains(socket.Key) || socketFilters.Blacklisteverything || componentFilters.Blacklisteverything)
        {
            if(socketFilters.disableremoval)
                componentPrefab = BundleManager.Instance.GetHullComponent(socket.DefaultComponent);
            else
                return false;
        }
        else if (socketFilters.Whitelisteverything || componentFilters.Whitelisteverything)
            resizesocket = true;

        if (resizesocket && !componentPrefab.TestSocketFit(socket.Size))
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
        if (socket.Component == null && socketFilters.disableremoval)
        {
            //Debug.LogError("Comp Removed");
            HullComponent componentPrefab = BundleManager.Instance.GetHullComponent(socket.DefaultComponent);
            socket.SetComponent(componentPrefab);
        }

    }

}


