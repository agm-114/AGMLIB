namespace AGMLIB.Munitions.LightweightMunition.Basic
{
    [CreateAssetMenu(fileName = "New LW Modular Cluster Shell", menuName = "Nebulous/LW Shells/Modular Cluster Shell")]
    public class ModularLightweightClusterShell : LightweightClusterShell, IModular
    {
        [Header("Modular Components")]
        [SerializeField]
        protected List<ScriptableObject> _modules = new();
        List<ScriptableObject> IModular.Modules => _modules;
    }
}
