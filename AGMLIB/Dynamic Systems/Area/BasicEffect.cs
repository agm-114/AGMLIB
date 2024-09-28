using Munitions.ModularMissiles;
using Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace AGMLIB.Dynamic_Systems.Area
{
    public class BasicEffect : MonoBehaviour
    {
        [HideInInspector]
        public AreaEffect AreaEffect;
        public virtual bool RequireUpdate => false;
        public virtual bool RequireRange => false;

        public float Range = 0;
        protected bool Active => AreaEffect.active;

        public virtual void AreaUpdate()
        {

        }
        public virtual void Enter(MonoBehaviour target)
        {

        }

        public virtual void Exit(MonoBehaviour target)
        {

        }

        public virtual void Setup()
        {

        }
        public virtual void FixedUpdate()
        {

        }
    }

    public class GenericBasicEffect<TargetObject> : BasicEffect where TargetObject : MonoBehaviour
    {


        [HideInInspector]
        public HashSet<TargetObject> Targets = new();

        public override void Enter(MonoBehaviour target)
        {
            if (target is TargetObject targetObject)
                Targets.Add(targetObject);
        }

        public virtual void ApplyEffect(TargetObject target)
        {

        }

        public override void AreaUpdate()
        {
            foreach (TargetObject target in Targets)
            {
                if (Active)
                    this.ApplyEffect(target);
                else
                    this.ClearEffect(target);
            }
        }

        public override void FixedUpdate()
        {
            if(Active)
                foreach (TargetObject target in Targets)
                {
                    TargetFixedUpdate(target);
                }
        }

        public virtual void TargetFixedUpdate(TargetObject target)
        {

        }
        public virtual void ClearEffect(TargetObject target)
        {
            
        }

        public override void Exit(MonoBehaviour target)
        {
            if (target is TargetObject targetObject)
                Targets.Remove(targetObject);
        }



    }
}
