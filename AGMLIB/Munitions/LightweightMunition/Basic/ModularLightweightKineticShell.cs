using Munitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AGMLIB.Munitions.LightweightMunition.Basic
{
    [CreateAssetMenu(fileName = "New LW Modular Kinetic Shell", menuName = "Nebulous/LW Shells/Modular Kinetic Shell")]
    public class ModularLightweightKineticShell : LightweightKineticShell, IModular
    {
        [Header("Modular Components")]
        [SerializeField]
        protected List<ScriptableObject> _modules = new();
        List<ScriptableObject> IModular.Modules => _modules;
    }
}
