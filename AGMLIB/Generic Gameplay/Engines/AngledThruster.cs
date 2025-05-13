using UnityEngine.Profiling;

namespace Lib.Generic_Gameplay.Engines
{
    public class AngledThruster : HullPart, IDynamicCollectablePart
    {

        public float Power => _power;
        public bool ContributeAngular => _contributeAngular;
        [SerializeField] private bool _contributeAngular;
        [SerializeField] public bool _debugmode = true;
        [SerializeField] private float _power = 1f;
        [SerializeField] private Vector3 _overrideNormal;
        [SerializeField] private DynamicVisibleParticles _particles;
        [SerializeField] private bool _tweenParticlePower = false;
        [SerializeField] private float _particleThrottleRate = 0.5f;
        [SerializeField] public bool _mainEngine = false;
        [SerializeField] private GroupedAudioSource _audioSource;
        private const float CenterlineOffsetMargin = 0.1f;
        private IThrustController _thrustController;
        private List<RuntimeThruster> _subthrusters = new();
        private const float _flankDamagePeriod = 1f;
        private const float _flankDamage = 5f;
        private const string _paramPower = "Power";
        private const string _paramWarpOn = "Afterburner";
        private const string _paramWarpPercent = "Afterburner Power";
        private float _currentThrottle
        {
            get
            {
                if (_warpEffectPlaying)
                    return 1;
                if (_subthrusters.Count > 0)
                    return _subthrusters.ConvertAll(rt => rt.CurrentThottle).Sum();

                return 0;
            }
        }

        public bool MainEngine { get => _mainEngine; set => _mainEngine = value; }

        private float _currentParticlePower = 0f;
        private bool _providingLateralThrust = false;
        private bool _effectPlaying = false;
        private bool _warpEffectPlaying = false;
        private float _flankDamageAccumulator = 0f;

        private void OnValidate()
        {
            if (_particles == null)
            {
                _particles = GetComponent<DynamicVisibleParticles>();
            }
        }


        public void Initialize()
        {

            Vector3 thrustervector = base.transform.localRotation * Vector3.forward;
            //Debug.LogError(gameObject.name + " has vector" + thrustervector);

            if (_overrideNormal != Vector3.zero)
            {

                thrustervector = _overrideNormal;
                //Debug.LogError(gameObject.name + " has manually set to" + thrustervector);

            }

            for (int i = 0; i < 3; i++)
            {
                // Create a vector with 0s except for the current axis

                Vector3 subvector = thrustervector;
                switch (i)
                {
                    case 0:
                        subvector = new Vector3(subvector.x, 0f, 0f);
                        break;
                    case 1:
                        subvector = new Vector3(0f, subvector.y, 0f);
                        break;
                    case 2:
                        subvector = new Vector3(0f, 0f, subvector.z);
                        break;
                }
                if (subvector.magnitude < 0.01)
                    continue;
                RuntimeThruster subthruster = gameObject.AddComponent<RuntimeThruster>();
                subthruster.Setup(this, subvector);
                _subthrusters.Add(subthruster);
            }

        }

        protected override void Awake()
        {
            base.Awake();
            StopEffect();
        }

        public void SetThrustController(IThrustController controller)
        {
            //Debug.LogError("parent setting controller");
            _thrustController = controller;
            _thrustController.OnVisibilityChanged += delegate (bool visible)
            {
                UpdateDamageEffects(visible, base.IsFunctional);
            };
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            _thrustController = null;
            _subthrusters.Clear();
        }
        private void HandleBurnIn()
        {
            if (!_mainEngine)
                return;
            if (_thrustController.WarpMode)
            {
                _particles.SetFloat("Afterburner Power", _thrustController.WarpPercent);
                _particles.SetBool("Afterburner", value: true);
                _warpEffectPlaying = true;


            }
            else if (_warpEffectPlaying)
            {
                _particles.SetBool("Afterburner", value: false);
                _particles.SetFloat("Afterburner Power", 0f);
                _warpEffectPlaying = false;
            }
        }

        private void Update()
        {
            Profiler.BeginSample("Update Thrusters");
            if (_thrustController == null)
            {
                //Debug.LogError("null controller");

                return;
            }
            HandleBurnIn();
            if (_thrustController.WarpMode)
                return;
            //Debug.LogError("runtime update");

            foreach (RuntimeThruster runtimeThruster in _subthrusters)
                runtimeThruster.RuntimeUpdate();
            if (!_tweenParticlePower)
            {
                _currentParticlePower = _currentThrottle;
            }
            float targetPower = _currentThrottle;
            if (targetPower > _currentParticlePower)
            {
                _currentParticlePower = Mathf.Clamp(_currentParticlePower + _particleThrottleRate * Time.deltaTime, 0f, targetPower);
            }
            else if (targetPower < _currentParticlePower)
            {
                _currentParticlePower = Mathf.Clamp(_currentParticlePower - _particleThrottleRate * Time.deltaTime, targetPower, 1f);
            }
            _particles.SetFloat("Power", _currentParticlePower);
            if (_currentParticlePower == 0f)
            {
                StopEffect();
            }
            else
            {
                StartEffect();
            }


            Profiler.EndSample();
        }


        private void FixedUpdate()
        {
            if (!base._baseRpcProvider.IsHost || _thrustController == null || !_thrustController.Overdrive || _currentThrottle <= 0 || !_providingLateralThrust)
            {
                return;
            }
            _flankDamageAccumulator += Time.fixedDeltaTime;
            if (_flankDamageAccumulator >= 1f)
            {
                _flankDamageAccumulator = 0f;
                if (UnityEngine.Random.value <= _thrustController.FlankDamageProbability * _currentThrottle)
                {
                    DoDamageToSelf(5f);
                }
            }
        }

        private void StartEffect()
        {
            if (_effectPlaying)
                return;
            _particles?.Play();
            _audioSource?.Play();
            _effectPlaying = true;
        }

        private void StopEffect()
        {
            if (!_effectPlaying)
                return;
            _particles?.Stop();
            _audioSource?.Stop();
            _effectPlaying = false;
        }

        protected override void PartFunctionalChangedInternal(bool newFunctional)
        {
            base.PartFunctionalChangedInternal(newFunctional);
            UpdateDamageEffects(_thrustController.Visible, base.IsFunctional);
        }
        public void UpdateDamageEffects(bool visible, bool functional)
        {
            if (visible && !functional)
            {
                _particles.SendEvent("StartDamage");
                _particles.SetBool("Damaged", value: true);
            }
            else
            {
                _particles.SendEvent("StopDamage");
                _particles.SetBool("Damaged", value: false);
            }
        }


    }

}
