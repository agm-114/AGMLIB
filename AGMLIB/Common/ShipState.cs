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
    protected State flankState => ShipController.Throttle == MovementSpeed.Flank ? State.Enabled : State.Disabled;
    protected State battleshortState => ShipController.BattleShortEnabled == true ? State.Enabled : State.Disabled;
    protected State controlState => ShipController.CommandState == CommandFunctions.None ? State.Disabled : State.Enabled;
    protected State elimnatedState => ShipController.IsEliminated ? State.Disabled : State.Enabled;
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
    public HullComponent Module;
    [HideInInspector]
    public WeaponComponent Weapon;
    [HideInInspector]
    public EditorShipController EditorShipController;
    [HideInInspector]
    public Hull Hull;
    private Rigidbody _rigidbody = null;
    [HideInInspector]
    public Rigidbody? Rigidbody
    {
        get
        {
            if (ShipController != null && _rigidbody == null)
                _rigidbody = Common.GetVal<Rigidbody>(ShipController, "_rigidbody");
            return _rigidbody;
        }

    }
    public float velocity => Rigidbody?.velocity.magnitude ?? 0;
    public bool InEditor => EditorShipController != null;
    public bool InGame => !InEditor;
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
        if (Ship == null && ShipController != null && ShipController.Ship != null)
            Ship = ShipController.Ship;
        if (Ship == null)
            Ship = transform.gameObject.GetComponentInParent<Ship>();
        if (Ship == null)
            Ship = transform.gameObject.GetComponent<Ship>();

        if (ResourceComponent == null)
            ResourceComponent = transform.gameObject.GetComponentInParent<ResourceComponent>();
        if (ResourceComponent == null)
            ResourceComponent = transform.gameObject.GetComponent<ResourceComponent>();
        if (ResourceComponent == null)
            ResourceComponent = transform.gameObject.GetComponentInChildren<ResourceComponent>();
        if (ResourceComponent == null)
            ResourceComponent = Ship.gameObject.GetComponentInChildren<ResourceComponent>();
        if (ResourceComponent == null)
            ResourceComponent = Ship.gameObject.GetComponent<ResourceComponent>();
        if (ResourceComponent == null)
            ResourceComponent = Ship.gameObject.GetComponentInParent<ResourceComponent>();

        Module = gameObject.GetComponent<HullComponent>();
        if (Module == null)
            Module = gameObject.GetComponentInParent<HullComponent>();
        Weapon = gameObject.GetComponent<WeaponComponent>();
        if (Weapon == null)
            Weapon = gameObject.GetComponentInParent<WeaponComponent>();

        if (Hull == null)
            Hull = transform.gameObject.GetComponentInParent<Hull>();
        if (Hull == null)
            Hull = transform.gameObject.GetComponent<Hull>();
        if (Hull == null)
            Hull = transform.gameObject.GetComponentInChildren<Hull>();


        //module = gameObject.GetComponent<HullComponent>();
    }
}
#pragma warning restore IDE1006 // Naming Styles
