using Bundles;
using Ships;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;
using System.Linq;


public class BrokenDesignator : MonoBehaviour
{
    // Start is called before the first frame update
    public string PrefabName = "Stock/E55 'Spotlight' Illuminator";
    RezFollowingMuzzle optionaltargeMuzle;
    void Awake()
    {
        //Debug.LogError("ALIVED");
        //Dictionary<string, HullComponent> componentDictionary = (Dictionary<string, HullComponent>)GetPrivateField(BundleManager.Instance, "_components");
        IReadOnlyCollection<HullComponent> hullComponents = BundleManager.Instance.AllComponents;
        if (optionaltargeMuzle == null)
            optionaltargeMuzle = gameObject.GetComponent<RezFollowingMuzzle>();
        if (optionaltargeMuzle == null)
            optionaltargeMuzle = gameObject.GetComponentInChildren<RezFollowingMuzzle>();
        if (optionaltargeMuzle == null)
            optionaltargeMuzle = gameObject.GetComponentInParent<RezFollowingMuzzle>();
        if (optionaltargeMuzle == null)
            return;
        
        HullComponent goodewar = hullComponents.FirstOrDefault(x => x.SaveKey == PrefabName);
        //Debug.LogError("Found Target " + goodewar.SaveKey);
        RezFollowingMuzzle goodmuzzel = goodewar.gameObject.GetComponentInChildren<RezFollowingMuzzle>();
        GameObject prefab = (GameObject)GetPrivateField(goodmuzzel, "_followingPrefab");
        if (prefab != null && optionaltargeMuzle != null)
            SetPrivateField(optionaltargeMuzle, "_followingPrefab", prefab);

    }

    public static System.Object GetPrivateField(System.Object instance, string fieldName)
    {
        static System.Object GetPrivateFieldInternal(System.Object instance, string fieldName, System.Type type)
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

    public static void SetPrivateField(System.Object instance, string fieldName, System.Object value)
    {
        static void SetPrivateFieldInternal(System.Object instance, string fieldName, System.Object value, System.Type type)
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
