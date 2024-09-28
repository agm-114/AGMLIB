using Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AGMLIB.Dynamic_Systems.Area
{

    public class ShipModiferEffect : FalloffEffect<Ship>
    {
        public List<StatModifier> Modifiers;
        protected List<FreeModifierSource> _modifierssources = new();
        protected Guid Guid = Guid.NewGuid();

         public override void Setup()
        {
            Debug.LogError("ShipModiferEffect Setup " + Modifiers.Count());
            foreach (StatModifier modifier in Modifiers)
            {
                _modifierssources.Add(new FreeModifierSource(Guid.ToString() + modifier.StatName, modifier));
                Debug.LogError("Setup: " + modifier.StatName.ToString());

            }
        }


        public override void ApplyEffect(Ship target)
        {
            Debug.LogError(target.ShipDisplayName + " Ship Being Debuffed");
            if (!Active)
            {
                Debug.LogError("Ship not active");

                ClearEffect(target);
                return;
            }

            foreach (FreeModifierSource freeModifierSource in _modifierssources)
            {
                StatModifier statModifier = freeModifierSource.Modifier;
                Debug.LogError("Applying: " + statModifier.StatName.ToString());

                if (UseFallOff)
                {
                    float falloff = DistanceCurve.Evaluate(Range / AreaEffect.Radius);
                    statModifier = new(statModifier.StatName, statModifier.Literal * falloff, statModifier.Modifier * falloff);
                }
                Debug.LogError(statModifier.ToString());

                target.AddStatModifier(freeModifierSource, statModifier);
            }
        }

        public override void ClearEffect(Ship ship)
        {
            foreach (FreeModifierSource freeModifierSource in _modifierssources)
                ship.RemoveStatModifier(freeModifierSource, freeModifierSource.Modifier.StatName);
        }
    }
}
