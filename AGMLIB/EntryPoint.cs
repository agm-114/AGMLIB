using Modding;
using Munitions;
using UnityEngine;
using UnityEngine.VFX;
//using System;
using System.Reflection;
using System.Collections.Generic;
using Ships;
using Bundles;
using System;
using System.Threading.Tasks;
using System.Threading;
using Utility;
using Steamworks.Ugc;
using UI;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using HarmonyLib;
using System.Linq;
using Steamworks;
using UnityEngine.PlayerLoop;
using System.IO;
using Missions.Nodes;
using UnityEditor;

[HarmonyPatch (typeof(HullComponent), "GetMissingResources")]
class HullComponentGetMissingResources : MonoBehaviour
{

    static bool Prefix(HullComponent __instance, ResourceValue[] ____requiredResources, ref string __result)
    {
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

[HarmonyPatch(typeof(BundleManager), nameof(BundleManager.LoadAssetBundleAsync))]
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
[HarmonyPatch(typeof(ModDatabase), "FindModInfo")]
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


        if (!__result.DownloadedFromWorkshop || __result.WorkshopItem == null || __result.Assemblies == null || !__result.Assemblies.Contains("AGMLIB.dll"))
            return;


        //Debug.LogError("Processing :" + __result.ModName);
        if (__result.Assemblies == null ||  __result.ModName == "AGMLIB")
            return;
        //Debug.LogError("Contains AGMLIB :" + __result.ModName);




        Debug.Log(__result.ModName + " Postfix");
        Item workshopstats = __result.WorkshopItem.Value;
        if (__result.ModDescription == null || __result.ModDescription.Length < 1)
            __result.ModDescription = workshopstats.Description;



        //__result.Assemblies = new string[0] { };
        List<string> newassemblies = new();
        foreach (string s in __result.Assemblies)
            if (s != "AGMLIB.dll")
                newassemblies.Add(s);


        string[] array = new string[newassemblies.Capacity];
        array = newassemblies.Select(i => i.ToString()).ToArray();
        __result.Assemblies = array;
        ModRecord modByID = ModDatabase.Instance.GetModByID(2960504230);
        if (modByID.Loaded || modByID.MarkedForLoad)//&& modByID.Info.DownloadedFromWorkshop
        {
            //Debug.LogError("Injecting Dependency for " + __result.ModName);
            __result.Dependencies = new ulong[1] { 2960504230 };
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


public class EntryPoint : IModEntryPoint 
{
    private static readonly FilePath[] _stockBundles = new FilePath[5]
    {
                new FilePath("stock", "Assets/AssetBundles/"),
                new FilePath("stock-f1", "Assets/AssetBundles/"),
                new FilePath("stock-f2", "Assets/AssetBundles/"),
                new FilePath("stock-maps", "Assets/AssetBundles/"),
                new FilePath("stock-voice", "Assets/AssetBundles/")
    };

    public void PreLoad()
    {
        DependencyPatch.window = false;
        //Application.Quit();
        Debug.Log("AGMLIB: 0.16 Preload");
        var harmony = new Harmony("neb.lib.harmony.product");
        harmony.PatchAll();
        //LobbyModPane modPane = new LobbyModPane();
        //List<ulong> _missingmods = ModDatabase.Instance.GetMissingMods(new ulong[1] { 2960504230 });
        
        ModRecord modByID = ModDatabase.Instance.GetModByID(2960504230);  
        IEnumerable<ModRecord> mods = new List<ModRecord>();
        //Debug.LogError("Checking Missing Mods");

        //Debug.LogError("Checking Loaded AGMLIB");
        //Debug.LogError("Checking if AGMLIB isn't loaded");
        if (!ModDatabase.Instance.MarkedForLoad.Any(p => p.Info.ModName == "AGMLIB"))//First().Info.ModName != "AGMLIB"
            FixLoadOrder();
        //Debug.LogError("Checking if AGMLIB isn ordered correctly");
        List<ModRecord> modlist = (List<ModRecord>)ModDatabase.Instance.MarkedForLoad;



        foreach (ModRecord record in modlist)
        {
            ModInfo info = record.Info;
            //Debug.LogError("Checking " + info.ModName);
            DependencyPatch.Postfix(ModDatabase.Instance, ref info);
            SetPrivateField(record, "Info", info);

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


        
        foreach (FilePath path in _stockBundles)
        {
            //Debug.LogError("Recompiling Assetbundle at " + path.FullPath);
            //AssetBundle.RecompressAssetBundleAsync(path.FullPath, path.FullPath + "x", BuildCompression.LZ4Runtime);
        }
        
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
            SetPrivateField(ModDatabase.Instance, "LoadOrder", num++);
        Debug.LogError("MODLIST SORTED");

        ModDatabase.Instance.SetModsToLoad(correctLoadOrder);
        Process.Start(Application.dataPath.Replace("_Data", ".exe"));



        Debug.Log("NOFIX");
        Application.Quit();
    }

    public void PostLoad()
    {
        //Application.Quit();
        //Debug.LogError("AGMLIB: 1.0");
        return;
        //updateAllDesignators();
        //createDrones();
    }



    public static System.Object GetPrivateField(System.Object instance, string fieldName)
    {
        static System.Object GetPrivateFieldInternal(System.Object instance, string fieldName, Type type)
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
        static void SetPrivateFieldInternal(System.Object instance, string fieldName, System.Object value, Type type)
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