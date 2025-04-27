using Munitions.ModularMissiles.Descriptors;
using Munitions.ModularMissiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using FilePath = Utility.FilePath;
using Ships;
using System.IO;
using System.Xml.Serialization;


public class DamageCollector
{
}

//based on SerializedFleet

[Serializable]
[XmlType("Inventory")]
public class SerializedInventory 
{
    //SerializedFleet
    public string[] Stuff { get; set; } = [];
}

//FleetEditorController save fleet
[HarmonyPatch(typeof(ShipController), nameof(ShipController.GetAfterActionDetails))]
class ShipControllerGetAfterActionSummary
{

    static void Prefix(ShipController __instance)
    {
        return;
        SerializedInventory inventory = new();
        List<String>  list = new List<String>();
        foreach (HullComponent comp in __instance.Ship.Hull.AllComponents)
        {
            list.Add(comp.SaveKey);
            Common.Hint(comp.ComponentName + comp.HealthPercentage);
        }
        inventory.Stuff = list.ToArray();
        FilePath path = Fleet.FullLocalFleetPath(FilePath.Empty.Directory, "test.inv");
        if (!Directory.Exists(path.Directory))
        {
            Debug.Log("Creating missing save directory");
            Directory.CreateDirectory(path.Directory);
        }
        using FileStream stream = new FileStream(path.RelativePath, FileMode.Create);
        XmlSerializer serializer = new XmlSerializer(typeof(SerializedInventory));
        serializer.Serialize(stream, inventory);
        stream.Close();
    }

}