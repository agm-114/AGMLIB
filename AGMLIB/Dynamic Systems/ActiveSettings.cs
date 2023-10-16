using Game.Units;
using Munitions;
using Ships;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
using Utility;


public class ActiveSettings : ShipState
{






    // Start is called before the first frame update

    protected State hullComponentState
    {
        get {
            if (module != null && module.IsFunctional)
                return State.Enabled;
            else
                return State.Disabled;
        }
    }


    protected State firingState
    {
        get
        {
            if (weapon != null && weapon.CurrentlyFiring)
                return State.Enabled;
            else
                return State.Disabled;
        }
    }

    protected State resourceState
    {
        get
        {
            if (ResourceComponent != null && ResourceComponent.fillpercentage > threshold)
                return State.Enabled;
            else
                return State.Disabled;
        }
    }


    [Space]
    [Header("Activation Conditions")]
    [Space]
    //[TextArea]
    //[Header("Ship State Settings")]
    //[TextArea]
    [Tooltip("Arbitary text message")]
    //public string Notes = "T$$anonymous$$s component shouldn't be removed, it does important stuff.";
    public State activateFlankState;
    public State activateBattleshortState;
    //public State activateModuleState;
    public State activateControlState;
    public State activateHullComponentstate;
    //public HullComponent optionalHullComponent;
    //[Header("Resource State Settings")]
    public State activateResourceState;
    public State activateEliminatedState;
    public State activateFiringState;
    public float threshold = 1f;
    //[TextArea]
    [HideInInspector]
    public bool active = false;




    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        if (ShipController == null)
            return;



        if (activateFlankState != State.Ignore && activateFlankState != flankState)
        {
            //Debug.LogError("Setting " + gameObject.name + " inactive because active state " + activateFlankState +  " does not match current state " + flankState);
            active = false;
        }
        else if (activateBattleshortState != State.Ignore && activateBattleshortState != battleshortState)
            active = false;
        else if (activateResourceState != State.Ignore && activateResourceState != resourceState)
            active = false;
        else if (activateControlState != State.Ignore && activateControlState != controlState)
            active = false;
        else if (activateHullComponentstate != State.Ignore && activateHullComponentstate != hullComponentState)
            active = false;
        else if (activateEliminatedState != State.Ignore && activateEliminatedState != elimnatedState)
            active = false;
        else if (activateFiringState != State.Ignore && activateFiringState != firingState)
            active = false;
        else
            active = true;
    }

 
}
