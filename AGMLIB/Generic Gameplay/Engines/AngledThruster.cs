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
        private IThrustController? _thrustController;
        private List<RuntimeThruster> _subthrusters = new();
        private const float _flankDamagePeriod = 1f;
        private const float _flankDamage = 5f;
        private const string _paramPower = "Power";
        private const string _paramWarpOn = "Afterburner";
        private const string _paramWarpPercent = "Afterburner Power";
        internal bool CanThrust => isActiveAndEnabled && IsFunctional && _thrustController != null &&
            (_thrustController is not ShipController ship || (!ship.DrivesDisabled && !ship.IsEliminated));
        private float _currentThrottle => !CanThrust ? 0f :
            _afterburnerEffectPlaying ? 1f : _subthrusters.Sum(thruster => thruster.CurrentThottle);

        public bool MainEngine { get => _mainEngine; set => _mainEngine = value; }

        private float _currentParticlePower = 0f;
        private bool _providingLateralThrust = false;
        private bool _effectPlaying = false;
        private bool _afterburnerEffectPlaying = false;
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

        public void SetThrustController(IThrustController? controller)
        {
            if (_thrustController == controller)
                return;
            if (_thrustController != null)
                _thrustController.OnVisibilityChanged -= HandleVisibilityChanged;
            _thrustController = controller;
            if (_thrustController != null)
                _thrustController.OnVisibilityChanged += HandleVisibilityChanged;
        }

        private void HandleVisibilityChanged(bool visible) => UpdateDamageEffects(visible, IsFunctional);
        private void OnDisable() => StopThrust();

        private void StopThrust()
        {
            _subthrusters.ForEach(thruster => thruster.ResetThrottle());
            _currentParticlePower = 0f;
            _afterburnerEffectPlaying = false;
            _flankDamageAccumulator = 0f;
            _particles?.SetFloat(_paramPower, 0f);
            _particles?.SetBool(_paramWarpOn, false);
            _particles?.SetFloat(_paramWarpPercent, 0f);
            StopEffect();
        }

        protected override void OnDestroy()
        {
            SetThrustController(null);
            base.OnDestroy();
            _subthrusters.Clear();
        }

        private void HandleBurnIn()
        {
            bool afterburner = _mainEngine && _thrustController!.AfterburnerEffect;
            if (afterburner != _afterburnerEffectPlaying)
            {
                _particles?.SetBool(_paramWarpOn, afterburner);
                _afterburnerEffectPlaying = afterburner;
            }
            _particles?.SetFloat(_paramWarpPercent, afterburner ? _thrustController!.AfterburnerPercent : 0f);
        }

        private void Update()
        {
            if (!CanThrust)
            {
                StopThrust();
                return;
            }
            Profiler.BeginSample("Update Thrusters");
            HandleBurnIn();

            _subthrusters.ForEach(thruster => thruster.RuntimeUpdate());
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
            _particles?.SetFloat(_paramPower, _currentParticlePower);
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
            if (!newFunctional)
                StopThrust();
            if (_thrustController != null)
                UpdateDamageEffects(_thrustController.Visible, newFunctional);
        }
        public void UpdateDamageEffects(bool visible, bool functional)
        {
            if (visible && !functional)
            {
                _particles?.SendEvent("StartDamage");
                _particles?.SetBool("Damaged", value: true);
            }
            else
            {
                _particles?.SendEvent("StopDamage");
                _particles?.SetBool("Damaged", value: false);
            }
        }

    }

}
