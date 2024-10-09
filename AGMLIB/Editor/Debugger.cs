public class KeyDebugger : ShipState
{
    // Start is called before the first frame update
    public string key = "";
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GameObject dcboardgo = Common.GetVal<GameObject>(Ship.Hull, "_damageControlBoardPrefab");
        if (dcboardgo == null)
        {
            Debug.LogError("No DC Board Detected");

        }
        List<ShipStatusDisplayPart> display = dcboardgo?.GetComponentsInChildren<ShipStatusDisplayPart>().ToList();
        if(display == null || display.Count == 0)
        {
            Debug.LogError("No ShipStatusDisplayPart Detected");

        }
        List<String> keys = display.ConvertAll(displaypart => displaypart.PartKeys.ToList()).SelectMany(a => a).ToList();
        foreach (string key in Ship.Hull.AllParts.ConvertAll(part => part.Key))
        {
            if (keys.Contains(key))
            {

                continue; 
            }
            else
            {
                Debug.LogError("Missing part key on dc board " + key);
            }
        }
        foreach (string key in Ship.Hull.AllSockets.ConvertAll(part => part.Key))
        {
            if (keys.Contains(key))
            {

                continue;
            }
            else
            {
                Debug.LogError("Missing socket key on dc board " + key);
            }
        }
        
    }
}

//[HarmonyPatch(typeof(HullComponent), nameof(HullComponent.GetFormattedDesc))]
class Described
{
    static void Postfix(HullComponent __instance, ref string __result) => __result += "<color=" + GameColors.RedTextColor + ">Colored Text</color>\n";
}