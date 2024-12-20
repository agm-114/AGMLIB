public class BrokenDesignator : MonoBehaviour
{
    // Start is called before the first frame update
    public string PrefabName = "Stock/E55 'Spotlight' Illuminator";
    RezFollowingMuzzle optionaltargeMuzle;
    void Awake()
    {
        //Debug.LogError("Alive");
        //Dictionary<string, HullComponent> componentDictionary = (Dictionary<string, HullComponent>)GetPrivateField(BundleManager.Instance, "_components");
        IReadOnlyCollection<HullComponent> hullComponents = BundleManager.Instance.AllComponents;
        if (optionaltargeMuzle == null)
            optionaltargeMuzle = gameObject.GetComponent<RezFollowingMuzzle>();
        if (optionaltargeMuzle == null)
            optionaltargeMuzle = gameObject.GetComponentInChildren<RezFollowingMuzzle>();
        if (optionaltargeMuzle == null)
            optionaltargeMuzle = gameObject.GetComponentInParent<RezFollowingMuzzle>();
        if (optionaltargeMuzle == null)
        {
            Common.Hint("Could not find muzzle on weapon");
            return;
        }


        
        HullComponent goodewar = BundleManager.Instance.GetHullComponent(PrefabName);
        if (goodewar == null)
        {
            Common.Hint("Could not find prefab with savekey " + PrefabName);
            return;
        }
        //Debug.LogError("Found Target " + goodewar.SaveKey);
        RezFollowingMuzzle goodmuzzel = goodewar.gameObject.GetComponentInChildren<RezFollowingMuzzle>();
        if (goodmuzzel == null)
        {
            Common.Hint("Could not find rez following muzzle on indicated prefab " + PrefabName);
            return;
        }
        GameObject prefab = Common.GetVal<GameObject>(goodmuzzel, "_followingPrefab");
        if (prefab == null)
        {
            Common.Hint("Could not find following prefab in rez following muzzle on indicated prefab " + PrefabName);
            return;
        }
        if (prefab != null && optionaltargeMuzle != null)
            Common.SetVal(optionaltargeMuzle, "_followingPrefab", prefab);
        else
            Common.Hint("Jammer Setup Failure");
        Destroy(this);
    }
}
