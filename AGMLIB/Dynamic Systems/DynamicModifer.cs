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
    [Header("Component Specfic Settings")]
    [Space]
    public StatModifier[] Modifiers;

    //public ModiferOverride modiferOverride;


    private void ApplyAllStatModifiers()
    {
        StatModifier[] modifiers = module.Modifiers;
        foreach (StatModifier modifier in modifiers)
        {
            base.ship.AddStatModifier(module, modifier);
        }
    }

    private void RemoveAllStatModifiers()
    {
        StatModifier[] modifiers = module.Modifiers;
        foreach (StatModifier statModifier in modifiers)
        {
            base.ship.RemoveStatModifier(module, statModifier.StatName);
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
            return;
        }  
        else
        {
            //Debug.LogError("Active");
        }


        bool changes = false;

        if(module.Modifiers.Length < Modifiers.Length)
        {
            //Debug.LogError("Possible Dynamic Modifer Misconfiguration");
        }

        for (int i = 0; i < module.Modifiers.Length; i++)
        {
            for (int j = 0; j < Modifiers.Length; j++)
            {
                if (module.Modifiers[i].StatName == Modifiers[j].StatName && module.Modifiers[i].Modifier != Modifiers[j].Modifier)
                {
                    //Debug.LogError("Updating " + Modifiers[j].StatName + "  values at index[" + i + "] from" + module.Modifiers[i].Modifier + " to " + Modifiers[j].Modifier);
                    module.Modifiers[i] = Modifiers[j];//new StatModifier(@override.Modifiers[j].StatName, 0, @override.Modifiers[j].Modifier);
                    changes = true;
                }
            }
        }




        if (changes)
            ApplyAllStatModifiers();




    }

    public static object GetPrivateField(object instance, string fieldName)
    {
        static object GetPrivateFieldInternal(object instance, string fieldName, Type type)
        {
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null)
            {
                return field.GetValue(instance);
            }
            else if (type.BaseType != null)
            {
                return GetPrivateFieldInternal(instance, fieldName, type.BaseType);
            }
            else
            {
                return null;
            }
        }

        return GetPrivateFieldInternal(instance, fieldName, instance.GetType());
    }

    public static void SetPrivateField(object instance, string fieldName, object value)
    {
        static void SetPrivateFieldInternal(object instance, string fieldName, object value, Type type)
        {
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null)
            {
                field.SetValue(instance, value);
                return;
            }
            else if (type.BaseType != null)
            {
                SetPrivateFieldInternal(instance, fieldName, value, type.BaseType);
                return;
            }
        }

        SetPrivateFieldInternal(instance, fieldName, value, instance.GetType());
    }
}
/*

`
time = time - interpolationPeriod;

*/