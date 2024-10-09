using Munitions.ModularMissiles.Descriptors.Support;
[CreateAssetMenu(fileName = "New Modular Missile Support", menuName = "Nebulous/Missiles/Support/Modular Generic")]
public class ModularBaseSupportDescriptorr : BaseSupportDescriptor, IModular
{
    [Header("Modular Components")]
    [SerializeField]
    protected List<ScriptableObject> _modules = new();
    List<ScriptableObject> IModular.Modules => _modules;
}