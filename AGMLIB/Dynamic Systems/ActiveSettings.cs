using Game.Units;
using HarmonyLib;
using Ships;
using Sound;
using System.Collections.Generic;
using System.Linq;

//using System.Diagnostics;
using UnityEngine;
public class ActiveSettings : ShipState
{
    // Start is called before the first frame update

    protected State HullComponentState => Module != null && Module.IsFunctional ? State.Enabled : State.Disabled;

    protected State FiringState => Weapon != null && Weapon.CurrentlyFiring ? State.Enabled : State.Disabled;
    protected State ResourceState => ResourceComponent != null && ResourceComponent.fillpercentage > threshold? State.Enabled : State.Disabled;
    protected State OnFireState => lastfired + fireactivetime >  Time.fixedTime ? State.Enabled : State.Disabled;

    protected State Buttonstate
    {
        get
        {
            if (activateButtonState == State.Ignore)
                return activateButtonState;
            
            foreach(KeyValuePair<string, string> StateToTest in ActiveStates)
            {
                if (!_buttons.TryGetValue(StateToTest.Key, out ShipInfoButton button)) {
                    _buttons.Add(StateToTest.Key, ShipInfoButton.FindButton(ShipController, StateToTest.Key));
                }

                if (button == null)
                    return State.Ignore;

                if (StateToTest.Value[0] == '!')
                {
                    if (button.CurrentOption.Text == StateToTest.Value.Substring(1))
                        return State.Disabled;
                }
                else if (button.CurrentOption.Text != StateToTest.Value)
                    return State.Disabled;
            }
            
            return State.Enabled;
        }
    }
    protected State VelocityState => velocity > velocitythreshold ? State.Enabled : State.Disabled;

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
    public float threshold = 1f;
    public State activateEliminatedState;
    public State activateFiringState;
    public State activateButtonState;
    public State activateOnFireReportState;
    public float fireactivetime = 1;
    public State activateVelocity;
    public float velocitythreshold = 1;
    [HideInInspector]
    public float lastfired = -1000;
    public State activateJamState;//Dead
    public State activateNearbyShipState;//Dead
    public State activateTrackedState;//Dead
    public State activateUnderAttack;//Dead

    public List<string> ActiveButtonNames = new(1);
    public List<string> ActiveButtonStateName = new(1);
    protected IEnumerable<KeyValuePair<string, string>> ActiveStates => ActiveButtonNames.Zip(ActiveButtonStateName, (ButtonName, StateName) =>  new KeyValuePair<string, string>(ButtonName, StateName));
    protected Dictionary<string, ShipInfoButton> _buttons = new();


    [HideInInspector]
    public bool lastactive = false;
    //[TextArea]
    [HideInInspector]
    public bool active = false;

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        if (ShipController == null)
            return;
        lastactive = active;
        if (activateFlankState != State.Ignore && activateFlankState != flankState)
        {
            //Debug.LogError("Setting " + gameObject.name + " inactive because active state " + activateFlankState +  " does not match current state " + flankState);
            active = false;
        }
        else if (activateBattleshortState != State.Ignore && activateBattleshortState != battleshortState)
            active = false;
        else if (activateResourceState != State.Ignore && activateResourceState != ResourceState)
            active = false;
        else if (activateControlState != State.Ignore && activateControlState != controlState)
            active = false;
        else if (activateHullComponentstate != State.Ignore && activateHullComponentstate != HullComponentState)
            active = false;
        else if (activateEliminatedState != State.Ignore && activateEliminatedState != elimnatedState)
            active = false;
#pragma warning disable IDE0045 // Convert to conditional expression
        else if (activateFiringState != State.Ignore && activateFiringState != FiringState)
            active = false;
        else if (activateButtonState != State.Ignore && activateButtonState != Buttonstate)
            active = false;
        else if (activateOnFireReportState != State.Ignore && activateOnFireReportState != OnFireState)
            active = false;
        else if (activateVelocity != State.Ignore && activateVelocity != VelocityState)
            active = false;
        else
            active = true;
#pragma warning restore IDE0045 // Convert to conditional expression
    }
}
[HarmonyPatch(typeof(ContinuousWeaponComponent), nameof(ContinuousWeaponComponent.ReportFired))]
public class ContinuousWeaponComponentReportFired
{
    static void Prefix(ContinuousWeaponComponent __instance)
    {
        foreach (ActiveSettings setting in __instance.GetComponentsInChildren<ActiveSettings>())
            setting.lastfired = Time.fixedTime;
    }
}
[HarmonyPatch(typeof(DiscreteWeaponComponent), "OnShellFired")]
public class DiscreteWeaponComponentReportFired
{
    static void Prefix(DiscreteWeaponComponent __instance)
    {
        //Debug.LogError("Fired");
        
        foreach (ActiveSettings setting in __instance.GetComponentsInChildren<ActiveSettings>())
            setting.lastfired = Time.fixedTime;
    }
}