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

            foreach (IMuzzleEffect effect in muzzle.gameObject.GetComponentsInChildren<IMuzzleEffect>())
            {
                //Common.Trace("Spawning Impact");

                effect.SpawnHit(hit);
            }
        }
    }

    public abstract class BaseMuzzleEffects : MonoBehaviour, IMuzzleEffect
    {
        public abstract void FireEffect();
        public virtual void CancelEffect()
        {

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
            if (rayHit == null || _hitEffect == null)
                return;
            Common.Trace("Custom Hit Effect");

            if (rayHit.HitObject == null)
                Common.Hint("Hit object was null");

            Poolable hitob = ObjectPooler.Instance.GetNextOrNew(_hitEffect, rayHit.HitObject.transform);
            hitob.transform.position = rayHit.Point;
            //hitob = ObjectPooler.Instance.GetNextOrNew(_hitEffect, rayHit.HitObject.transform);

            //hitob = ObjectPooler.Instance.GetNextOrNew(_hitEffect);



            //hitob = ObjectPooler.Instance.GetNextOrNew(_hitEffect);


            //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //sphere.transform.position = rayHit.HitObject.transform.position;
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
