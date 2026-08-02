using System.Collections.Generic;
using Munitions;
using UnityEngine;
using Utility;

namespace AGMLIB.Compatibility.HaloModernization;

/// <summary>
/// Adds position/TRP authoring support to the current command seeker without
/// reaching into its private track, comms, or save-state fields.
/// </summary>
public class ModernCommandGuidedSeeker : CommandGuidedSeeker
{
    [SerializeField]
    private bool _supportsPositionTargeting = true;

    [SerializeField]
    private bool _supportsWaypoints = true;

    public override bool SupportsPositionTargeting => _supportsPositionTargeting;

    public override bool SupportsWaypoints => _supportsWaypoints;

    public override WaypointPath CruisePathToTarget(Vector3? trackDoglegPoint)
    {
        Vector3? target = InitialTargetPosition();
        if (!target.HasValue || _launchingPlatform == null)
            return null;

        var points = new List<Vector3> { _launchingPlatform.Position };
        if (trackDoglegPoint.HasValue)
            points.Add(trackDoglegPoint.Value);

        Vector3 from = trackDoglegPoint ?? _launchingPlatform.Position;
        Vector3 destination = target.Value;
        float tooCloseDistance = _missile?.TooCloseDistance ?? 0f;
        Vector3 toTarget = from.To(destination);
        if (tooCloseDistance > 0f && toTarget.sqrMagnitude > tooCloseDistance * tooCloseDistance)
            destination -= toTarget.normalized * tooCloseDistance;

        points.Add(destination);
        var path = new WaypointPath(points);
        path.NextWaypoint();
        return path;
    }

    public override string GetTooltipText()
    {
        return $"Type: Command Guided\nSupports TRPs: {(SupportsWaypoints ? "Yes" : "No")}";
    }
}
