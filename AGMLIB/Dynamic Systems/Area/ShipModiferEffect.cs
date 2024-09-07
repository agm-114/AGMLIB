using Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGMLIB.Dynamic_Systems.Area
{

    public class ShipModiferEffect : FalloffEffect<Ship>
    {
        protected List<StatModifier> Modifiers = new(1);
        protected List<FreeModifierSource> _modifierssources = new();

        public override void Enter(Ship target)
        {
            if (!_active)
            {
                Exit(target);
                return;
            }

            foreach (FreeModifierSource freeModifierSource in _modifierssources)
            {
                StatModifier statModifier = freeModifierSource.Modifier;
                if (UseFallOff)
                {
                    float falloff = DistanceCurve.Evaluate(Range / AreaEffect.Radius);
                    statModifier = new(statModifier.StatName, statModifier.Literal * falloff, statModifier.Modifier * falloff);
                }
                target.AddStatModifier(freeModifierSource, statModifier);
            }
        }

        public override void Exit(Ship ship)
        {
            foreach (FreeModifierSource freeModifierSource in _modifierssources)
                ship.RemoveStatModifier(freeModifierSource, freeModifierSource.Modifier.StatName);
        }
    }
}
