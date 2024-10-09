using Munitions.ModularMissiles.Descriptors.Warheads;
[CreateAssetMenu(fileName = "New Modular Missile Warhead", menuName = "Nebulous/Missiles/Warhead/Modular Kinetic Penetrator")]
public class ModularKineticPenetratorWarheadDescriptor : KineticPenetratorWarheadDescriptor, IModular
{
    [Header("Modular Components")]
    [SerializeField]
    protected List<ScriptableObject> _modules = new();
    List<ScriptableObject> IModular.Modules => _modules;
}