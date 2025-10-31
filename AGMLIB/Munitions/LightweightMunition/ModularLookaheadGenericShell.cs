namespace AGMLIB.Munitions.LightweightMunition
{
    public class LightweightGenericShell : LightweightKineticShell
    {
        public override float MaxComponentDamagePotential => Mathf.Max(DamageableImpactShell?.MaxComponentDamagePotential ?? 0, TriggerImpactShell?.MaxComponentDamagePotential ?? 0);
        public override bool CustomLookaheadMethod => DoLookaheadShell != null;

        [Header("Generic Shell")]
        [SerializeField]
        protected float _armDelay = 0.5f;

        LightweightKineticShell DamageableImpactShell;
        LightweightKineticShell TriggerImpactShell;
        LightweightKineticShell GenericImpactShell;
        LightweightKineticShell DoLookaheadShell;

        [TextArea(5, 10)]
        public string CustomDamageText = "";

        [TextArea(5, 10)]
        public string ExtraDamageText = "";


        protected override string GetDamageStatsText()
        {
            if (CustomDamageText.Length > 0)
                return CustomDamageText;
            return base.GetDamageStatsText() + ExtraDamageText;
        }


        public override bool DoLookAhead(Vector3 position, Quaternion rotation, Vector3 velocity, out RaycastHit hit, out bool isTrigger)
        {
            if (!CustomLookaheadMethod)
                return base.DoLookAhead(position, rotation, velocity, out hit, out isTrigger);
            return (DoLookaheadShell ?? TriggerImpactShell).DoLookAhead(position, rotation, velocity, out hit, out isTrigger);
        }
        public override bool DamageableImpact(LightweightKineticMunitionContainer attachedTo, IDamageable hitObject, MunitionHitInfo hitInfo, bool trigger, out HitResult hitRes, out float damageDone, out bool targetDestroyed, out Vector3 repoolPosition)
        {
            if (attachedTo.TimeElapsed < _armDelay)
            {
                damageDone = 0f;
                repoolPosition = Vector3.zero;
                hitRes = HitResult.None;
                targetDestroyed = false;
                return false;
            }
            if (trigger)
                return TriggerImpactShell.DamageableImpact(attachedTo, hitObject, hitInfo, trigger, out hitRes, out damageDone, out targetDestroyed, out repoolPosition);
            return TriggerImpactShell.DamageableImpact(attachedTo, hitObject, hitInfo, trigger, out hitRes, out damageDone, out targetDestroyed, out repoolPosition);
        }

        public override bool GenericImpact(LightweightKineticMunitionContainer attachedTo, MunitionHitInfo hitInfo, bool trigger, out HitResult hitRes, out Vector3 repoolPosition) => (GenericImpactShell ?? DamageableImpactShell).GenericImpact(attachedTo, hitInfo, trigger, out hitRes, out repoolPosition);

    }

    [CreateAssetMenu(fileName = "New LW Modular GenericShell Shell", menuName = "Nebulous/LW Shells/Modular Generic Shell")]
    public class ModularLightweightGenericShell : LightweightGenericShell, IModular
    {
        [Header("Modular Components")]
        [SerializeField]
        protected List<ScriptableObject> _modules = new();
        List<ScriptableObject> IModular.Modules => _modules;
    }
}
