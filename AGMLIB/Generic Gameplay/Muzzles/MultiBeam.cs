using Lib.Generic_Gameplay.Ewar;
using Utility;
using static Ships.HullPart;
using static Ships.WeaponComponent;
using Object = System.Object;
public class MultiBeam : MultiTarget { }

public class MultiTarget : MonoComponent
{
    //public int TruePostionWeight = 10;
    //public int KnownPositionWeight = 1;
    public bool SingleTargetMode = false;
    public bool RespectMuzzleRange = true;
    //public List<FireControlSensor> fireControlSensors = new List<FireControlSensor>();
    public List<BaseTrackLogic> trackLogics = new();
    public List<Muzzle> SingleTargetMuzzles = new();
    public List<Muzzle> MultiTargetMuzzles = new();

    public WeaponComponent Weapon => this.GetComponentInParent<WeaponComponent>();
    public bool MuzzlesActive => Common.GetVal<bool>(Weapon, "_muzzlesActive");
    private bool Reloading => Common.GetVal<bool>(Weapon, "_reloading");
    private bool WaitingForMuzzle => Common.GetVal<bool>(Weapon, "_waitingForMuzzle");
    private bool NoTarget => Weapon.CurrentTargetingMode == TargetingMode.None;
    bool SplitBeams(bool fire)
    {
        if (Weapon.TargetAssignedByPlayer && SingleTargetMuzzles.Count == 0)
        {
            //Trace($"Error should not be triggered");
            return false;
        }
        else if (Weapon is ContinuousWeaponComponent continuous)
        {
            //Trace($"Error should not be triggered");
            return MuzzlesActive;

        }
        else if (Weapon is DiscreteWeaponComponent discrete)
        {
            return fire;
            //Trace($"Checking Discrete Weapon reloading {Reloading} muzzle {WaitingForMuzzle}");
            //return !NoTarget && discrete.CanFire && !Reloading && !WaitingForMuzzle;


        }
        return false;
    }
    List<Muzzle> Muzzles => [.. Common.GetVal<Muzzle[]>(Weapon, "_muzzles")];



    public ITrack AssignedTrack => Weapon.CurrentlyTargetedTrack();
    [HideInInspector] public List<ITrack> AssignedTracks = [];
    public void AssignTracks(List<ITrack> tracks)
    {
        AssignedTracks = tracks;
        //Common.Trace($"AssignTracks {AssignedTracks.Count}");

        //CachedOtherPossibleTracks = null;
        //CachedOtherAssignedTracks = null;
    }

    public List<ITrack> ValidTrackList => AssignedTracks
        .Prepend(AssignedTrack)
        .WhereNotNull()//NOTE: Not needed due to later null check
        .Where(track =>
        track?.IsValid ?? false && Weapon.CanTrainOnTarget(track.TruePosition) &&
        Vector3.Distance(transform.position, track.TruePosition) <= Weapon.MaxEffectiveRange)
        .ToList();

    IEnumerable<MultiTarget> Friends => gameObject.transform.parent.parent
        .GetComponentsInChildren<MultiTarget>()
        .Except([this]);
    IEnumerable<ITrack> OtherAssignedTracks => Friends
            .ConvertAll(a => a.AssignedTrack)
            .Intersect(AssignedTracks)
            .WhereNotNull();
    //public IEnumerable<ITrack> CachedOtherAssignedTracks = null;

    IEnumerable<ITrack> OtherPossibleTracks => Friends
                .SelectMany(a => a?.ValidTrackList ?? [])
                .GroupBy(a => a)
                .OrderByDescending(a => a?.Count() ?? 0)
                .ConvertAll(a => a.Key)
                .Intersect(AssignedTracks)
                .WhereNotNull();
    //public IEnumerable<ITrack> CachedOtherPossibleTracks = null;
    public void StartFire(int muzzle, ITrack target, bool fire = false)
    {
        //return;
        //Common.Trace(this.gameObject, "StartFire");
        if (!(target?.IsValid ?? false))
            return;
        BaseTrackLogic.DefaultUpdateTrack(target, out Vector3 pos, out Vector3 _);

        if (trackLogics.Count > 0)
        {
            trackLogics[muzzle % trackLogics.Count]?.UpdateTrack(target, out pos, out Vector3 _);

        }
        //  Time.timeScale = 0.1f;
        Muzzles[muzzle].transform.rotation = Quaternion.LookRotation(pos - Muzzles[muzzle].transform.position);
        if (fire)
            Muzzles[muzzle].Fire();
        Muzzles[muzzle].FireEffect();

    }

    public void StopFire(int muzzle)
    {
        //return; 
        //Common.Trace((this.gameObject), "StopFire");
        //Muzzles[muzzle].gameObject.transform.localRotation = Quaternion.identity;
        //if (muzzle == 0)
        //    return;
        Muzzles[muzzle]?.StopFire();
        Muzzles[muzzle]?.StopFireEffect();

        if (Weapon is ContinuousWeaponComponent beamwep && !_baseRpcProvider.IsHost)
        {
            IWeaponComponentRPC _weaponRpcProvider = Common.GetVal<IWeaponComponentRPC>(Weapon, "_weaponRpcProvider");

            if (!_weaponRpcProvider.IsHost)
                return;
            _weaponRpcProvider.RpcStopFiringEffect(Weapon.RpcKey, muzzle);
        }


    }
    public List<ITrack> CalcTargetList(List<ITrack> inputtracklist)
    {
        //Trace($"Step 0 Filter {inputtracklist.Count}");

        //CachedOtherAssignedTracks ??= OtherAssignedTracks.ToList();
        List<ITrack> tracklist = inputtracklist.Except(OtherAssignedTracks.Take(inputtracklist.Count - Muzzles.Count)).ToList();
        //Trace($"Step 1 Filter {tracklist.Count}");

        if (ValidTrackList.Count <= Muzzles.Count)
            return tracklist;
        Trace("Lots of valid targets");
        //CachedOtherPossibleTracks ??= OtherPossibleTracks.ToList();
        tracklist = tracklist.Except(OtherPossibleTracks.Take(tracklist.Count - Muzzles.Count)).ToList();
        //Trace($"Step 2 Filter {tracklist.Count}");

        return tracklist;
    }

    public bool UpdateTargets(bool fire = false)
    {
        bool splitbeam = this.SplitBeams(fire);
        //Trace($"{splitbeam} runing multibeam {fire}");

        if (!splitbeam)
        {
            for (int i = 1; i < Muzzles.Count; i++)
                StopFire(i);
            //Trace("No SimBeam");
            return Common.RunFunction;
        }
        List<ITrack> tracks = ValidTrackList;

        if (tracks.Count <= 0)
            return Common.SkipFunction;

        //List<FireControlSensor> fireControlSensors = _weapon.GetComponentsInChildren<FireControlSensor>().ToList(); //____muzzles.ToList();

        if (SingleTargetMode || tracks.Count <= 1)
        {
            Trace("SimBeam singletarget");

            tracks = Enumerable.Repeat(Weapon.CurrentlyTargetedTrack(), Muzzles.Count).ToList();

        }
        else if (tracks.Count <= Muzzles.Count)
        {
            Trace("SimBeam multitarget " + tracks.Count);
        }
        else
        {
            tracks = CalcTargetList(tracks);
            Trace("SimBeam multitarget reduced from " + ValidTrackList.Count + " to " + tracks.Count);

        }

        int targetindex = 0;
        List<Muzzle> ActiveMuzzles;
        if(tracks[0].IsPointDefenseTarget)
        {
            Trace("SimBeam multitarget point defense");
            ActiveMuzzles = MultiTargetMuzzles;
        }
        else
        {
            Trace("SimBeam multitarget antiship defense");
            ActiveMuzzles = SingleTargetMuzzles;
        }
        for (int i = 0; i < Muzzles.Count; i++)
        {
            

            if (targetindex >= tracks.Count || !ActiveMuzzles.Contains(Muzzles[i]))
            {
                StopFire(i);
                continue;
            }
            if (Muzzles[i] is IRangedMuzzle rangedmuzzle && RespectMuzzleRange)
            {
                Vector3 toSig = transform.position.To(tracks.ElementAt(targetindex).TruePosition);
                if (toSig.magnitude > rangedmuzzle.MaxRange)
                {
                    StopFire(i);
                    continue;
                }
            }

            Trace("Firing Muzzle " + i);
            StartFire(i, tracks.ElementAt(targetindex), fire);

            targetindex++;

        }

        return Common.RunFunction;
    }

    private IHullPartRPC __baseRpcProvider;
    protected IHullPartRPC _baseRpcProvider
    {
        get
        {
            if (__baseRpcProvider == null)
            {
                __baseRpcProvider = base.gameObject.GetComponentInParent<IHullPartRPC>();
            }
            return __baseRpcProvider;
        }
    }
}




[HarmonyPatch(typeof(PointDefenseController), "TaskDirectWeapon")]
class PointDefenseControllerTaskDirectWeapon
{

    static void Postfix(PointDefenseController __instance, Object turret, NoAllocEnumerable<Object> targetList)
    {
        Common.LogPatch();
        if (turret == null) { return; }
        if (Common.GetVal<IWeapon>(turret, "Wep") is not WeaponComponent cwp) { return; }
        if (cwp.gameObject?.GetComponent<MultiTarget>() is not MultiTarget beamdata) { return; }
        //Common.SetVal(cwp, "_cooldownStyle", CooldownType.Proportional);
        List<Object> targetList2 = new List<Object>();
        targetList.ToList(targetList2);
        beamdata.AssignTracks(targetList2.ConvertAll(target => Common.GetVal<ITrack>(target, "Track")) ?? new());
    }
}
/*
[HarmonyPatch(typeof(TurretedContinuousWeaponComponent), "BearToTarget")]
class TurretedContinuousWeaponComponentBearToTarget
{
    static void Postfix(TurretedContinuousWeaponComponent __instance)
        //Debug.LogError(typeof(TurretedContinuousWeaponComponentBearToTarget));
        => MultiBeam.HandleBeam(__instance);
}

[HarmonyPatch(typeof(FixedContinuousWeaponComponent), "BearToTarget")]
class FixedContinuousWeaponComponentBearToTarget
{
    static void Postfix(FixedContinuousWeaponComponent __instance)
        //Debug.LogError(typeof(FixedContinuousWeaponComponentBearToTarget));
        => MultiBeam.HandleBeam(__instance);
}

[HarmonyPatch(typeof(ContinuousWeaponComponent), "StopFiring")]
class ContinuousWeaponComponentStopFiring
{
    static void Postfix(FixedContinuousWeaponComponent __instance)
        //Debug.LogError(typeof(ContinuousWeaponComponentStopFiring));
        => MultiBeam.HandleBeam(__instance);//Time.timeScale = 1f;
}

[HarmonyPatch(typeof(ContinuousWeaponComponent), "StartFiring")]
class ContinuousWeaponComponentStartFiring
{
    static void Postfix(ContinuousWeaponComponent __instance)
        //Debug.LogError(typeof(ContinuousWeaponComponentStartFiring));
        => MultiBeam.HandleBeam(__instance);
}
*/

/*
if (fireControlSensors.Count > muzzle)
{
    if (muzzle >= 1)
    {
        fireControlSensors[muzzle].SetLockedTarget(target.ID);
        fireControlSensors[muzzle].AcquireContacts(transform);

    }

    fireControlSensors[muzzle].UpdateTrack(target.Trackable, out pos, out Vector3 velocity);
}
else
    pos = ((target.KnownPosition * KnownPositionWeight) + (target.TruePosition * TruePostionWeight)) / (TruePostionWeight + KnownPositionWeight);
*/