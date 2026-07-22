using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Ships;
using UnityEngine;
using Utility;

public static class OwnedTypeReflection
{
    private const BindingFlags DeclaredInstanceFields =
        BindingFlags.Instance |
        BindingFlags.Public |
        BindingFlags.NonPublic |
        BindingFlags.DeclaredOnly;

    public static bool IsOwnedType(Type type)
    {
        return type.Assembly == typeof(OwnedTypeReflection).Assembly;
    }

    public static FieldInfo? FindInstanceField(
        Type concreteType,
        string fieldName,
        Type expectedFieldType)
    {
        if (!IsOwnedType(concreteType))
        {
            return null;
        }

        for (Type? current = concreteType; current != null; current = current.BaseType)
        {
            FieldInfo? field = current.GetField(fieldName, DeclaredInstanceFields);
            if (field?.FieldType == expectedFieldType)
            {
                return field;
            }
        }

        return null;
    }

    public static bool TryInitializeStatValues(
        object target,
        out List<StatValue>? initializedStats)
    {
        initializedStats = null;
        if (target == null || !IsOwnedType(target.GetType()))
        {
            return false;
        }

        initializedStats = InitializeStatValues(target);
        return true;
    }

    private static List<StatValue> InitializeStatValues(object target)
    {
        Type concreteType = target.GetType();
        List<StatValue> initializedStats = new List<StatValue>();
        List<Tuple<ShipStatAttribute, FieldInfo>> statFields =
            StatHelpers.GetAllStatFields(concreteType);

        if (statFields == null)
        {
            return initializedStats;
        }

        foreach (Tuple<ShipStatAttribute, FieldInfo> statField in statFields)
        {
            ShipStatAttribute attribute = statField.Item1;
            FieldInfo destinationField = statField.Item2;

            if (destinationField.FieldType != typeof(StatValue))
            {
                Debug.LogError("ShipStatAttribute was found on field which is not of type StatValue");
                continue;
            }

            float baseValue;
            if (string.IsNullOrEmpty(attribute.InitializeFrom))
            {
                baseValue = attribute.InitialValue;
            }
            else
            {
                FieldInfo? initializationField = FindInstanceField(
                    concreteType,
                    attribute.InitializeFrom,
                    attribute.AllowInitializeType);

                if (initializationField == null)
                {
                    Debug.LogError($"No field named {attribute.InitializeFrom} found (or incorrect type: not {attribute.AllowInitializeType}) to initialize stat {attribute.StatName} from");
                    continue;
                }

                baseValue = (float)Convert.ChangeType(
                    initializationField.GetValue(target),
                    typeof(float));
            }

            string? subtypeName = null;
            if (!string.IsNullOrEmpty(attribute.NameSubtypeFrom))
            {
                FieldInfo? subtypeField = FindInstanceField(
                    concreteType,
                    attribute.NameSubtypeFrom,
                    typeof(string));

                if (subtypeField != null)
                {
                    subtypeName = subtypeField.GetValue(target) as string;
                }
                else
                {
                    Debug.LogError($"No field name {attribute.NameSubtypeFrom} found (or incorrect type, not string) to set stat name postfix/subtype for stat {attribute.StatName}");
                }
            }

            StatValue statValue = new StatValue(attribute, baseValue, subtypeName);
            destinationField.SetValue(target, statValue);
            initializedStats.Add(statValue);
        }

        return initializedStats;
    }
}

[HarmonyPatch(typeof(StatHelpers), nameof(StatHelpers.InitializeStatValues))]
internal static class InitializeOwnedTypeStatValuesPatch
{
    private static bool Prefix(object target, ref List<StatValue> __result)
    {
        if (!OwnedTypeReflection.TryInitializeStatValues(
                target,
                out List<StatValue>? initializedStats))
        {
            return true;
        }

        __result = initializedStats!;
        return false;
    }
}
