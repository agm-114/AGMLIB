using Game;
using Mirror;
using Munitions;
using Ships;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using Utility;
using Game.Intel;
using Game.Units;
using Modding;
using Debug = UnityEngine.Debug;
using HarmonyLib;
using UnityEngine.PlayerLoop;
using System.Xml.Serialization;
using System.Runtime.CompilerServices;
using static Game.WaypointPath;
using Pixelplacement;
using UnityEngine.UI.Extensions;
using Munitions.ModularMissiles.Descriptors.Warheads;
using Munitions.ModularMissiles;
using Munitions.ModularMissiles.Runtime;
using Shapes;
using Bundles;
using Game.Reports;
public class CraftKinematics2
{
    public static void MoveTarget(ICraft craft, MovementTarget target = null, float speed = 1)
    {
        craft.Rigidbody.velocity = craft.Rigidbody.transform.forward * craft.FlightSpeed * speed;
        craft.Rigidbody.rotation = Quaternion.RotateTowards(craft.Rigidbody.rotation, Quaternion.LookRotation(target.transform.position - craft.Rigidbody.transform.position, target.transform.up), 1f);   
    }
}
public class Craft : DumbfireRocket, ICraft
{
    public Rigidbody Rigidbody => _body;
    public Transform SocketTransform => sockets[0].transform;
    [SerializeField]
    protected float fuzeTime = 5f;
    [SerializeField]
    protected Avoider _avoider;
    [SerializeField]
    protected List<GameObject> sockets;
    [Range(0.0f, 1f)]
    [SerializeField]
    protected float _rtbthreshold = 1;
    MovementTarget ICraft.Target { get => _target; set => _target = value; }
    private float Rtbthreshold => Math.Min(_rtbthreshold, _target?.Rtbthreshold ?? 1);
    public PID _pid;
    public string GUID = "0318fdb1c69d4aca8a48d40e0723e3e5";
    public override Guid SavedNetworkSpawnKey => new(GUID);
    private MovementTarget _target;    
    protected string _filter = "";
    protected ComponentHullPaint[] _paints;
    protected bool NeedsHealing => HealthPercentage < Rtbthreshold;
    protected int Priority => _target?.Priority ?? int.MaxValue;
    protected float _speedoveride = 1;
    protected bool _evasive = false;
    protected float _time = 0;
    protected Vector3 _randomps = Vector3.zero;
    protected Vector3 _targetps = Vector3.zero;
    protected override void Awake()
    {
        base.Awake();
        _pid ??= new PID(0.5f, 0.5f, 0.5f, 0.5f, 0.5f);
        _paints = gameObject.GetComponentsInChildren<ComponentHullPaint>();
        Rigidbody.angularDrag = Rigidbody.angularDrag * 10;
    }
    protected override void FixedUpdate()
    {
        foreach (ComponentHullPaint paint in _paints)
            paint.SetColors(LaunchedFrom.OwnedBy.Colors.BaseColor, LaunchedFrom.OwnedBy.Colors.StripeColor);
        SetTarget();
        EngineOn();
        if(_target != null)
        {
            if (NeedsHealing && (_target?.CanRepair ?? false))
                (this as ISubDamageable).DoDamage(-_target.RepairRate);

            Rigidbody.drag = 5;
            //if(!_evasive && Vector3.Angle(_targetps - Rigidbody.transform.position, Rigidbody.transform.forward) > 90)
            //    Rigidbody.velocity = Rigidbody.transform.forward * (FlightSpeed * 1.1f);
            //else if(!_evasive)
            //    Rigidbody.velocity = Rigidbody.transform.forward * (FlightSpeed * 0.9f);

            //Rigidbody.MovePosition(Rigidbody.position + (_targetps - Rigidbody.position).normalized / 10);
            //if (Vector3.Angle(Rigidbody.velocity, Rigidbody.transform.forward) > 5)
            if (Rigidbody.velocity.magnitude < FlightSpeed)
                Rigidbody.AddRelativeForce(Vector3.forward * (FlightSpeed - Rigidbody.velocity.magnitude) * 100, ForceMode.Acceleration);
            Rigidbody.velocity = Rigidbody.transform.forward * FlightSpeed;
            Quaternion target = Quaternion.LookRotation(_target.transform.position - Rigidbody.transform.position, Rigidbody.transform.up);
            if (_time < 0.5)
            {
                target = Quaternion.LookRotation(_randomps, Rigidbody.transform.up);
                Rigidbody.MovePosition(Rigidbody.position + ((_targetps - Rigidbody.position).normalized / 10));

            }
            if (_time > 1)
            {
                if (_evasive)
                    _randomps = UnityEngine.Random.onUnitSphere;
                _time = 0;
            }
            if(!_evasive)
                _randomps = _targetps - Rigidbody.transform.position;
            Rigidbody.rotation = Quaternion.RotateTowards(Rigidbody.rotation, target, 2f);
            _time += Time.fixedDeltaTime;
        }
    }
    public void SetLeadFactor(Vector3 formtarget, bool evasive = false)
    {
        //_speedoveride = Mathf.Clamp(leadFactor, 0.5f, 1.1f);
        _targetps = formtarget;
        _evasive = evasive;
    }
    void SetTarget()
    {
        if (NeedsHealing && !(_target?.RepairTarget ?? true))
            SetTarget(null);
        if (_target?.PermantTarget ?? false)
            return;
        IEnumerable<MovementTarget> targets = MovementTarget.Instances.Where(ptarget => ptarget.Valid(this, _filter));
        if (NeedsHealing)
            targets = targets.Where(ptarget => ptarget.RepairTarget);
        targets = targets.Where(ptarget => ptarget.Priority < Priority);
        SetTarget(targets ?? new List<MovementTarget>());
    }
    new public void Launch(NetworkIdentity launchingPlatform, int salvoId, bool playerOrder)
    {
        base.Launch(launchingPlatform, salvoId, playerOrder);
        this._collider.isTrigger = false;
    }
    public void SetTarget(IEnumerable<MovementTarget> targets = null)
    {
        if (_target != null)
            _target.Craft = null;
        _target = null;
        if (targets != null && targets.Any())
        {
            _target = targets.OrderBy(ptarget => ptarget.Priority).Last();
            _target.Craft = this;
        }
    }
    protected override void OnUnpooled()
    {
        base.OnUnpooled();
        Awake();
    }
    protected override void OnRepooled()
    {
        SetTarget(null);
        base.OnRepooled();
    }

    protected override void Destroyed()
    {
        SetTarget(null);
        base.Destroyed();
    }
    protected override bool FuzeActive() => false;
    protected override bool GenericImpact(MunitionHitInfo hitInfo, bool trigger) => false;
    protected override bool DamageableImpact(IDamageable hitObject, MunitionHitInfo hitInfo, bool trigger) => false;
}
