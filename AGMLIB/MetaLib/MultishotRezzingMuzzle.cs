// Nebulous, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// Ships.RezzingMuzzle
using Munitions;
using Ships;
using System;
using UnityEngine;
using Utility;

public class MultishotRezzingMuzzle : RezzingMuzzle
{
    [Tooltip("The amount of shots that will come from the muzzle when it is fired. This applies every time the muzzle is fired.")]
    [SerializeField]
    private int shots = 1;

    public override MunitionSimulationMethod SimMethod => MunitionSimulationMethod.Spawned;

    new public event Action<NetworkPoolable> OnFired;

    public override void Fire()
    {
        Fire(base.transform.forward);
    }

    new public void Fire(Vector3 shotDirection)
    {
        if (base._ammoSource != null && base._ammoSource.AmmoType != null)
        {
            for (int i = 0; i < shots; i++)
            {
                base._ammoSource.Withdraw(1u);
                base._reportTo?.ReportFired(1);
                shotDirection = MathHelpers.RandomRayInCone(shotDirection, base._accuracy);
                NetworkPoolable round = base._ammoSource.AmmoType.InstantiateSelf(base.transform.position, false ? Quaternion.LookRotation(shotDirection) : Quaternion.identity, shotDirection * base._ammoSource.AmmoType.FlightSpeed);
                if (round is ILocalImbued imbued)
                {
                    imbued.ImbueLocal(base._weapon.Platform);
                    imbued.SetWeaponReportPath(base._reportTo);
                }
                this.OnFired?.Invoke(round);
            }
        }
    }
}
