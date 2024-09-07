using Game.EWar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AGMLIB.Dynamic_Systems.Area
{
    public class FalloffEffect<TargetObject> : BasicEffect<TargetObject>where TargetObject : MonoBehaviour
    {

        public AnimationCurve DistanceCurve = new(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
        public bool UseFallOff = false;
        void Start()
        {
            DistanceCurve.preWrapMode = WrapMode.Clamp;
            DistanceCurve.postWrapMode = WrapMode.Clamp;

        }
    }
}
