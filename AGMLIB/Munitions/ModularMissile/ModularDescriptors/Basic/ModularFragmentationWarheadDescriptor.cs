using UnityEngine;
using System.Collections.Generic;
using Munitions.ModularMissiles.Descriptors.Warheads;
[CreateAssetMenu(fileName = "New Modular Missile Warhead", menuName = "Nebulous/Missiles/Warhead/Modular Fragmentation")]
public class ModularFragmentationWarheadDescriptor : FragmentationWarheadDescriptor, IModular
{
    [Header("Modular Components")]
    [SerializeField]
    protected List<ScriptableObject> _modules = new();
    List<ScriptableObject> IModular.Modules => _modules;
}