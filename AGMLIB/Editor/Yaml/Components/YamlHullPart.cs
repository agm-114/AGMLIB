using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Serialization;
using Priority = Ships.Priority;

namespace Lib.Editor.Yaml.Components
{
    public class YamlHullPart
    {
        public string PartKey = ShortGuid.NewGuid().ToString();
        public float MaxHealth = 100f;
        public float FunctioningThreshold = 10f;
        public float DamageThreshold = 15f;
        public bool Reinforced = false;
        public string BindTag = "";
        public Priority DcPriority = Priority.Medium;
        public CasualtyType CasualtyClass = CasualtyType.CombatSystems;
        public bool DcVoiceDisabledOnly = false;
    }
}
