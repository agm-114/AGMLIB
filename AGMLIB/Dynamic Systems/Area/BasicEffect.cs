using Munitions.ModularMissiles;
using Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AGMLIB.Dynamic_Systems.Area
{
    public class BasicEffect : MonoBehaviour
    {
        [HideInInspector]
        public AreaEffect AreaEffect;
        public virtual bool RequireUpdate => false;
        public virtual bool RequireRange => false;

        public float Range = 0;
        protected bool _active => AreaEffect.active;

        public virtual void AreaUpdate()
        {

        }
    }

    public class GenericBasicEffect<TargetObject> : BasicEffect where TargetObject : MonoBehaviour
    {


        [HideInInspector]
        public HashSet<TargetObject> Targets;

        public virtual void Enter(TargetObject target)
        {
            Targets.Add(target);
        }

        public virtual void ApplyEffect(TargetObject target)
        {
            
        }

        public override void AreaUpdate()
        {
            foreach(TargetObject target in Targets)
            {
                if (_active)
                    this.ApplyEffect(target);
                else
                    this.ClearEffect(target);
            }
        }
        public virtual void ClearEffect(TargetObject target)
        {
            
        }

        public virtual void Exit(TargetObject target)
        {
            Targets.Remove(target);
        }



    }
}
