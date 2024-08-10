using Ships;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FleetEditor;
using Munitions;
using System.Linq;
using System.Reflection;
using Utility;
using HarmonyLib;
using System.Diagnostics;

using UI;
using Debug = UnityEngine.Debug;
using static UnityEngine.ParticleSystem;
using Bundles;
using static UnityEngine.EventSystems.EventTrigger;
using System.Runtime.CompilerServices;
using Game;
using Ships.Serialization;
using Game.Units;
using Mirror;
using System.Xml.Linq;
using Factions;
using Procedural.Naming;
using Steamworks;
using System;
using UnityEngine.UI;
using Munitions.ModularMissiles;
using FleetEditor.MissileEditor;
using Munitions.ModularMissiles.Descriptors.Seekers;
using Munitions.ModularMissiles.Descriptors;
using Munitions.ModularMissiles.Descriptors.Controls;
using Game.Orders.Tasks;
using static UI.SequentialButton;
using UnityEngine.Serialization;
using System.Configuration;
using Steamworks.Ugc;

public class IndexedSocketFilter : MonoBehaviour, IFilterIndexed
{
    [SerializeField]
    protected List<string> _whitelist = new();
    [SerializeField]
    protected bool _whitelisteverything = false;
    [SerializeField]
    protected List<string> _blacklist = new();
    [SerializeField]
    protected bool _blacklisteverything = false;
    public List<string> Whitelist => _whitelist;
    public bool Whitelisteverything => _whitelisteverything;
    public List<string> Blacklist => _blacklist;
    public bool Blacklisteverything => _blacklisteverything;
    [SerializeField]
    protected bool _allowIllegal = false;
    public bool AllowIllegal => _allowIllegal;
    [SerializeField]
    protected bool _bypassFactionRestrictions = true;
    public bool BypassFactionRestrictions => _bypassFactionRestrictions;

    [SerializeField]
    protected int _index = 0;
    [SerializeField]
    protected bool _allindexes = false;
    public int Index => _index;
    public bool AllIndexes => _allindexes;

}
