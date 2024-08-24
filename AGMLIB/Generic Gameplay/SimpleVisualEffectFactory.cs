using Bundles;
using Mono.Cecil;
using Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;
using Utility;

namespace AGMLIB.Generic_Gameplay
{
    public class SimpleVisualEffectFactory : MonoBehaviour
    {
        public PrefabType SourceType = PrefabType.HullComponent;
        public ComponentName SourceComponent = ComponentName.HullComponent;
        public FieldName SourceFieldName = FieldName.Auto;
        public int SourceComponentIndex = 0;
        public string SourceSaveKey = "Stock/Mk68 Cannon";

        public enum PrefabType
        {
            HullComponent
        }

        public enum FieldName
        {
            Auto,
            _particles,
            _flash,
            _muzzleEffects,
            _hitEffects,
            _disabledParticles,
        }
        public enum ComponentName
        {
            DynamicVisibleParticles,
            RezzingMuzzle,
            BeamMuzzleEffects,
            ContinuousRaycastMuzzle,
            PulsedRaycastMuzzle,
            SinglePulseRaycastMuzzle,
            HullPart,
            HullComponent,
            ThrusterPart
        }
        private VisualEffect effect;

        public MonoBehaviour TargetComponent;
        public FieldName TargetFieldName = FieldName._particles;

        MonoBehaviour VisualEffectDestination;
        public void Awake ()
        {
            HullComponent componentSource;
            Dictionary<string, HullComponent> componentDictionary = Common.GetVal<Dictionary<string, HullComponent>>(BundleManager.Instance, "_components");
            componentDictionary.TryGetValue(SourceSaveKey, out componentSource);
            
            GameObject target = componentSource.gameObject;


            
        }
    }

   
}
