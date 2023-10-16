using UnityEngine;
using System.Collections.Generic;
using Munitions.ModularMissiles.Descriptors.Support;
[CreateAssetMenu(fileName = "New Modular Jammer Support", menuName = "Nebulous/Missiles/Support/Modular Jammer")]
public class ModularJammerSupportDescriptor : JammerSupportDescriptor, IModular
{
    [Header("Modular Components")]
    [SerializeField]
    protected List<ScriptableObject> _modules = new();
    List<ScriptableObject> IModular.Modules => _modules;
}