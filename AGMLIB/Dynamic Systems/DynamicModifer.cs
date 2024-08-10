using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using Bundles;
using Utility;
using Ships;
using Modding;
using Munitions;
using Game.Units;
//using ICSharpCode.NRefactory.Ast;
using static Game.Sensors.SensorTrackableObject.SavedSTOState;
using static UnityEngine.ParticleSystem;
using System.Linq;
//using Game.Ships;
public class DynamicModifer : ActiveSettings
{
    //public float launcherindex = 0;
    [Space]
    [Header("Modifier Settings")]
    [Space]
    public StatModifier[] Modifiers;

    public StatModifier[] TargetModifiers
    {
        get
        {
            //Debug.LogError(Mode);
            if (Mode == ModifierMode.Module)
                return Module.Modifiers;
            else if (Mode == ModifierMode.Hull)
                return Hull.BaseModifiers;
            return null;
        }
        set
        {
            if (Mode == ModifierMode.Module)
                Module.Modifiers = value;
            else if (Mode == ModifierMode.Hull)
                Hull.BaseModifiers = value;
        }
    }
    public ModifierMode Mode = ModifierMode.Module;
    public enum ModifierMode
    {
        Module,
        Hull,
        Disabled

    }

    private void ModFromSource(IModifierSource source)
    {
        foreach (StatModifier modifier in Modifiers)
        {
            base.Ship.AddStatModifier(source, modifier);
        }
    }
    // Update is called once per frame

    protected override void FixedUpdate()
    {
        //time += Time.deltaTime;
        base.FixedUpdate();
        if (!base.active)
        {
            //Debug.LogError("Not Active");
            //Debug.LogError("Not Active " + gameObject.name + " " + Modifiers[0].ToString());

            return;
        }  
        else
        {
            //Debug.LogError("Active " + gameObject.name + " " + Modifiers[0].ToString());
        }
        bool changes = false;

        if(TargetModifiers.Length < Modifiers.Length)
        {
            //Debug.LogError("Possible Dynamic Modifer Misconfiguration");
        }

        //Debug.LogError(TargetModifiers.Length);

        foreach (StatModifier Modifier in Modifiers)
        {
            int i = Array.FindIndex(TargetModifiers, TargetModifier => TargetModifier.StatName == Modifier.StatName);
            if (i == -1)
            {
                //Debug.LogError("Adding " + Modifier.StatName);

                TargetModifiers = TargetModifiers.Append(new StatModifier(Modifier.StatName, 0, 0)).ToArray();
            }
            else if (TargetModifiers[i].Modifier != Modifier.Modifier || TargetModifiers[i].Literal != Modifier.Literal)
            {
                //Debug.LogError("Updating " + Modifier.StatName + "  values at index[" + i + "] from" + TargetModifiers[i].Modifier + " to " + Modifier.Modifier);
                TargetModifiers[i] = Modifier;//new StatModifier(@override.Modifiers[j].StatName, 0, @override.Modifiers[j].Modifier);
                changes = true;
            }
        }

        if (changes)
        {
            if( Mode == ModifierMode.Module)
                ModFromSource(Module);
            else if( Mode == ModifierMode.Hull)
                ModFromSource(Hull);
        }
    }
    private void ApplyAllStatModifiers()
    {
        StatModifier[] modifiers = Module.Modifiers;
        foreach (StatModifier modifier in modifiers)
        {
            base.Ship.AddStatModifier(Module, modifier);
        }
    }
    private void RemoveAllStatModifiers()
    {
        StatModifier[] modifiers = Module.Modifiers;
        foreach (StatModifier statModifier in modifiers)
        {
            base.Ship.RemoveStatModifier(Module, statModifier.StatName);
        }
    }
}
/*

`
time = time - interpolationPeriod;

*/