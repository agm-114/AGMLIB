using Munitions;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;

namespace AGMLIB.Compatibility.HaloModernization;

/// <summary>
/// A current CruiseMissile with the legacy plasma-torpedo terminal weave or
/// corkscrew applied after native cruise/search/seek state processing.
/// </summary>
public class ModernEvasiveCruiseMissile : CruiseMissile
{
    public enum TerminalManeuver
    {
        None,
        Weave,
        Corkscrew,
    }

    [Header("Terminal Evasion")]
    [SerializeField]
    [FormerlySerializedAs("_terminalManuever")]
    private TerminalManeuver _terminalManeuver = TerminalManeuver.None;

    [SerializeField]
    [Min(0f)]
    private float _evasionStartDistance = 200f;

    [SerializeField]
    [Min(0f)]
    private float _evasionEndDistance = 20f;

    private Quaternion? _evasionSpace;

    public override bool HotLaunch => true;

    public override string GetWeaponSummary()
    {
        string summary = base.GetWeaponSummary();
        return _terminalManeuver switch
        {
            TerminalManeuver.Weave => summary + " - WEAVE",
            TerminalManeuver.Corkscrew => summary + " - CORKSCREW",
            _ => summary,
        };
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (
            !isServer
            || !isActiveAndEnabled
            || _terminalManeuver == TerminalManeuver.None
            || _seeker == null
            || !_seeker.SearchForTarget(
                out Vector3 position,
                out Vector3 velocity,
                out Vector3 acceleration
            )
        )
        {
            return;
        }

        Vector3 target = MathHelpers.EstimateLeadPosition(
            transform.position,
            __flightSpeed,
            position,
            velocity,
            acceleration,
            out _
        );
        Vector3? offset = GetEvasionOffset(target);
        if (!offset.HasValue)
            return;

        Vector3 direction = transform.position.To(target + offset.Value).normalized;
        PointTowards(direction, Vector3.up, _turnRate);
        Thrust(direction, _motorForce, __flightSpeed, _maxOffAngleThrust);
        UpdateThrusters(direction);
    }

    protected override void OnRepooled()
    {
        base.OnRepooled();
        _evasionSpace = null;
    }

    private Vector3? GetEvasionOffset(Vector3 targetPosition)
    {
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance > _evasionStartDistance)
        {
            _evasionSpace = null;
            return null;
        }

        float distancePhase =
            Mathf.Max(0f, distance - _evasionEndDistance) * 10f;
        _evasionSpace ??= Quaternion.LookRotation(
            transform.position.To(targetPosition).normalized
        );

        float direction = (netId & 1u) == 0u ? 2f : -2f;
        float amplitude = direction * Mathf.Log((distancePhase + 100f) / 100f);
        float horizontal = amplitude * Mathf.Sin(distancePhase / 100f);
        if (_terminalManeuver == TerminalManeuver.Weave)
            return _evasionSpace.Value * (Vector3.right * horizontal);

        float vertical = amplitude * Mathf.Cos(distancePhase / 100f);
        return _evasionSpace.Value
            * (Vector3.right * horizontal + Vector3.up * vertical);
    }
}
