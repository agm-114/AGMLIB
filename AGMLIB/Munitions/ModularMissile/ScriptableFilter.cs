using UnityEngine;
using Munitions.ModularMissiles.Descriptors.Controls;
using System.Collections.Generic;
using System;
using System.Reflection;

[CreateAssetMenu(fileName = "New Scriptable Filter", menuName = "Nebulous/New Filter")]
public class ScriptableFilter : ScriptableObject, IFilterIndexed
{
    [SerializeField]
    protected List<string> _whitelist = new();
    [SerializeField]
    protected bool _whitelisteverything = false;
    [SerializeField]
    protected List<string> _blacklist = new();
    [SerializeField]
    protected bool _blacklisteverything = false;
    [SerializeField]
    protected int _index = 0;
    [SerializeField]
    protected bool _allindexes = false;
    public List<string> Whitelist => _whitelist;
    public bool Whitelisteverything => _whitelisteverything;
    public List<string> Blacklist => _blacklist;
    public bool Blacklisteverything => _blacklisteverything;
    public int Index => _index;
    public bool AllIndexes => _allindexes;
}

