using Game.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Serialization;

namespace Lib.Generic_Gameplay
{

    public interface IWeightedEffect
    {
        public void SetWeight(float weight);
    }
    public class ScalingAOEExplosionEffectModule : MonoBehaviour, IDamageDealer, IDamageCharacteristic, IEffectModule, ILocalImbued, IOwned, IWeightedEffect
    {


        [Tooltip("Percentage chance of doing any damage to a target in the area of effect")]
        [SerializeField]
        private float _damageChance = 1f;

        [Tooltip("The number of fixed frames this explosion will spherecast and collect targets to damage.  All targets will be damaged at once at the end.")]
        [SerializeField]
        private int _lingerFrames = 1;

        [SerializeField]
        protected float _heatPower = 0f;

        [SerializeField]
        private float _componentDamage = 10f;

        [SerializeField]
        private AnimationCurve _componentDamageRangeScaling = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

        [SerializeField]
        private AnimationCurve _componentDamageWeightScaling = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

        [SerializeField]
        private float _blastRadius = 1f;

        [SerializeField]
        private float _armorPenetration = 10f;

        [SerializeField]
        private bool _scaleDamageOverArea = false;

        [SerializeField]
        private AnimationCurve _armorPenetrationRangeScaling = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

        [SerializeField]
        private AnimationCurve _armorPenetrationWeightScaling = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

        [SerializeField]
        private float _randomEffectMultiplier = 1f;

        [SerializeField]
        private bool _neverCrit = false;

        [SerializeField]
        private float _crewVulnerabilityMultiplier = 1f;

        [Tooltip("Negative values will go all the way through the ship")]
        [SerializeField]
        private float _penetrationDistance = 5f;

        [SerializeField]
        private bool _munitionsOnly = false;

        [SerializeField]
        private bool _checkObstructions = false;

        [SerializeField]
        private bool _scaleDamageOverDirection = false;
        //very weird, ignore this
        [SerializeField]
        private AnimationCurve _damageDirectionScaling = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

        [Tooltip("For extremely large explosions, we don't want to miss an armor segment if a raycast fails so fallback to (0.5,0.5) coordinates")]
        [SerializeField]
        private bool _fallbackArmorUVCoordinates = false;

        private float _weight = 0f;
        public void SetWeight(float weight) => _weight = weight;

        private float _currentTargetDistance = 1f;

        private float _currentBrushSize = 1f;

        private float _currentSpherecastSize = 1f;

        private HashSet<Collider> _collected = new HashSet<Collider>();

        private HashSet<IDamageable> _damaged = new HashSet<IDamageable>();

        private HashSet<object> _armorSegments = new HashSet<object>();

        private bool _alreadyDamagedInternals = false;

        private IWeaponStatReportReceiver _reportTo;

        float IDamageCharacteristic.ArmorPenetration
        {
            get
            {
                float _armorScaling = 1;
                _armorScaling *= _componentDamageWeightScaling.Evaluate(_weight);
                if (_scaleDamageOverArea)
                {
                    _armorScaling *= _armorPenetrationRangeScaling.Evaluate(_currentTargetDistance / _blastRadius);
                }
                return _armorScaling * _armorPenetration;
            }
        }

        float IDamageCharacteristic.OverpenetrationDamageMultiplier => 1f;

        float? IDamageCharacteristic.MaxPenetrationDepth => null;

        bool IDamageCharacteristic.NeverOverpen => true;

        float IDamageCharacteristic.DamageBrushSize => _currentBrushSize;

        float IDamageCharacteristic.HeatDamage => _heatPower;

        float IDamageCharacteristic.ComponentDamage => _componentDamage;

        float IDamageCharacteristic.RandomEffectMultiplier => _randomEffectMultiplier;

        float IDamageCharacteristic.CrewVulnerabilityMultiplier => _crewVulnerabilityMultiplier;

        bool IDamageCharacteristic.IgnoreEffectiveThickness => false;

        bool IDamageCharacteristic.NeverCrit => _neverCrit;

        bool IDamageCharacteristic.NeverRicochet => true;

        bool IDamageCharacteristic.AlwaysSpreadThroughStructure => false;




        IPlayer IOwned.OwnedBy
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool IDamageDealer.HasComponentDamageCapacityRemaining => false;

        public void Play()
        {
            if (GameManager.Instance.IsHost && base.gameObject.activeInHierarchy)
            {
                StartCoroutine(CoroutineExplode());
            }
        }

        private IEnumerator CoroutineExplode()
        {
            _collected.Clear();
            _damaged.Clear();
            _armorSegments.Clear();
            int i = 0;
            while (i < _lingerFrames)
            {
                ListExtensions.AddRange(add: Physics.OverlapSphere(base.transform.position, _blastRadius, _munitionsOnly ? 524288 : 524801, QueryTriggerInteraction.Ignore), set: _collected);
                yield return new WaitForFixedUpdate();
                int num = i + 1;
                i = num;
            }
            if (_collected.Count <= 0)
            {
                yield break;
            }
            foreach (Collider hit in _collected)
            {
                if (UnityEngine.Random.Range(0f, 1f) > _damageChance)
                {
                    continue;
                }
                IDamageable damageable = hit.transform.root.GetComponent<IDamageable>();
                if (damageable == null)
                {
                    continue;
                }
                _alreadyDamagedInternals = _damaged.Contains(damageable);
                object segment = damageable.GetColliderGroup(hit);
                if (segment != null)
                {
                    if (_armorSegments.Contains(segment))
                    {
                        continue;
                    }
                    _armorSegments.Add(segment);
                }
                if (_checkObstructions)
                {
                    if (Physics.Linecast(base.transform.position, hit.transform.position, out var rayHit, 512, QueryTriggerInteraction.Ignore))
                    {
                        if (segment != null)
                        {
                            _armorSegments.Add(segment);
                        }
                        continue;
                    }
                    rayHit = default(RaycastHit);
                }
                MunitionHitInfo hitInfo = MunitionHitInfo.Take();
                hitInfo.HitCollider = hit;
                hitInfo.HitObject = hit.transform.root.gameObject;
                hitInfo.Point = hit.ClosestPoint(base.transform.position);
                hitInfo.LocalPoint = hitInfo.HitObject.transform.InverseTransformPoint(hitInfo.Point);
                hitInfo.Normal = hit.transform.position.To(base.transform.position).normalized;
                hitInfo.LocalNormal = hitInfo.HitObject.transform.InverseTransformDirection(hitInfo.Normal);
                hitInfo.HitNormal = base.transform.position.To(hit.transform.position).normalized;
                hitInfo.LocalHitNormal = hitInfo.HitObject.transform.InverseTransformDirection(hitInfo.HitNormal);
                Collider hitCollider = hitInfo.HitCollider;
                if (hitCollider is MeshCollider mesh)
                {
                    hitInfo.HitUV = damageable.SampleUV(mesh, hitInfo.LocalPoint, hitInfo.LocalNormal);
                    if (!hitInfo.HitUV.HasValue && _fallbackArmorUVCoordinates)
                    {
                        hitInfo.HitUV = new Vector2(0.5f, 0.5f);
                    }
                }
                _currentTargetDistance = Vector3.Distance(hitInfo.Point, base.transform.position);
                _currentBrushSize = Mathf.Sqrt(_blastRadius * _blastRadius - _currentTargetDistance * _currentTargetDistance);
                _currentSpherecastSize = Mathf.Min(_currentBrushSize, damageable.BoundingRadius);
                float damage;
                bool destroyed;
                HitResult hitRes = damageable.DoDamage(hitInfo, this, out damage, out destroyed);
                if (hitRes >= HitResult.Penetrated)
                {
                    _damaged.Add(damageable);
                }
                if (_reportTo != null && hitRes != HitResult.None)
                {
                    _reportTo.ReportHit(hitRes, damage, destroyed);
                }
                hitInfo.Dispose();
            }
        }

        public void Stop()
        {
            _reportTo = null;
        }

        public void SetEffectParameter(string name, float value)
        {
        }

        bool IDamageDealer.GetComponentHits(Vector3 hitPosition, Vector3 penDirection, float penDistance, HitResult hitRes, ref ISubDamageable[] hits, out int hitCount)
        {
            if (_alreadyDamagedInternals)
            {
                hitCount = 0;
                return false;
            }
            hitCount = MunitionsHelpers.SpherecastComponents(hitPosition, penDirection, _currentSpherecastSize, _penetrationDistance, ref hits);
            _alreadyDamagedInternals = true;
            return true;
        }

        int IDamageDealer.DamageComponents(IDamageable parent, IEnumerable<ISubDamageable> parts, MunitionHitInfo hitInfo, HitResult hitRes, out float damageDone)
        {
            float scaling = 1f;
            scaling *= _componentDamageWeightScaling.Evaluate(_weight);
            if (_scaleDamageOverArea)
            {
                float _componentScaling = _componentDamageRangeScaling.Evaluate(_currentTargetDistance / _blastRadius);
                scaling *= _componentScaling;
            }
            if (_scaleDamageOverDirection)
            {
                scaling *= _damageDirectionScaling.Evaluate(Vector3.Dot(base.transform.forward, base.transform.position.To(hitInfo.Point).normalized));
            }
            float totalDamage = scaling * _componentDamage;
            return MunitionsHelpers.SpreadDamageToPartsEvenly(this, hitInfo, parts, parent.ApplyDamageReduction(totalDamage), includeDestroyed: true, out damageDone);
        }

        void IDamageDealer.ConsumeArmorPenetrationCapacity(float damage)
        {
        }

        void ILocalImbued.ImbueLocal(IImbuedObjectSource launchingPlatform)
        {
        }

        void ILocalImbued.SetWeaponReportPath(IWeaponStatReportReceiver receiver)
        {
            _reportTo = receiver;
        }

        IFF IOwned.GetIFF(IPlayer toPlayer)
        {
            throw new NotImplementedException();
        }

    }

}
