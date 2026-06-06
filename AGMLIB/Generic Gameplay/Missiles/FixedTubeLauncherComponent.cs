using Utility.Localization;

public class FixedTubeLauncherComponent : TubeLauncherComponent, IFixedWeapon
{

    public bool NeedsTightPID => false;

    public Direction FacingDirection => base.Socket.Hull.transform.InverseTransformDirection(base.transform.up).normalized.RemoveTransients().ClosestSide();

    protected override void Update()
    {
        //Debug.LogError("State " + GetActivityStatus() + " " + CurrentShotTime + " " + Time.time);
        //Debug.LogError("Time " + (CurrentShotTime - Time.time) + " " + BlockingFire);
        base.Update();
        if (_lastShot != CurrentShot || _lastShot != CurrentShot || _lastBlock != BlockingFire)
        {

            _lastShot = CurrentShot;
            _lastShotTime = CurrentShotTime;
            _lastBlock = BlockingFire;
            FireActivityChangedEvent();
        }
    }
    protected bool _lastBlock = false;
    protected int _lastShot = 0;
    protected float _lastShotTime = 0;
    protected int RawShotValue => Common.GetVal<int>(this, "_currentShot");
    protected int CurrentShot => RawShotValue == 0 ? _launchesPerLoad : RawShotValue;
    public override string ActiveStatusText => "F-" + CurrentShot.ToString();
    protected float CurrentShotTime => Common.GetVal<float>(this, "_nextShotTime") + 0.1f;
    protected bool BlockingFire => CurrentShotTime >= Time.time; // _isCyclingNextShot inverse?

    protected override ComponentActivity GetFunctionalActivityStatus() => BlockingFire ? ComponentActivity.Active : base.GetFunctionalActivityStatus();

    public override void GetDesignWarnings(List<DesignWarning> warnings)
    {
        List<IMagazine> mags = base.Socket.AmmoFeed.GetAllCompatibleAmmoSources(this);
        if (mags.Count == 0)
        {
            warnings.Add(ShipDesignWarning.Warning("$UI_FLTED_WARNINGS_NOAMMO_TITLE", "$UI_FLTED_WARNINGS_NOAMMO_BODY".Localize(("compName", base.ComponentName), ("socketName", base.Socket.Name)), null));
        }
        foreach (IMagazine mag in mags)
        {
            if (mag.QuantityAvailable == 1)
            {
                warnings.Add(ShipDesignWarning.Risk("$UI_FLTED_WARNINGS_ONEROUND_TITLE", "$UI_FLTED_WARNINGS_ONEROUND_BODY".Localize(("compName", base.ComponentName), ("socketName", base.Socket.Name), ("ammoName", mag.AmmoType.MunitionDisplayName)), null));
            }
        }
    }
    //public bool CyclingShot => Time.fixedTime >= Common.GetVal<float>(this, "_nextShotTime");
}

