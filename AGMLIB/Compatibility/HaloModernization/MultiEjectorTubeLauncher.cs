using System.Collections;
using System.Collections.Generic;
using Game.Sensors;
using Game.Units;
using Munitions;
using Ships;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

namespace AGMLIB.Compatibility.HaloModernization;

/// <summary>
/// A current-API tube launcher that rotates ordinary native launch operations
/// across several ejectors. BaseTubeLauncherComponent retains programming,
/// reload, resource, report, and RPC ownership.
/// </summary>
public class MultiEjectorTubeLauncher : BaseTubeLauncherComponent
{
    [SerializeField]
    private MissileEjector[] _ejectors = [];

    [SerializeField]
    private VisualEffect _activeVisualEffect;

    [SerializeField]
    [FormerlySerializedAs("_resetMuzzleIndexOnReload")]
    private bool _resetEjectorOnReload = true;

    [SerializeField]
    [FormerlySerializedAs("_withdrawPerShot")]
    [Min(1)]
    private uint _withdrawPerLaunch = 1;

    private Coroutine _burstCoroutine;
    private int _nextEjector;

    public override void SetShipIdentity(ShipController ship)
    {
        base.SetShipIdentity(ship);
        foreach (MissileEjector ejector in _ejectors)
            ejector?.SetShip(ship);
    }

    protected override bool CheckCanFire()
    {
        return _burstCoroutine == null && base.CheckCanFire();
    }

    protected override IEnumerator EjectMissilePath(
        IMagazine magazine,
        List<Vector3> path,
        int salvoId,
        bool playerOrder
    )
    {
        if (isActiveAndEnabled && _burstCoroutine == null)
        {
            _burstCoroutine = StartCoroutine(
                CoroutineEjectBurst(magazine, salvoId, playerOrder, path, null, null)
            );
        }
        yield break;
    }

    protected override IEnumerator EjectMissileTrack(
        IMagazine magazine,
        ITrack track,
        int salvoId,
        Vector3? doglegPoint,
        bool playerOrder
    )
    {
        if (isActiveAndEnabled && _burstCoroutine == null)
        {
            _burstCoroutine = StartCoroutine(
                CoroutineEjectBurst(
                    magazine,
                    salvoId,
                    playerOrder,
                    null,
                    track,
                    doglegPoint
                )
            );
        }
        yield break;
    }

    protected override void OnAmmoSourceChangedInternal(IMunition ammo)
    {
        base.OnAmmoSourceChangedInternal(ammo);
        CancelBurst();
    }

    protected override void OnDestroy()
    {
        CancelBurst();
        base.OnDestroy();
    }

    public override void CallRpcFireTubeEffect(int tube, bool hot)
    {
        if (tube >= 0 && tube < _ejectors.Length)
            _ejectors[tube]?.FireEffect(hot);
    }

    protected override void PartFunctionalChangedInternal(bool newFunctional)
    {
        base.PartFunctionalChangedInternal(newFunctional);
        if (_activeVisualEffect == null)
            return;

        if (newFunctional)
            _activeVisualEffect.Play();
        else
            _activeVisualEffect.Stop();
    }

    private IEnumerator CoroutineEjectBurst(
        IMagazine magazine,
        int salvoId,
        bool playerOrder,
        List<Vector3> path,
        ITrack track,
        Vector3? doglegPoint
    )
    {
        if (_ejectors == null || _ejectors.Length == 0 || magazine == null)
        {
            _burstCoroutine = null;
            yield break;
        }

        ReloadIfNeeded();
        if (_resetEjectorOnReload)
            _nextEjector = 0;

        uint withdrawAmount = _withdrawPerLaunch == 0 ? 1u : _withdrawPerLaunch;
        for (int launch = 0; launch < _launchesPerLoad; launch++)
        {
            while (
                isActiveAndEnabled
                && (_isCyclingNextShot || CheckReloading() || !CheckLaunchAreaClear())
            )
            {
                yield return new WaitForFixedUpdate();
            }

            if (!isActiveAndEnabled || magazine.AmmoType is not IMissile missile)
                break;
            if ((uint)Mathf.Max(0, magazine.QuantityAvailable) < withdrawAmount)
                break;

            int ejectorIndex = Mathf.Clamp(_nextEjector, 0, _ejectors.Length - 1);
            MissileEjector ejector = _ejectors[ejectorIndex];
            if (ejector == null)
                break;
            if (magazine.Withdraw(withdrawAmount) < withdrawAmount)
                break;

            if (path != null)
                ejector.Fire(missile, path, salvoId, playerOrder, null);
            else
                ejector.Fire(missile, track, salvoId, doglegPoint, playerOrder, null);

            _launcherRpcProvider.RpcFireTubeEffect(
                RpcKey,
                ejectorIndex,
                missile.HotLaunch
            );
            _nextEjector = (ejectorIndex + 1) % _ejectors.Length;
            Cycle();
        }

        ReloadIfNeeded();
        _burstCoroutine = null;
    }

    private void CancelBurst()
    {
        if (_burstCoroutine == null)
            return;

        StopCoroutine(_burstCoroutine);
        _burstCoroutine = null;
    }
}
