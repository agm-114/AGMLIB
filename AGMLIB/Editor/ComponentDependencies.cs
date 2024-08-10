using Munitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ComponentDependencies : MonoBehaviour
{

    public bool InstallDepedenents = true;
    public bool InstallRequirements = true;
    public bool HardInstallDepedenents = true;
    public bool HardInstallRequirements = true;
    public bool RemoveDepedenents = true;
    public bool RemoveRequirements = true;

    public List<String> DependendentSocketKeys = new();
    public List<String> DependendentComponentKeys = new();
    public List<String> RequirementSocketKeys = new();
    public List<String> RequirementComponentKeys = new();

    public IEnumerable<KeyValuePair<String, String>> Dependendents => DependendentSocketKeys.Zip(DependendentComponentKeys, (key, value) => new KeyValuePair<String, String>(key, value));
    public IEnumerable<KeyValuePair<String, String>> Requirements => RequirementSocketKeys.Zip(RequirementComponentKeys, (key, value) => new KeyValuePair<String, String>(key, value));

}