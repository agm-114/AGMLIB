using System;
using System.Collections.Generic;
using Game.EWar;
using Game.Intel;
using Game.Sensors;
using Game.Units;
using Mirror;
using Ships.Controls;
using UnityEngine.Serialization;
using Utility;

namespace Lib.Generic_Gameplay.Missiles;

/// <summary>
/// A turreted, internally loaded cell launcher. Authors can arrange its ejectors as a rotating
/// VLS bank or as exposed rails; unlike <see cref="BanditLauncherComponent"/>, fired cells are
/// not replenished from the ship's ammunition feed.
/// </summary>
[RequireComponent(typeof(AutomaticMissileIlluminator))]
[RequireComponent(typeof(AutomaticLauncherFireControl))]
public class TurretedCellLauncherComponent : BaseTurretedLauncherComponent,
    IHasIntegratedSensor,
    INeedsSensorData,
    INeedsShipIdentity
{
    [Header("Turreted Cells")]
    [SerializeField]
    private MissileEjector[] _cells = Array.Empty<MissileEjector>();

    [SerializeField]
    private Vector3 _launchBlockCheckNormal = Vector3.up;

    [FormerlySerializedAs("LaunchBlockRadius")]
    [SerializeField]
    private float _coldLaunchBlockRadius = 1f;

    [SerializeField]
    private float _hotLaunchBlockRadius = 5f;

    [Tooltip("Keep training on a track after the last launch so an attached illuminator can support the missile until the target is lost or the order is cancelled.")]
    [SerializeField]
    private bool _retainTrackAfterLaunch = true;

    [SerializeField]
    private AutomaticLauncherFireControl? _fireControl;

    public override int Capacity => _cells.Length;

    protected override IReadOnlyList<MissileEjector> _allCells => _cells;

    ISensorComponent IHasIntegratedSensor.Sensor => FireControl?.Sensor!;

    private AutomaticLauncherFireControl? FireControl =>
        _fireControl ??= GetComponent<AutomaticLauncherFireControl>();

    protected override void InitStats()
    {
        base.InitStats();
        FireControl?.InitializeStats();
    }

    protected override void CollectAllStatReferencesForRegistering(
        List<Tuple<ShipStatAttribute, StatValue>> statList)
    {
        base.CollectAllStatReferencesForRegistering(statList);
        FireControl?.CollectStatReferences(statList);
    }

    void INeedsSensorData.SetSensorProvider(ISensorProvider provider)
    {
        FireControl?.SetProvider(provider);
    }

    void INeedsShipIdentity.SetShipIdentity(ShipController ship)
    {
        base.SetShipIdentity(ship);
        FireControl?.SetShipIdentity(ship);
    }

    public override bool CheckLaunchAreaClear(IMissile missile)
    {
        float radius = missile != null && missile.HotLaunch
            ? _hotLaunchBlockRadius
            : _coldLaunchBlockRadius;
        Vector3 checkPosition = transform.TransformPoint(_launchBlockCheckNormal.normalized * radius);
        return !Physics.CheckSphere(checkPosition, radius, 512, QueryTriggerInteraction.Ignore);
    }

    protected override bool CheckOnTarget(IMissile missile)
    {
        return base.CheckOnTarget(missile) &&
            (FireControl?.ReadyToLaunch ?? true);
    }

    protected override void OnLaunchQueueEmpty()
    {
        if (!_retainTrackAfterLaunch)
        {
            base.OnLaunchQueueEmpty();
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        DebugExtension.DebugWireSphere_Always(
            transform.TransformPoint(_launchBlockCheckNormal.normalized * _coldLaunchBlockRadius),
            Color.cyan,
            _coldLaunchBlockRadius);
        DebugExtension.DebugWireSphere_Always(
            transform.TransformPoint(_launchBlockCheckNormal.normalized * _hotLaunchBlockRadius),
            Color.red,
            _hotLaunchBlockRadius);
    }
}

/// <summary>
/// The vanilla Bandit rail/reload state machine with the automatic missile illuminator authoring
/// contract attached. Use <see cref="SlidingRailMissileEjector"/> rails and a
/// <see cref="QuadModeTurretController"/> exactly as on the vanilla Ward/Talisman launchers.
/// </summary>
[RequireComponent(typeof(AutomaticMissileIlluminator))]
[RequireComponent(typeof(AutomaticLauncherFireControl))]
public class IlluminatingBanditLauncherComponent : BanditLauncherComponent,
    IHasIntegratedSensor,
    INeedsSensorData,
    INeedsShipIdentity
{
    [SerializeField]
    private AutomaticLauncherFireControl? _fireControl;

    ISensorComponent IHasIntegratedSensor.Sensor => FireControl?.Sensor!;

    private AutomaticLauncherFireControl? FireControl =>
        _fireControl ??= GetComponent<AutomaticLauncherFireControl>();

    protected override void InitStats()
    {
        base.InitStats();
        FireControl?.InitializeStats();
    }

    protected override void CollectAllStatReferencesForRegistering(
        List<Tuple<ShipStatAttribute, StatValue>> statList)
    {
        base.CollectAllStatReferencesForRegistering(statList);
        FireControl?.CollectStatReferences(statList);
    }

    void INeedsSensorData.SetSensorProvider(ISensorProvider provider)
    {
        FireControl?.SetProvider(provider);
    }

    void INeedsShipIdentity.SetShipIdentity(ShipController ship)
    {
        base.SetShipIdentity(ship);
        FireControl?.SetShipIdentity(ship);
    }

    protected override bool CheckOnTarget(IMissile missile)
    {
        return base.CheckOnTarget(missile) &&
            (FireControl?.ReadyToLaunch ?? true);
    }
}

/// <summary>
/// Connects a child fire-control sensor to a turreted missile launcher. The launcher exposes the
/// sensor to the ship through <see cref="IHasIntegratedSensor"/> and forwards the native sensor
/// provider and owner lifecycle through this component. Put the sensor transform on the rotating
/// turret body or barrel so its field of view follows the launcher.
/// </summary>
[DisallowMultipleComponent]
public sealed class AutomaticLauncherFireControl : MonoBehaviour
{
    [Header("Integrated Launcher Fire Control")]
    [Tooltip("Launcher to observe. If omitted, the component finds one on the same GameObject.")]
    [SerializeField]
    private BaseTurretedLauncherComponent? _launcher;

    [Tooltip("Integrated fire-control sensor. If omitted, the first child FireControlSensor is used.")]
    [SerializeField]
    private FireControlSensor? _sensor;

    [Tooltip("For track-targeted launches, hold the missile until the integrated sensor has acquired its native fire-control lock. Position-targeted launches are unaffected.")]
    [SerializeField]
    private bool _waitForLockBeforeLaunch = true;

    private ISensorTrackable? _assignedTarget;

    public FireControlSensor? Sensor
    {
        get
        {
            EnsureReferences();
            return _sensor;
        }
    }

    public bool ReadyToLaunch
    {
        get
        {
            EnsureReferences();
            if (!_waitForLockBeforeLaunch || _sensor == null)
            {
                return true;
            }

            ITrack? track = _launcher?.CurrentlyTargetedTrack();
            return track == null || !track.IsValid || _sensor.HasLock;
        }
    }

    private void Awake()
    {
        EnsureReferences();
    }

    private void FixedUpdate()
    {
        EnsureReferences();
        if (_sensor == null)
        {
            return;
        }

        ITrack? track = _launcher?.CurrentlyTargetedTrack();
        ISensorTrackable? target = track is { IsValid: true, Trackable: not null }
            ? track.Trackable
            : null;
        if (ReferenceEquals(_assignedTarget, target))
        {
            return;
        }

        _sensor.SetTargetedObject(target);
        _assignedTarget = target;
    }

    private void OnDisable()
    {
        ClearTarget();
    }

    private void OnDestroy()
    {
        ClearTarget();
    }

    internal void InitializeStats()
    {
        FireControlSensor? sensor = Sensor;
        if (sensor != null)
        {
            StatHelpers.InitializeStatValues(sensor);
        }
    }

    internal void CollectStatReferences(
        List<Tuple<ShipStatAttribute, StatValue>> statList)
    {
        FireControlSensor? sensor = Sensor;
        if (sensor == null)
        {
            return;
        }

        List<Tuple<ShipStatAttribute, StatValue>> values =
            StatHelpers.GetAllStatValues(sensor);
        if (values != null && values.Count > 0)
        {
            statList.AddRange(values);
        }
    }

    internal void SetProvider(ISensorProvider provider)
    {
        Sensor?.SetProvider(provider);
    }

    internal void SetShipIdentity(ShipController ship)
    {
        Sensor?.SetOwner(ship.OwnedBy);
    }

    private void EnsureReferences()
    {
        _launcher ??= GetComponent<BaseTurretedLauncherComponent>();
        _sensor ??= GetComponentInChildren<FireControlSensor>(includeInactive: true);
    }

    private void ClearTarget()
    {
        if (_sensor != null && _assignedTarget != null)
        {
            _sensor.SetTargetedObject(null);
        }

        _assignedTarget = null;
    }
}

/// <summary>
/// An active fire-control radar intended for a child transform of either launcher above. It binds
/// its native hull-component dependency automatically; authors still configure the inherited
/// range, field of view, power, gain, aperture, noise filtering, and lock-SNR fields.
/// </summary>
public class LauncherActiveFireControlSensor : ActiveFireControlSensor
{
    protected override void Awake()
    {
        _mainComponent ??= GetComponentInParent<BaseTurretedLauncherComponent>();
        base.Awake();
    }
}

/// <summary>
/// Drives one or more following EWar muzzles whenever its turreted launcher is assigned a live
/// missile track. The spawned effect should contain <see cref="SensorIlluminator"/> so the beam
/// supplies real radar illumination instead of only a visual effect.
/// </summary>
[DisallowMultipleComponent]
public sealed class AutomaticMissileIlluminator : MonoBehaviour, IMuzzleWeapon
{
    [Header("Automatic Missile Illuminator")]
    [Tooltip("Launcher to observe. If omitted, the component finds a turreted launcher on the same GameObject.")]
    [SerializeField]
    private BaseTurretedLauncherComponent? _launcher;

    [Tooltip("Following-instance muzzles whose prefabs contain SensorIlluminator. If empty, child following muzzles are discovered automatically.")]
    [SerializeField]
    private FollowingInstanceMuzzle[] _muzzles = Array.Empty<FollowingInstanceMuzzle>();

    [SerializeField]
    private SignatureType _signatureType = SignatureType.Radar;

    [Min(0f)]
    [SerializeField]
    private float _coneFov = 2.5f;

    [Min(0f)]
    [SerializeField]
    private float _maxRange = 1200f;

    [Range(0f, 1f)]
    [SerializeField]
    private float _effectAreaRatio = 0.4f;

    [Min(0f)]
    [SerializeField]
    private float _radiatedPower = 1000f;

    [SerializeField]
    private float _gain = 60f;

    [Range(0f, 1f)]
    [SerializeField]
    private float _edgeFalloff = 0.75f;

    [SerializeField]
    private bool _showLineOfBearing = true;

    private bool _initialized;
    private bool _illuminating;

    bool IMuzzleWeapon.IsHost => NetworkServer.active;

    IImbuedObjectSource IMuzzleWeapon.Platform =>
        _launcher?.Socket?.Controller?.ImbueingSource!;

    private void Awake()
    {
        EnsureInitialized();
    }

    private void FixedUpdate()
    {
        EnsureInitialized();
        if (!NetworkServer.active)
        {
            return;
        }

        ITrack? track = _launcher?.CurrentlyTargetedTrack();
        SetIlluminating(
            _launcher != null &&
            _launcher.IsFunctional &&
            IsMissileTarget(track));
    }

    private void OnDisable()
    {
        SetIlluminating(false);
    }

    private void OnDestroy()
    {
        SetIlluminating(false);
        foreach (FollowingInstanceMuzzle muzzle in _muzzles)
        {
            if (muzzle != null)
            {
                muzzle.OnInstanceSpawned -= HandleInstanceSpawned;
            }
        }
    }

    public static bool IsMissileTarget(ITrack? track)
    {
        return track is { IsValid: true, Trackable: not null } &&
            track.Trackable.ContactType == ContactClassification.Missile;
    }

    private void EnsureInitialized()
    {
        _launcher ??= GetComponent<BaseTurretedLauncherComponent>();
        if (_initialized || _launcher == null)
        {
            return;
        }

        if (_muzzles == null || _muzzles.Length == 0)
        {
            _muzzles = GetComponentsInChildren<FollowingInstanceMuzzle>(includeInactive: true);
        }

        int muzzleIndex = 0;
        foreach (FollowingInstanceMuzzle muzzle in _muzzles)
        {
            if (muzzle == null)
            {
                continue;
            }

            muzzle.Initialize(this, muzzleIndex++);
            muzzle.OnInstanceSpawned += HandleInstanceSpawned;
        }

        _initialized = true;
    }

    private void SetIlluminating(bool illuminating)
    {
        if (_illuminating == illuminating)
        {
            return;
        }

        _illuminating = illuminating;
        foreach (FollowingInstanceMuzzle muzzle in _muzzles)
        {
            if (muzzle == null)
            {
                continue;
            }

            if (illuminating)
            {
                muzzle.Fire();
            }
            else
            {
                muzzle.StopFire();
            }
        }
    }

    private void HandleInstanceSpawned(NetworkPoolable instance)
    {
        if (instance is ISettableEWarParameters parameters)
        {
            parameters.SetParams(
                _signatureType,
                omni: false,
                _coneFov,
                _maxRange,
                _effectAreaRatio,
                _radiatedPower,
                _gain,
                _edgeFalloff,
                _showLineOfBearing);
        }
    }

    void IMuzzleWeapon.TriggerHitEffect(int muzzle, HitResult hit, Vector3 position, Quaternion rotation)
    {
    }
}
