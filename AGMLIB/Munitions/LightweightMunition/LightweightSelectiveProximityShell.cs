using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Munitions;
using UnityEngine;

namespace AGMLIB.Munitions.LightweightMunition
{



    public class LightweightSelectiveProximityShell : LightweightProximityShell
    {
        [Header("Selective Shell")]

        [SerializeField]
        private float _triggerRadius = 1f;


        [Flags]

        public enum FuseOptions
        {
            Generic = 1,
            Ships = 512,
            Missiles = 524288,
        }
        public FuseOptions SelectedFuseOptions = FuseOptions.Generic | FuseOptions.Ships | FuseOptions.Missiles;

        public override bool DoLookAhead(Vector3 position, Quaternion rotation, Vector3 velocity, out RaycastHit hit, out bool isTrigger)
        {
            Ray ray = new Ray(position + velocity.normalized * _triggerRadius, velocity.normalized);
            if (Physics.SphereCast(ray, _triggerRadius, out hit, velocity.magnitude * Time.fixedDeltaTime, (int)SelectedFuseOptions, QueryTriggerInteraction.Ignore))
            {
                isTrigger = true;
                return true;
            }

            isTrigger = false;
            return false;
        }
    }

    [CreateAssetMenu(fileName = "New LW Mod Selective Prox Shell", menuName = "Nebulous/LW Shells/Modular Selective Proximity Shell")]
    public class ModularLightweightSelectiveProximityShell : LightweightSelectiveProximityShell, IModular
    {
        [Header("Modular Components")]
        [SerializeField]
        protected List<ScriptableObject> _modules = new();
        List<ScriptableObject> IModular.Modules => _modules;
    }
}
