using Game.Map;
using Ships.Controls;
using Sound;
using UnityEngine;
using UnityEngine.Profiling;
using Utility;
using System;

namespace Ships
{
    public class CustomBehaviorThrusterPart : HullPart, IThruster
    {
        private const float _flankDamagePeriod = 1f;
        private const float _flankDamage = 5f;
        private const string _paramPower = "Power";
        private const string _paramWarpOn = "Afterburner";
        private const string _paramWarpPercent = "Afterburner Power";
        [SerializeField]
        private float _power = 1f;
        [SerializeField]
        private DynamicVisibleParticles _particles;
        [SerializeField]
        private bool _tweenParticlePower = false;
        [SerializeField]
        private float _particleThrottleRate = 0.5f;
        [SerializeField]
        private bool _mainEngine;
        [SerializeField]
        private GroupedAudioSource _audioSource;
        private const float CenterlineOffsetMargin = 0.1f;
        private IThrustController _thrustController;
        private CustomBehaviorThrusterPart.Throttle _currentThrottle = CustomBehaviorThrusterPart.Throttle.Idle;
        private float _currentParticlePower = 0.0f;
        private Sides _lastLateral = Sides.None;
        private AttitudeControl _lastAngular = AttitudeControl.None;
        private bool _effectPlaying = false;
        private bool _warpEffectPlaying = false;
        private float _flankDamageAccumulator = 0.0f;

        private int _currentBehaviorIndex = 0;
        private CustomBehaviorThrusterPartConfig[] _customBehaviors;
        private CustomBehaviorThrusterPartConfig _behavior => this._customBehaviors[this._currentBehaviorIndex];

        float IThruster.LateralPower => this._power;

        Direction IThruster.LateralDirection => this._behavior.thrustDirection;

        AttitudeControl IThruster.AttitudeDirections => this._behavior.attitudeInfluence;

        public bool MainEngine => throw new NotImplementedException();

        private void OnValidate()
        {
            if (this._particles != null)
                return;
            this._particles = ((Component)this).GetComponent<DynamicVisibleParticles>();
        }

        protected override void Awake()
        {
            base.Awake();
            this.InitCustomBehaviorThrusterPartConfig();
            this.StopEffect();
        }

        private void InitCustomBehaviorThrusterPartConfig()
        {
            this._customBehaviors = Array.ConvertAll<Component, CustomBehaviorThrusterPartConfig>(
                this.gameObject.GetComponents<CustomBehaviorThrusterPartConfig>(),
                item => (CustomBehaviorThrusterPartConfig)item
            );
            if (this._customBehaviors.Length == 0) {
                throw new InvalidOperationException(
                    "CustomBehaviorThrusterPart gameObject does not contain any CustomBehaviorThrusterPartConfig components"
                );
            }
        }

        private void Update()
        {
            Profiler.BeginSample("Update Thrusters");
            if (this._thrustController != null)
            {
                if (this._mainEngine)
                {
                    if (this._thrustController.WarpMode)
                    {
                        if (!this._warpEffectPlaying)
                        {
                            this._particles.SetBool("Afterburner", true);
                            this._particles.SetFloat("Afterburner Power", this._thrustController.WarpPercent);
                            this._warpEffectPlaying = true;
                            this._currentThrottle = CustomBehaviorThrusterPart.Throttle.Full;
                        }
                        this._particles.SetFloat("Afterburner Power", this._thrustController.WarpPercent);
                    }
                    else if (this._warpEffectPlaying)
                    {
                        this._particles.SetBool("Afterburner", false);
                        this._particles.SetFloat("Afterburner Power", 0.0f);
                        this._warpEffectPlaying = false;
                    }
                }
                if (this._lastLateral != this._thrustController.LateralThrust || this._lastAngular != this._thrustController.AngularThrust || this._thrustController.WarpMode)
                {
                    this._lastAngular = this._thrustController.AngularThrust;
                    this._lastLateral = this._thrustController.LateralThrust;

                    int bestTranslationScore = 0;
                    int bestRotationScore = 0;

                    for (int idx = 0; idx < this._customBehaviors.Length; idx++)
                    {
                        CustomBehaviorThrusterPartConfig candidate = this._customBehaviors[idx];

                        int translationScore = GetTranslationScore(candidate.thrustDirection);
                        int rotationScore = GetRotationScore(candidate.attitudeInfluence);

                        if ((translationScore > bestTranslationScore)
                            || ((translationScore == bestTranslationScore) && (rotationScore > bestRotationScore))
                        )
                        {
                            bestTranslationScore = translationScore;
                            bestRotationScore = rotationScore;
                            this._currentBehaviorIndex = idx;
                        }
                    }

                    bool flag1 = this._lastLateral.IsSet(((IThruster)this).LateralDirection);
                    bool flag2 = this._lastLateral.IsSet(((IThruster)this).LateralDirection.Flip());
                    bool flag3 = (this._lastAngular & ((IThruster)this).AttitudeDirections) != 0;
                    bool flag4 = (this._lastAngular & ((IThruster)this).AttitudeDirections.Invert()) != 0;
                    if (!this._thrustController.WarpMode)
                        this._currentThrottle = !flag1 || flag3 ? (!flag3 ? CustomBehaviorThrusterPart.Throttle.Idle : (!flag2 ? CustomBehaviorThrusterPart.Throttle.Full : CustomBehaviorThrusterPart.Throttle.Idle)) : (!flag4 ? CustomBehaviorThrusterPart.Throttle.Full : CustomBehaviorThrusterPart.Throttle.Half);
                    if (!this._tweenParticlePower)
                    {
                        if (this._currentThrottle != 0 && this._behavior.playsEffects)
                        {
                            this.StartEffect();
                            this._particles.SetFloat("Power", this.GetParticlePower());
                        }
                        else
                            this.StopEffect();
                    }
                }
                if (this._tweenParticlePower)
                {
                    float particlePower = this.GetParticlePower();
                    if ((double)particlePower > (double)this._currentParticlePower)
                        this._currentParticlePower = Mathf.Clamp(this._currentParticlePower + this._particleThrottleRate * Time.deltaTime, 0.0f, particlePower);
                    else if ((double)particlePower < (double)this._currentParticlePower)
                        this._currentParticlePower = Mathf.Clamp(this._currentParticlePower - this._particleThrottleRate * Time.deltaTime, particlePower, 1f);
                    this._particles.SetFloat("Power", this._currentParticlePower);
                    if (!this._behavior.playsEffects || (this._effectPlaying && this._currentThrottle == CustomBehaviorThrusterPart.Throttle.Idle && (double)this._currentParticlePower == 0.0))
                        this.StopEffect();
                    else if (!this._effectPlaying && this._currentThrottle != 0 && this._behavior.playsEffects)
                        this.StartEffect();
                }
            }
            Profiler.EndSample();
        }

        private float GetParticlePower()
        {
            if (this._thrustController.WarpMode)
                return 1f;
            switch (this._currentThrottle)
            {
                case CustomBehaviorThrusterPart.Throttle.Half:
                    return 0.25f;
                case CustomBehaviorThrusterPart.Throttle.Full:
                    return 1f;
                default:
                    return 0.0f;
            }
        }

        private void FixedUpdate()
        {
            if (!this._baseRpcProvider.IsHost || this._thrustController == null || !this._thrustController.Overdrive || this._currentThrottle <= CustomBehaviorThrusterPart.Throttle.Idle)
                return;
            this._flankDamageAccumulator += Time.fixedDeltaTime;
            if ((double)this._flankDamageAccumulator >= 1.0)
            {
                this._flankDamageAccumulator = 0.0f;
                if ((double)UnityEngine.Random.value <= (double)this._thrustController.FlankDamageProbability)
                    this.DoDamageToSelf(5f);
            }
        }

        private void StartEffect()
        {
            if (this._effectPlaying)
                return;
            if (this._particles != null)
                this._particles.Play();
            if (this._audioSource != null)
                this._audioSource.Play();
            this._effectPlaying = true;
        }

        private void StopEffect()
        {
            if (!this._effectPlaying)
                return;
            if (this._particles != null)
                this._particles.Stop();
            if (this._audioSource != null)
                this._audioSource.Stop();
            this._effectPlaying = false;
        }

        protected override void PartFunctionalChangedInternal(bool newFunctional)
        {
            base.PartFunctionalChangedInternal(newFunctional);
            this.UpdateDamageEffects(this._thrustController.Visible, this.IsFunctional);
        }

        void IThruster.SetThrustController(IThrustController controller)
        {
            this._thrustController = controller;
            this._thrustController.OnVisibilityChanged += (VisibleObject.VisibilityChanged)(visible => this.UpdateDamageEffects(visible, this.IsFunctional));
        }

        private void UpdateDamageEffects(bool visible, bool functional)
        {
            if (this._particles == null)
                return;
            if (visible && !functional)
            {
                this._particles.SendEvent("StartDamage");
                this._particles.SetBool("Damaged", true);
            }
            else
            {
                this._particles.SendEvent("StopDamage");
                this._particles.SetBool("Damaged", false);
            }
        }

        private enum Throttle
        {
            Idle,
            Half,
            Full,
        }

        private int GetTranslationScore(Direction thrustDirection)
        {
            bool helpsTranslation = this._lastLateral.IsSet(thrustDirection);
            bool hurtsTranslation = this._lastLateral.IsSet(thrustDirection.Flip());

            if (helpsTranslation && !hurtsTranslation)
                return 1;
            if (hurtsTranslation && !helpsTranslation)
                return -1;
            return 0;
        }

        private int GetRotationScore(AttitudeControl attitudeInfluence)
        {
            bool helpsRotation = (this._lastAngular & attitudeInfluence) != 0;
            bool hurtsRotation = (this._lastAngular & attitudeInfluence.Invert()) != 0;
            bool hasIrrelevantRotation = (this._lastAngular.Invert() & attitudeInfluence) != 0;

            int score = 0;
            if (helpsRotation) score += 1;
            if (hurtsRotation) score -= 2;
            if (hasIrrelevantRotation) score -= 1;

            return score;
        }
    }
}
