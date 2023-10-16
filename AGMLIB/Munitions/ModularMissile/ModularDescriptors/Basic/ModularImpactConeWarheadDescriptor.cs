using UnityEngine;
using System.Collections.Generic;
using Munitions.ModularMissiles.Descriptors.Warheads;
[CreateAssetMenu(fileName = "New Modular Missile Warhead", menuName = "Nebulous/Missiles/Warhead/Modular Impact Cone")]
public class ModularImpactConeWarheadDescriptor : ImpactConeWarheadDescriptor, IModular
{
    [Header("Modular Components")]
    [SerializeField]
    protected List<ScriptableObject> _modules = new();
    List<ScriptableObject> IModular.Modules => _modules;
}