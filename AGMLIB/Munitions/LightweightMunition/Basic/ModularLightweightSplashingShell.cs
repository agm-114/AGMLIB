﻿namespace AGMLIB.Munitions.LightweightMunition.Basic
{

    [CreateAssetMenu(fileName = "New LW Modular Splashing Shell", menuName = "Nebulous/LW Shells/Modular Splashing Shell")]
    public class ModularLightweightSplashingShell : LightweightSplashingShell, IModular
    {
        [Header("Modular Components")]
        [SerializeField]
        protected List<ScriptableObject> _modules = new();
        List<ScriptableObject> IModular.Modules => _modules;
    }
}
