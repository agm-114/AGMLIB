using Game.Map;
using Missions;
using Munitions.ModularMissiles;
using Munitions.ModularMissiles.Descriptors;
using SmallCraft;

namespace AGMLIB.Generic_Gameplay
{
    public class PrefabVisualEffectFactory : MonoBehaviour
    {
        public BundleLocation Location = BundleLocation.Auto;
        public ComponentName SourceComponent = ComponentName.Root;
        public FieldName SourceFieldName = FieldName.Auto;
        public int SourceComponentIndex = 0;
        public string SourceSaveKey = "Stock/Mk68 Cannon";
        public string ChildGameObjectName = "";


        public enum BundleLocation
        {
            Auto,
            Hulls,
            Components,
            Debuffs,
            Missiles,
            Spaceframes,
            Munitions
           
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
            Root,
            DynamicVisibleParticles,
            BeamMuzzleEffects,
            Muzzle,
            HullPart,
            HullComponent,
            ModularMissile,
            ThrusterPart
        }
        private VisualEffect effect;

        public MonoBehaviour TargetComponent;
        public FieldName TargetFieldName = FieldName._particles;

        private BundleManager Bundle => BundleManager.Instance;
        private Dictionary<string, BaseHull> _hulls => Common.GetVal<Dictionary<string, BaseHull>>(Bundle, "_hulls");
        private Dictionary<string, HullComponent> _components => Common.GetVal<Dictionary<string, HullComponent>>(Bundle, "_components");
        private Dictionary<string, ModularMissile> _missileBodies => Common.GetVal<Dictionary<string, ModularMissile>>(Bundle, "_missileBodies");
        private Dictionary<string, Spacecraft> _spaceframes => Common.GetVal<Dictionary<string, Spacecraft>>(Bundle, "_spaceframes");
        private Dictionary<string, IMunition> _munitionsBySaveKey => Common.GetVal<Dictionary<string, IMunition>>(Bundle, "_munitionsBySaveKey");


        public MonoBehaviour VisualEffectDestination;


        protected MonoBehaviour GetComponentFromSpecificBundle(BundleLocation location, string key)
        {
            switch (location)
            {
                case BundleLocation.Hulls:
                    if (_hulls.TryGetValue(key, out var hull)) return hull;
                    break;
                case BundleLocation.Components:
                    if (_components.TryGetValue(key, out var comp)) return comp;
                    break;
                case BundleLocation.Missiles:
                    if (_missileBodies.TryGetValue(key, out var missile)) return missile;
                    break;
                case BundleLocation.Spaceframes:
                    if (_spaceframes.TryGetValue(key, out var frame)) return frame;
                    break;
                case BundleLocation.Munitions:
                    if (_munitionsBySaveKey.TryGetValue(key, out var munition) && munition is MonoBehaviour munMono) return munMono;
                    break;
            }
            return null;
        }

        private MonoBehaviour ScanAllBundlesForFirstComponent(string key)
        {
            if (_components.TryGetValue(key, out var comp) && comp != null) return comp;
            if (_hulls.TryGetValue(key, out var hull) && hull != null) return hull;
            if (_munitionsBySaveKey.TryGetValue(key, out var munition) && munition is MonoBehaviour munMono) return munMono;
            if (_missileBodies.TryGetValue(key, out var missile) && missile != null) return missile;
            if (_spaceframes.TryGetValue(key, out var frame) && frame != null) return frame;

            return null;
        }
        private T GetElementSafe<T>(T[] array, int index) where T : MonoBehaviour
        {
            if (array != null && index >= 0 && index < array.Length)
            {
                return array[index];
            }
            return null;
        }

        protected MonoBehaviour GetSpecificComponent(GameObject target, ComponentName componentType, int index)
        {
            switch (componentType)
            {
                case ComponentName.Root:
                    return target.GetComponent<MonoBehaviour>();
                case ComponentName.DynamicVisibleParticles:
                    return GetElementSafe(target.GetComponentsInChildren<DynamicVisibleParticles>(true), index);
                case ComponentName.BeamMuzzleEffects:
                    return GetElementSafe(target.GetComponentsInChildren<BeamMuzzleEffects>(true), index);

                case ComponentName.Muzzle:
                    return GetElementSafe(target.GetComponentsInChildren<Muzzle>(true), index);

                case ComponentName.HullPart:
                    return GetElementSafe(target.GetComponentsInChildren<HullPart>(true), index);

                case ComponentName.HullComponent:
                    return GetElementSafe(target.GetComponentsInChildren<HullComponent>(true), index);

                case ComponentName.ModularMissile:
                    return GetElementSafe(target.GetComponentsInChildren<ModularMissile>(true), index);

                case ComponentName.ThrusterPart:
                    return GetElementSafe(target.GetComponentsInChildren<ThrusterPart>(true), index);

                default:
                    //write some error code here
                    return null;
            }
        }
        protected GameObject FindChildGameObject(GameObject parent, string childName)
        {
            if (string.IsNullOrEmpty(childName))
                return parent;
            Transform childTransform = parent.transform.Find(childName);
            if (childTransform != null)
                return childTransform.gameObject;
            Common.LogPatch($"Child GameObject '{childName}' not found under '{parent.name}'");
            return null;
        }
        public void Awake()
        {
            MonoBehaviour sourceComponent = null;
            if (Location != BundleLocation.Auto)
            {
                sourceComponent = GetComponentFromSpecificBundle(Location, SourceSaveKey);
            }
            else
            {
                sourceComponent = ScanAllBundlesForFirstComponent(SourceSaveKey);
            }

            if(SourceComponent != ComponentName.Root)
            {
                GameObject target = sourceComponent.gameObject;
                GameObject? child = null;
                if(ChildGameObjectName != "")
                {
                    child = FindChildGameObject(target, ChildGameObjectName);
                    if (child != null)
                        target = child;
                }
                if (sourceComponent == null)
                {
                    Common.LogPatch($"Failed to find component {SourceComponent} on GameObject {target.name}");
                    return;
                }
                sourceComponent = GetSpecificComponent(target, SourceComponent, SourceComponentIndex) ?? sourceComponent;
                //switch statement to find component based on value
            }
            
            VisualEffect? effect;
            DynamicVisibleParticles? visibleParticles;
            effect = Common.GetVal<VisualEffect>(sourceComponent, SourceFieldName.ToString());

            if (effect == null)
            {
                visibleParticles = Common.GetVal<DynamicVisibleParticles>(sourceComponent, SourceFieldName.ToString());
                if (visibleParticles != null)
                    effect = Common.GetVal<VisualEffect>(visibleParticles, FieldName._particles.ToString());
            }

            if (effect == null)
            {
                Common.LogPatch($"Failed to find VisualEffect in {sourceComponent} using field name {SourceFieldName}");
                return;
            }

            if (VisualEffectDestination is DynamicVisibleParticles)
            {
                Common.SetVal(VisualEffectDestination, FieldName._particles.ToString(), effect);
            }
            else
            {
                Common.SetVal(VisualEffectDestination, TargetFieldName.ToString(), effect);
            }
        }

        
    }

}
