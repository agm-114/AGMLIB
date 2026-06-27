using System.Diagnostics;

public class DynamicStun : ActiveSettings
{
    public enum KnockbackMode
    {
        None,
        COM,
        ComponentPostion,
        TransformPostion
    }
    public Vector3 Localize(Transform basis, Vector3 force)
    {
        //Debug.LogError(" b: " + basis.forward + " f: " +  force);
        Vector3 returnval = Vector3.zero;
        returnval += basis.forward * force.z;
        returnval += basis.right * force.x;
        returnval += basis.up * force.y;
        return returnval;
    }
    protected override void FixedUpdate()
    {

        base.FixedUpdate();
        /*
        if (!active)
        {
            if (DisableWeapons)
                Common.SetVal(ShipController, "_weaponsControl", WeaponsControlStatus.Hold);
        }

        //Debug.LogError("DynamicStun FixedUpdate Active: " + active + " LastActive: " + lastactive + " DisableWeapons: " + DisableWeapons + " WeaponsControl: " + ShipController.WeaponsControl);

        if (DisableWeapons)
        {
            //Debug.LogError("DisableWeapons: " + DisableWeapons + " WeaponsControl: " + ShipController.WeaponsControl);
            if (ShipController == null)
            {
                Debug.LogError("Null ShipController");
            }
            lastactive = active;
            if (Ship != null && !this.Active(Ship))
            {
                Debug.LogError("Inactive Ship");
            }
            else if (activateHullComponentstate != ConditionalState.Ignore && activateHullComponentstate != HullComponentState)
                Debug.LogError("Inactive Component");
            else if (activateFiringState != ConditionalState.Ignore && activateFiringState != FiringState)
                Debug.LogError("Inactive Firing State");
            else if (activateButtonState != ConditionalState.Ignore && activateButtonState != Buttonstate)
                Debug.LogError("Inactive Button State");
            else if (activateOnFireReportState != ConditionalState.Ignore && activateOnFireReportState != OnFireState)
                Debug.LogError("Inactive OnFire State");
            else if (activateVelocity != ConditionalState.Ignore && activateVelocity != VelocityState)
                Debug.LogError("Inactive Velocity State");
            else
                Debug.LogError("Active");
        }
        */
        if (active != lastactive)
        {
            Common.RunFunc(ShipController, "HandlePowerplantsWorkingChanged", new object[] { null });
            Common.RunFunc(ShipController, "HandleDrivesWorkingChanged", new object[] { null });

            if (Knockback == KnockbackMode.COM)
                Rigidbody?.AddRelativeForce(Force, Mode);
            else if (Knockback == KnockbackMode.TransformPostion && Postion != null)
                Rigidbody?.AddForceAtPosition(Localize(Rigidbody.transform, Force), Postion.position, Mode);
            else if (Knockback == KnockbackMode.ComponentPostion)
                Rigidbody?.AddForceAtPosition(Localize(Rigidbody.transform, Force), transform.position, Mode);

        }
        //Debug.LogError("Last Fired " + lastfired + " Current Time " + Time.fixedTime + " Activate Time " +  (lastfired + fireactivetime) + " Status " + OnFireState);
        //Rigidbody?.AddForceAtPosition(transform.TransformDirection(Force/10), transform.position, Mode);

    }
    public bool DisableDrives = true;
    public bool DisableReactors = true;
    public bool DisableWeapons = true;
    public ForceMode Mode = ForceMode.Impulse;
    public KnockbackMode Knockback = KnockbackMode.ComponentPostion;
    public Transform Postion;
    public Vector3 Force = new Vector3(0, 0, -1000);// Vector3.zero;
    //public enum ForceMode { Start, Impulse, Acceleration, Force, VelocityChange };
}
public class DynamicWorkingCache : MonoBehaviour
{
    public List<DriveComponent> _allDrives;
    public List<PowerplantComponent> _allPowerplants;
}
public static class DynamicWorkingHelpers
{
    public static IEnumerable<DynamicStun> GetActive(this ShipController ship)
    {
        return ship.gameObject.GetComponentsInChildren<DynamicStun>().ToList().Where(a => a.active);
    }
}

[HarmonyPatch(typeof(ShipController))]//, nameof(BaseCellLauncherComponent.)
[HarmonyPatch("WeaponsControl", MethodType.Getter)]
class ShipControllerWeaponsControl
{
    public static void Postfix(ref WeaponsControlStatus __result, ShipController __instance)
    {
        Common.LogPatch();
         
        //Debug.LogError("WeaponsControl Patch");
        GameObject ship = __instance.gameObject;
        if (ShipInfoBarMatchAllButtons.MatchingAllButtons)
        {
            //Debug.LogError("Buttons Not Patched");
            return;
        }
        if(!__instance.GetActive().Any())  
        {
            //Debug.LogError("No DynamicStun components found");
        }
        if (__instance.GetActive().Count(a => a.DisableWeapons) <= 0)
        {
            //Debug.LogError("No DisableWeapons components found");
            return;
        }
        __result = WeaponsControlStatus.Hold;

    }
}


[HarmonyPatch(typeof(ShipController), "HandleDrivesWorkingChanged")]
class ShipControllerHandleDrivesWorkingChanged
{
    static void Prefix(ShipController __instance)
    {
        Common.LogPatch();
        GameObject ship = __instance.gameObject;
        DynamicWorkingCache cache = ship.GetComponent<DynamicWorkingCache>() ?? ship.AddComponent<DynamicWorkingCache>();
        if (__instance.GetActive().Count(a => a.DisableDrives) <= 0)
            return;
        //Debug.LogError("Drive Patch");
        cache._allDrives = Common.GetVal<List<DriveComponent>>(__instance, "_allDrives");
        Common.SetVal(__instance, "_allDrives", new List<DriveComponent>());
    }

    static void Postfix(ShipController __instance)
    {
        Common.LogPatch();
        GameObject ship = __instance.gameObject;
        DynamicWorkingCache cache = ship.GetComponent<DynamicWorkingCache>() ?? ship.AddComponent<DynamicWorkingCache>();
        if (cache._allDrives != null)
            Common.SetVal(__instance, "_allDrives", cache._allDrives);
        cache._allDrives = null;
    }
}


[HarmonyPatch(typeof(ShipController), "HandlePowerplantsWorkingChanged")]
class ShipControllerHandlePowerplantsWorkingChanged
{
    static void Prefix(ShipController __instance)
    {
        Common.LogPatch();
        GameObject ship = __instance.gameObject;
        DynamicWorkingCache cache = ship.GetComponent<DynamicWorkingCache>() ?? ship.AddComponent<DynamicWorkingCache>();
        if (__instance.GetActive().Count(a => a.DisableReactors) <= 0)
            return;
        //Debug.LogError("Reactor Patch");
        cache._allPowerplants = Common.GetVal<List<PowerplantComponent>>(__instance, "_allPowerplants");
        Common.SetVal(__instance, "_allPowerplants", new List<PowerplantComponent>());
    }

    static void Postfix(ShipController __instance)
    {
        Common.LogPatch();
        GameObject ship = __instance.gameObject;
        DynamicWorkingCache cache = ship.GetComponent<DynamicWorkingCache>() ?? ship.AddComponent<DynamicWorkingCache>();
        if (cache._allPowerplants != null)
            Common.SetVal(__instance, "_allPowerplants", cache._allPowerplants);
        cache._allPowerplants = null;
    }
}