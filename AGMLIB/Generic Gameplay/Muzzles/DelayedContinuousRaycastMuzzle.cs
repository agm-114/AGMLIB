namespace Lib.Generic_Gameplay.Muzzles
{
    public class DelayedContinuousRaycastMuzzle : RaycastMuzzle, IDirectDamageMuzzle
    {
        [SerializeField]
        private bool _cylinderRaycast = false;

        [SerializeField]
        private float _fireDelay = 0.5f;

        [SerializeField]
        private float _damagePeriod = 0.5f;

        [Header("Immediate Effects")]
        [SerializeField] private VisualEffect? _flashImmediate;
        [SerializeField] private VariedSoundEffect? _fireSoundImmediate;
        [SerializeField] protected AudioSource? _immediateSoundSource;
        [SerializeField] private Animation? _fireAnimationImmediate;
        [SerializeField] private BarrelGlow? _glowerImmediate;

        [Header("Delayed Effects")]
        [SerializeField] private VisualEffect? _flashDelayed;
        [SerializeField] private VariedSoundEffect? _fireSoundDelayed;
        [SerializeField] private AudioSource? _delayedSoundSource;
        [SerializeField] private Animation? _fireAnimationDelayed;
        [SerializeField] private BarrelGlow? _glowerDelayed;

        private Coroutine? _delayedFireCoroutine;
        private Coroutine? _delayedFireEffectCoroutine;
        private bool _fireRequested;
        private bool _isFiring;
        private float _fireAccum;
        private BeamMuzzleEffects Effects => Common.GetVal<BeamMuzzleEffects>(this, "_effects");
        private float BeamLength
        {
            get => Common.GetVal<float>(this, "_beamLength");
            set => Common.SetVal(this, "_beamLength", value);
        }

        float IDirectDamageMuzzle.ArmorPenetration => ((IDamageCharacteristic)this).ArmorPenetration * (1f / _damagePeriod);

        float IDirectDamageMuzzle.ComponentDamage => ((IDamageCharacteristic)this).ComponentDamage * (1f / _damagePeriod);

        string IDirectDamageMuzzle.ArmorDamageUnit => "$UNIT_CENTIMETERSPERSEC";

        string IDirectDamageMuzzle.ComponentDamageUnit => "$UNIT_HPPERSEC";

        float IDirectDamageMuzzle.DamagePeriod => _damagePeriod;

        void IDirectDamageMuzzle.GetExtraStatDetails(List<(string, string)> rows)
        {
        }

        protected override void Awake()
        {
            if(_immediateSoundSource != null)
            {
                _immediateSoundSource.outputAudioMixerGroup = AudioGroupBinding.Instance.VaccumEffectsGroup;
            }
            if(_delayedSoundSource != null) 
            {
                _delayedSoundSource.outputAudioMixerGroup = AudioGroupBinding.Instance.VaccumEffectsGroup;
            }
            base.Awake();
        }

        public override void Fire()
        {
            _fireRequested = true;
            _isFiring = false;
            _fireAccum = 0f;
            if (_delayedFireCoroutine != null)
            {
                StopCoroutine(_delayedFireCoroutine);
            }

            _delayedFireCoroutine = StartCoroutine(CoroutineDelayedFire());
        }

        public override void StopFire()
        {
            _fireRequested = false;
            _isFiring = false;
            _fireAccum = 0f;
            if (_delayedFireCoroutine != null)
            {
                StopCoroutine(_delayedFireCoroutine);
                _delayedFireCoroutine = null;
            }

            base.StopFire();
        }

        public override void FireEffect()
        {
            //base.FireEffect(); disabled cause it plays the beam effect
            _flashImmediate?.Play();
            _glowerImmediate?.SetFiring(firing: true);
            _fireAnimationImmediate?.Play();

            if(_immediateSoundSource != null)
            {
                _fireSoundImmediate?.PlayFromSource(_immediateSoundSource);
            }
            else if (_fireSoundImmediate != null)
            {
                GlobalSFX.PlayOneShotSpatial(_fireSoundImmediate, base.transform);
            }

            if (_delayedFireEffectCoroutine != null)
            {
                StopCoroutine(_delayedFireEffectCoroutine);
            }

            _delayedFireEffectCoroutine = StartCoroutine(CoroutineDelayedFireEffect());
        }

        public override void StopFireEffect()
        {
            if (_delayedFireEffectCoroutine != null)
            {
                StopCoroutine(_delayedFireEffectCoroutine);
                _delayedFireEffectCoroutine = null;
            }
            _glowerImmediate?.SetFiring(firing: false);
            _glowerDelayed?.SetFiring(firing: false);
            _flashImmediate?.Stop();
            _flashDelayed?.Stop();
            _fireAnimationImmediate?.Stop();
            _fireAnimationDelayed?.Stop();
            _immediateSoundSource?.Stop();
            _delayedSoundSource?.Stop();
            base.StopFireEffect();
        }

        private IEnumerator CoroutineDelayedFire()
        {
            yield return new WaitForSeconds(_fireDelay);
            _delayedFireCoroutine = null;
            if (!_fireRequested)
            {
                yield break;
            }

            base.Fire();
            _isFiring = true;
            _fireAccum = 0f;
        }

        private IEnumerator CoroutineDelayedFireEffect()
        {
            if (_fireDelay > 0f)
            {
                yield return new WaitForSeconds(_fireDelay);
            }

            _delayedFireEffectCoroutine = null;
            if (!_fireRequested)
            {
                yield break;
            }

            BeamLength = 0f;
            Effects?.StartEffect();
            _flashDelayed?.Play();
            _glowerDelayed?.FireInstant();
            _fireAnimationDelayed?.Play();

            if (_delayedSoundSource != null)
            {
                _fireSoundDelayed?.PlayFromSource(_delayedSoundSource);
            }
            else if (_fireSoundDelayed != null)
            {
                GlobalSFX.PlayOneShotSpatial(_fireSoundDelayed, base.transform);
            }
        }


        private void FixedUpdate()
        {
            if (_isFiring)
            {
                MunitionHitInfo munitionHitInfo = (!_cylinderRaycast)
                    ? DoRaycast(base.transform.position, MathHelpers.RandomRayInCone(base.transform.forward, base._accuracy), _raycastRange)
                    : DoRaycast(base.transform.position + MathHelpers.RandomRayInConeToCylinder(base.transform.forward, base._accuracy / 2f, _raycastRange), base.transform.forward, _raycastRange);

                _fireAccum += Time.fixedDeltaTime;
                if (munitionHitInfo != null && _fireAccum >= _damagePeriod)
                {
                    _fireAccum = 0f;
                    base._reportTo?.ReportFired(1);
                    if (_cachedDamageable != null)
                    {
                        HitResult result = _cachedDamageable.DoDamage(munitionHitInfo, this, out float damageDone, out bool destroyed);
                        base._weapon.Platform.ReportDamageDealt(damageDone);
                        base._reportTo?.ReportHit(result, damageDone, destroyed);
                    }

                    munitionHitInfo.Dispose();
                }
            }
            else if (base._isFiringEffect)
            {
                DoRaycast(base.transform.position, MathHelpers.RandomRayInCone(base.transform.forward, base._accuracy), _raycastRange)?.Dispose();
            }
        }

    }
}
