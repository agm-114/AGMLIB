[CreateAssetMenu(fileName = "New Modular Beam Warhead", menuName = "Nebulous/Missiles/Warhead/Modular Beam")]
public class ModularBeamWarheadDescriptor : BeamWarheadDescriptor, IModular
{
    [Header("Modular Components")]
    [SerializeField]
    protected List<ScriptableObject> _modules = new();
    List<ScriptableObject> IModular.Modules => _modules;
}