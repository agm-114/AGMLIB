interface IMonoBehaviourFilter<FilteredMonoBehaviour> where FilteredMonoBehaviour : MonoBehaviour
{
    bool Active(FilteredMonoBehaviour filter);
}

public class ActiveSettings : ShipState, IMonoBehaviourFilter<Ship>
{



    [HideInInspector]
    public HullComponent Module;
    [HideInInspector]
    public WeaponComponent Weapon;
    // Start is called before the first frame update

    protected ConditionalState HullComponentState => Module != null && Module.IsFunctional ? ConditionalState.Enabled : ConditionalState.Disabled;

    protected ConditionalState FiringState => Weapon != null && Weapon.CurrentlyFiring ? ConditionalState.Enabled : ConditionalState.Disabled;
    protected ConditionalState OnFireState => lastfired + fireactivetime > Time.fixedTime ? ConditionalState.Enabled : ConditionalState.Disabled;

    protected ConditionalState Buttonstate
    {
        get
        {
            if (activateButtonState == ConditionalState.Ignore)
                return activateButtonState;

            foreach (KeyValuePair<string, string> StateToTest in ActiveStates)
            {
                ShipInfoButton button = GetButton(StateToTest.Key);

                if (button == null)
                    return ConditionalState.Ignore;

                if (StateToTest.Value[0] == '!')
                {
                    if (button.CurrentOption.Text == StateToTest.Value.Substring(1))
                        return ConditionalState.Disabled;
                }
                else if (button.CurrentOption.Text != StateToTest.Value)
                    return ConditionalState.Disabled;
            }

            return ConditionalState.Enabled;
        }
    }
    protected ConditionalState VelocityState => Velocity > velocitythreshold ? ConditionalState.Enabled : ConditionalState.Disabled;

    [Space]
    [Header("Activation Conditions")]
    [Space]
    //[TextArea]
    //[Header("Ship State Settings")]
    //[TextArea]
    [Tooltip("Arbitary text message")]
    //public string Notes = "T$$anonymous$$s component shouldn't be removed, it does important stuff.";
    public ConditionalState activateFlankState;
    public ConditionalState activateBattleshortState;
    //public State activateModuleState;
    public ConditionalState activateControlState;
    public ConditionalState activateHullComponentstate;
    //public HullComponent optionalHullComponent;
    //[Header("Resource State Settings")]
    public ConditionalState activateResourceState;
    public float threshold = 1f;
    public ConditionalState activateEliminatedState;
    public ConditionalState activateFiringState;
    public ConditionalState activateButtonState;
    public ConditionalState activateOnFireReportState;
    public float fireactivetime = 1;
    public ConditionalState activateVelocity;
    public float velocitythreshold = 1;
    [HideInInspector]
    public float lastfired = -1000;
    public ConditionalState activateJamState;//Dead
    public ConditionalState activateNearbyShipState;//Dead
    public ConditionalState activateTrackedState;//Dead
    public ConditionalState activateUnderAttack;//Dead

    public List<string> ActiveButtonNames = new(1);
    public List<string> ActiveButtonStateName = new(1);
    protected IEnumerable<KeyValuePair<string, string>> ActiveStates => ActiveButtonNames.Zip(ActiveButtonStateName, (ButtonName, StateName) => new KeyValuePair<string, string>(ButtonName, StateName));


    [HideInInspector]
    public bool lastactive = false;
    //[TextArea]
    [HideInInspector]
    public bool active = false;

    public override void Awake()
    {
        base.Awake();
        if (ActiveStates.Any() && activateButtonState == ConditionalState.Ignore)
            Common.Hint(this, "activateButtonState may be misconfigured");
        if (!ActiveStates.Any() && activateButtonState != ConditionalState.Ignore)
            Common.Hint(this, "activateButtonState may be misconfigured");
        Module = gameObject.GetComponent<HullComponent>();
        if (Module == null)
            Module = gameObject.GetComponentInParent<HullComponent>();
        Weapon = gameObject.GetComponent<WeaponComponent>();
        if (Weapon == null)
            Weapon = gameObject.GetComponentInParent<WeaponComponent>();
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        if (ShipController == null)
            return;
        lastactive = active;
        if (Ship != null && !this.Active(Ship))
        {
            active = false;
        }
        else if (activateHullComponentstate != ConditionalState.Ignore && activateHullComponentstate != HullComponentState)
            active = false;
        else if (activateFiringState != ConditionalState.Ignore && activateFiringState != FiringState)
            active = false;
        else if (activateButtonState != ConditionalState.Ignore && activateButtonState != Buttonstate)
            active = false;
        else if (activateOnFireReportState != ConditionalState.Ignore && activateOnFireReportState != OnFireState)
            active = false;
        else if (activateVelocity != ConditionalState.Ignore && activateVelocity != VelocityState)
            active = false;
        else
            active = true;
    }

    public bool Active(Ship filter)
    {
        ShipState state = ShipState.GetShipState(filter);
        ConditionalState ShipResourceState = state.ResourceComponent != null && state.ResourceComponent.fillpercentage > threshold ? ConditionalState.Enabled : ConditionalState.Disabled;
        bool objectactive = false;
        if (activateFlankState != ConditionalState.Ignore && activateFlankState != state.FlankState)
            objectactive = false;
        else if (activateBattleshortState != ConditionalState.Ignore && activateBattleshortState != state.BattleshortState)
            objectactive = false;
        else if (activateControlState != ConditionalState.Ignore && activateControlState != state.ControlState)
            objectactive = false;
        else if (activateEliminatedState != ConditionalState.Ignore && activateEliminatedState != state.ElimnatedState)
            objectactive = false;
        else if (activateResourceState != ConditionalState.Ignore && activateResourceState != ShipResourceState)
            objectactive = false;
        else
            objectactive = true;
        return objectactive;
    }
}
[HarmonyPatch(typeof(ContinuousWeaponComponent), nameof(ContinuousWeaponComponent.ReportFired))]
public class ContinuousWeaponComponentReportFired
{
    static void Prefix(ContinuousWeaponComponent __instance)
    {
        Common.LogPatch();
        foreach (ActiveSettings setting in __instance.GetComponentsInChildren<ActiveSettings>())
            setting.lastfired = Time.fixedTime;
    }
}
[HarmonyPatch(typeof(DiscreteWeaponComponent), "OnShellFired")]
public class DiscreteWeaponComponentReportFired
{
    static void Prefix(DiscreteWeaponComponent __instance)
    {
        Common.LogPatch();
        foreach (ActiveSettings setting in __instance.GetComponentsInChildren<ActiveSettings>())
            setting.lastfired = Time.fixedTime;
    }
}