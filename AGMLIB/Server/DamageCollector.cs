using System.Xml.Serialization;
using FilePath = Utility.FilePath;
using Random = UnityEngine.Random;

public static class FleetTools
{
    public static Dictionary<string, int> Components = new();
    public static Dictionary<string, int> FleetComponents = new();

    public static SerializedInventory Inventory
    {
        get
        {
            return new SerializedInventory
            {
                Components = Components
                    .Select(kv => new InventoryItem { EmpName = kv.Key, Count = kv.Value })
                    .ToArray()
            };
        }
        set
        {
            Components.Clear();
            if (value?.Components != null)
            {
                foreach (var item in value.Components)
                {
                    if (!string.IsNullOrEmpty(item.EmpName))
                    {
                        Components[item.EmpName] = item.Count;
                    }
                }
            }
        }
    }

    public static string SaveDir => Path.Combine("Saves", "Fleets");

    public static SerializedInventory DefaultInventory(int amount = 10)
    {
        SerializedInventory inventory = new SerializedInventory();

        List<InventoryItem> items = new();

        foreach (HullComponent comp in BundleManager.Instance.AllComponents)
        {
            InventoryItem item = new InventoryItem
            {
                EmpName = comp.SaveKey,
                Count = amount
            };
            items.Add(item);
        }

        inventory.Components = items.ToArray();
        return inventory;
    }

    private static FilePath GetInventoryFilePath(string savename)
    {
        if (!savename.EndsWith(".inventory"))
        {
            savename += ".inventory";
        }

        FilePath invpath = new FilePath(savename, SaveDir);

        if (!Directory.Exists(invpath.Directory))
        {
            Debug.Log("Creating missing save directory");
            Directory.CreateDirectory(invpath.Directory);
        }

        return invpath;
    }

    public static void SaveInventory(string savename = "default")
    {
        FilePath invpath = GetInventoryFilePath(savename);

        using FileStream stream = new FileStream(invpath.RelativePath, FileMode.Create);
        XmlSerializer serializer = new XmlSerializer(typeof(SerializedInventory));
        serializer.Serialize(stream, Inventory);
    }

    public static void LoadInventory(string savename = "default")
    {
        FilePath invpath = GetInventoryFilePath(savename);

        if (!File.Exists(invpath.RelativePath))
        {
            Debug.LogWarning($"Inventory file not found at {invpath.RelativePath}");
            return;
        }

        using FileStream stream = new FileStream(invpath.RelativePath, FileMode.Open);
        XmlSerializer serializer = new XmlSerializer(typeof(SerializedInventory));
        Inventory = (SerializedInventory)serializer.Deserialize(stream);
    }
}

//based on SerializedFleet

public class InventoryItem
{
    public string EmpName;
    public int Count;
}


[Serializable]
public class SerializedInventory
{
    //SerializedFleet
    public InventoryItem[] Components { get; set; } = [];
}

//FleetEditorController save fleet
[HarmonyPatch(typeof(ShipController), nameof(ShipController.GetAfterActionDetails))]
class ShipControllerGetAfterActionSummary
{

    static void Prefix(ShipController __instance)
    {
        IPlayer player = SkirmishGameManager.Instance.LocalPlayer;
        IFF shipiff = IFFExtensions.GetIFF(__instance.OwnedBy, player);
        SalvageRules rules = null;
        foreach (FactionDescription faction in BundleManager.Instance.AllFactions)
        {
            if (faction is not IModular modularFaction)
                continue;
            foreach (var mod in modularFaction.Modules)
            {
                if (mod is not SalvageRules salvageRules)
                    continue;
                rules = salvageRules;
            }
        }

        if (rules == null)
        {
            return;
        }
        foreach (var comp in __instance.Ship.Hull.AllComponents)
        {
            if (FleetTools.Components.ContainsKey(comp.SaveKey))
            {
               // if (shipiff == IFF.Mine)
               //     FleetTools.Components[comp.SaveKey] -= 1;
            }

            float partHealthRatio = comp.CurrentHealth / comp.MaxHealth;

            // Select the appropriate animation curve based on the ship's IFF.
            AnimationCurve activeCurve = null;
            if (shipiff == IFF.Mine) // Treat allies as friendly for salvage
            {
                activeCurve = rules.FriendlySalvageCurve;
                if (partHealthRatio > 0.05)
                    continue;
            }
            else if (shipiff == IFF.Enemy)
            {
                activeCurve = rules.HostileSalvageCurve;
            }

            // Evaluate the chosen curve at the component's health ratio to get the salvage chance.
            // The AnimationCurve.Evaluate(x) method returns the y-value for a given x-value.
            float salvageChance = activeCurve.Evaluate(partHealthRatio);
                
            // Generate a random roll between 0.0 and 1.0.
            float roll = (float)Random.Range(0f, 1f);

            Common.Hint($"  Component: {comp.SaveKey}, Health: {partHealthRatio:P0}, ");
            Common.Hint($"Salvage Chance (from curve): {salvageChance:P0}, Roll: {roll:P0} -> ");

            // If the random roll is less than the calculated salvage chance, the component is salvaged.
            if (roll < salvageChance)
            {
                // Add or increment the salvaged component in FleetTools.Components.

                if (shipiff == IFF.Mine) // Treat allies as friendly for salvage
                {
                    Common.Hint("Repaired!");

                }
                else if (shipiff == IFF.Enemy)
                {
                    Common.Hint("SALVAGED!");
                    if (FleetTools.Components.ContainsKey(comp.SaveKey))
                    {
                        FleetTools.Components[comp.SaveKey] += 1;
                    }
                    else
                    {
                        FleetTools.Components.Add(comp.SaveKey, 1);
                    }
                }
            }
            else
            {
                if (shipiff == IFF.Enemy)
                {
                    Common.Hint("FAILED to salvage.");

                }
                else if (shipiff == IFF.Mine)
                {
                    Common.Hint("DESTROYED!");
                    if (FleetTools.Components.ContainsKey(comp.SaveKey))
                    {
                        FleetTools.Components[comp.SaveKey] -= 1;
                    }
                    else
                    {
                        FleetTools.Components.Add(comp.SaveKey, -1);
                    }
                }
            }
        }
    }

}