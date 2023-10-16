using Game.Units;
using Munitions;
using Ships;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Utility;
using UnityEngine;
using Mirror;

public class AOEModifer : NetworkBehaviour, IFilter
{
    [SerializeField]
    protected HashSet<Ship> _detectedships = new();
    [SerializeField]
    protected HullComponent _hullComponent = null;
    [SerializeField]
    protected Collider _trigger;
    [SerializeField]
    protected List<StatModifier>  _modifiers;
    [SerializeField]
    protected List<string> _whitelist = new();
    [SerializeField]
    protected bool _whitelisteverything = true;
    [SerializeField]
    protected List<string> _blacklist = new();
    [SerializeField]
    protected bool _blacklisteverything = false;
    public List<string> Whitelist => _whitelist;
    public bool Whitelisteverything => _whitelisteverything;
    public List<string> Blacklist => _blacklist;
    public bool Blacklisteverything => _blacklisteverything;

    protected List<FreeModifierSource> _modifierssources;
    protected bool _laststate = false;

    // Start is called before the first frame update
    void Start()
    {
        _modifiers = new List<StatModifier>();
        foreach(StatModifier modifier in _modifiers)
            _modifierssources.Add(new FreeModifierSource(base.netIdentity.ToString() + modifier.StatName, modifier));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(_hullComponent  != null)
        {
            if (_laststate != _hullComponent.IsFunctional)
                foreach (Ship ship in _detectedships)
                    UpdateModifiers(ship);

            _laststate = _hullComponent.IsFunctional;
        }
    }

    void UpdateModifiers(Ship target)
    {
        bool application = true;
        if (_hullComponent != null && !_hullComponent.IsFunctional)
            application = false;
        UpdateModifiers(target, application);
    }
    void UpdateModifiers(Ship target, bool application)
    {
        foreach (FreeModifierSource freeModifierSource in _modifierssources)
            if (application)
                target.AddStatModifier(freeModifierSource, freeModifierSource.Modifier);
            else
                target.RemoveStatModifier(freeModifierSource, freeModifierSource.Modifier.StatName);
    }


    public void OnTriggerEnter(Collider other)
    {
        //Debug.LogError("MINE DETECTED");
        Ship newship = other.transform.root.gameObject.GetComponent<Ship>();
        BaseHull baseHull = other.transform.root.gameObject.GetComponent<BaseHull>();
        if (!Whitelist.Contains(baseHull.SaveKey) && Blacklist.Contains(baseHull.SaveKey))
            return;
        if (Blacklisteverything && !Whitelisteverything)
            return;
        if (newship != null)
        {
            _detectedships.Add(newship);
            UpdateModifiers(newship);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        //Debug.LogError("MINE LOST");
        Ship newship = other.transform.root.gameObject.GetComponent<Ship>();
        if (newship != null && _detectedships.Contains(newship))
        {
            _detectedships.Remove(newship);
            UpdateModifiers(newship, false);
        }
    }
}
