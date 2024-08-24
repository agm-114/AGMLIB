using Munitions.ModularMissiles;
using Munitions;
using Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using UnityEngine;
using Bundles;
using Game.Units;
using HarmonyLib;
using Ships.Serialization;
using static Ships.BaseCellLauncherComponent;
using static Ships.BulkMagazineComponent;
using Game.Sensors;
using System.Collections;
using System.CodeDom.Compiler;
using System.Net.NetworkInformation;
using Sound;

public class KinematicLauncher : ActiveSettings
{

    public float SpawnVelocity => 0;
    public float ReleaseVelocity = 500;
    public float EndVelocity = 0;
    public float PostReleaseBoostTime = 5;
    public DynamicVisible DynamicVisible;
    public GameObject ParticleEffect;
    public BaseSoundEffect Sound;
    public float Delay = 0;

    //private Rigidbody Body => Obj?.gameObject?.GetComponent<Rigidbody>();

    public void PlayFiringEffect(MissileEjector ejector = null) => StartCoroutine(CoroutineDelayedPlayParticles(ejector.transform));
    private IEnumerator CoroutineDelayedPlayParticles(Transform target)
    {
        yield return new WaitForSeconds(Delay);

        if(active && (DynamicVisible?.IsVisible ?? true))
        {
            if (Sound != null)
            {
                GlobalSFX.PlayOneShotSpatial(Sound, base.transform);
            }
            if (ParticleEffect != null)
            {
                //Debug.LogError("Spawn Particle Effect");
                ObjectPooler.Instance.GetNextOrNew(ParticleEffect, Vector3.zero, Quaternion.identity, base.transform, localCoordinates: true);
            }
        }
        else
        {
            //Debug.LogError("Null Particle Effect");
        } 
    }
    public void Launch(NetworkPoolable obj = null)
    {
        if (obj != null && active)
        {
            //Debug.LogError("Step 4 Missile Setup");
            KinematicMissile missile = obj?.gameObject?.AddComponent<KinematicMissile>()?.Setup(this);
        }
    }
}

public interface IMissileFixedUpdate
{
    public void Start();
    public void FixedUpdate();
}
public class MissileLaunchKinematics : LaunchKinematics
{
    private Missile _missile; // field
    public Missile Missile   // property
    {
        get {
            if (_missile == null)
                _missile = gameObject.GetComponent<Missile>();
            return _missile; 
        }   // get method
    }
    public bool HotLaunch = true;
    public override void FixedUpdate()
    {
        if(Missile?.HotLaunch == HotLaunch) 
            base.FixedUpdate();
    }

}

public class LaunchKinematics : MonoBehaviour, IMissileFixedUpdate
{
    public ClampMode Mode = ClampMode.Ceil;

    public AnimationCurve VelocityCurve = new(new Keyframe(0f, 1f), new Keyframe(10f, 100f));
    public float Velocity => VelocityCurve.Evaluate(Age);
    public float EffectDuration = 0;
    private bool _fixedupdate = false;
    private Rigidbody Body => gameObject.GetComponent<Rigidbody>();
    private float _startTime = 0;
    private float Age => Time.fixedTime - _startTime;
    public void Start() => _startTime = Time.fixedTime;
    public enum ClampMode
    {
        Ceil,
        Floor
    }
    virtual public void FixedUpdate()
    {

        if (Age > EffectDuration)
        {
            return;
        }

        if (Mode == ClampMode.Floor && Body.velocity.magnitude < Velocity)
        {
            Body.velocity = Body.velocity.normalized * Velocity;
        }
        if (Mode == ClampMode.Ceil && Body.velocity.magnitude > Velocity)
        {
            Body.velocity = Body.velocity.normalized * Velocity;
        }
        _fixedupdate = false;
    }

}

public class KinematicMissile : MonoBehaviour, IMissileFixedUpdate
{
    public KinematicLauncher Launcher = new();
    public float StartVelocity => Launcher?.ReleaseVelocity ?? 0;
    public float EndVelocity => Launcher?.EndVelocity ?? 0;
    public float Velocity => Mathf.Lerp(StartVelocity, EndVelocity, Age/TimeToLive);
    public float TimeToLive => Launcher?.PostReleaseBoostTime ?? 0;
    private bool _fixedupdate = false;
    private Rigidbody Body => gameObject.GetComponent<Rigidbody>();
    private float _startTime = 0;
    private float Age =>  Time.fixedTime - _startTime;
    public void Start() => _startTime = Time.fixedTime;
    public KinematicMissile Setup(KinematicLauncher launcher)
    {
        Start();
        if(launcher != null)
            Launcher = launcher;
        return this;
    }
    public void FixedUpdate()
    {

        if (Age > TimeToLive)
        {
            //Debug.LogError("Destroy");
            Destroy(this);
            return;
        }

        if ( Body.velocity.magnitude < Velocity)
        {
            //Debug.LogError("Update      " + Age / TimeToLive + " tV " + Velocity + " rV " + Body.velocity.magnitude);
            Body.velocity = Body.velocity.normalized * Velocity;
            //Debug.LogError("Post Update " + Age / TimeToLive + " tV " + Velocity + " rV " + Body.velocity.magnitude);
        }
        else
        {
            //Debug.LogError("No Update " + Age / TimeToLive + " tV " + Velocity + "rV" + Body.velocity.magnitude);
        }

        _fixedupdate = false;
    }
 
}

[HarmonyPatch(typeof(MissileEjector), nameof(MissileEjector.FireEffect))]
class MissileEjectorFireEffect
{

    public static void Postfix(MissileEjector __instance)
    {
        //Debug.LogError("1 MissileInstantiated Postfix");
        List<KinematicLauncher> launchers = __instance.gameObject.GetComponentsInParent<KinematicLauncher>()?.ToList() ?? new(); //??
                                                                                                                                 //__instance.gameObject.AddComponent<KinematicLauncher>();

        foreach (KinematicLauncher launcher in launchers)
        {
            launcher?.PlayFiringEffect(__instance);

        }

    }
}


[HarmonyPatch(typeof(MissileEjector), "MissileInstantiated")]
class MissileEjectorMissileInstantiated
{
    public static void Prefix(MissileEjector __instance, NetworkPoolable obj, IMissile missile)
    {
        //Debug.LogError("1 MissileInstantiated Prefix");

        if (obj == null)
            Debug.LogError("obj null");
        if (missile == null)
            Debug.LogError("missile null");
    }
    public static void Postfix(MissileEjector __instance, NetworkPoolable obj, IMissile missile)
    {
        //Debug.LogError("1 MissileInstantiated Postfix");

        List<KinematicLauncher> launchers = __instance.gameObject.GetComponentsInParent<KinematicLauncher>()?.ToList() ?? new(); //??
          //__instance.gameObject.AddComponent<KinematicLauncher>();

        foreach(KinematicLauncher launcher in launchers)
        {
            launcher?.Launch(obj);

            //kmissile.Launcher = launcher;
            //Debug.LogError("2 Launcher Detected");
        }
        obj.gameObject.GetComponents<IMissileFixedUpdate>()?.ToList()?.ForEach(f => f?.FixedUpdate());
        //else
        //    Debug.LogError("Launcher Not Found");
    }
}

[HarmonyPatch(typeof(Missile), "FixedUpdate")]
class MissileFixedUpate
{
    public static void Postfix(Missile __instance) => __instance?.GetComponents<IMissileFixedUpdate>()?.ToList()?.ForEach(a => a?.FixedUpdate());
}

[HarmonyPatch(typeof(Missile), "Thrust")]
class MissileThrust
{
    public static void Postfix(Missile __instance) => __instance?.GetComponents<IMissileFixedUpdate>()?.ToList()?.ForEach(a => a?.FixedUpdate());
}



/*

[HarmonyPatch(typeof(MissileEjector), "LaunchMissile")]
class MissileEjectorLaunchMissile
{
    public static void PostFix(MissileEjector __instance)
    {
        //Debug.LogError("3 Launch");

    }


}

    public static void Prefix(MissileEjector __instance, IMissile missile, List<Vector3> path, ITrack track, int salvoId, Vector3? doglegPoint, bool playerOrder, Action<IMissile> callback)
    {
        return;
        MissileEjector ejector = __instance;
        KinematicLauncher launcher = ejector.gameObject.GetComponentInChildren<KinematicLauncher>() ??
          ejector.gameObject.AddComponent<KinematicLauncher>();
        //if (launcher == null)
        //    return true;

        //Debug.LogError("LaunchMissile Postfix");

        launcher.StartLaunch(ejector, missile, path, track, salvoId, doglegPoint, playerOrder, callback);

        //return false;
    }
*/


/*
[HarmonyPatch(typeof(MissileEjector), nameof(MissileEjector.Fire), new Type[] { typeof(IMissile), typeof(List<Vector3>), typeof(int), typeof(bool), typeof(Action<IMissile>) })
]
class MissileEjectorFire1
{
    public bool Prefix(MissileEjector __instance, IMissile missile, List<Vector3> path, int salvoId, bool playerOrder, Action<IMissile> callback)
    {
        MissileEjector ejector = __instance;
        KinematicLauncher launcher = ejector.gameObject.GetComponentInChildren<KinematicLauncher>() ??
          ejector.gameObject.AddComponent<KinematicLauncher>();
        if (launcher == null)
            return true;

        //Debug.LogError("LaunchMissile Postfix");
        launcher.StartLaunch(ejector, missile, path, null, salvoId, null, playerOrder, callback);
        return false;    
    }
}*/
/*
[HarmonyPatch(typeof(MissileEjector), nameof(MissileEjector.Fire), new Type[] { typeof(IMissile), typeof(ITrack), typeof(int), typeof(Vector3?), typeof(bool), typeof(Action<IMissile>) })
]
class MissileEjectorFire2
{
    public bool Prefix(MissileEjector __instance, IMissile missile, ITrack track, int salvoId, Vector3? doglegPoint, bool playerOrder, Action<IMissile> callback)
    {
        MissileEjector ejector = __instance;
        KinematicLauncher launcher = ejector.gameObject.GetComponentInChildren<KinematicLauncher>() ??
          ejector.gameObject.AddComponent<KinematicLauncher>();
        if (launcher == null)
            return true;

        //Debug.LogError("LaunchMissile Postfix");
        launcher.StartLaunch(ejector, missile, null, track, salvoId, doglegPoint, playerOrder, callback);
        return false;
    }
}
*/

/*
 * 
 *     public bool StartLaunch(MissileEjector ejector, IMissile missile, List<Vector3> path, ITrack track, int salvoId, Vector3? doglegPoint, bool playerOrder, Action<IMissile> callback)
{
    _ejector = ejector;
    StartCoroutine(LaunchMissile(missile, path, track, salvoId, doglegPoint, playerOrder, callback));
    return true;
}
private IEnumerator LaunchMissile(IMissile missile, List<Vector3> path, ITrack track, int salvoId, Vector3? doglegPoint, bool playerOrder, Action<IMissile> callback)
{
    ShipController _myShip = Common.GetVal<ShipController>(_ejector, "_myShip");
    Vector3 _ejectionDirection = Common.GetVal<Vector3>(_ejector, "_ejectionDirection");
    float _ejectionVelocity = Common.GetVal<float>(_ejector, "_ejectionVelocity");
    float _launchDelay = 5; Common.GetVal<float>(_ejector, "_launchDelay");
    float _ignitionDelay = 5; Common.GetVal<float>(_ejector, "_ignitionDelay");
    float _pathDeviation = Common.GetVal<float>(_ejector, "_pathDeviation");


    NetworkPoolable missileObj = missile.InstantiateSelf(_ejector.transform.position, _ejector.transform.rotation, Vector3.zero);
    Rigidbody body = missileObj.GetComponent<Rigidbody>();
    //if (body == null)
        //Debug.LogError("No rigidbody on missile, cannot launch");
    IMissile basicMissile = missileObj.GetComponent<IMissile>();
    KinematicLauncher launcher = _ejector.gameObject.GetComponentInChildren<KinematicLauncher>();

    missile.Imbue(_myShip);
    missile.Imbue(_myShip.NetID);
    body.isKinematic = true;
    for (float time = 0f; time < _ignitionDelay; time += Time.fixedDeltaTime)
    {
        missileObj.transform.position += _ejector.transform.rotation * _ejectionDirection * Mathf.Lerp(launcher.SpawnVelocity, launcher.ReleaseVelocity, time / _ignitionDelay) * Time.fixedDeltaTime;
        yield return new WaitForFixedUpdate();
    }
    body.isKinematic = false;
    missileObj.gameObject.AddComponent<KinematicMissile>().Launcher = launcher;
    body.velocity = _myShip.Velocity + _ejector.transform.rotation * _ejectionDirection * launcher.ReleaseVelocity;

    IProgrammableMissile control = missileObj.GetComponent<IProgrammableMissile>();
    if (control == null)
    {
        //NOOP
    }
    else if (path != null)
    {
        List<Vector3> newPath = new List<Vector3> { _myShip.Position };
        newPath.AddRange(path.ConvertAll((Vector3 v) => v += UnityEngine.Random.insideUnitSphere * _pathDeviation));
        control.SetProgramPath(_myShip.NetID, newPath);
    }
    else if (track != null)
    {
        control.SetProgramTrack(_myShip.NetID, track.ID, track.TruePosition, doglegPoint.HasValue, doglegPoint.HasValue ? doglegPoint.Value : Vector3.zero);
    }

    if (basicMissile != null)
        callback?.Invoke(basicMissile);
    basicMissile?.Launch(_myShip.NetID, salvoId, playerOrder);

}
    */