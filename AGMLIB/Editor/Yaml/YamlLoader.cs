using Lib.Editor.Yaml.Components;
using Modding;
//using YamlDotNet.Serialization;


//using static YamlDotNet.Serialization.DeserializerBuilder;

namespace Lib.Editor.Yaml
{

    [HarmonyPatch(typeof(SocketItem), nameof(SocketItem.ButtonLeftClick))]
    class clicktest
    {
        static void Prefix(SocketItem __instance)
        {
            Common.LogPatch();
            ShipEditorPane _editor = __instance.Internals().Editor;
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
            Common.LogPatch();
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

                    Dictionary<string, HullComponent> _components = BundleManager.Instance?.Internals().Components;

                    YamlHullComponent yamlObject = new();//deserializer.Deserialize<YamlHullComponent>(reader); // Replace YourYamlClass with your actual class
                    GameObject prefab = new GameObject(yamlObject.ShortUIName);
                    prefab.AddComponent<YamlComp>().Freeze(prefab);

                    prefab.SetActive(false);
                    HullComponent comp = prefab.AddComponent<HullComponent>();

                    //comp.enabled = false;

                    ((HullPart)comp).Internals().PartKey = yamlObject.PartKey;
                    ((HullPart)comp).Internals().MaxHealth = yamlObject.MaxHealth;
                    ((HullPart)comp).Internals().FunctioningThreshold = yamlObject.FunctioningThreshold;
                    ((HullPart)comp).Internals().DamageThreshold = yamlObject.DamageThreshold;
                    ((HullPart)comp).Internals().Reinforced = yamlObject.Reinforced;
                    ((HullPart)comp).Internals().BindTag = yamlObject.BindTag;
                    ((HullPart)comp).Internals().DcPriority = yamlObject.DcPriority;
                    ((HullPart)comp).Internals().CasualtyClass = yamlObject.CasualtyClass;
                    ((HullPart)comp).Internals().DcVoiceDisabledOnly = yamlObject.DcVoiceDisabledOnly;
                    comp.Internals().SaveKey = yamlObject.SaveKey;
                    comp.Internals().ModId = record.Info.UniqueIdentifier;
                    comp.Internals().ShortUIName = yamlObject.ShortUIName;
                    //Common.SetVal(comp, "_smallImage", yamlObject.);
                    //Common.SetVal(comp, "_largeImage", yamlObject.);
                    comp.Internals().FactionKey = yamlObject.FactionKey;
                    comp.Internals().ShortDescription = yamlObject.ShortDescription;
                    comp.Internals().LongDescription = yamlObject.LongDescription;
                    comp.Internals().FlavorText = yamlObject.FlavorText;
                    comp.Internals().Category = yamlObject.Category;
                    comp.Internals().CostBreakdownClass = yamlObject.CostBreakdownClass;
                    comp.Internals().CompoundingCost = yamlObject.CompoundingCost;
                    comp.Internals().FirstInstanceFree = yamlObject.FirstInstanceFree;
                    comp.Internals().CompoundingMultiplier = yamlObject.CompoundingMultiplier;
                    comp.Internals().PointCost = yamlObject.PointCost;
                    comp.Internals().Type = yamlObject.Type;
                    comp.Internals().Size = new Vector3Int(yamlObject.Size.X, yamlObject.Size.Y, yamlObject.Size.X);
                    comp.Internals().InteriorOverhang = yamlObject.InteriorOverhang;
                    comp.Internals().RemoveSocketCap = yamlObject.RemoveSocketCap;
                    comp.Internals().Mass = yamlObject.Mass;
                    comp.Internals().BindToTag = yamlObject.BindToTag;
                    comp.Internals().RotateToFit = yamlObject.RotateToFit;
                    comp.Internals().CanTile = yamlObject.CanTile;
                    comp.Modifiers = yamlObject.Modifiers.ConvertAll(yamlstat => new StatModifier(yamlstat.StatName, yamlstat.Literal, yamlstat.Modifier)).ToArray();
                    ((HullPartResourceConnected)comp).Internals().ResourceDemandPriority = yamlObject.ResourceDemandPriority;



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

                    comp.ResourcesProvided = yamlObject.ResourcesProvided.ConvertAll(resource => ConvertYamlToNeb(resource)).ToArray();
                    comp.ResourcesRequired = yamlObject.ResourcesRequired.ConvertAll(resource => ConvertYamlToNeb(resource)).ToArray();
                    comp.Internals().UniqueDebuffs = Array.Empty<ComponentDebuff>();
                    comp.Internals().RareDebuffChance = yamlObject.RareDebuffChance;
                    comp.Internals().RareDebuffSubtype = yamlObject.RareDebuffSubtype;
                    //Common.SetVal(comp, "_debuffProbability", yamlObject.);
                    //Common.SetVal(comp, "replace", yamlObject.);
                    //prefab.SetActive(true);

                    // Process the loaded YAML object
                    Debug.LogError(yamlObject.ToString()); // Or use specific properties of your YAML object
                    //foreach (HullComponent pcomp in BundleManager.Instance.AllComponents)
                    //    Debug.Log(pcomp.ComponentName + " bndl " + pcomp.SaveKey);
                    Debug.LogError("Saving " + yamlObject.SaveKey);
                    _components.Add(yamlObject.SaveKey, comp);
                    BundleManager.Instance.Internals().Components = _components;
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
