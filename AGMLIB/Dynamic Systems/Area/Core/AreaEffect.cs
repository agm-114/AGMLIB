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
using UnityEngine.UI.Extensions;
using System.Xml.Linq;
using Game.EWar;
using Game.Sensors;
using HarmonyLib;
using Bundles;
using static System.Net.Mime.MediaTypeNames;
using Missions.Nodes;
using Shapes;
using static XNode.NodePort;
using System.Data;
using System.Security.Cryptography;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using static Utility.GameColors;
using Munitions.ModularMissiles.Runtime;
using Munitions.ModularMissiles;
using Munitions.ModularMissiles.Runtime.Seekers;
using Steamworks;
using static UnityEngine.Rendering.HighDefinition.CameraSettings;
using System.Runtime.InteropServices;
using Game;
using Random = UnityEngine.Random;
using Munitions.ModularMissiles.Descriptors.Seekers;
using AGMLIB.Dynamic_Systems.Area;
using System.Reflection;

public class AreaEffect : ActiveSettings
{
    protected static List<Type> SupportedTypes = new() { typeof(ModularMissile), typeof(Ship) };
    protected HashSet<MonoBehaviour> _detectedmonobhaviors = new();


    protected bool? _laststate = null;
    protected Guid Guid = Guid.NewGuid();

    //public bool UseTrigger = true;
    public Collider Trigger;
    public float Radius = 100f;

    public bool CustomVFX = true;
    public ColorName Color = ColorName.Orange;
    public EffectFilter Filter = null;
    private float _updateInterval = 0.5f;
    private bool _updateEveryFrame = true;
    private NetworkPoolable _followingInstance;
    private const string matproperty = "Color_67F96457";
    private Color? _oldcolor;
    private float _updateAccum = 0f;

    public Material Material => _followingInstance.GetComponentInChildren<MeshRenderer>().material;

    public List<BasicEffect<MonoBehaviour>> Effects = new();

    public void Fire()
    {
        StopFire();//_EmissionColor
        //Debug.LogError("Creating Sphere");
        string PrefabName = "Stock/E70 'Interruption' Jammer";
        HullComponent goodewar = BundleManager.Instance.AllComponents.FirstOrDefault(x => x.SaveKey == PrefabName);
        if (goodewar == null)
            Debug.LogError("Did not get following bundle");
        //Debug.LogError("Found Target " + goodewar.SaveKey);
        RezFollowingMuzzle goodmuzzel = goodewar.gameObject.GetComponentInChildren<RezFollowingMuzzle>();
        if (goodmuzzel == null)
            Debug.LogError("Did not get following muzzel");
        GameObject prefab = Common.GetVal<GameObject>(goodmuzzel, "_followingPrefab");
        if(prefab == null)
            Debug.LogError("Did not get following prefab");
        else
            _followingInstance = NetworkObjectPooler.Instance?.GetNextOrNew(prefab, transform.position, transform.rotation );
        if (_followingInstance is ISettableEWarParameters settableEWarParameters)
        {
            settableEWarParameters.SetParams(SignatureType.Radar, omni: true, 360f, Radius, 1, 0, 0, 0f, true);
            _followingInstance.GetComponent<IImbued>()?.Imbue(ShipController.NetID);
            _oldcolor = Material.GetColor(matproperty);
            Material.SetColor(matproperty, GetColor(Color));
            _updateAccum = 0f;
        }

    }

    public void StopFire()
    {
        if (_followingInstance != null)
        {
            if(_oldcolor != null)
                Material.SetColor(matproperty, _oldcolor.Value);
            _followingInstance.RepoolSelf();
            _followingInstance = null;
            _oldcolor = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        if(Trigger == null)
        {
            SphereCollider sphereCollider = gameObject.GetOrAddComponent<SphereCollider>();
            Trigger = sphereCollider;
            sphereCollider.radius = Radius;
        }
        else if(Trigger is SphereCollider sphere)
        {
            Radius = sphere.radius;
        }

        Trigger.isTrigger = true;



    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        base.FixedUpdate();



        if (_laststate == null || _laststate != active)
        {
            foreach (BasicEffect<MonoBehaviour> basicEffect in Effects)
            {
                basicEffect.AreaUpdate();
            }

        }
                
        _laststate = active;

        _updateAccum += Time.fixedDeltaTime;
        if(CustomVFX && InGame)
        {

            if (active && _followingInstance == null)
                Fire();
            else if (!active)
                StopFire();
            if ((_updateEveryFrame || _updateAccum >= _updateInterval) && _followingInstance != null)
            {
                _updateAccum = 0f;
                _followingInstance.transform.position = base.transform.position;
                _followingInstance.transform.rotation = base.transform.rotation;
            }
        }
    }



    void UpdateDetectedList(Ship target, bool applicationarg = true)
    {
        //if(_detectedships.Contains(target) )
        if (applicationarg == true  && (Filter?.ValidTarget(target) ?? true))
        {
            _detectedmonobhaviors.Add(target);
            _laststate = null;
        }
        else if(target != null)
        {
            _detectedmonobhaviors.Remove(target);
        }

    }
    void UpdateDetectedList(ModularMissile target, bool applicationarg = true)
    {
        if(target ==  null) return;
        //if(_detectedships.Contains(target) )
        if (applicationarg == true)
        {
            _detectedmonobhaviors.Add(target);
            _laststate = null;
        }
        else if(target != null)
        {
            _detectedmonobhaviors.Remove(target);
        }

    }

    public void OnTriggerEnter(Collider other) => OnTrigger(other.transform, true);
    public void OnTriggerExit(Collider other) => OnTrigger(other.transform, false);
    public void OnTrigger(Transform target, bool applicationarg = true)
    {
        //Debug.LogError(target.name);
        target = target?.root;
        if (target == null)
            return;
        //Debug.LogError(target.name);
        //foreach (MonoBehaviour behaviour in target.GetComponents<MonoBehaviour>())
        //    Debug.LogError(behaviour.GetType().Name);
        UpdateDetectedList(target?.GetComponent<Ship>(), applicationarg);
        UpdateDetectedList(target?.GetComponent<ModularMissile>(), applicationarg);
    }

    public static void HandleJammer(ActiveEWarEffect eWarEffect, IEWarTarget target, bool applicationarg)
    {
        FratricidalWeapon weapon = eWarEffect.GetComponent<FratricidalWeapon>();
        WeaponComponent weaponcom = weapon?.Source;
        AreaEffect areaeffect = weaponcom?.GetComponentInChildren<AreaEffect>();
        if (target is not MonoBehaviour targetmono)
            return;
        areaeffect?.OnTrigger(targetmono?.transform, applicationarg);
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    protected struct ColliderComparer : IEqualityComparer<Collider>
    {
        public bool Equals(Collider x, Collider y)
        {
            return x.transform.root.gameObject == y.transform.root.gameObject;
        }

        public int GetHashCode(Collider obj)
        {
            return ((object)obj).GetHashCode();
        }
    }
    protected static ColliderComparer _colliderComparer = new();


}

[HarmonyPatch(typeof(ActiveJammingEffect), "TargetGained")]
class ActiveEWarEffectTargetGained2
{
    static void Postfix(ActiveEWarEffect __instance, IEWarTarget newTarget) => AreaEffect.HandleJammer(__instance, newTarget, true);
}

[HarmonyPatch(typeof(ActiveJammingEffect), "TargetLost")]
class ActiveEWarEffectTargetLost2
{
    static void Postfix(ActiveEWarEffect __instance, IEWarTarget lostTarget) => AreaEffect.HandleJammer(__instance, lostTarget, false);
}