using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ships.RaycastMuzzle;




public class DelayedRezzingMuzzle : RezzingMuzzle
{




    public float FireDelay = 0.5f;

    [SerializeField]
    protected BookendedAudioPlayer _fullBurstAudioEffect;
    [SerializeField]
    protected bool _killBeamOnFire = true;
    [SerializeField]
    protected bool _killBeamOnRefactoryPeriodEnd = true;
    [SerializeField]
    protected RaycastType _castType = RaycastType.Ray;
    [SerializeField]
    protected float _castWidth = 0f;
    [SerializeField]
    protected float _raycastRange = 150f;
    [SerializeField]
    protected float _beamSpeed = float.MaxValue;
    [SerializeField]
    private BeamMuzzleEffects _effects;

    private float _beamLength = 0f;



    [SerializeField]
    private VisualEffect _flashDelayed;

    [SerializeField]
    private Animation _fireAnimationDelayed;

    [SerializeField]
    private VariedSoundEffect _fireSoundDelayed;

    [SerializeField]
    private BarrelGlow _glowerDelayed;


    [SerializeField]
    private float fireEffectRefractoryTime = 0.25f;
    [SerializeField]
    private float _debugFireTimeScale = 0.1f;
    [SerializeField]
    private float _debugFireSlowDuration = 0.25f;

    private float _nextFireEffectTime;
    private bool _inRefractoryPeriod;
    private Vector3 _beamDirection;
    private Vector3 _beamDirectionOffset;
    private Coroutine _debugSlowGameCoroutine;
    private float _debugRestoreGameSpeedAt;
    private float _debugPreviousTimeScale = 1f;
    private bool _delayedShotReady;

     public Transform beamBase => _effects.gameObject.transform;
    public override void Fire()
    {
        StartCoroutine(FireAfterDelay());
    }

    //Return value indicates if the patch should let the call go through or not
    public bool HandleFire(Vector3 shotDirection)
    {
        if (_delayedShotReady)
        {
            _delayedShotReady = false;
            return true;//Let the original Fire() call go through
        }

        bool trackMuzzleForward = shotDirection == transform.forward;
        StartCoroutine(FireAfterDelay(shotDirection, trackMuzzleForward));
        return false;//Don't let the original Fire() call go through, we will call it after the delay
    }

    private IEnumerator FireAfterDelay()
    {
        yield return new WaitForSeconds(FireDelay);
        FireDelayedShot(transform.forward);
    }

    private IEnumerator FireAfterDelay(Vector3 shotDirection, bool trackMuzzleForward)
    {
        yield return new WaitForSeconds(FireDelay);

        if (trackMuzzleForward)
        {
            shotDirection = transform.forward;
        }
        else if (_weapon is FixedDiscreteWeaponComponent fixedWeapon && ((IWeapon)fixedWeapon).CurrentAimPoint() is Vector3 aimPoint)
        {
            shotDirection = transform.position.To(aimPoint).normalized;
        }

        FireDelayedShot(shotDirection);
    }

    private void FireDelayedShot(Vector3 shotDirection)
    {
        _delayedShotReady = true;// Set this flag so that the next call to Fire() will go through
        Fire(shotDirection);
    }

    protected virtual void Awake()
    {
        if (_effects != null)
        {
            _effects.SetMaxLength(_raycastRange);
        }
    }

    protected void Start()
    {
        _effects.StopEffect();
    }

    protected void Update()
    {
        if (_inRefractoryPeriod)
        {
            DoRaycast();
        }
        if (_inRefractoryPeriod && Time.time >= _nextFireEffectTime)
        {
            //Debug.LogError("ENDING REFRACTORY PERIOD");
            _inRefractoryPeriod = false;
            if (_killBeamOnRefactoryPeriodEnd)
            {
                _effects?.StopEffect();
            }
            _fullBurstAudioEffect?.Stop();
        }
    }


    public override void FireEffect()
    {
        if (Time.time >= _nextFireEffectTime)
        {
            base.FireEffect();
            _beamLength = 0f;
            _beamDirectionOffset = MathHelpers.RandomRayInCone(Vector3.forward, base._accuracy) - Vector3.forward;
            _beamDirectionOffset = Vector3.zero;
            DoRaycast();
            _effects?.StopEffect();
            _effects?.StartEffect();
            if (!_inRefractoryPeriod)
            {
                _fullBurstAudioEffect?.Play();
            }
            _inRefractoryPeriod = true;
        }

        _nextFireEffectTime = Time.time + fireEffectRefractoryTime;
        StartCoroutine(FireEffectAfterDelay());
    }

    private IEnumerator FireEffectAfterDelay()
    {
        yield return new WaitForSeconds(FireDelay);
        DelayedFireEffect();
    }

    protected void DelayedFireEffect()
    {
        //RequestGameSlowdown();
        if(_killBeamOnFire)
        {
            _effects?.StopEffect();
        }
        _flashDelayed?.Play();
        _glowerDelayed?.FireInstant();

        if ((UnityEngine.Object)(object)_fireAnimationDelayed != null)
        {
            _fireAnimationDelayed.Play();
        }

        if (_fireSoundDelayed != null)
        {
            GlobalSFX.PlayOneShotSpatial(_fireSoundDelayed, base.transform);
        }
    }

    protected void DoRaycast()
    {
        
        float length = _raycastRange;
        if (_effects == null)
            return;
        _beamDirection = beamBase.TransformDirection(Vector3.forward + _beamDirectionOffset).normalized;
        Ray r = new Ray(beamBase.position, _beamDirection);
        if ((_castType == RaycastType.Ray) ? Physics.Raycast(r, out var hit, length, 524801, QueryTriggerInteraction.Ignore) : Physics.SphereCast(r, _castWidth, out hit, length, 524801))
        {
            //SpawnImpactCube(hit.point);
            Vector3 toHit = beamBase.position.To(hit.point);
            _beamLength = Mathf.Clamp(_beamLength + _beamSpeed * Time.deltaTime, 0f, Mathf.Min(length, toHit.magnitude));
            if (Mathf.Abs(_beamLength - toHit.magnitude) <= 1f)
            {
                _effects.PositionHitEffect(on: true, hit.point, null);
            }
            else
            {
                _effects.PositionHitEffect(on: false, Vector3.zero, null);
            }

        }
        else
        {
            _beamLength = Mathf.Clamp(_beamLength + _beamSpeed * Time.deltaTime, 0f, length);
            _effects.PositionHitEffect(on: false, Vector3.zero, null);
        }
        _effects.SetBeamLength(_beamLength);

        
    }

    private void SpawnImpactCube(Vector3 position)
    {
        GameObject impactCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        impactCube.name = $"{nameof(DelayedRezzingMuzzle)} Impact Cube";
        impactCube.transform.position = position;
        impactCube.transform.localScale = Vector3.one * 0.1f;
        if (impactCube.TryGetComponent(out Collider collider))
        {
            Destroy(collider);
        }
    }

    private void RequestGameSlowdown()
    {
        if (_debugFireSlowDuration <= 0f || _debugFireTimeScale <= 0f)
            return;

        _debugRestoreGameSpeedAt = Time.unscaledTime + _debugFireSlowDuration;
        if (_debugSlowGameCoroutine == null)
        {
            _debugPreviousTimeScale = Time.timeScale;
        }
        Time.timeScale = Mathf.Min(Time.timeScale, _debugFireTimeScale);
    }

    private IEnumerator RestoreGameSpeedAfterDelay()
    {
        while (Time.unscaledTime < _debugRestoreGameSpeedAt)
        {
            yield return null;
        }

        Time.timeScale = _debugPreviousTimeScale;
        _debugSlowGameCoroutine = null;
    }
}
