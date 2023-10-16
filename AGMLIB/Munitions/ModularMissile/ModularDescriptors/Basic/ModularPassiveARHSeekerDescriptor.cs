using UnityEngine;
using System.Collections.Generic;
using Munitions.ModularMissiles.Descriptors.Seekers;
[CreateAssetMenu(fileName = "New Modular Missile Seeker", menuName = "Nebulous/Missiles/Seekers/Modular Anti-Radiation")]
public class ModularPassiveARHSeekerDescriptor : PassiveARHSeekerDescriptor, IModular
{
    [Header("Modular Components")]
    [SerializeField]
    protected List<ScriptableObject> _modules = new();
    List<ScriptableObject> IModular.Modules => _modules;
}