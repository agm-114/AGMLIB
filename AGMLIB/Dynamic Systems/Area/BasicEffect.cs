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
    public class BasicEffect<TargetObject> : ShipState where TargetObject : MonoBehaviour
    {
        [HideInInspector]
        public AreaEffect AreaEffect;
        public virtual bool RequireUpdate => false;
        public virtual bool RequireRange => false;

        public float Range = 0;
        protected bool _active => AreaEffect.active;

        public virtual void Enter(TargetObject target)
        {
            
        }
        public virtual void AreaEffectStateChange()
        {

        }
        public virtual void Exit(TargetObject target)
        {

        }



    }
}
