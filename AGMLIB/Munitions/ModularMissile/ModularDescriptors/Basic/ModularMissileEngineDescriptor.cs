using Munitions.ModularMissiles.Descriptors;

[CreateAssetMenu(fileName = "New Modular Missile Engine", menuName = "Nebulous/Missiles/Modular Engine Component")]
public class ModularMissileEngineDescriptor : MissileEngineDescriptor, IModular
{
    [Header("Modular Components")]
    [SerializeField]
    protected List<ScriptableObject> _modules = new();
    List<ScriptableObject> IModular.Modules => _modules;
}