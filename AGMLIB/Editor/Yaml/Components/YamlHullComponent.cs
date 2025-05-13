using static Ships.HullComponent;
using Priority = Ships.Priority;

namespace Lib.Editor.Yaml.Components
{
    public struct YamlVector3Int
    {
        public int X;
        public int Y;
        public int Z;
    }

    public struct YamlStatModifier
    {
        public string StatName;
        public float Literal;
        public float Modifier;
        public bool Permanent;

    }
    public struct YamlResourceModifier
    {
        public string ResourceName = "";
        public int Amount = 1;
        public bool PerUnit = true;
        public bool OnlyWhenOperating = true;

        public YamlResourceModifier()
        {
        }
    }



    public class YamlHullComponent : YamlHullPart
    {

        public string SaveKey = "";
        //private ulong _modId;
        public string ShortUIName = "";
        //private Sprite _smallImage;
        //private Sprite _largeImage;
        public string FactionKey = "";
        public string ShortDescription = "";
        public string LongDescription = "";
        public string FlavorText = "";
        public string Category = "";
        public ComponentCostClass CostBreakdownClass = ComponentCostClass.Other;
        public bool CompoundingCost = false;
        public string CompoundingCostClass = "";
        public bool FirstInstanceFree = false;
        public float CompoundingMultiplier = 1f;
        public int PointCost = 0;
        public HullSocketType Type = HullSocketType.Module;
        public YamlVector3Int Size;
        public Sides InteriorOverhang = Sides.None;
        public bool RemoveSocketCap = false;
        public float Mass = 0;
        public string BindToTag = "";
        public RotateAxis RotateToFit = RotateAxis.None;
        public bool CanTile = false;
        public YamlStatModifier[] Modifiers = [];
        public Priority ResourceDemandPriority = Priority.Medium;
        public YamlResourceModifier[] ResourcesProvided = [];
        public YamlResourceModifier[] ResourcesRequired = [];
        //private ComponentDebuff[] _uniqueDebuffs = [];
        public float RareDebuffChance = 0.05f;
        public string RareDebuffSubtype = "";
        //private AnimationCurve _debuffProbability = new AnimationCurve(new Keyframe(0f, 0f, 0f, 0.25f), new Keyframe(1f, 0.5f, 1f, 0f));
    }
}
