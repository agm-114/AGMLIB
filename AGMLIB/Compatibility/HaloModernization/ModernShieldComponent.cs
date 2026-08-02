using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Game;
using Game.Units;
using HarmonyLib;
using Munitions;
using Munitions.ModularMissiles.Descriptors;
using Ships;
using Ships.SaveGame;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;

namespace AGMLIB.Compatibility.HaloModernization;

/// <summary>
/// Host-authoritative shield gameplay core. The component is deliberately
/// opt-in: only ships carrying this component are intercepted by the damage
/// adapter. A rebuilt mod still needs to bind its shield VFX and expose a
/// player order for Toggle().
/// </summary>
public class ModernShieldComponent : CycledComponent
{
    [Serializable]
    private sealed class ShieldState : SavedHullComponentStates.StateElement
    {
        public float Health;

        public bool ToggledOn;

        protected override void WriteToDocumentInternal(XmlElement self)
        {
            Health.AppendToDocument(self, "Health");
            ToggledOn.AppendToDocument(self, "ToggledOn");
        }

        protected override void ReadFromDocumentInternal(XmlElement self)
        {
            Health = self.ReadFromDocumentParent("Health", Health);
            ToggledOn = self.ReadFromDocumentParent("ToggledOn", ToggledOn);
        }
    }

    [SerializeField]
    [FormerlySerializedAs("_shieldObject")]
    private GameObject _shieldVisual;

    [SerializeField]
    [Min(0f)]
    private float _shieldCapacity = 1000f;

    [SerializeField]
    [FormerlySerializedAs("_shieldThickness")]
    [Min(0f)]
    private float _shieldArmorThickness = 5f;

    [SerializeField]
    [Min(0f)]
    private float _missileDamageMultiplier = 0.2f;

    [SerializeField]
    [FormerlySerializedAs("_shieldCooldownTime")]
    [Min(0f)]
    private float _cooldownTime = 30f;

    [SerializeField]
    private bool _startToggledOn = true;

    [ShipStat(
        "modernshield-capacity",
        "Shield Capacity",
        "HP",
        InitializeFrom = "_shieldCapacity"
    )]
    private StatValue _statShieldCapacity;

    [ShipStat(
        "modernshield-armor",
        "Shield Thickness",
        "cm",
        InitializeFrom = "_shieldArmorThickness"
    )]
    private StatValue _statShieldArmor;

    [ShipStat(
        "modernshield-cooldown",
        "Shield Cooldown",
        "s",
        InitializeFrom = "_cooldownTime",
        PositiveBad = true
    )]
    private StatValue _statCooldown;

    private ShipController _owner;
    private float _currentShieldHealth;
    private bool _toggledOn;
    private bool _wasCycling;
    private bool _healthInitialized;

    public event Action<Vector3, float> OnShieldHit;

    public float CurrentShieldHealth => _currentShieldHealth;

    public float MaximumShieldHealth => _statShieldCapacity?.Value ?? _shieldCapacity;

    public float ShieldHealthPercent =>
        MaximumShieldHealth <= 0f ? 0f : _currentShieldHealth / MaximumShieldHealth;

    public ShipController Owner => _owner;

    public bool IsToggledOn => _toggledOn;

    public bool CanAbsorb =>
        _toggledOn
        && IsFunctional
        && IsDoingWork
        && !CycleActive
        && _currentShieldHealth > 0f;

    public override bool HasCycleTimer => _cooldownTime > 0f;

    protected override bool _operatingConsumingResources => _toggledOn;

    protected override float _cycleLength => _statCooldown?.Value ?? _cooldownTime;

    public override float BurstPercent => _toggledOn ? 1f - ShieldHealthPercent : 1f;

    public override string OverrideActivityTooltip
    {
        get
        {
            if (!_toggledOn)
                return "Deactivated";
            if (CycleActive)
                return "Charging Shields";

            int current = Mathf.RoundToInt(_currentShieldHealth);
            int maximum = Mathf.RoundToInt(MaximumShieldHealth);
            int percent = Mathf.RoundToInt(ShieldHealthPercent * 100f);
            return $"Shield Integrity: {current} / {maximum} HP ({percent}%)";
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _toggledOn = _startToggledOn;
        _currentShieldHealth = _shieldCapacity;
        _healthInitialized = false;
        RefreshVisual();
    }

    protected override void OnDestroy()
    {
        ShieldRegistry.Unregister(_owner, this);
        _owner = null;
        OnShieldHit = null;
        base.OnDestroy();
    }

    protected override void SocketSet()
    {
        base.SocketSet();
        ShieldRegistry.Unregister(_owner, this);
        _owner = Socket?.Hull?.Controller as ShipController;
        ShieldRegistry.Register(_owner, this);
        if (!_healthInitialized)
        {
            _currentShieldHealth = MaximumShieldHealth;
            _healthInitialized = true;
        }
        else
        {
            _currentShieldHealth = Mathf.Clamp(
                _currentShieldHealth,
                0f,
                MaximumShieldHealth
            );
        }
        RefreshVisual();
    }

    protected override void Update()
    {
        _wasCycling = CycleActive;
        base.Update();
        if (_wasCycling && !CycleActive && _toggledOn && IsFunctional)
            _currentShieldHealth = MaximumShieldHealth;
        RefreshVisual();
    }

    protected override void PartFunctionalChangedInternal(bool newFunctional)
    {
        base.PartFunctionalChangedInternal(newFunctional);
        RefreshVisual();
    }

    protected override void PartDestroyedInternal()
    {
        ShieldRegistry.Unregister(_owner, this);
        base.PartDestroyedInternal();
        RefreshVisual();
    }

    protected override void PartRestoredInternal()
    {
        base.PartRestoredInternal();
        ShieldRegistry.Register(_owner, this);
        RefreshVisual();
    }

    public void Toggle()
    {
        if (_cycleRpcProvider == null || !_cycleRpcProvider.IsHost)
            return;

        _toggledOn = !_toggledOn;
        if (_toggledOn && HasCycleTimer)
            _cycleRpcProvider.MarkCycle(this, 0f);
        else if (!_toggledOn && CycleActive)
            _cycleRpcProvider.CancelCycle(this);
        RefreshVisual();
        FireActivityChangedEvent();
    }

    public bool TryAbsorb(Vector3 hitPosition, IDamageDealer damager, out float damage)
    {
        damage = 0f;
        if (!CanAbsorb || damager == null)
            return false;

        float armor = _statShieldArmor?.Value ?? _shieldArmorThickness;
        if (damager is IShieldDisruptingDamageDealer)
        {
            damage = _currentShieldHealth;
        }
        else
        {
            damage = Mathf.Max(0f, damager.ComponentDamage);
            if (damager is MissileWarhead or MissileComponentDescriptor)
                damage *= Mathf.Max(0f, _missileDamageMultiplier);

            if (armor > 0f && damager.ArmorPenetration < armor)
            {
                float ratio = Mathf.Clamp01(damager.ArmorPenetration / armor);
                damage *= ratio * ratio;
            }
        }
        if (damage <= 0f)
            return false;

        float healthBeforeHit = _currentShieldHealth;
        float penetrationConsumed =
            damage < healthBeforeHit ? damager.ArmorPenetration : armor;
        damager.ConsumeArmorPenetrationCapacity(Mathf.Max(0f, penetrationConsumed));
        _currentShieldHealth = Mathf.Max(0f, _currentShieldHealth - damage);
        OnShieldHit?.Invoke(hitPosition, damage);
        if (_currentShieldHealth <= 0f && HasCycleTimer)
            _cycleRpcProvider.MarkCycle(this, 0f);
        RefreshVisual();
        FireActivityChangedEvent();
        return true;
    }

    public bool CanBlockArmorOnly()
    {
        return CanAbsorb;
    }

    public override void WriteSaveState(SavedHullComponentStates state)
    {
        base.WriteSaveState(state);
        state.Write(
            this,
            new ShieldState
            {
                Health = _currentShieldHealth,
                ToggledOn = _toggledOn,
            }
        );
    }

    public override void RestoreFromSaveState(SavedHullComponentStates state)
    {
        base.RestoreFromSaveState(state);
        ShieldState saved = state.Read<ShieldState>(this);
        if (saved == null)
            return;

        _currentShieldHealth = Mathf.Clamp(saved.Health, 0f, MaximumShieldHealth);
        _toggledOn = saved.ToggledOn;
        _healthInitialized = true;
        RefreshVisual();
    }

    private void RefreshVisual()
    {
        if (_shieldVisual != null)
            _shieldVisual.SetActive(CanAbsorb);
    }
}

internal static class ShieldRegistry
{
    private static readonly Dictionary<ShipController, List<ModernShieldComponent>> Shields = [];

    public static void Register(ShipController owner, ModernShieldComponent shield)
    {
        if (owner == null || shield == null)
            return;
        if (!Shields.TryGetValue(owner, out List<ModernShieldComponent> entries))
        {
            entries = [];
            Shields.Add(owner, entries);
        }
        if (!entries.Contains(shield))
            entries.Add(shield);
    }

    public static void Unregister(ShipController owner, ModernShieldComponent shield)
    {
        if (owner == null || shield == null)
            return;
        if (!Shields.TryGetValue(owner, out List<ModernShieldComponent> entries))
            return;

        entries.Remove(shield);
        if (entries.Count == 0)
            Shields.Remove(owner);
    }

    public static bool IsRegistered(
        ShipController owner,
        ModernShieldComponent shield
    )
    {
        return owner != null
            && shield != null
            && Shields.TryGetValue(owner, out List<ModernShieldComponent> entries)
            && entries.Contains(shield);
    }
}

/// <summary>
/// Binds one authored shield collider to the shield component that owns it.
/// Damage interception requires this exact marker, preserving vanilla damage
/// for hull colliders and for ships that merely have a shield component.
/// </summary>
[DisallowMultipleComponent]
public class ShieldHitSurface : MonoBehaviour
{
    [SerializeField]
    [FormerlySerializedAs("shieldComponent")]
    private ModernShieldComponent _shield;

    public bool TryGetShield(
        ShipController hitOwner,
        out ModernShieldComponent shield
    )
    {
        if (_shield == null)
            _shield = GetComponentInParent<ModernShieldComponent>();

        shield = _shield;
        return shield != null
            && shield.Owner == hitOwner
            && ShieldRegistry.IsRegistered(hitOwner, shield);
    }

    public static bool TryResolve(
        MunitionHitInfo hitInfo,
        ShipController hitOwner,
        out ModernShieldComponent shield
    )
    {
        shield = null;
        if (hitInfo?.HitCollider == null)
            return false;

        ShieldHitSurface surface =
            hitInfo.HitCollider.GetComponent<ShieldHitSurface>();
        return surface != null && surface.TryGetShield(hitOwner, out shield);
    }
}

[HarmonyPatch]
internal static class ShipDamageShieldPatch
{
    private static MethodBase TargetMethod()
    {
        return AccessTools.Method(typeof(ShipController), "Game.IDamageable.DoDamage");
    }

    private static bool Prefix(
        ShipController __instance,
        MunitionHitInfo hitInfo,
        IDamageDealer damager,
        ref float damageDone,
        ref bool destroyed,
        ref HitResult __result
    )
    {
        if (
            !__instance.isServer
            || !ShieldHitSurface.TryResolve(
                hitInfo,
                __instance,
                out ModernShieldComponent shield
            )
            || !shield.TryAbsorb(hitInfo.Point, damager, out damageDone)
        )
        {
            return true;
        }

        destroyed = false;
        __result = HitResult.Stopped;
        return false;
    }
}

[HarmonyPatch]
internal static class ShipArmorOnlyShieldPatch
{
    private static MethodBase TargetMethod()
    {
        return AccessTools.Method(
            typeof(ShipController),
            "Game.IDamageable.DoArmorDamageOnly"
        );
    }

    private static bool Prefix(
        ShipController __instance,
        MunitionHitInfo hitInfo,
        IDamageDealer damager,
        ref HitResult __result
    )
    {
        if (
            !__instance.isServer
            || !ShieldHitSurface.TryResolve(
                hitInfo,
                __instance,
                out ModernShieldComponent shield
            )
            || !shield.CanBlockArmorOnly()
        )
        {
            return true;
        }

        __result = HitResult.Ricochet;
        return false;
    }
}
