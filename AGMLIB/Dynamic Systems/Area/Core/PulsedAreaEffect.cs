using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AGMLIB.Dynamic_Systems.Area
{
    public class PulsedAreaEffect : AreaEffect
    {
        public float PulseTime = 1;
        private float _accum = 0;

        protected override void FixedUpdate()
        {
            _accum += Time.fixedDeltaTime;
            if (_accum > PulseTime)
            {
                IEnumerable<Collider> array = Physics.OverlapSphere(transform.position, Radius, 524801)?.Distinct(_colliderComparer) ?? new List<Collider>();
                foreach (Collider collider in array)
                    OnTrigger(collider.transform, true);
                _accum = 0;
            }
            base.FixedUpdate();
        }
    }
}
