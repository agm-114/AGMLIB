
// Nebulous, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// Munitions.MissileImpactWarhead
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Bundles;
using Game;
using Game.Reports;
using Munitions;
using Ships;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;
using Random = UnityEngine.Random;

public class SimpleMagazine : MonoBehaviour, IMagazine
{
    //[SerializeField]

    //public Dictionary<LightweightMunitionBase, int> _weightedammo;

    [SerializeField]
    protected List<LightweightMunitionBase> _ammotypes = new(1);
    [SerializeField]
    protected List<float> _weights = new(1);

    IMunition IMagazine.AmmoType
    {

        get
        {
            float randomNumber = Random.Range(0f, _weights.Sum());

            for (int i = 0; i < _ammotypes.Count; i++)
                if (randomNumber < _weights[i])
                    return _ammotypes[i];
                else
                    randomNumber -= _weights[i];

            return BundleManager.Instance.GetMunition("Stock/120mm HE Shell");
        }
    }

    int IMagazine.QuantityNotReserved => throw new NotImplementedException();

    int IMagazine.QuantityAvailableNotReserved => throw new NotImplementedException();

    string IQuantityHolder.HeldName => throw new NotImplementedException();

    int IQuantityHolder.Quantity => throw new NotImplementedException();

    int IQuantityHolder.QuantityAvailable => throw new NotImplementedException();

    int IQuantityHolder.PeakQuantity => throw new NotImplementedException();

    float IQuantityHolder.PercentageAvailable => throw new NotImplementedException();

    event QuantityChanged IQuantityHolder.OnQuantityChanged
    {
        add => throw new NotImplementedException();

        remove => throw new NotImplementedException();
    }

    int IMagazine.Reserve(uint quantity) => throw new NotImplementedException();

    void IMagazine.UnReserve(uint quantity) => throw new NotImplementedException();

    int IMagazine.UpperQuantityBound() => throw new NotImplementedException();

    int IMagazine.Withdraw(uint quantity) => (int)quantity;
}
