using Modding;
using Bundles;
using Steamworks.Ugc;
using UI;
using System.Diagnostics;
using Shapes;
using System.Text;
using AGMLIB.Dynamic_Systems.Area;
using Steamworks;
using Networking;
using System.Reflection;

public class EntryPoint : IModEntryPoint 
{
    private static readonly FilePath[] _stockBundles = new FilePath[5]
    {
                new("stock", "Assets/AssetBundles/"),
                new("stock-f1", "Assets/AssetBundles/"),
                new("stock-f2", "Assets/AssetBundles/"),
                new("stock-maps", "Assets/AssetBundles/"),
                new("stock-voice", "Assets/AssetBundles/")
    };
    private static readonly FilePath[] _compressedBundles = new FilePath[5]
{
                new("stock", "Assets/ComAssetBundles/"),
                new("stock-f1", "Assets/ComAssetBundles/"),
                new("stock-f2", "Assets/ComAssetBundles/"),
                new("stock-maps", "Assets/ComAssetBundles/"),
                new("stock-voice", "Assets/ComAssetBundles/")
};
    public void HandleLog(string logString, string stackTrace, LogType logType)
    {
        logString = $"LOGA: {DateTime.Now}: {logString}";
        if(!logString.Contains("LOGA"))
            Debug.LogError(logString);
    }
    public void PreLoad()
    {


        DependencyPatch.window = false;
        Debug.Log($"AGMLIB: {Assembly.GetExecutingAssembly().GetName().Version.ToString()} Preload");

        if (Harmony.HasAnyPatches("neb.lib.harmony.product")) {
            //Debug.LogError("Illegal Load Order");
            return;
        }
        var harmony = new Harmony("neb.lib.harmony.product");
        harmony.PatchAll();
        return;

        //Debug.LogError("AGMLIB: 0.3.2.2.12 Preload");
        foreach (var hullComponent in BundleManager.Instance.AllComponents)
        {
            foreach (BasicEffect basicEffect in hullComponent.GetComponentsInChildren<BasicEffect>(includeInactive: true))
            {
                {
                    //Debug.LogError("Effect Type: " + basicEffect.GetType().Name + " Active: " + basicEffect.isActiveAndEnabled + " Gameobject " + basicEffect.gameObject.activeInHierarchy);

                }
            }
        }
        /*
        Dictionary<string, FactionTraitCard> traits = Common.GetVal<Dictionary<string, FactionTraitCard>>(BundleManager.Instance, "_conquestTraits");
        KeyValuePair<string, FactionTraitCard> newtrait = new();
        foreach (KeyValuePair<string, FactionTraitCard> trait in traits)
        {
            newtrait = new(trait.Key + "x", trait.Value);
        }
        newtrait.Value.name = "Ship's Cat";
        newtrait.Value.PointCost = 100;
        newtrait.Value.Description = "Don't be fooled by the purring engines and sunbeam naps. Every vessel in the [Faction Name] fleet boasts a seasoned feline navigator. These furred veterans aren't just mousers – they sn
        out storms, predict fair winds with a twitch of the tail, and even lend a paw with repairs (mostly by napping strategically on loose wires). So remember, matey, if you see a cat on our decks, show some respect – you might just owe your next fair voyage to a whiskered oracle.\r\n\r\n";
        //newtrait.Value.Modifiers.AddItem(new("test", 0, 1));
        //traits.Add(newtrait.Key, newtrait.Value);
        newtrait.Value.Modifiers[0] = new("hull-crew-cheer", 0, 1);
        newtrait.Value.Modifiers.AddItem(new("conq-team-initialofficerbonusskills", 0, 0.5f));
        newtrait.Value.Modifiers.AddItem(new("hull-turnrate", 0, 0.25f));
        newtrait.Value.Modifiers.AddItem(new("hull-angularmotor", 0, 0.125f));
        Debug.LogError("Error");
        Common.SetVal(BundleManager.Instance, "_conquestTraits", traits);
        */

        //Application.logMessageReceived += HandleLog;

        //SteamManager.SetActivityDetails("map_name", "guys help Im trapped in a dll factory its not much longer before I hit the char limit but you have to send help");
        //SteamManager.SetActivity(RichPresenceActivity.Skirmish);
        //Environment.SetEnvironmentVariable("UNITY_EXT_LOGGING",   "1", EnvironmentVariableTarget.User);
        //Environment.SetEnvironmentVariable("UNITY_LOG_TIMESTAMP", "1", EnvironmentVariableTarget.User);
        string box = @" A box (plural: boxes) is a container with rigid sides used for the storage or transportation of its contents. Most boxes have flat, parallel, rectangular sides (typically rectangular prisms). Boxes can be very small (like a matchbox) or very large (like";
        string CAT = @"
                     ／＞　 フ
                    | 　_　_| 
                  ／` ミ＿xノ 
                 /　　　　 |
                /　 ヽ　　 ﾉ
                │　　|　|　|
            ／￣|　　 |　|　|
            (￣ヽ＿_ヽ_)__)
            ＼二)";
        for (int i = 1; i <= box.Length; i++)
        {
            //SteamManager.SetActivityDetails("map_name", box.Substring(0, i));

        }


        //SteamManager.SetActivityDetails("map_name", CAT);
        string newlines = "Newline\n\rTest";
        //SteamManager.SetActivityDetails("map_name", newlines);
        //SteamManager.SetActivityDetails("map_name", "with weird bugs ≽^•⩊•^≼");


        //Debug.LogError(nameof(LookaheadMunition.UseableByFaction));
        foreach (var path in _stockBundles.Zip(_compressedBundles, (n, w) => new { Source = n, Dest = w }))
        {
            if (File.Exists(path.Dest.FullPath))
                continue;
            Debug.LogError("Recompiling Assetbundle at " + path.Source.FullPath);
            FilePath test = new("Assets/ComAssetBundles/");
            Directory.CreateDirectory(test.FullPath);
            
            AssetBundle.RecompressAssetBundleAsync(path.Source.FullPath, path.Dest.FullPath, BuildCompression.LZ4Runtime);
        }

        return;

        List<ModRecord> modlist = (List<ModRecord>)ModDatabase.Instance.MarkedForLoad;
        foreach (ModRecord record in modlist)
        {
            ModInfo info = record.Info;
            //Debug.LogError("Checking " + info.ModName);
            DependencyPatch.Postfix(ModDatabase.Instance, ref info);
            return;
        }
        //LobbyModPane modPane = new LobbyModPane();
        //List<ulong> _missingmods = ModDatabase.Instance.GetMissingMods(new ulong[1] { 2960504230 });

        ModRecord modByID = ModDatabase.Instance.GetModByID(2960504230);  
        IEnumerable<ModRecord> mods = new List<ModRecord>();
        //Debug.LogError("Checking Missing Mods");

        //Debug.LogError("Checking Loaded AGMLIB");
        //Debug.LogError("Checking if AGMLIB isn't loaded");
        if (!ModDatabase.Instance.MarkedForLoad.Any(p => p.Info.ModName == "AGMLIB"))//First().Info.ModName != "AGMLIB"
            FixLoadOrder();

        foreach (ModRecord record in modlist)
        {
            ModInfo info = record.Info;
            //Debug.LogError("Checking " + info.ModName);
            DependencyPatch.Postfix(ModDatabase.Instance, ref info);
            Common.SetVal(record, "Info", info);

            //Debug.Log("Checking order " + info.ModName);
            if (info.Dependencies == null)
                continue;
            foreach (ulong depedencyid in info.Dependencies)
            {
                ModRecord dependencyrecord = ModDatabase.Instance.GetModByID(depedencyid);
                if(dependencyrecord == null || dependencyrecord.Missing)
                    dependencyrecord.SubscribeAndDownload(new AsyncGroupProgress(1).Report, new CancellationTokenSource(1).Token);
                if (record.LoadOrder < dependencyrecord.LoadOrder)
                {
                    Debug.LogError("Fixing load order");
                    FixLoadOrder(depedencyid);
                }         //
            }
            //Debug.Log("Writing field " + modlist[i].Info.ModName);
            
            //Debug.Log("Wrote field " + modlist[i].Info.ModName);

        }

        //IProgress<float> progress = null;

        /*
        foreach (AssetBundle bundle in await BundleManager.LoadAssetBundleListAsync(_stockBundles, progress))
        {
            BundleManager.Instance.ProcessAssetBundle(bundle, null);
        }
        */

        foreach (ModRecord record in (List<ModRecord>)ModDatabase.Instance.AllMods)
        {

            ModInfo info = record.Info;
            if (info.UniqueIdentifier != 2977225446)
                continue;
            foreach (ulong depedencyid in info.Dependencies)
            {
                ModRecord dependencyrecord = ModDatabase.Instance.GetModByID(depedencyid);
                if (dependencyrecord == null || dependencyrecord.Missing)
                {
                    
                    Debug.LogError("Grabbing Depedency");
                    dependencyrecord.SubscribeAndDownload(new AsyncGroupProgress(1).Report, new CancellationTokenSource(1).Token);
                }
            }

            if (record.Info == null || record.Info.AssetBundles == null || record.Info.FileLocation != null)
                continue;

            if (!record.MarkedForLoad)
                continue;
            if (record.Loaded)
                continue;

            foreach (string bundleName in record.Info.AssetBundles)
            {
                FilePath path = new(bundleName, record.Info.FileLocation.Directory);
                if (!BundleCache.Preloadedbundles.ContainsKey(path))
                {
                    //BundleCache.Preloadedbundles.Add(path, AssetBundle.LoadFromFileAsync(path.RelativePath));
                }
            }
        }
        //Debug.LogError("End Preload");
        DependencyPatch.window = true;
        return;
    }

    void FixLoadOrder(ulong modid = 2960504230)
    {
        Debug.LogError("FIXING LOAD ORDER");
        ModRecord modByID = modByID = ModDatabase.Instance.GetModByID(modid);
        if (modByID == null || modByID.Missing)
            modByID.SubscribeAndDownload(new AsyncGroupProgress(1).Report, new CancellationTokenSource(1).Token);
        IEnumerable<ModRecord> mods = new List<ModRecord>();
        if (!ModDatabase.Instance.MarkedForLoad.Any(p => p.Info.ModName == "AGMLIB"))//First().Info.ModName != "AGMLIB"
            mods = mods.AddItem(modByID);
        foreach (ModRecord mod in ModDatabase.Instance.MarkedForLoad)
            mods = mods.AddItem(mod);
        Debug.LogError("MODLIST GENERATED");
        ModDependencyGraph modDependencyGraph = new(mods);
        List<ModRecord> correctLoadOrder = modDependencyGraph.GetCorrectLoadOrder();
        int num = 0;
        foreach (ModRecord item in correctLoadOrder)
            Common.SetVal(ModDatabase.Instance, "LoadOrder", num++);
        Debug.LogError("MODLIST SORTED");

        ModDatabase.Instance.SetModsToLoad(correctLoadOrder);
        Process.Start(Application.dataPath.Replace("_Data", ".exe"));

        Debug.Log("NOFIX");
        Application.Quit();
    }

    public void PostLoad()
    {
        var csv = new StringBuilder();
        return;
        foreach (BaseHull _hull in BundleManager.Instance.AllHulls)
        {
            FilePath path = new FilePath(_hull.ClassName + ".txt", "Assets/ComAssetBundles/");
            //File.WriteAllText(path.FullPath, _hull.LongDescription);
            static string tocsv(string? input) => '"' + (input?.Replace('"', ' ') ?? "") + '"' + ",";
            string newLine = tocsv(_hull.ClassName);
            newLine += tocsv("(" + _hull.HullClassification + " Class)" );
            //image = _hull.HullScreenshot;
            newLine += tocsv(_hull.LongDescription);
            //newLine += tocsv(_hull.EditorFormatHullStats(showBreakdown: false)); ;
            newLine += tocsv("<b>Modifiers:</b>\n" + _hull.EditorFormatHullBuffs());
            newLine += tocsv("<i><color=#FFEF9E>" + _hull.FlavorText+ "</color></i>");
            csv.AppendLine(newLine);

            static string toheader(string? input) => "== " + (input ?? "") + " ==\n";

            newLine = toheader(_hull.ClassName);
            newLine += "(" + _hull.HullClassification + " Class)" + "\n";
            //image = _hull.HullScreenshot;
            newLine += toheader("Description");
            newLine += (_hull.LongDescription) + "\n";
            newLine += toheader("Stats");
            //newLine += (_hull.EditorFormatHullStats(showBreakdown: false)) + "\n";
            newLine += toheader("Hull Buffs");
            newLine += _hull.EditorFormatHullBuffs() + "\n";
            newLine += toheader("Flavor Text");
            newLine += _hull.FlavorText + "\n";
            File.WriteAllText(path.FullPath, newLine);

        }
        File.WriteAllText(new FilePath("hulldump.csv", "Assets/ComAssetBundles/").FullPath, csv.ToString());

        //Application.Quit();
        //Debug.LogError("AGMLIB: 1.0");
        return;
        //updateAllDesignators();
        //createDrones();
    }
}

[HarmonyPatch(typeof(HullComponent), "GetMissingResources")]
class HullComponentGetMissingResources : MonoBehaviour
{

    static bool Prefix(HullComponent __instance, ResourceValue[] ____requiredResources, ref string __result)
    {
        Common.LogPatch();
        string text = "";
        ResourceValue[] requiredResources = ____requiredResources;
        if (requiredResources == null)
        {
            //Debug.LogError("requiredResources is NULL");
            __result = text;
            return false;
        }
        foreach (ResourceValue resourceValue in requiredResources)
        {
            try
            {
                if (!resourceValue.HasAll)
                {
                    try
                    {
                        text = text + "  - " + resourceValue.Resource.Name + "\n";
                    }
                    catch
                    {
                        //Debug.LogError("String gen error");
                    }
                }
            }
            catch
            {
                //Debug.LogError("If check error");
            }
        }

        __result = text;
        return false;
    }
}
class BundleCache
{
    public static Dictionary<FilePath, AssetBundleCreateRequest> Preloadedbundles = new();
}

//[HarmonyPatch(typeof(BundleManager), nameof(BundleManager.LoadAssetBundleAsync))]
class QuickLoad
{
    public static async Task<AssetBundle> AGMLoadAssetBundleAsync(FilePath path, IProgress<float> progress)
    {
        var watch = Stopwatch.StartNew();
        if (!BundleCache.Preloadedbundles.TryGetValue(path, out AssetBundleCreateRequest createRequest))
            createRequest = AssetBundle.LoadFromFileAsync(path.RelativePath);
        //string start = "Load for achived an estimated " + path.Name + " %" + createRequest.progress + "reduction ";

        while (!createRequest.isDone)
        {
            progress?.Report(createRequest.progress);
            await Task.Delay(50);
        }
        watch.Stop();

        // Print the execution time in milliseconds
        // by using the property elapsed milliseconds
        //Debug.LogError(start + $"and took {watch.ElapsedMilliseconds}ms");

        return createRequest.assetBundle;
    }

    public static bool Prefix(BundleManager __instance, FilePath path, IProgress<float> progress, ref Task<AssetBundle> __result)
    {

        if (BundleCache.Preloadedbundles.ContainsKey(path))
        {
            // = AssetBundle.LoadFromFileAsync(path.RelativePath);

            __result = AGMLoadAssetBundleAsync(path, progress);
            return false;
        }
        else
        {
            //Debug.LogError(path.ToString());
            //__result = AGMLoadAssetBundleAsync(path, progress);
            //return false;
            return true;
        }
    }
}
//[HarmonyPatch(typeof(ModDatabase), "FindModInfo")]
class DependencyPatch
{
    public static bool window = false;
    public static void Postfix(ModDatabase __instance, ref ModInfo __result)
    {

        //Debug.LogError(__result.ModName + " Postfix");

        /*
        if(__result.AssetBundles != null && __result.FileLocation != null)
        {
            foreach (string bundleName in __result.AssetBundles)
            {
                FilePath path = new FilePath(bundleName, __result.FileLocation.Directory);
                if (!BundleCache.Preloadedbundles.ContainsKey(path))
                {
                    //BundleCache.Preloadedbundles.Add(path, AssetBundle.LoadFromFileAsync(path.RelativePath));
                }
            }
        }
        */

        if (!__result.DownloadedFromWorkshop || __result.WorkshopItem == null || __result.Assemblies == null || !__result.Assemblies.Contains("AGMLIB.dll") || __result.ModName == "AGMLIB")
            return;

        Debug.LogError(__result.ModName + " Postfix");
        Item workshopstats = __result.WorkshopItem.Value;
        if (__result.ModDescription == null || __result.ModDescription.Length < 1)
            __result.ModDescription = workshopstats.Description;

        //__result.Assemblies = new string[0] { };
        List<string> newassemblies = new();
        foreach (string s in __result.Assemblies)
        {
            if (s != "AGMLIB.dll")
                newassemblies.Add(s);
        }

        string[] array = new string[newassemblies.Capacity];
        array = newassemblies.Select(i => i.ToString()).ToArray();
        __result.Assemblies = array;
        __result.Dependencies = new ulong[1] { 2960504230 };
        ModRecord modByID = ModDatabase.Instance.GetModByID(2960504230);
        if (modByID.Loaded || modByID.MarkedForLoad)//&& modByID.Info.DownloadedFromWorkshop
        {
            //Debug.LogError("Injecting Dependency for " + __result.ModName);

        }

        if (workshopstats.IsDownloading || workshopstats.IsDownloadPending || workshopstats.NeedsUpdate)
        {
            Debug.LogError("Launched with updating mods");
            Debug.Log("NOFIX");
            if (!window)
                return;
            ModalConfirm modalConfirm = MenuController.Instance.OpenMenu<ModalConfirm>("Confirm");
            modalConfirm.Set("<color=" + GameColors.RedTextColor + ">INSTALL WARNING</color>\n The workshop version of the mod " + __result.ModName + " is not fully downloaded",
            "Quit",
            showCancel: true,
            delegate { Application.Quit(); },
            delegate { Debug.Log("NOFIX"); });
        }
        else if (false && workshopstats.Updated > File.GetLastWriteTime(__result.FileLocation.ToString()))
        {
            //Debug.LogError("Launched with out of date mods");
            string error =
                "The workshop version of the mod " + __result.ModName + " is not fully up to date"
            + "\nWorkshop Version : " + workshopstats.Updated.ToString()
            + "\nLocal       Version : " + File.GetLastWriteTime(__result.FileLocation.ToString()).ToString();
            if (workshopstats.NeedsUpdate || !workshopstats.IsInstalled)
                error += "\nFlagged by steam as out of date";
            else
                error += "\nFlagged by timestamps as out of date";

            Debug.Log("NOFIX");
            if (!window)
            {
                Debug.LogError(error);
                return;
            }
            //__result.ModDescription = workshopstats.Updated.ToString() + " " + File.GetLastWriteTime(__result.FileLocation.ToString()).ToString();
            ModalConfirm modalConfirm = MenuController.Instance.OpenMenu<ModalConfirm>("Confirm");
            modalConfirm.Set("<color=" + GameColors.RedTextColor + ">UPDATE WARNING</color>\n" + error,
            "Quit",
            showCancel: true,
            delegate { Application.Quit(); },
            delegate { Debug.Log("NOFIX"); });
        }
    }
}

//SetPrivateField(ModDatabase.Instance, "_allMods", modslsit);

//mods[i] = new ModRecord(info, mods[i])

//List<ModRecord> modSet = ModDatabase.Instance.ExpandModListForDependencies(missingmods);
/*
IReadOnlyList<ModRecord> mods = ModDatabase.Instance.AllMods;
ModRecord goodewar = mods.FirstOrDefault(x => x.Info.ModName == "AGMLIB");
//List<ModRecord> modSet = ModDatabase.Instance.ExpandModListForDependencies(hostList);

if (goodewar == null)
{

    Task<bool> resolveTask = new ModRecord(new ModInfo()).ResolveModWithWorkshop();
}*/

//IReadOnlyList<ModRecord> mods = ModDatabase.Instance.AllMods;
//ModDatabase.Instance.SetModsToLoad(_activeMods.ConvertAll((ModListItem x) => x.Mod));

/*
 * 

warning.Set("Activate AGMLIB?", "OK", showCancel: true, delegate
{

}, 
delegate { Debug.LogError("NOT ACTIVATING AGMLIB CAN CAUSE ERRORS"); }
);
return;
        public static void createDrones()
        {
            Dictionary<string, IMunition> componentDictionary = (Dictionary<string, IMunition>)GetPrivateField(BundleManager.Instance, "_munitionsBySaveKey");
            foreach (var item in componentDictionary)
            {
                //Debug.LogError(item.Key);



            }

            //updateTurret(componentDictionary, "Stock/Mk68 Cannon", "Mk65-B Cannon");
        }
        public static void updateAllDesignators()
        {
            Dictionary<string, HullComponent> componentDictionary = (Dictionary<string, HullComponent>)GetPrivateField(BundleManager.Instance, "_components");
        
            foreach (var item in componentDictionary)
            {
                //Debug.LogError(item.Key);

                updateDesignator(item.Value, componentDictionary);
            }

            //updateTurret(componentDictionary, "Stock/Mk68 Cannon", "Mk65-B Cannon");
        }

        public static void updateDesignator(HullComponent fix, Dictionary<string, HullComponent> componentDictionary)
        {
            if (fix.gameObject.GetComponentInChildren<BrokenDesignator>())
            {
                HullComponent spotlight;
                componentDictionary.TryGetValue(fix.gameObject.GetComponentInChildren<BrokenDesignator>().PrefabName, out spotlight);
                RezFollowingMuzzle badmuzzel = fix.gameObject.GetComponentInChildren<RezFollowingMuzzle>();
                if (spotlight != null && badmuzzel != null)
                {
                    RezFollowingMuzzle goodmuzzel = spotlight.gameObject.GetComponentInChildren<RezFollowingMuzzle>();
                    GameObject prefab = (GameObject)GetPrivateField(goodmuzzel, "_followingPrefab");
                    if(prefab != null)
                        SetPrivateField(badmuzzel, "_followingPrefab", prefab);
                }


                //UnityEngine.Object.Destroy(badmuzzel);

            }

        }
*/