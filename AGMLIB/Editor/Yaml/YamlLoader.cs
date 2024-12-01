using Lib.Editor.Yaml.Components;
using Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
//using YamlDotNet.Serialization;
using static Steamworks.InventoryItem;


//using static YamlDotNet.Serialization.DeserializerBuilder;

namespace Lib.Editor.Yaml
{

    [HarmonyPatch(typeof(SocketItem), nameof(SocketItem.ButtonLeftClick))]
    class clicktest
    {
        static void Prefix(SocketItem __instance)
        {
            ShipEditorPane _editor = Common.GetVal<ShipEditorPane>(__instance, "_editor");
            if (_editor == null)
                Debug.LogError("editornull");
        }

    }

    public class YamlComp : MonoBehaviour
    {
        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
        public void Freeze(GameObject freeze)
        {
            DontDestroyOnLoad(freeze);
        }
    }


    [HarmonyPatch(typeof(ModRecord), "LoadMod")]
    class ModRecordLoadMod
    {
        static void Postfix(ModRecord __instance)
        {
            //return;
            ModRecord record = __instance;

            string directory = record.Info.FileLocation.Directory;
            string[] yamlFiles = Directory.GetFiles(directory, "*.yaml");
            //Debug.LogError("Loading yamls in" + directory + yamlFiles);

            // Load and process each YAML file
            foreach (string file in yamlFiles)
            {
                continue;
                //Debug.LogError($"Loading YAML from {file}:");
                using (var reader = new StreamReader(file))
                {
                    //var deserializer = new DeserializerBuilder().Build();

                    Dictionary<string, HullComponent> _components = Common.GetVal<Dictionary<string, HullComponent>>(BundleManager.Instance, "_components");

                    YamlHullComponent yamlObject =  new();//deserializer.Deserialize<YamlHullComponent>(reader); // Replace YourYamlClass with your actual class
                    GameObject prefab = new GameObject(yamlObject.ShortUIName);
                    prefab.AddComponent<YamlComp>().Freeze(prefab);

                    prefab.SetActive(false);
                    HullComponent comp = prefab.AddComponent<HullComponent>();
                    
                    //comp.enabled = false;

                    Common.SetVal(comp, "_partKey", yamlObject.PartKey);
                    Common.SetVal(comp, "_maxHealth", yamlObject.MaxHealth);
                    Common.SetVal(comp, "_functioningThreshold", yamlObject.FunctioningThreshold);
                    Common.SetVal(comp, "_damageThreshold", yamlObject.DamageThreshold);
                    Common.SetVal(comp, "_reinforced", yamlObject.Reinforced);
                    Common.SetVal(comp, "_bindTag", yamlObject.BindTag);
                    Common.SetVal(comp, "_dcPriority", yamlObject.DcPriority);
                    Common.SetVal(comp, "_casualtyClass", yamlObject.CasualtyClass);
                    Common.SetVal(comp, "_dcVoiceDisabledOnly", yamlObject.DcVoiceDisabledOnly);
                    Common.SetVal(comp, "_saveKey", yamlObject.SaveKey);
                    Common.SetVal(comp, "_modId", record.Info.UniqueIdentifier);
                    Common.SetVal(comp, "_shortUIName", yamlObject.ShortUIName);
                    //Common.SetVal(comp, "_smallImage", yamlObject.);
                    //Common.SetVal(comp, "_largeImage", yamlObject.);
                    Common.SetVal(comp, "_factionKey", yamlObject.FactionKey);
                    Common.SetVal(comp, "_shortDescription", yamlObject.ShortDescription);
                    Common.SetVal(comp, "_longDescription", yamlObject.LongDescription);
                    Common.SetVal(comp, "_flavorText", yamlObject.FlavorText);
                    Common.SetVal(comp, "_category", yamlObject.Category);
                    Common.SetVal(comp, "_costBreakdownClass", yamlObject.CostBreakdownClass);
                    Common.SetVal(comp, "_compoundingCost", yamlObject.CompoundingCost);
                    Common.SetVal(comp, "_firstInstanceFree", yamlObject.FirstInstanceFree);
                    Common.SetVal(comp, "_compoundingMultiplier", yamlObject.CompoundingMultiplier);
                    Common.SetVal(comp, "_pointCost", yamlObject.PointCost);
                    Common.SetVal(comp, "_type", yamlObject.Type);
                    Common.SetVal(comp, "_size", new Vector3Int(yamlObject.Size.X, yamlObject.Size.Y, yamlObject.Size.X));
                    Common.SetVal(comp, "_interiorOverhang", yamlObject.InteriorOverhang);
                    Common.SetVal(comp, "_removeSocketCap", yamlObject.RemoveSocketCap);
                    Common.SetVal(comp, "_mass", yamlObject.Mass);
                    Common.SetVal(comp, "_bindToTag", yamlObject.BindToTag);
                    Common.SetVal(comp, "_rotateToFit", yamlObject.RotateToFit);
                    Common.SetVal(comp, "_canTile", yamlObject.CanTile);
                    Common.SetVal(comp, "Modifiers", yamlObject.Modifiers.ConvertAll(yamlstat => new StatModifier(yamlstat.StatName, yamlstat.Literal, yamlstat.Modifier)).ToArray());
                    Common.SetVal(comp, "_resourceDemandPriority", yamlObject.ResourceDemandPriority);



                    ResourceModifier ConvertYamlToNeb(YamlResourceModifier yaml)
                    {
                        //ResourceModifier modifier = new ResourceModifier();


                        // Create a JSON string representing the struct
                        string json = $@"{{
                                ""_resourceName"": ""{yaml.ResourceName}"",
                                ""_amount"": {yaml.Amount},
                                ""_perUnit"": {yaml.PerUnit.ToString().ToLower()},
                                ""_onlyWhenOperating"": {yaml.OnlyWhenOperating.ToString().ToLower()}
                            }}";
                        Debug.Log("JSON String: " + json);
                        string yamlString = $@"
                        _resourceName: '{yaml.ResourceName}'
                        _amount: {yaml.Amount}
                        _perUnit: {yaml.PerUnit}
                        _onlyWhenOperating: {yaml.OnlyWhenOperating}

                        ";

                        // Deserialize the JSON string into a ResourceModifier instance
                        ResourceModifier modifier = JsonUtility.FromJson<ResourceModifier>(json);
                        //ResourceModifier modifier = deserializer.Deserialize<ResourceModifier>(yamlString);

                        Debug.LogError("Resource: " + modifier.ResourceName);
                        return modifier;
                    }

                    Common.SetVal(comp, "ResourcesProvided", yamlObject.ResourcesProvided.ConvertAll(resource => ConvertYamlToNeb(resource)).ToArray());
                    Common.SetVal(comp, "ResourcesRequired", yamlObject.ResourcesRequired.ConvertAll(resource => ConvertYamlToNeb(resource)).ToArray());
                    Common.SetVal(comp, "_uniqueDebuffs", Array.Empty<ComponentDebuff>());
                    Common.SetVal(comp, "_rareDebuffChance", yamlObject.RareDebuffChance);
                    Common.SetVal(comp, "_rareDebuffSubtype", yamlObject.RareDebuffSubtype);
                    //Common.SetVal(comp, "_debuffProbability", yamlObject.);
                    //Common.SetVal(comp, "replace", yamlObject.);
                    //prefab.SetActive(true);

                    // Process the loaded YAML object
                    Debug.LogError(yamlObject.ToString()); // Or use specific properties of your YAML object
                    //foreach (HullComponent pcomp in BundleManager.Instance.AllComponents)
                    //    Debug.Log(pcomp.ComponentName + " bndl " + pcomp.SaveKey);
                    Debug.LogError("Saving " + yamlObject.SaveKey);
                    _components.Add(yamlObject.SaveKey, comp);
                    Common.SetVal(BundleManager.Instance, "_components", _components);
                    foreach (HullComponent component in BundleManager.Instance.AllComponents)
                        Debug.Log("plt " + component.name + "  " + component.SaveKey + " " + component.Type + " " + component.Category);

                }
                try
                {

                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error loading YAML file {file}: {ex.Message}");
                }
            }
        }
    }
}
