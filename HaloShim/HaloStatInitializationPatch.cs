using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Ships;
using Utility;
using UnityEngine;

[HarmonyPatch(typeof(StatHelpers), nameof(StatHelpers.InitializeStatValues))]
internal static class HaloStatInitializationPatch
{
    private const BindingFlags FieldFlags =
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    [HarmonyPrefix]
    private static bool Prefix(object target, ref List<StatValue> __result)
    {
        if (
            target == null
            || target.GetType().Assembly != typeof(ModEntryPoint).Assembly
        )
            return true;

        Type targetType = target.GetType();
        List<StatValue> values = [];
        List<Tuple<ShipStatAttribute, FieldInfo>> statFields =
            StatHelpers.GetAllStatFields(targetType);
        if (statFields == null)
        {
            __result = values;
            return false;
        }

        foreach (
            Tuple<ShipStatAttribute, FieldInfo> statField in statFields
        )
        {
            FieldInfo valueField = statField.Item2;
            if (valueField.FieldType != typeof(StatValue))
            {
                Debug.LogError(
                    "ShipStatAttribute was found on field which is not of type StatValue"
                );
                continue;
            }

            ShipStatAttribute attribute = statField.Item1;
            if (
                !TryResolveBaseValue(
                    target,
                    targetType,
                    attribute,
                    out float baseValue
                )
            )
                continue;

            string subtype = ResolveSubtype(target, targetType, attribute);
            StatValue value = new(attribute, baseValue, subtype);
            valueField.SetValue(target, value);
            values.Add(value);
        }

        __result = values;
        return false;
    }

    private static bool TryResolveBaseValue(
        object target,
        Type targetType,
        ShipStatAttribute attribute,
        out float baseValue
    )
    {
        if (string.IsNullOrEmpty(attribute.InitializeFrom))
        {
            baseValue = attribute.InitialValue;
            return true;
        }

        FieldInfo sourceField = targetType.GetFieldFullHierarchy(
            attribute.InitializeFrom,
            FieldFlags
        );
        if (
            sourceField == null
            || sourceField.FieldType != attribute.AllowInitializeType
        )
        {
            Debug.LogError(
                $"No field named {attribute.InitializeFrom} found "
                    + $"(or incorrect type: not {attribute.AllowInitializeType}) "
                    + $"to initialize stat {attribute.StatName} from"
            );
            baseValue = default;
            return false;
        }

        baseValue = (float)
            Convert.ChangeType(sourceField.GetValue(target), typeof(float));
        return true;
    }

    private static string ResolveSubtype(
        object target,
        Type targetType,
        ShipStatAttribute attribute
    )
    {
        if (string.IsNullOrEmpty(attribute.NameSubtypeFrom))
            return null;

        FieldInfo subtypeField = targetType.GetFieldFullHierarchy(
            attribute.NameSubtypeFrom,
            FieldFlags
        );
        if (subtypeField != null && subtypeField.FieldType == typeof(string))
            return (string)subtypeField.GetValue(target);

        Debug.LogError(
            $"No field named {attribute.NameSubtypeFrom} found "
                + $"(or incorrect type: not {typeof(string)}) "
                + $"to set stat name postfix/subtype for stat {attribute.StatName}"
        );
        return null;
    }
}
