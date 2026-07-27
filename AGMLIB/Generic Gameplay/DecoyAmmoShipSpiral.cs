using Game.Units;
using UnityEngine;

namespace Lib.Generic_Gameplay;

[DisallowMultipleComponent]
[RequireComponent(typeof(ShipController))]
internal sealed class DecoyAmmoShipSpiral : MonoBehaviour
{
    private const int CoverageProbeCount = 128;
    private const int MaxPointCount = 2048;
    private const float CoverageProbeDistance = 1000f;

    private ShipController _ship = null!;
    private int _pointCount = 1;
    private int _nextPoint;
    private bool[] _claimed = new bool[1];

    private void Awake() => _ship = GetComponent<ShipController>();

    internal void Recalculate()
    {
        DecoyAmmoFibonacciSlew[] participants =
            GetComponentsInChildren<DecoyAmmoFibonacciSlew>(true);
        float liveShells = 0f;
        foreach (DecoyAmmoFibonacciSlew participant in participants)
        {
            if (participant.ParticipatesInSpiral)
            {
                liveShells += participant.LiveShellCapacity;
            }
        }

        if (liveShells <= 0f)
        {
            SetPointCount(1);
            _nextPoint = 0;
            _claimed[0] = false;
            return;
        }

        float reachableFraction = EstimateReachableFraction(participants);
        SetPointCount(Mathf.Clamp(
            Mathf.CeilToInt(Mathf.Max(1f, liveShells) / reachableFraction),
            1,
            MaxPointCount));
    }

    internal bool TryGetNextTarget(
        DecoyAmmoFibonacciSlew requester,
        float distance,
        out Vector3 target)
    {
        if (TryClaimNextTarget(requester, distance, out target))
        {
            return true;
        }

        ReleaseClaimsReachableBy(requester, distance);
        return TryClaimNextTarget(requester, distance, out target);
    }

    private bool TryClaimNextTarget(
        DecoyAmmoFibonacciSlew requester,
        float distance,
        out Vector3 target)
    {
        Vector3 origin = _ship.Position;
        for (int offset = 0; offset < _pointCount; offset++)
        {
            int point = (_nextPoint + offset) % _pointCount;
            if (_claimed[point])
            {
                continue;
            }

            target = origin + GetDirection(point, _pointCount) * distance;
            if (!requester.CanTrainOnTarget(target))
            {
                continue;
            }

            _claimed[point] = true;
            _nextPoint = (point + 1) % _pointCount;
            return true;
        }

        target = default;
        return false;
    }

    private void ReleaseClaimsReachableBy(
        DecoyAmmoFibonacciSlew requester,
        float distance)
    {
        Vector3 origin = _ship.Position;
        for (int point = 0; point < _pointCount; point++)
        {
            if (!_claimed[point])
            {
                continue;
            }

            Vector3 target = origin + GetDirection(point, _pointCount) * distance;
            if (requester.CanTrainOnTarget(target))
            {
                _claimed[point] = false;
            }
        }
    }

    private void SetPointCount(int pointCount)
    {
        if (_pointCount == pointCount)
        {
            return;
        }

        _pointCount = pointCount;
        _nextPoint %= _pointCount;
        _claimed = new bool[_pointCount];
    }

    internal Vector3 GetTopTarget(float distance) =>
        _ship.Position + transform.up * distance;

    private float EstimateReachableFraction(
        DecoyAmmoFibonacciSlew[] participants)
    {
        int reachable = 0;
        Vector3 origin = _ship.Position;
        for (int point = 0; point < CoverageProbeCount; point++)
        {
            Vector3 target =
                origin +
                GetDirection(point, CoverageProbeCount) * CoverageProbeDistance;
            foreach (DecoyAmmoFibonacciSlew participant in participants)
            {
                if (participant.ParticipatesInSpiral &&
                    participant.CanTrainOnTarget(target))
                {
                    reachable++;
                    break;
                }
            }
        }

        return Mathf.Max(
            reachable / (float)CoverageProbeCount,
            1f / CoverageProbeCount);
    }

    private Vector3 GetDirection(int point, int pointCount)
    {
        if (pointCount == 1)
        {
            return transform.up;
        }

        float progress = point / (pointCount - 1f);
        float vertical = 1f - progress * 2f;
        float radius = Mathf.Sqrt(1f - vertical * vertical);
        float turns = Mathf.Sqrt(pointCount / Mathf.PI);
        float theta = Mathf.PI * 2f * turns * progress;
        return transform.right * (Mathf.Cos(theta) * radius) +
            transform.forward * (Mathf.Sin(theta) * radius) +
            transform.up * vertical;
    }
}
