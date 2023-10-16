using Munitions;
using Ships;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

[ExecuteInEditMode]
public class AdvancedRezzingMuzzle : RezzingMuzzle
{
    [Serializable]
    public struct AccuracyModifer
    {
        public ShellMunition ammo;
        public float accuracy;
    }
    public AccuracyModifer[] AccuracyModifers;

    // Start is called before the first frame update
    [HideInInspector]
    [SerializeField]
    //private List<ShellMunition> ammo;
    //[HideInInspector]
    //[SerializeField]
    //private List<float> accuracy;

    public override void Fire()
    {
        if (base._ammoSource != null && base._ammoSource.AmmoType != null)
        {
            //IMunition test = base._ammoSource.AmmoType.
        }
        base.Fire(base.transform.forward);
    }


    // Update is called once per frame
    void Update()
    {
        #if (UNITY_EDITOR)
        ammo = new List<ShellMunition>();
        accuracy = new List<float>();
        foreach (AccuracyModifer mod in AccuracyModifers)
        {
            ammo.Add(mod.ammo);
            accuracy.Add(mod.accuracy);
        }
        #endif
    }
}
