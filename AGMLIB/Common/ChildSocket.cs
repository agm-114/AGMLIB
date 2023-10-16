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
using System;
using Object = UnityEngine.Object;

public class ChildSocket : MonoBehaviour
{
    public List<string> Components = new();


    private readonly List<GameObject> _children = new();

    private HullSocket _hullsocket;

    void Start()
    {
        _hullsocket = gameObject.GetComponentInParent<HullSocket>();
        if(_hullsocket == null)
            _hullsocket = gameObject.GetComponent<HullSocket>();

        foreach (HullComponent componentPrefab in BundleManager.Instance.AllComponents)
        {
            GameObject socketObject = Object.Instantiate(new GameObject(), _hullsocket.transform);
            HullSocket newsocket = socketObject.AddComponent<HullSocket>();
            SetPrivateField(newsocket, "_key", ShortGuid.NewGuid().ToString());
            SetPrivateField(gameObject.GetComponentInParent<BaseHull>(), "_socketLookup", null);
            //socketObject.AddComponent<SocketFilters>().Whitelisteverything = true;
            newsocket.SetComponent(componentPrefab);

            //GameObject gameObject = Object.Instantiate(componentPrefab.gameObject, _hullsocket.transform);
            //gameObject.name = componentPrefab.name;
            //HullComponent _component = gameObject.GetComponent<HullComponent>();
            //_component.SetSocket(newsocket);
            //gameObject.transform.SetParent(newsocket.transform);
            //gameObject.transform.localPosition = newsocket.AttachPoint / 10f;


            //_children.Add(.gameObject);
            _children.Add(socketObject);

        }

        /*
        foreach (string componentname in Components)
        {
            HullComponent componentPrefab = BundleManager.Instance.GetHullComponent(componentname);
        }
        */
    }

    void FixedUpdate()
    {
    }

    void OnDestroy()
    {
        foreach(GameObject gameObject in _children)
        {
            Destroy(gameObject);
        }
    }

    public static void SetPrivateField(System.Object instance, string fieldName, System.Object value)
    {
        static void SetPrivateFieldInternal(System.Object instance, string fieldName, System.Object value, System.Type type)
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

}

