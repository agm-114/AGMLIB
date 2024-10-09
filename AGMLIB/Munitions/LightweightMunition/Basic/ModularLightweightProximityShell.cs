namespace AGMLIB.Munitions.LightweightMunition.Basic
{
    [CreateAssetMenu(fileName = "New LW Modular Prox Shell", menuName = "Nebulous/LW Shells/Modular Proximity Shell")]
    public class ModularLightweightProximityShell : LightweightProximityShell, IModular
    {
        [Header("Modular Components")]
        [SerializeField]
        protected List<ScriptableObject> _modules = new();
        List<ScriptableObject> IModular.Modules => _modules;
    }
}
