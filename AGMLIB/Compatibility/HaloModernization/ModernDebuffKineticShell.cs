using System.Collections.Generic;
using System.Linq;
using Game;
using Game.Units;
using Munitions;
using Munitions.InstancedDamagers;
using Ships;
using UnityEngine;
using UnityEngine.Serialization;

namespace AGMLIB.Compatibility.HaloModernization;

/// <summary>
/// Current lightweight kinetic shell plus the legacy overload/depletion delta.
/// Native LightweightKineticShell still owns flight, pooling, ordinary damage,
/// optional dedicated structure damage, effects, and save-key behavior.
/// </summary>
[CreateAssetMenu(
    fileName = "New Modern Debuff Kinetic Shell",
    menuName = "Nebulous/AGMLIB/Compatibility/Modern Debuff Kinetic Shell"
)]
public class ModernDebuffKineticShell : LightweightKineticShell
{
    private enum DebuffTriggerMode
    {
        Never,
        StructureBroken,
        Always,
    }

    [Header("Debuff Delta")]
    [SerializeField]
    private ComponentDebuff _debuff = null;

    [SerializeField]
    [Min(0f)]
    private float _debuffRadius = 1f;

    [SerializeField]
    private DebuffTriggerMode _debuffTriggerMode = DebuffTriggerMode.Always;

    [SerializeField]
    [FormerlySerializedAs("_destroyShields")]
    private bool _depleteShieldOnHit = true;

    protected override IDamageDealer MakeDamageDealer(MunitionHitInfo hitInfo)
    {
        IDamageDealer damageDealer = base.MakeDamageDealer(hitInfo);
        if (_debuff != null && _debuffTriggerMode != DebuffTriggerMode.Never)
        {
            damageDealer = new ChainedThrowawayDamager(
                damageDealer,
                new ImpactComponentDebuffDamager(
                    this,
                    _debuff,
                    _debuffRadius,
                    _debuffTriggerMode == DebuffTriggerMode.StructureBroken,
                    OverrideComponentSearchDistance
                )
            );
        }

        return _depleteShieldOnHit
            ? new ShieldDisruptingDamageDealer(damageDealer)
            : damageDealer;
    }
}

/// <summary>
/// Marker used by the opt-in shield surface. It replaces the legacy empty
/// DestroyShieldsDamager while allowing the ordinary damage dealer to remain
/// fully native when no shield surface intercepts the hit.
/// </summary>
internal interface IShieldDisruptingDamageDealer
{
}

internal sealed class ShieldDisruptingDamageDealer
    : IDamageDealer,
        IDamageCharacteristic,
        IShieldDisruptingDamageDealer
{
    private readonly IDamageDealer _inner;

    public ShieldDisruptingDamageDealer(IDamageDealer inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    float IDamageCharacteristic.ArmorPenetration => _inner.ArmorPenetration;

    float IDamageCharacteristic.OverpenetrationDamageMultiplier =>
        _inner.OverpenetrationDamageMultiplier;

    float? IDamageCharacteristic.MaxPenetrationDepth => _inner.MaxPenetrationDepth;

    bool IDamageCharacteristic.NeverOverpen => _inner.NeverOverpen;

    float IDamageCharacteristic.HeatDamage => _inner.HeatDamage;

    float IDamageCharacteristic.DamageBrushSize => _inner.DamageBrushSize;

    float IDamageCharacteristic.ComponentDamage => _inner.ComponentDamage;

    float IDamageCharacteristic.RandomEffectMultiplier =>
        _inner.RandomEffectMultiplier;

    float IDamageCharacteristic.CrewVulnerabilityMultiplier =>
        _inner.CrewVulnerabilityMultiplier;

    bool IDamageCharacteristic.NeverCrit => _inner.NeverCrit;

    bool IDamageCharacteristic.IgnoreEffectiveThickness =>
        _inner.IgnoreEffectiveThickness;

    bool IDamageCharacteristic.NeverRicochet => _inner.NeverRicochet;

    bool IDamageCharacteristic.AlwaysSpreadThroughStructure =>
        _inner.AlwaysSpreadThroughStructure;

    bool IDamageDealer.HasComponentDamageCapacityRemaining =>
        _inner.HasComponentDamageCapacityRemaining;

    bool IDamageDealer.GetComponentHits(
        Vector3 hitPosition,
        Vector3 penDirection,
        float penDistance,
        HitResult hitRes,
        ref ISubDamageable[] hits,
        out int hitCount
    )
    {
        return _inner.GetComponentHits(
            hitPosition,
            penDirection,
            penDistance,
            hitRes,
            ref hits,
            out hitCount
        );
    }

    int IDamageDealer.DamageComponents(
        IDamageable parent,
        IEnumerable<ISubDamageable> parts,
        MunitionHitInfo hitInfo,
        HitResult hitRes,
        out float damageDone
    )
    {
        return _inner.DamageComponents(
            parent,
            parts,
            hitInfo,
            hitRes,
            out damageDone
        );
    }

    void IDamageDealer.ConsumeArmorPenetrationCapacity(float damage)
    {
        _inner.ConsumeArmorPenetrationCapacity(damage);
    }
}

internal sealed class ImpactComponentDebuffDamager : BaseThrowawayDamager
{
    private readonly ComponentDebuff _debuff;
    private readonly float _radius;
    private readonly bool _onlyWhenStructureDestroyed;
    private bool _castedRay;

    public ImpactComponentDebuffDamager(
        IDamageCharacteristic characteristic,
        ComponentDebuff debuff,
        float radius,
        bool onlyWhenStructureDestroyed,
        float? overrideSearchDistance
    )
        : base(characteristic, overrideSearchDistance)
    {
        _debuff = debuff;
        _radius = Mathf.Max(0f, radius);
        _onlyWhenStructureDestroyed = onlyWhenStructureDestroyed;
    }

    protected override bool GetComponentHitsInternal(
        Vector3 hitPosition,
        Vector3 penDirection,
        float penDistance,
        HitResult hitRes,
        ref ISubDamageable[] hits,
        out int hitCount
    )
    {
        if (_castedRay)
        {
            hitCount = 0;
            return false;
        }

        _castedRay = true;
        hitCount = MunitionsHelpers.SpherecastComponents(
            hitPosition,
            penDirection,
            _radius,
            penDistance,
            ref hits
        );
        return true;
    }

    protected override int DamageComponentsInternal(
        IDamageable parent,
        IEnumerable<ISubDamageable> parts,
        MunitionHitInfo hitInfo,
        HitResult hitRes,
        out float damageDone
    )
    {
        damageDone = 0f;
        if (
            _debuff == null
            || parent is not ShipController ship
            || (
                _onlyWhenStructureDestroyed
                && !ship.Ship.Hull.StructureDestroyed
            )
        )
        {
            return 0;
        }

        HullComponent component =
            parts?.OfType<HullComponent>().FirstOrDefault()
            ?? ship.Ship.Hull.CollectComponents<HullComponent>().FirstOrDefault();
        component?.Internals().AddDebuff(_debuff, hitInfo);
        return 0;
    }
}
