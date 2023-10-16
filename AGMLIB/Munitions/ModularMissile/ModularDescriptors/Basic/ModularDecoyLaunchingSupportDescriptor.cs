using UnityEngine;
using System.Collections.Generic;
using Munitions.ModularMissiles.Descriptors.Support;
[CreateAssetMenu(fileName = "New Modular Decoy Support", menuName = "Nebulous/Missiles/Support/Modular Decoy Launcher")]
public class ModularDecoyLaunchingSupportDescriptor : DecoyLaunchingSupportDescriptor, IModular
{
    [Header("Modular Components")]
    [SerializeField]
    protected List<ScriptableObject> _modules = new();
    List<ScriptableObject> IModular.Modules => _modules;
}