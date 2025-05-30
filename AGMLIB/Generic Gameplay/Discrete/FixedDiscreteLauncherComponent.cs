﻿public class BlockingWeapon : MonoBehaviour
{
    public bool InEditor => GetComponentInParent<EditorShipController>() != null;
    ComponentActivity Status => gameObject.GetComponentInChildren<HullComponent>().GetActivityStatus();
    public bool Blocking => InEditor || Status == ComponentActivity.Active || Status == ComponentActivity.ActiveTimed || Status == ComponentActivity.OutOfRange;

    public void FixedUpdate()
    {
        //Common.Hint($"Status: {Status}");
    }
}

public class FixedDiscreteLauncherComponent : FixedDiscreteWeaponComponent
{
    [SerializeField]
    private float _traverseRate = 1;//=> Common.GetVal<float>(this, "_traverseRate");
    [SerializeField]
    private float _elevationRate = 1;//=> Common.GetVal<float>(this, "_elevationRate");

    public bool Firing => this.CheckFire();


    private BlockingWeapon Tube => transform.parent.parent.GetComponentInChildren<BlockingWeapon>();
    private bool Blocked => Tube?.Blocking ?? false;

    protected bool PastBlocked = false;
    //BaseCellLauncherComponent


    protected override void Update()
    {
        base.Update();
        if (PastBlocked != Blocked)
        {
            PastBlocked = Blocked;
            FireActivityChangedEvent();
        }
    }

    protected override ComponentActivity GetFunctionalActivityStatus() => Blocked ? ComponentActivity.NotAuthorized : base.GetFunctionalActivityStatus();

    protected override bool AimCheck(Vector3 aimPoint, bool ignoreRange)
    {

        if (Blocked) // CurrentlyFiring tube.BlockingFire &&
        {
            //Debug.LogError("Blocked" + tube.GetActivityStatus());
            return false;
        }
        return base.AimCheck(aimPoint, ignoreRange);
    }

}