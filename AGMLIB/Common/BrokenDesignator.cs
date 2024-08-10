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
        GameObject prefab = Common.GetVal<GameObject>(goodmuzzel, "_followingPrefab");
        if (prefab != null && optionaltargeMuzle != null)
            Common.SetVal(prefab, "_followingPrefab", prefab);

    }
}
