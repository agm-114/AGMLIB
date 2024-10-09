[CreateAssetMenu(fileName = "New Modular Position Seeker", menuName = "Nebulous/Missiles/Seekers/Modular Position")]
public class ModularPositionSeekerDescriptor : PositionSeekerDescriptor, IModular
{
    [Header("Modular Components")]
    [SerializeField]
    protected List<ScriptableObject> _modules = new();
    List<ScriptableObject> IModular.Modules => _modules;
}

[CreateAssetMenu(fileName = "New Modular Position Seeker", menuName = "Nebulous/Missiles/Seekers/Modular Range Based Command")]
public class ModularRangedCommandSeekerDescriptor : RangedCommandSeekerDescriptor, IModular
{
    [Header("Modular Components")]
    [SerializeField]
    protected List<ScriptableObject> _modules = new();
    List<ScriptableObject> IModular.Modules => _modules;
}