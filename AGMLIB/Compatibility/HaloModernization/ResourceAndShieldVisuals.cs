using System.Linq;
using Ships;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

namespace AGMLIB.Compatibility.HaloModernization;

/// <summary>
/// Drives an emissive property from the current native resource pool. This
/// replaces the legacy stored-resource subscriber and does not mutate the
/// ship's ResourcePool implementation.
/// </summary>
public class ResourceUsageEmissive : MonoBehaviour
{
    [SerializeField]
    private Renderer _renderer;

    [SerializeField]
    [FormerlySerializedAs("_hullSegmentBasic")]
    private HullSegmentBasic _hullSegment;

    [SerializeField]
    private string _resourceName = "Power";

    [SerializeField]
    private string _shaderProperty = "_EmissivePowerFraction";

    [SerializeField]
    [FormerlySerializedAs("_maxPowerConsumption")]
    [Min(1f)]
    private float _consumptionAtFullEmission = 50000f;

    [SerializeField]
    [Min(0.02f)]
    private float _updateInterval = 0.25f;

    private Ship _ship;
    private Material _material;
    private bool _ownsMaterial;
    private int _propertyId;
    private float _nextUpdate;

    private void Awake()
    {
        _ship = GetComponentInParent<Ship>();
        if (_renderer != null)
        {
            _material = _renderer.material;
            _ownsMaterial = _material != null;
        }
        else
        {
            _material = _hullSegment?.ArmorMaterials?.FirstOrDefault();
        }
        _propertyId = Shader.PropertyToID(_shaderProperty);
    }

    private void OnDestroy()
    {
        if (_ownsMaterial && _material != null)
            Destroy(_material);
        _material = null;
        _ship = null;
    }

    private void Update()
    {
        if (Time.time < _nextUpdate)
            return;
        _nextUpdate = Time.time + _updateInterval;

        IReadOnlyResourcePool resource = _ship
            ?.Resources
            ?.FirstOrDefault(pool => pool.ResourceName == _resourceName);
        if (
            _material == null
            || resource == null
            || !_material.HasProperty(_propertyId)
        )
            return;

        float emission = Mathf.Clamp01(
            resource.AmountConsumed / Mathf.Max(1f, _consumptionAtFullEmission)
        );
        _material.SetFloat(_propertyId, emission);
    }
}

/// <summary>
/// Local hit presentation for ModernShieldComponent. A rebuilt network VFX
/// prefab can invoke AddReplicatedHit on remote clients while the host uses
/// the component event directly.
/// </summary>
public class ShieldHitVisuals : MonoBehaviour
{
    [SerializeField]
    private ModernShieldComponent _shield;

    [SerializeField]
    private VisualEffect _visualEffect;

    [SerializeField]
    private string _hitPositionProperty = "HitPosition";

    [SerializeField]
    private string _hitDamageProperty = "HitDamage";

    [SerializeField]
    private string _hitEvent = "Hit";

    private void Awake()
    {
        if (_shield == null)
            _shield = GetComponentInParent<ModernShieldComponent>();
    }

    private void OnEnable()
    {
        if (_shield != null)
            _shield.OnShieldHit += AddReplicatedHit;
    }

    private void OnDisable()
    {
        if (_shield != null)
            _shield.OnShieldHit -= AddReplicatedHit;
    }

    public void AddReplicatedHit(Vector3 worldPosition, float damage)
    {
        if (_visualEffect == null)
            return;

        _visualEffect.SetVector3(
            _hitPositionProperty,
            _visualEffect.transform.InverseTransformPoint(worldPosition)
        );
        _visualEffect.SetFloat(_hitDamageProperty, damage);
        _visualEffect.SendEvent(_hitEvent);
    }
}
