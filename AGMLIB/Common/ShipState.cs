using FleetEditor;
using Game.Units;
using Munitions;
using Ships;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
#pragma warning disable IDE1006 // Naming Styles
public class ShipState : MonoBehaviour
{

    [System.Serializable]
    public enum State
    {
        Ignore,
        Enabled,
        Disabled

    }
    // Start is called before the first frame update



    protected State flankState
    {
        get
        {
            if (ShipController.Throttle == MovementSpeed.Flank)
                return State.Enabled;
            else
                return State.Disabled;
        }
    }
    protected State battleshortState
    {
        get
        {
            if (ShipController.BattleShortEnabled == true)
                return State.Enabled;
            else
                return State.Disabled;
        }
    }

    protected State controlState
    {
        get
        {
            if (ShipController.CommandState == CommandFunctions.None)
                return State.Disabled;
            else
                return State.Enabled;
        }
    }

    protected State elimnatedState
    {
        get
        {
            if (ShipController.IsEliminated)
                return State.Disabled;
            else
                return State.Enabled;
        }
    }
    [Space]
    [Header("Activation Conditions")]
    [Space]
    //[TextArea]
    //[Header("Ship State Settings")]
    //[TextArea]
    [Tooltip("Arbitary text message")]
    //[TextArea]
    [HideInInspector]
    public Ship Ship;
    [HideInInspector]
    public ShipController ShipController;
    [HideInInspector]
    public ResourceComponent ResourceComponent;
    [HideInInspector]
    public Ship ship;
    [HideInInspector]
    public HullComponent module;
    [HideInInspector]
    public WeaponComponent weapon;
    [HideInInspector]
    public EditorShipController EditorShipController;
    

    protected virtual void Awake()
    {
        if (EditorShipController == null)
            EditorShipController = transform.gameObject.GetComponentInParent<EditorShipController>();
        if (EditorShipController == null)
            EditorShipController = transform.gameObject.GetComponent<EditorShipController>();
        if (EditorShipController == null)
            EditorShipController = transform.gameObject.GetComponentInChildren<EditorShipController>();
        if (ShipController == null)
            ShipController = transform.gameObject.GetComponentInParent<ShipController>();
        if (ShipController == null)
            ShipController = transform.gameObject.GetComponent<ShipController>();
        if (ShipController == null)
            ShipController = transform.gameObject.GetComponentInChildren<ShipController>();
        if (ship == null && ShipController != null && ShipController.Ship != null)
            ship = ShipController.Ship;
        if (ship == null)
            ship = transform.gameObject.GetComponentInParent<Ship>();
        if (ship == null)
            ship = transform.gameObject.GetComponent<Ship>();

        if (ResourceComponent == null)
            ResourceComponent = transform.gameObject.GetComponentInParent<ResourceComponent>();
        if (ResourceComponent == null)
            ResourceComponent = transform.gameObject.GetComponent<ResourceComponent>();
        if (ResourceComponent == null)
            ResourceComponent = transform.gameObject.GetComponentInChildren<ResourceComponent>();
        if (ResourceComponent == null)
            ResourceComponent = ship.gameObject.GetComponentInChildren<ResourceComponent>();
        if (ResourceComponent == null)
            ResourceComponent = ship.gameObject.GetComponent<ResourceComponent>();

        module = gameObject.GetComponent<HullComponent>();
        if (module == null)
            module = gameObject.GetComponentInParent<HullComponent>();
        weapon = gameObject.GetComponent<WeaponComponent>();
        if (weapon == null)
            weapon = gameObject.GetComponentInParent<WeaponComponent>();
        //module = gameObject.GetComponent<HullComponent>();
    }

}
#pragma warning restore IDE1006 // Naming Styles
