// Nebulous, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// Ships.TurretController
using Ships;
using System;
using UnityEngine;
using Utility;

public class AnimatedTurretController : TurretController
{
    private float _minElevation = -5f;

    private float _maxElevation = 85f;

    private float _muzzleVerticalOffset = 0f;

    private float _motorDecelAngle = 7.5f;

    private bool _forceHardTraversalLimits = false;

    [SerializeField]
    private Animator _idleTransitionAnimator;

    [SerializeField]
    private string _openParamName = "played";

    [SerializeField]
    private string _openTagName = "played";

    [Tooltip("The delay after a weapon aims that it is considered neutral and goes to its idle position.")]
    [SerializeField]
    private float _idleTimeDelay = 5;

    [Tooltip("The rate at which the weapon will go toward the idle position via left/right")]
    [SerializeField]
    private float _idleTraverseRate = 5;

    [Tooltip("The rate at which the weapon will go toward the idle position via up/down")]
    [SerializeField]
    private float _idleElevationRate = 10;

    [Tooltip("The direction the body faces when idle. 0,0,0 is directly forward")]
    [SerializeField]
    private Vector3 _idleDirectionBody = Vector3.zero;

    [Tooltip("The direction the barrel faces when idle. 0,0,0 is directly forward")]
    [SerializeField]
    private Vector3 _idleDirectionBarrel = Vector3.zero;

    [Tooltip("The amount of degrees the barrel and body must be from their idle positions to trigger the idle transition, and to snap to the idle rotation")]
    [SerializeField]
    private float _snapToEpsilon = 0.1f;

    private float _distanceToTarget;

    private bool _insideLimits = true;

    private bool _outsideForwardLimits = true;

    private TraversalLimits? _limits;

    private TraversalLimits? _forwardLimits;

    private bool _hitTraversalLimit = false;

    new public event Action OnHitTraversalLimit;

    private float accum_s = 0;

    public bool played
    {
        get
        {
            if (_idleTransitionAnimator != null)
            {
                return _idleTransitionAnimator.GetCurrentAnimatorStateInfo(0).IsTag(_openTagName);
            }
            return true;
        }
    }

    private bool calced = false;
    Quaternion startrotbarrel;
    Quaternion startrotbody;
    Quaternion idledirectionbarrel;
    Quaternion idledirectionbody;
    float timetomaxrotbarrel;
    float timetomaxrotbody;
    private void Update()
    {
        accum_s -= Time.deltaTime;
        if (accum_s < 0)
        {
            //Debug.LogError("neutral untasked");
            if (!calced)
            {
                startrotbarrel = _barrel.localRotation;
                startrotbody = _body.localRotation;
                idledirectionbarrel = Quaternion.Euler(_idleDirectionBarrel);
                idledirectionbody = Quaternion.Euler(_idleDirectionBody);
                timetomaxrotbarrel = Quaternion.Angle(startrotbarrel, idledirectionbarrel) / (_idleElevationRate + 0.01f);
                timetomaxrotbody = Quaternion.Angle(startrotbody, idledirectionbody) / (_idleTraverseRate + 0.01f);
                calced = true;
            }
            if (Quaternion.Angle(_body.localRotation, idledirectionbody) >= _snapToEpsilon || Quaternion.Angle(_barrel.localRotation, idledirectionbarrel) >= _snapToEpsilon)
            {
                _barrel.localRotation = Quaternion.RotateTowards(_barrel.localRotation, idledirectionbarrel, timetomaxrotbarrel * Time.deltaTime);
                _body.localRotation = Quaternion.RotateTowards(_body.localRotation, idledirectionbody, timetomaxrotbody * Time.deltaTime);
            }
            if (Quaternion.Angle(_body.localRotation, idledirectionbody) <= _snapToEpsilon && Quaternion.Angle(_barrel.localRotation, idledirectionbarrel) <= _snapToEpsilon)
            {
                _barrel.localRotation = idledirectionbarrel;
                _body.localRotation = idledirectionbody;
                accum_s = float.MaxValue;
                //Debug.LogError("reversing");
                SetStateParam(_openParamName, false);
                calced = false;
            }
        }
    }

    public override void FaceTarget(Vector3 target, float traverseRate, float elevationRate)
    {
        if (played == false)
        {
            //Debug.LogError("play");
            SetStateParam(_openParamName, true);
        }
        while (_idleTransitionAnimator.IsInTransition(0) == true)
        {
        }
        accum_s = _idleTimeDelay;
        FaceTargetBasic(target, traverseRate, elevationRate);
    }

    public virtual void FaceTargetBasic(Vector3 target, float traverseRate, float elevationRate)
    {
        traverseRate = _traverseRateStat?.Value ?? traverseRate;
        elevationRate = _elevateRateStat?.Value ?? elevationRate;
        Debug.DrawLine(_barrel.position + _barrel.up * _muzzleVerticalOffset, _barrel.position + _barrel.up * _muzzleVerticalOffset + _barrel.forward * 100f);
        Vector3 vector = base.transform.InverseTransformPoint(target);
        _distanceToTarget = vector.magnitude;
        Vector3 vector2 = new Vector3(vector.x, 0f, vector.z);
        Quaternion quaternion = Quaternion.LookRotation(vector2);
        float num = Quaternion.Angle(quaternion, _body.localRotation);
        Vector3 rhs = _body.localRotation * Vector3.forward;
        Vector3 lhs = _body.localRotation * Vector3.right;
        bool flag = Vector3.Dot(lhs, vector2) >= 0f;
        bool flag2 = Vector3.Dot(Vector3.right, vector2) >= 0f;
        float num2 = Mathf.Lerp(traverseRate * 0.1f, traverseRate, Mathf.Min(num, _motorDecelAngle) / _motorDecelAngle) * Time.fixedDeltaTime;
        float maxDegreesDelta = elevationRate * Time.fixedDeltaTime;
        float num3 = 0f;
        float num4 = 0f;
        _insideLimits = true;
        bool flag3 = false;
        if (_limits.HasValue)
        {
            bool flag4 = Vector3.Dot(Vector3.right, rhs) >= 0f;
            if ((_limits.Value.LimitFiringOnly && !_forceHardTraversalLimits) || flag4 == flag2)
            {
                num3 = (flag ? num2 : (0f - num2));
            }
            else if (!flag4 && flag2)
            {
                num3 = num2;
            }
            else if (flag4 && !flag2)
            {
                num3 = 0f - num2;
            }
            float num5 = MathHelpers.ConvertAngle360to180(_body.localRotation.eulerAngles.y);
            _insideLimits = num5 >= 0f - _limits.Value.LeftAngle && num5 <= _limits.Value.RightAngle;
            if (_forwardLimits.HasValue)
            {
                _outsideForwardLimits = num5 <= 0f - _forwardLimits.Value.LeftAngle || num5 >= _forwardLimits.Value.RightAngle;
            }
            if (!_limits.Value.LimitFiringOnly || _forceHardTraversalLimits)
            {
                if (num5 <= 0f - _limits.Value.LeftAngle && num3 <= 0f)
                {
                    num4 = 0f - _limits.Value.LeftAngle;
                    flag3 = true;
                }
                else if (num5 >= _limits.Value.RightAngle && num3 >= 0f)
                {
                    num4 = _limits.Value.RightAngle;
                    flag3 = true;
                }
                else
                {
                    num4 = ((num <= Mathf.Abs(num3)) ? quaternion.eulerAngles.y : (_body.localRotation.eulerAngles.y + num3));
                }
            }
            else
            {
                num4 = ((num <= Mathf.Abs(num3)) ? quaternion.eulerAngles.y : (_body.localRotation.eulerAngles.y + num3));
            }
        }
        else
        {
            num3 = (flag ? num2 : (0f - num2));
            num4 = ((num <= Mathf.Abs(num3)) ? quaternion.eulerAngles.y : (_body.localRotation.eulerAngles.y + num3));
        }
        float num6 = Mathf.Abs(num4 - _body.localRotation.eulerAngles.y);
        _body.localRotation = Quaternion.Euler(0f, num4, 0f);
        Plane plane = new Plane(quaternion * Vector3.right, 0f);
        if (_muzzleVerticalOffset != 0f)
        {
            vector -= base.transform.InverseTransformDirection(_barrel.up) * _muzzleVerticalOffset;
        }
        Vector3 vector3 = new Vector3(0f, _body.localPosition.y + _barrel.localPosition.y, _barrel.localPosition.z);
        Vector3 vector4 = plane.ClosestPointOnPlane(vector - quaternion * vector3);
        float num7 = Vector3.Angle(quaternion * Vector3.forward, vector4);
        if (vector.y < 0f)
        {
            num7 *= -1f;
        }
        bool flag5 = false;
        Quaternion quaternion2;
        if (num7 < _minElevation || num7 > _maxElevation)
        {
            num7 = Mathf.Clamp(num7, _minElevation, _maxElevation);
            quaternion2 = Quaternion.AngleAxis(0f - num7, Vector3.right);
            flag5 = true;
        }
        else
        {
            quaternion2 = Quaternion.Inverse(quaternion) * Quaternion.LookRotation(vector4);
        }
        float num8 = Quaternion.Angle(quaternion2, _barrel.localRotation);
        _barrel.localRotation = Quaternion.RotateTowards(_barrel.localRotation, quaternion2, maxDegreesDelta);
        if (_limits.HasValue && _limits.Value.LimitFiringOnly && !_forceHardTraversalLimits && _limits.Value.UseElevationLimit && !_insideLimits)
        {
            _insideLimits = num7 >= _limits.Value.ElevationAngle;
        }
        else if (_forwardLimits.HasValue && _forwardLimits.Value.UseElevationLimit && !_outsideForwardLimits)
        {
            _outsideForwardLimits = num7 >= 180f - _forwardLimits.Value.ElevationAngle;
        }
        if (num6 > 0.001f || num8 > 0.001f)
        {
            if (num6 > num8)
            {
                PlayTraverseSound(num6, traverseRate);
            }
            else
            {
                PlayTraverseSound(num8, elevationRate);
            }
        }
        else
        {
            StopTraverseSound();
        }
        SetHitTraversalLimit(flag3 || flag5);
    }

    private void SetHitTraversalLimit(bool limited)
    {
        if (_hitTraversalLimit != limited)
        {
            _hitTraversalLimit = limited;
            this.OnHitTraversalLimit?.Invoke();
        }
    }

    public void SetStateParam(string paramName, bool state)
    {
        if (_idleTransitionAnimator != null)
        {
            _idleTransitionAnimator.SetBool(paramName, state);
        }
    }

    public void SetStateParam(string paramName, float val)
    {
        if (_idleTransitionAnimator != null)
        {
            _idleTransitionAnimator.SetFloat(paramName, val);
        }
    }
}
