public class FixedTubeLauncherComponent : TubeLauncherComponent, IFixedWeapon
{

    public bool NeedsTightPID => false;

    public Direction FacingDirection => base.Socket.MyHull.MyShip.transform.InverseTransformDirection(base.transform.up).normalized.RemoveTransients().ClosestSide();

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

    public override void GetDesignWarnings(List<string> warnings)
    {
        if (_myHull.MyShip.AmmoFeed.GetAllCompatibleAmmoTypes(this).Count == 0)
        {
            warnings.Add("No ammunition for weapon: " + base.ComponentName + " on " + base.Socket.name);
        }
    }
    //public bool CyclingShot => Time.fixedTime >= Common.GetVal<float>(this, "_nextShotTime");
}

