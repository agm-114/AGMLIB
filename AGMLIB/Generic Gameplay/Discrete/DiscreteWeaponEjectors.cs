using FleetEditor;
using Ships.Controls;
using Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Munitions;
using Game.Sensors;
using static HarmonyLib.Code;
using System.Reflection;
using Utility;
using Game.Units;
using Game.EWar;
using HarmonyLib;
using UnityEngine.Profiling;

public class DiscreteWeaponEjectors : MonoBehaviour
{

    [SerializeField]
    public float BaseAccuracy = 0.01f;

    [ShipStat("muzzle-accuracy", "Spread", "MoA", InitializeFrom = "_baseAccuracy", TextValueMultiplier = 3437.75f, PositiveBad = true)]
    protected StatValue _statAccuracy;
    public List<MissileEjector> Cells = new();
    public AmmoFilter AmmoCompatiblity;
    public DiscreteWeaponComponent DiscreteWeaponComponent;
    public float TimeBetweenCells = 0.2f;


    private int _index = 0;
    private bool _waitingForCell;
    private float _cellAccum;
    private bool _weaponprecharge = true;
    protected IMagazine _ammoSource => Common.GetVal<IMagazine>(DiscreteWeaponComponent, "_ammoSource");
    protected int _magazineSize => Common.GetVal<int>(DiscreteWeaponComponent, "_magazineSize");
    protected bool _reloading => Common.GetVal<bool>(DiscreteWeaponComponent, "_reloading");

    protected bool _waitingForMuzzle => Common.GetVal<bool>(DiscreteWeaponComponent, "_waitingForMuzzle");
    protected int _magazineFired => Common.GetVal<int>(DiscreteWeaponComponent, "_magazineFired");
    BaseHull _myHull => Common.GetVal<BaseHull>(DiscreteWeaponComponent, "_myHull");

    protected ShipController _controller => Common.GetVal<ShipController>(DiscreteWeaponComponent, "_controller");

    protected void Start()
    {
            
        if(DiscreteWeaponComponent == null)
            DiscreteWeaponComponent = gameObject.GetComponentInChildren<DiscreteWeaponComponent>();
    }

    public bool SkipPatch()
    {
        if (DiscreteWeaponComponent.SelectedAmmoType is not IMissile missile)
            return true;
        return false;
    }

    //Need to patch
    public void OnTarget(Vector3 aimPoint)
    {
        if (DiscreteWeaponComponent.CurrentTargetingMode == 0 || !DiscreteWeaponComponent.CanFire || _reloading || _waitingForMuzzle)
            return;
        if (DiscreteWeaponComponent.SelectedAmmoType is not IMissile missile)
            return;

        //Debug.LogError(_cellAccum);
        Common.SetVal<int>(DiscreteWeaponComponent, "_magazineFired", _magazineFired + 1);

        if (_magazineFired >= _magazineSize)
            DiscreteWeaponComponent.ReloadIfNeeded();
        else if (!_reloading)
        {
            Common.SetVal<bool>(DiscreteWeaponComponent, "_waitingForMuzzle", true);

            Common.SetVal<float>(DiscreteWeaponComponent, "_muzzleAccum", 0);
            //_waitingForCell = true;
            //_cellAccum = 0;
        }

        FireTrack(missile, DiscreteWeaponComponent.CurrentlyTargetedTrack(), 0, DiscreteWeaponComponent.TargetAssignedByPlayer, aimPoint);

    }

    public void FireTrack(IMissile missile, ITrack track, int salvoId, bool playerOrder, Vector3? aimPoint = null)
    {

        //Common.SetVal(Cells[_index], "_coldLaunchEffect", null);
        //Common.SetVal(Cells[_index], "_hotLaunchEffect", null);

        //Debug.LogError("Meme moment");
        //this.gameObject.AddComponent<DynamicStun>();

        //return;
        if(missile == null) return; 

        if (aimPoint != null)
        {

            Vector3 shotDirection = MathHelpers.RandomRayInCone(Cells[_index].transform.position.To(aimPoint.Value).normalized, 0);//_statAccuracy.Value
            Cells[_index].transform.rotation = Quaternion.LookRotation(shotDirection);

        }
        Cells[_index].SetShip(this._controller);
        if (this._controller == null)
            Debug.LogError("null controller");
        Cells[_index].Fire(missile, track, salvoId, null, playerOrder, null);
        Cells[_index].FireEffect(true);
        _myHull.MyShip.AmmoFeed.GetAmmoSource(DiscreteWeaponComponent.SelectedAmmoType).Withdraw(1);
        //_ammoSource.Withdraw(1);
        _index++;
        if (_index >= Cells.Count) { _index = 0; }

    }


    protected void RunTimers(float deltaTime)
    {


        /*
        if (_waitingForCell)
        {
            _cellAccum += Time.fixedDeltaTime;
            if (_cellAccum > TimeBetweenCells)
            {
                _cellAccum = 0f;
                _waitingForCell = false;
            }
        }
        if (_reloading && _weaponprecharge)
        {
            _weaponprecharge = false;
            _reloadAccum = 10000;
            _reloading = false;
            _magazineFired = 0;
        }*/
    }
}


[HarmonyPatch(typeof(FixedDiscreteWeaponComponent), "OnTarget")]
class FixedDiscreteWeaponComponentOnTarget
{
    static bool Prefix(FixedDiscreteWeaponComponent __instance, Vector3 aimPoint)
    {
        DiscreteWeaponEjectors ejectors = __instance?.GetComponentInChildren< DiscreteWeaponEjectors>();
        if (ejectors == null || ejectors.SkipPatch())
            return true;//true

        ejectors.OnTarget(aimPoint);
        return false;//false
        //Common.SetVal(__instance, "Mode", TrackingMode.Ping);
    }
}
[HarmonyPatch(typeof(DiscreteWeaponComponent), "OnTarget")]
class DiscreteWeaponComponentOnTarget
{
    static bool Prefix(DiscreteWeaponComponent __instance, Vector3 aimPoint)
    {
        DiscreteWeaponEjectors ejectors = __instance?.GetComponentInChildren<DiscreteWeaponEjectors>();
        if (ejectors == null || ejectors.SkipPatch())
            return true;

        ejectors.OnTarget(aimPoint);
        return false;
        //Common.SetVal(__instance, "Mode", TrackingMode.Ping);
    }
}
