using Game.Units;
using Munitions;
using Ships;
using Ships.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Utility;

using UnityEngine;


using System.Reflection;
using Game.EWar;
using Steamworks.Data;

public class IFFComponent : MonoBehaviour
{
    private HashSet<LoiteringMissile> _detectedmines = new HashSet<LoiteringMissile>();

    private ShipController _shipController;
    bool jammed = false;
    InternalActiveSensorComponent disabledsensor = null;
    public CommsAntennaComponent antenna;
    public bool antennastatus = true;

    // Start is called before the first frame update
    void Start()
    {
        _shipController = transform.gameObject.GetComponentInParent<ShipController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_shipController == null)
            return;
        //Debug.LogError("IFF UPDATE");
        jammed = _shipController.Comms.AnyJamming;

        foreach (LoiteringMissile loiteringMissile in _detectedmines)
            if(((Communicator)GetPrivateField(loiteringMissile, "_comms")).AnyJamming)
                jammed = true;
        if (jammed)
            Debug.LogError("IFF SHIP JAMMED" + jammed);
        //Debug.LogError("IFF MINES JAMMED" + jammed);
        if (!_shipController.CommsEnabled || (_shipController.SensorsEnabled && !jammed))
        {
            if (disabledsensor != null)
            {
                Debug.LogError("REENABLING SEARCH RADAR");
                disabledsensor.SetSensorEnabled(false);
                disabledsensor = null;
                SetAntennaPower(0);
            }
        }
        else if (disabledsensor == null && (jammed || !_shipController.SensorsEnabled))//(!_shipController.Comms.HasWorkingAntenna)
        {
                
            //if (!_shipController.Comms.HasWorkingAntenna)
            //    antennastatus = false;
            Debug.LogError("TRIGGERING IFF");
            List<InternalActiveSensorComponent> internals = _shipController.GetComponentsInChildren<InternalActiveSensorComponent>().ToList();
            Debug.LogError("FINDING TARGET SENSOR");
            disabledsensor = internals.Where((InternalActiveSensorComponent x) => x.IsFunctional).SelectMax((InternalActiveSensorComponent x) => ((StatValue)GetPrivateField(x, "_statRadiatedPower")).Value);
            Debug.LogError("TRANSMITTING VIA RADAR");
            SetAntennaPower(((StatValue)GetPrivateField(disabledsensor, "_statRadiatedPower")).Value);
            Debug.LogError("RECALCULATING");

            //typeof(StatValue).GetMethod("Recalculate").Invoke((StatValue)GetPrivateField(antenna, "_statTransmitPower"), null);
            Debug.LogError("DISABLING RADAR");
            disabledsensor.SetSensorEnabled(false);

        }


        //base.FixedUpdate();
    }

    void SetAntennaPower(float power)
    {
        SetPrivateField((StatValue)GetPrivateField(antenna, "_statTransmitPower"), "_valueCached", power);

        FreeModifierSource mod = new FreeModifierSource("Toggle Radar", new StatModifier("test", 0f, 0f));
        ((StatValue)GetPrivateField(antenna, "_statTransmitPower")).AddModifier(mod, mod.Modifier);
        ((StatValue)GetPrivateField(antenna, "_statTransmitPower")).RemoveModifier(mod);
    }

    public void OnTriggerEnter(Collider other)
    {
        //Debug.LogError("MINE DETECTED");
        LoiteringMissile loiteringMissile = other.transform.root.gameObject.GetComponent<LoiteringMissile>();
        if (loiteringMissile != null)
        {
            _detectedmines.Add(loiteringMissile);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        //Debug.LogError("MINE LOST");
        LoiteringMissile loiteringMissile = other.transform.root.gameObject.GetComponent<LoiteringMissile>();
        if (loiteringMissile != null && _detectedmines.Contains(loiteringMissile))
        {
            _detectedmines.Remove(loiteringMissile);
        }
    }

    public static void SetPrivateField(object instance, string fieldName, object value)
    {
        static void SetPrivateFieldInternal(object instance, string fieldName, object value, Type type)
        {
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null)
            {
                field.SetValue(instance, value);
                return;
            }
            else if (type.BaseType != null)
            {
                SetPrivateFieldInternal(instance, fieldName, value, type.BaseType);
                return;
            }
        }

        SetPrivateFieldInternal(instance, fieldName, value, instance.GetType());
    }

    public static object GetPrivateField(object instance, string fieldName)
    {
        static object GetPrivateFieldInternal(object instance, string fieldName, Type type)
        {
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null)
            {
                return field.GetValue(instance);
            }
            else if (type.BaseType != null)
            {
                return GetPrivateFieldInternal(instance, fieldName, type.BaseType);
            }
            else
            {
                return null;
            }
        }

        return GetPrivateFieldInternal(instance, fieldName, instance.GetType());
    }
}
