﻿using UnityEngine;
using Munitions.ModularMissiles.Descriptors.Controls;
using System.Collections.Generic;
using System.Linq;
using System;

[CreateAssetMenu(fileName = "New Modular Missile Avionics", menuName = "Nebulous/Missiles/Avionics/Modular Direct")]
public class ModularDirectGuidanceDescriptor : DirectGuidanceDescriptor, IModular, ILimited
{
    public override void LoadSettings(MissileComponentSettings data)
    {

        if (data is BaseAvionicsSettings avionics)
        {

            if (_weaponrole != WeaponRole.Unlocked)
            {
                if (_weaponrole == WeaponRole.Offensive)
                    avionics.Role = Utility.WeaponRole.Offensive;
                else if (_weaponrole == WeaponRole.Defensive)
                    avionics.Role = Utility.WeaponRole.Defensive;
            }
            if (_launchType != LaunchType.Unlocked)
            {
                if (_launchType == LaunchType.Hot)
                    avionics.HotLaunch = true;
                else if (_launchType == LaunchType.Cold)
                    avionics.HotLaunch = false;
            }
            if (_targetlost != TargetLost.Unlocked)
            {
                if (_targetlost == TargetLost.SelfDestruct)
                    avionics.SelfDestructOnLost = true;
                else if (_targetlost == TargetLost.Resume)
                    avionics.SelfDestructOnLost = false;
            }
            if (_terminal != Terminal.Unlocked)
            {
                if (_terminal == Terminal.Weave)
                    avionics.Maneuvers = BaseAvionicsDescriptor.TerminalManeuver.Weave;
                else if (_terminal == Terminal.Corkscrew)
                    avionics.Maneuvers = BaseAvionicsDescriptor.TerminalManeuver.Corkscrew;
                else if (_terminal == Terminal.None)
                    avionics.Maneuvers = BaseAvionicsDescriptor.TerminalManeuver.None;
            }
        }
        if (data is DirectGuidanceSettings direct)
        {
            if (_guidance != Guidance.Unlocked)
            {
                if (_guidance == Guidance.FreeApproach)
                    direct.ApproachAngleControl = false;
                else if (_guidance == Guidance.MinimumAngle)
                    direct.ApproachAngleControl = true;
            }
        }

        base.LoadSettings(data);
    }


    static protected int _defaultlock = -1;
    [SerializeField]
    protected List<ScriptableObject> _modules = new();
    [Header("Setting Restrictions")]
    [SerializeField]
    protected WeaponRole _weaponrole = (WeaponRole)_defaultlock;
    [SerializeField]
    protected LaunchType _launchType = (LaunchType)_defaultlock;
    [SerializeField]
    protected TargetLost _targetlost = (TargetLost)_defaultlock;
    [SerializeField]
    protected Terminal _terminal = (Terminal)_defaultlock;
    [SerializeField]
    protected Guidance _guidance = (Guidance)_defaultlock;
    List<ScriptableObject> IModular.Modules => _modules;

    public Dictionary<string, int> RestrictedOptions => new()
    {
        { "_roleButton", Convert.ToInt32(_weaponrole)},
        { "_launchButton",Convert.ToInt32( _launchType) },
        { "_targetLostBehavior", Convert.ToInt32(_targetlost) },
        { "_terminalManeuvers", Convert.ToInt32(_terminal) },
        { "_approachButton", Convert.ToInt32(_guidance) }
    };
}

//Avionics
//Weapon Role [Offensive, Defensive]
//Launch Type [Hot, Cold]
//Target Lost [Resume, Self Destruct]
//Terminal [None, Weave, CorkScrew]
//Trajectory [Minimum Angle, Free Approach]

//Seeker
//Mode [Targeting, Validation]
//Detect Small Targets [Ignore, Detect]