using Munitions.InstancedDamagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Munitions.Damagers
{
    public class RangeBasedDamageCharacteristic : IDamageCharacteristic
    {

        BeamWarheadDescriptor Warhead;
        SingleRayDamagerSettings Damager;
        public float Range = 0;
        public RangeBasedDamageCharacteristic(SingleRayDamagerSettings damager, BeamWarheadDescriptor warhead, float range = 0)
        {
            Damager = damager;
            Warhead = warhead;
            Range = range;
        }
        float IDamageCharacteristic.ArmorPenetration => Warhead.BeamArmorPenetrationSizeScaling.Evaluate(Warhead.WeightedSocketSize) * Warhead.BeamArmorPenetrationRangeScaling.Evaluate(Range);
        float IDamageCharacteristic.ComponentDamage => Warhead.BeamComponentDamageSizeScaling.Evaluate(Warhead.WeightedSocketSize) * Warhead.BeamComponentDamageRangeScaling.Evaluate(Range);
        float IDamageCharacteristic.HeatDamage => Damager.HeatDamage;
        float IDamageCharacteristic.DamageBrushSize => Damager.DamageBrushSize;
        float IDamageCharacteristic.OverpenetrationDamageMultiplier => Damager.OverpenetrationDamageMultiplier;
        float IDamageCharacteristic.RandomEffectMultiplier => Damager.RandomEffectMultiplier;
        float IDamageCharacteristic.CrewVulnerabilityMultiplier => Damager.CrewVulnerabilityMultiplier;
        float? IDamageCharacteristic.MaxPenetrationDepth => Damager.InternalPenetrationDepth;
        bool IDamageCharacteristic.NeverCrit => Damager.NeverCrit;
        bool IDamageCharacteristic.AlwaysSpreadThroughStructure => Damager.AlwaysSpreadThroughStructure;
        bool IDamageCharacteristic.NeverRicochet => Damager.NeverRicochet;
        bool IDamageCharacteristic.IgnoreEffectiveThickness => Damager.IgnoreEffectiveThickness;
        bool IDamageCharacteristic.NeverOverpen => Damager.NeverOverpen;
    }

    [CreateAssetMenu(fileName = "New SingleRayDamagerSettings", menuName = "Nebulous/Damagers/Single Ray Damager Settings")]
    public class SingleRayDamagerSettings : ScriptableObject
    {
        [Header("Fallback IDamageCharacteristic Damage Values")]
        public float ArmorPenetration;
        public float ComponentDamage;
        [Header("IDamageCharacteristic Damage Flags")]
        public float InternalPenetrationDepth = 1000;
        public float HeatDamage;
        public float DamageBrushSize;
        public float OverpenetrationDamageMultiplier;
        public float RandomEffectMultiplier;
        public float CrewVulnerabilityMultiplier;
        public bool NeverCrit;
        public bool NeverOverpen;
        public bool IgnoreEffectiveThickness;
        public bool NeverRicochet;
        public bool AlwaysSpreadThroughStructure;
        public bool IgnoreDamageReduction;
        public bool AlwaysSpreadDamage;

        public DamageSpreadingMethod SpreadingMethod;

        public IDamageCharacteristic MakeDamageCharacteristic(BeamWarheadDescriptor warhead, float range = 0)
        {
            return new RangeBasedDamageCharacteristic(this, warhead, range);
        }

        public virtual IDamageDealer MakeDamageDealer(BeamWarheadDescriptor warhead, float range = 0)
        {
            IDamageCharacteristic characteristic = MakeDamageCharacteristic(warhead, range);

            return new SingleRayDamager(characteristic, SpreadingMethod, AlwaysSpreadDamage, null, IgnoreDamageReduction);
        }
    }
}
