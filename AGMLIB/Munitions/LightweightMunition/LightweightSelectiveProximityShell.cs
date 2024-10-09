namespace AGMLIB.Munitions.LightweightMunition
{

    public class LightweightSelectiveProximityShell : LightweightProximityShell
    {


        [SerializeField]
        private float TriggerRadius => Common.GetVal<float>(this, "_triggerRadius");


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
            Ray ray = new(position + velocity.normalized * TriggerRadius, velocity.normalized);
            if (Physics.SphereCast(ray, TriggerRadius, out hit, velocity.magnitude * Time.fixedDeltaTime, (int)SelectedFuseOptions, QueryTriggerInteraction.Ignore))
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
