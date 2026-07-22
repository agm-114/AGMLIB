namespace Lib.Generic_Gameplay.Discrete
{
    public interface IMuzzleEffect
    {
        public void FireEffect();

        public void CancelEffect();
        public void SpawnHit(MunitionHitInfo? rayHit);
    }



    public class MuzzleEffects : MonoBehaviour
    {
        public static void FireEffects(Muzzle muzzle)
        {
            //Common.Trace("Spawning Flashs");


            foreach (BaseMuzzleEffects effect in muzzle.gameObject.GetComponentsInChildren<BaseMuzzleEffects>())
            {
                //Common.Trace("Spawning Flash");
                effect.FireEffect();
            }
        }

        public static void CancelEffects(Muzzle muzzle)
        {
            //Common.Trace("Spawning Flashs");


            foreach (BaseMuzzleEffects effect in muzzle.gameObject.GetComponentsInChildren<BaseMuzzleEffects>())
            {
                //Common.Trace("Spawning Flash");
                effect.CancelEffect();
            }
        }

        public static void SpawnImpacts(Muzzle muzzle, MunitionHitInfo hit)
        {


            //Common.Trace("Spawning Impacts");

            foreach (BaseMuzzleEffects effect in muzzle.gameObject.GetComponentsInChildren<BaseMuzzleEffects>())
            {
                //Common.Trace("Spawning Impact");

                effect.TrySpawnHit(hit);
            }
        }
    }

    public abstract class BaseMuzzleEffects : MonoBehaviour, IMuzzleEffect
    {
        [SerializeField] private float _impactRefractoryTime = 0f;
        private float _nextImpactEffectTime;

        public abstract void FireEffect();
        public virtual void CancelEffect()
        {

        }
        public bool TrySpawnHit(MunitionHitInfo? rayHit)
        {
            if (_impactRefractoryTime > 0f)
            {
                if (Time.time < _nextImpactEffectTime)
                    return false;

                _nextImpactEffectTime = Time.time + _impactRefractoryTime;
            }

            SpawnHit(rayHit);
            return true;
        }

        public virtual void SpawnHit(MunitionHitInfo? rayHit)
        {

        }
    }



    public class MuzzleSpawnedEffects : BaseMuzzleEffects
    {
        [SerializeField] protected Transform _muzzleEffectEffectlocation;
        [SerializeField] protected GameObject _muzzleEffect;
        [SerializeField] protected GameObject _hitEffect;
        [SerializeField] protected GameObject _singletonEffect;
        [SerializeField]
        [Tooltip("Spawn the hit effect at the hit object's hierarchy root instead of the raycast impact point.")]
        protected bool _spawnHitEffectAtTargetRoot = false;
        [SerializeField]
        [Tooltip("Spawn and share the singleton effect at the hit object's hierarchy root instead of the directly hit transform.")]
        protected bool _spawnSingletonEffectAtTargetRoot = true;
        public override void FireEffect()
        {
            if (_muzzleEffect == null)
                return;
            //Common.Trace("Spawn Muzzle");
            ObjectPooler.Instance.GetNextOrNew(_muzzleEffect, _muzzleEffectEffectlocation ?? transform);
            //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //sphere.transform.position = _muzzleEffectEffectlocation?.position ?? transform.position;
        }
        public override void SpawnHit(MunitionHitInfo? rayHit)
        {
            if (rayHit == null)
                return;

            if (rayHit.HitObject == null)
            {
                Common.Hint("Hit object was null");
                return;
            }

            Transform hitTransform = rayHit.HitObject.transform;
            SpawnHitEffect(rayHit.Point, hitTransform);
            SpawnOrPulseSingletonEffect(rayHit.Point, hitTransform);
        }

        private void SpawnHitEffect(Vector3 hitPosition, Transform hitTransform)
        {
            if (_hitEffect == null)
                return;

            Common.Trace("Custom Hit Effect");
            Transform effectParent = _spawnHitEffectAtTargetRoot ? hitTransform.root : hitTransform;
            Vector3 effectPosition = _spawnHitEffectAtTargetRoot ? effectParent.position : hitPosition;
            ObjectPooler.Instance.GetNextOrNew(_hitEffect, effectPosition, effectParent.rotation, effectParent);
        }

        private void SpawnOrPulseSingletonEffect(Vector3 hitPosition, Transform hitTransform)
        {
            if (_singletonEffect == null)
                return;

            Transform effectParent = _spawnSingletonEffectAtTargetRoot ? hitTransform.root : hitTransform;
            string singletonName = GetSingletonEffectName(effectParent);
            GameObject singleton = GameObject.Find(singletonName);

            if (singleton == null)
            {
                if (_singletonEffect.GetComponent<Poolable>() == null)
                {
                    Common.Hint($"Singleton effect '{_singletonEffect.name}' does not have a Poolable component");
                    return;
                }

                Vector3 effectPosition = _spawnSingletonEffectAtTargetRoot ? effectParent.position : hitPosition;
                Poolable pooledEffect = ObjectPooler.Instance.GetNextOrNew(
                    _singletonEffect,
                    effectPosition,
                    effectParent.rotation,
                    effectParent);

                if (pooledEffect == null)
                    return;

                singleton = pooledEffect.gameObject;
                singleton.name = singletonName;
            }
            else
            {
                Lib.FX.ShortDurationFollowingModularEffect durationEffect =
                    singleton.GetComponent<Lib.FX.ShortDurationFollowingModularEffect>();

                if (durationEffect != null)
                    durationEffect.ResetLifespan();
                else
                    Common.Hint($"Singleton effect '{singleton.name}' cannot reset its lifespan because it does not have a ShortDurationFollowingModularEffect component");
            }

            foreach (Lib.FX.PulsedLocalEffect pulsedEffect in singleton.GetComponentsInChildren<Lib.FX.PulsedLocalEffect>(true))
                pulsedEffect.Pulse(hitPosition);
        }

        private string GetSingletonEffectName(Transform effectParent)
        {
            return $"AGMLIB Singleton Effect [{_singletonEffect.GetInstanceID()}:{effectParent.GetInstanceID()}]";
        }
        public void SpwawnCustomHit(Vector3 pos)
        {

            Common.Hint("Experimental Custom Hit Effect");
            Poolable hitob = ObjectPooler.Instance.GetNextOrNew(_muzzleEffect);
            hitob.transform.position = pos;
        }
    }

    public class MuzzleFireHitEffects : BaseMuzzleEffects
    {
        [SerializeField] protected LineBeamMuzzleEffects _muzzleeffect;
        [SerializeField] private float _effectDuration = 1f;
        [SerializeField] private float _maxDisplayedLength = 100f;

        private float _timer = 0f;
        private bool _isTimerActive = false;
        private float _currentlen = 0f;

        public void Awake()
        {
            _muzzleeffect.SetMaxLength(_maxDisplayedLength);
            _muzzleeffect.SetBeamLength(0);
            _muzzleeffect.StopEffect();
            if (_muzzleeffect == null)
                Common.Hint("Line Beam muzzle effect not linked");
            //gameObject.GetComponent<LineRenderer>().useWorldSpace = false;

        }

        public override void FireEffect()
        {


        }
        public void Reset()
        {
            _isTimerActive = false;
            _muzzleeffect.StopEffect();
            _muzzleeffect.SetBeamLength(0);
            _currentlen = _maxDisplayedLength;
        }

        private void FixedUpdate()
        {
            if (_timer > 0)
            {
                _muzzleeffect.SetBeamLength(_currentlen);

                _timer -= Time.fixedDeltaTime;
                if (_timer <= 0f)
                {
                    Reset();
                }
            }
        }
        public override void SpawnHit(MunitionHitInfo? rayHit)
        {

            _currentlen = _maxDisplayedLength;
            if (rayHit != null)
            {
                _currentlen = Vector3.Distance(rayHit.Point, transform.position);
                Common.Trace($"hit {_currentlen}");
            }
            else
            {
                Common.Trace($"miss {_currentlen}");
            }
            _muzzleeffect.StartEffect();
            _muzzleeffect.SetBeamLength(_currentlen);
            _timer = _effectDuration;
        }
    }

    public class MuzzleSoundEffects : BaseMuzzleEffects
    {
        [SerializeField] protected VariedSoundEffect _muzzleSound;
        [SerializeField] protected VariedSoundEffect _impactSound;
        public override void FireEffect() => GlobalSFX.PlayOneShotSpatial(_muzzleSound, transform);
        public override void SpawnHit(MunitionHitInfo rayHit)
        {
            if (rayHit != null)
                GlobalSFX.PlayOneShotSpatial(_impactSound, rayHit.HitObject.transform);

        }
    }

    public class MuzzleGlowerEffects : BaseMuzzleEffects
    {
        [SerializeField] protected BarrelGlow _glower;
        public override void FireEffect() => _glower.SetFiring(firing: true);
        public override void CancelEffect() => _glower.SetFiring(firing: false);

        /*
        public class MuzzleSoundSource : MuzzleSoundEffects
        {
            public override void FireEffect() => GlobalSFX.PlayOneShotSpatial(_muzzleSound, transform);
            public override void SpawnHit(MunitionHitInfo rayHit) => GlobalSFX.PlayOneShotSpatial(_muzzleSound, rayHit.HitObject.transform);
        }
        */
    }

    public class LongPulseRaycastMuzzle : SinglePulseRaycastMuzzle
    {


    }
}
