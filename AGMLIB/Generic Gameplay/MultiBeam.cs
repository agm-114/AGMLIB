using FleetEditor;
using Game.Units;
using HarmonyLib;
using Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using Game.Sensors;
using Munitions;
using Ships.Controls;
using Utility;
using System.Reflection;
using Object = System.Object;
using static Testing.CameraGun;
using UnityEngine.UI.Extensions;
using static Ships.WeaponComponent;
using System.ComponentModel;
using Game;
using static UnityEngine.Rendering.HighDefinition.CameraSettings;
using static Ships.ContinuousWeaponComponent;

public class MultiBeam : MonoBehaviour
{
    
    public int TruePostionWeight = 10;
    public int KnownPositionWeight = 1;
    public bool SingleTargetMode = false;
    [HideInInspector]
    public IEnumerable<ITrack> AssignedTracks = new List<ITrack>();
    public IEnumerable<ITrack> Validtracklist => AssignedTracks.Where(track => track.IsValid && Weapon.CanTrainOnTarget(track.TruePosition) && Vector3.Distance(transform.position, track.TruePosition) <= Weapon.MaxEffectiveRange);
    public List<ITrack> Sortedtracklist => Validtracklist.Prepend(Weapon.CurrentlyTargetedTrack()).ToList();
    public ContinuousWeaponComponent Weapon => this.gameObject.GetComponent<ContinuousWeaponComponent>();
    bool Firing => Common.GetVal<bool>(Weapon, "_muzzlesActive");
    List<Muzzle> Muzzles => Common.GetVal<Muzzle[]>(Weapon, "_muzzles").ToList();
    List<ITrack> AvoidTracks => Root
                .GetComponentsInChildren<MultiBeam>()
                .Except(new List<MultiBeam>() { this })
                .SelectMany(a => a?.Sortedtracklist ?? new())
                .GroupBy(a => a)
                .OrderByDescending(a => a?.Count() ?? 0)
                .ConvertAll(a => a.Key);
    public List<ITrack> CachedAvoidTracks = null;
    public Transform Root => gameObject.transform.parent.parent;

    public static void HandleBeam(ContinuousWeaponComponent weapon, bool recalculate = false)
    {
        MultiBeam beamdata = weapon?.gameObject?.GetComponent<MultiBeam>();
        beamdata?.SimBeam(recalculate);
    }

    public void StartFire(int muzzle, ITrack target)
    {
        if (!(target?.IsValid ?? false))
            return;
        Vector3 pos = ((target.KnownPosition * KnownPositionWeight) + (target.TruePosition * TruePostionWeight)) / (TruePostionWeight + KnownPositionWeight);
        //  Time.timeScale = 0.1f;
        Muzzles[muzzle].transform.rotation = Quaternion.LookRotation(pos - Muzzles[muzzle].transform.position);
        Muzzles[muzzle].FireEffect();

    }

    public void StopFire(int muzzle)
    {
        IWeaponComponentRPC _weaponRpcProvider = Common.GetVal<IWeaponComponentRPC>(Weapon, "_weaponRpcProvider");
        Muzzles[muzzle].gameObject.transform.localRotation = Quaternion.identity;
        if (muzzle == 0)
            return;
        Muzzles[muzzle]?.StopFire();
        Muzzles[muzzle]?.StopFireEffect();
        _weaponRpcProvider.RpcStopFiringEffect(Weapon.RpcKey, muzzle);

    }
    public List<ITrack> CalcTargetList()
    {

        List<ITrack> workingtracks = new(Sortedtracklist);
;
        if (workingtracks.Count > Muzzles.Count)
        {

            CachedAvoidTracks ??= new(AvoidTracks);
            List<ITrack> tracks = new(CachedAvoidTracks);
            while (workingtracks.Count > Muzzles.Count && tracks.Count > 0)
            {
                workingtracks.Remove(tracks[0]);
                tracks.Remove(tracks[0]);
            }
        }
        return workingtracks;
    }

    public void SimBeam(bool recalculate = false)
    {
        if (Weapon.TargetAssignedByPlayer || !this.Firing)
        {
            Muzzles[0].gameObject.transform.localRotation = Quaternion.identity;
            for (int i = 1; i < Muzzles.Count; i++)
                StopFire(i);
            return;
        }
        //List<FireControlSensor> fireControlSensors = _weapon.GetComponentsInChildren<FireControlSensor>().ToList(); //____muzzles.ToList();
        List<ITrack> tracks = Enumerable.Repeat(Weapon.CurrentlyTargetedTrack(), Muzzles.Count()).ToList();
        if (!SingleTargetMode)
            tracks = CalcTargetList();

        //Debug.LogError("Tracklist reduced from " + Sortedtracklist.Count + " to " + Targetedtracks.Count);
        for (int i = 0; i < Muzzles.Count; i++)
        {
            if(i < tracks.Count)
                StartFire(i, tracks.ElementAt(i));
            else
                StopFire(i);
        }
    }
}

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

[HarmonyPatch(typeof(PointDefenseController), "TaskDirectWeapon")]
class PointDefenseControllerTaskDirectWeapon
{

    static void Postfix(PointDefenseController __instance, Object turret, IEnumerable<Object> targetList)
    {

        if (turret == null) { Debug.LogError("Null turret "); }
        if (Common.GetVal<IWeapon>(turret, "Wep") is ContinuousWeaponComponent  cwp)
        {
            MultiBeam beamdata = cwp?.gameObject?.GetComponent<MultiBeam>();
            if (beamdata == null) { return;  }
            Common.SetVal(cwp, "_cooldownStyle", CooldownType.Proportional);
            beamdata.AssignedTracks = targetList.ConvertAll(target => Common.GetVal<ITrack>(target, "Track")) ?? new();
            beamdata.CachedAvoidTracks = null ;
        }
    }
}