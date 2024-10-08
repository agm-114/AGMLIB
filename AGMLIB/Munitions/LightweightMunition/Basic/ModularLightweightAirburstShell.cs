﻿namespace AGMLIB.Munitions.LightweightMunition.Basic
{
    [CreateAssetMenu(fileName = "New LW Modular Airburst Shell", menuName = "Nebulous/LW Shells/Modular Airburst Shell")]
    public class ModularLightweightAirburstShell : LightweightAirburstShell, IModular
    {
        [Header("Modular Components")]
        [SerializeField]
        protected List<ScriptableObject> _modules = new();
        List<ScriptableObject> IModular.Modules => _modules;
    }
}
