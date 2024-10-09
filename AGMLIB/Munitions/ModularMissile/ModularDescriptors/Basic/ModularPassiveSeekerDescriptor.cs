using Munitions.ModularMissiles.Descriptors.Seekers;
[CreateAssetMenu(fileName = "New Modular Missile Seeker", menuName = "Nebulous/Missiles/Seekers/Modular Passive")]
public class ModularPassiveSeekerDescriptor : PassiveSeekerDescriptor, IModular
{
    [Header("Modular Components")]
    [SerializeField]
    protected List<ScriptableObject> _modules = new();
    List<ScriptableObject> IModular.Modules => _modules;
}