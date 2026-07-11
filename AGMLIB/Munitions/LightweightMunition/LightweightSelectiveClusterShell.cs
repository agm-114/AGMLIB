namespace AGMLIB.Munitions.LightweightMunition
{
    public class LightweightSelectiveClusterShell : LightweightClusterShell
    {

        [SerializeField]
        private float TriggerRadius => Common.GetVal<float>(this, "_lookaheadSphereRadius");


        [Flags]

        public enum FuseOptions
        {
            Generic = 1,
            Ships = 512,
            Missiles = 524288,
        }
        [Header("Selective Shell")]
        public FuseOptions SelectedFuseOptions = FuseOptions.Generic | FuseOptions.Ships | FuseOptions.Missiles;

        public override bool DoLookAhead(Vector3 position, Quaternion rotation, Vector3 velocity, out RaycastHit hit, out bool isTrigger)
        {
            isTrigger = false;
            Ray ray = new Ray(position + velocity.normalized * TriggerRadius, velocity.normalized);
            return Physics.SphereCast(ray, TriggerRadius, out hit, velocity.magnitude * Time.fixedDeltaTime, (int)SelectedFuseOptions, QueryTriggerInteraction.Ignore);
        }
    }

    [CreateAssetMenu(fileName = "New LW Mod Selective Cluster Shell", menuName = "Nebulous/LW Shells/Modular Selective Clusterd Shell")]
    public class ModularLightweightSelectiveClusterShell : LightweightSelectiveClusterShell, IModular
    {
        [Header("Modular Components")]
        [SerializeField]
        protected List<ScriptableObject> _modules = new();
        List<ScriptableObject> IModular.Modules => _modules;

        public override bool DamageableImpact(LightweightKineticMunitionContainer attachedTo, IDamageable hitObject, MunitionHitInfo hitInfo, bool trigger, out HitResult hitRes, out float damageDone, out bool targetDestroyed, out Vector3 repoolPosition)
        {
            bool result = base.DamageableImpact(attachedTo, hitObject, hitInfo, trigger, out hitRes, out damageDone, out targetDestroyed, out repoolPosition);
            this.HandleDamageableImpact(attachedTo, hitObject, hitInfo, trigger, hitRes, damageDone, targetDestroyed);
            return result;
        }

        public override bool GenericImpact(LightweightKineticMunitionContainer attachedTo, MunitionHitInfo hitInfo, bool trigger, out HitResult hitRes, out Vector3 repoolPosition)
        {
            bool result = base.GenericImpact(attachedTo, hitInfo, trigger, out hitRes, out repoolPosition);
            this.HandleGenericImpact(attachedTo, hitInfo, trigger, hitRes);
            return result;
        }
    }
}
