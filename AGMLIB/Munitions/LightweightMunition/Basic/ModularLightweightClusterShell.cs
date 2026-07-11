namespace AGMLIB.Munitions.LightweightMunition.Basic
{
    [CreateAssetMenu(fileName = "New LW Modular Cluster Shell", menuName = "Nebulous/LW Shells/Modular Cluster Shell")]
    public class ModularLightweightClusterShell : LightweightClusterShell, IModular
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
