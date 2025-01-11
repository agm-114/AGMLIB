public class BrokenDesignator : MonoBehaviour
{
    // Start is called before the first frame update
    public string PrefabName = "Stock/E55 'Spotlight' Illuminator";
    RezFollowingMuzzle optionaltargeMuzle;
    void Awake()
    {
        //Debug.LogError("Alive");
        //Dictionary<string, HullComponent> componentDictionary = (Dictionary<string, HullComponent>)GetPrivateField(BundleManager.Instance, "_components");
        //IReadOnlyCollection<HullComponent> hullComponents = BundleManager.Instance.AllComponents;
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
        if (BundleManager.Instance.GetHullComponent(PrefabName) is not HullComponent goodewar)
        {
            Common.Hint("Could not find prefab with savekey " + PrefabName);
            return;
        }
        if (goodewar.gameObject.GetComponentInChildren<RezFollowingMuzzle>() is not RezFollowingMuzzle goodmuzzel)
        {
            Common.Hint("Could not find rez following muzzle on indicated prefab " + PrefabName);
            return;
        }
        if (Common.GetVal<GameObject>(goodmuzzel, "_followingPrefab") is not GameObject prefab)
        {
            Common.Hint("Could not find following prefab in rez following muzzle on indicated prefab " + PrefabName);
            return;
        }
        Common.SetVal(optionaltargeMuzle, "_followingPrefab", prefab);
        Destroy(this);
    }
}
