using System;
using System.Collections.Generic;
using Game.Sensors;
using Game.UI.Chessboard;
using Game.Units;
using Lib.Testing;
using Shapes;
using Ships;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using Utility;


public enum DopplerConeAngleMode
{
    UseConeAngleAsMinimum,
    UseConeAngleAsBase
}

[Serializable]
public class DopplerNotchSettings : MonoBehaviour
{
    [Tooltip("A target breaks detection when its velocity is this many degrees or less from perpendicular to the sensor line of sight.")]
    [Range(0f, 90f)]
    public float BreakAngle = 5f;

    [Tooltip("Targets moving slower than this cannot notch the sensor. Set to -1 to disable the minimum-speed check.")]
    [Min(-1f)]
    public float MinimumTargetSpeed = -1f;

    [Tooltip("Absolute target velocity along the line of sight below which Doppler detection is lost. Set to 0 to disable.")]
    [Min(0f)]
    public float DopplerVelocityFloor = 1;

    [Tooltip("Treat Break Angle as either the minimum cone half-angle or the base angle added to the floor-derived half-angle.")]
    public DopplerConeAngleMode ConeAngleMode = DopplerConeAngleMode.UseConeAngleAsMinimum;

    [Tooltip("Use target velocity relative to the sensor platform instead of the target's world velocity.")]
    public bool UseRelativeVelocity = false;

    [Header("TacView")]
    public bool ShowTacViewCone = true;

    [Min(0f)]
    public float TacViewConeLength = 500f;

    [Tooltip("Distance from the target to the small end of the seeker-style TacView cone.")]
    [Min(0f)]
    public float TacViewConeStartDistance = 5f;

    [Tooltip("Seconds to keep showing the TacView cone after this sensor loses its track.")]
    [Min(0f)]
    public float TacViewPersistence = 60f;

    internal void AddFormattedStats(List<(string, string)> rows)
    {
        rows.Add(("Doppler Notch", $"Within {BreakAngle:0.##}° of perpendicular"));
        rows.Add(("Minimum Notch Speed", MinimumTargetSpeed < 0f ? "Disabled" : $"{MinimumTargetSpeed:0.##} m/s"));
        rows.Add(("Doppler Velocity Floor", DopplerVelocityFloor <= 0f ? "Disabled" : $"{DopplerVelocityFloor:0.##} m/s radial"));
        rows.Add(("Cone Angle Mode", ConeAngleMode == DopplerConeAngleMode.UseConeAngleAsBase ? "Configured angle is base" : "Configured angle is minimum"));
        rows.Add(("Notch Velocity", UseRelativeVelocity ? "Relative to sensor" : "World velocity"));
        rows.Add(("TacView Track Memory", $"{Mathf.Max(0f, TacViewPersistence):0.##} s"));
    }
}

public interface IDopplerNotchSensor
{
    DopplerNotchSettings DopplerSettings { get; }

    Component SensorComponent { get; }
}

public static class DopplerNotchMath
{
    private const float DirectionEpsilon = 0.0001f;

    public static bool IsNotching(IDopplerNotchSensor sensor, ISensorTrackable target)
    {
        if (sensor == null || target == null)
        {
            return false;
        }

        Component sensorComponent = sensor.SensorComponent;
        if (sensorComponent == null)
        {
            return false;
        }

        DopplerNotchSettings settings = sensor.DopplerSettings;
        Vector3 velocity = GetTargetVelocity(sensor, target);
        float velocitySquared = velocity.sqrMagnitude;
        if (settings.MinimumTargetSpeed >= 0f && velocitySquared < settings.MinimumTargetSpeed * settings.MinimumTargetSpeed)
        {
            return false;
        }

        Vector3 lineOfSight = sensorComponent.transform.position.To(target.Position);
        if (lineOfSight.sqrMagnitude < DirectionEpsilon)
        {
            return false;
        }

        if (velocitySquared < DirectionEpsilon)
        {
            return settings.DopplerVelocityFloor > 0f;
        }

        float radialFraction = Mathf.Abs(Vector3.Dot(velocity.normalized, lineOfSight.normalized));
        float effectiveBreakAngle = GetEffectiveBreakAngleDegrees(settings, velocity);
        float maximumRadialFraction = Mathf.Sin(effectiveBreakAngle * Mathf.Deg2Rad);
        return radialFraction <= maximumRadialFraction;
    }

    internal static float GetEffectiveBreakAngleDegrees(DopplerNotchSettings settings, Vector3 velocity)
    {
        float configuredAngle = Mathf.Clamp(settings.BreakAngle, 0f, 90f);
        float velocityFloor = Mathf.Max(0f, settings.DopplerVelocityFloor);
        if (velocityFloor <= 0f)
        {
            return configuredAngle;
        }

        float floorAngle = velocity.sqrMagnitude < DirectionEpsilon
            ? 90f
            : Mathf.Asin(Mathf.Clamp01(velocityFloor / velocity.magnitude)) * Mathf.Rad2Deg;
        if (settings.ConeAngleMode == DopplerConeAngleMode.UseConeAngleAsBase)
        {
            return Mathf.Min(90f, configuredAngle + floorAngle);
        }

        return Mathf.Max(configuredAngle, floorAngle);
    }

    internal static Vector3 GetTargetVelocity(IDopplerNotchSensor sensor, ISensorTrackable target)
    {
        Vector3 velocity = target.Velocity;
        if (!sensor.DopplerSettings.UseRelativeVelocity)
        {
            return velocity;
        }

        Rigidbody sensorBody = sensor.SensorComponent.GetComponentInParent<Rigidbody>();
        return velocity - (sensorBody == null ? Vector3.zero : sensorBody.velocity);
    }

    internal static bool ApplyNotchFilter(IDopplerNotchSensor sensor, ISignature signature, bool otherwiseVisible)
    {
        if (!otherwiseVisible || signature?.Trackable == null)
        {
            return false;
        }

        DopplerNotchContactRegistry.RefreshExisting(sensor, signature.Trackable);
        return !IsNotching(sensor, signature.Trackable);
    }
}

public class DopplerInternalActiveSensorComponent : InternalActiveSensorComponent, IDopplerNotchSensor
{
    [SerializeField]
    private DopplerNotchSettings _dopplerSettings = new DopplerNotchSettings();

    public DopplerNotchSettings DopplerSettings => _dopplerSettings ??= new DopplerNotchSettings();

    public Component SensorComponent => this;

    protected override bool CanSeeSignature(IActiveSignature sig)
    {
        return DopplerNotchMath.ApplyNotchFilter(this, sig, base.CanSeeSignature(sig));
    }

    public override void GetFormattedStats(List<(string, string)> rows, bool full, int groupSize = 1)
    {
        base.GetFormattedStats(rows, full, groupSize);
        DopplerSettings.AddFormattedStats(rows);
    }
}

public class DopplerActiveFireControlSensor : ActiveFireControlSensor, IDopplerNotchSensor
{
    [SerializeField]
    private DopplerNotchSettings _dopplerSettings = new DopplerNotchSettings();

    public DopplerNotchSettings DopplerSettings => _dopplerSettings ??= new DopplerNotchSettings();

    public Component SensorComponent => this;

    public override bool CanSeeSignature(ISignature sig)
    {
        return DopplerNotchMath.ApplyNotchFilter(this, sig, base.CanSeeSignature(sig));
    }

    protected override bool CanMaintainLock(ISignature sig)
    {
        return base.CanMaintainLock(sig) && (sig?.Trackable == null || !DopplerNotchMath.IsNotching(this, sig.Trackable));
    }

    public override void GetFormattedStats(List<(string, string)> rows, bool full, int groupSize = 1)
    {
        base.GetFormattedStats(rows, full, groupSize);
        DopplerSettings.AddFormattedStats(rows);
    }

    public override void GetFormattedStats(List<(string, string)> rows, bool full, IEnumerable<IHullComponent> group)
    {
        GetFormattedStats(rows, full, group.Count());
    }
}

public class DopplerNotchContact
{
    internal DopplerNotchContact(IDopplerNotchSensor sensor, ISensorTrackable target)
    {
        Sensor = sensor;
        Target = target;
    }

    public IDopplerNotchSensor Sensor { get; }

    public ISensorTrackable Target { get; }

    public bool Tracking { get; internal set; }

    public bool Locked { get; internal set; }

    internal float LastUpdateTime { get; set; }
}

public static class DopplerNotchContactRegistry
{
    private static readonly List<DopplerNotchContact> Contacts = new List<DopplerNotchContact>();

    public static void TrackAcquired(SensorTrack track, ISensor sensor)
    {
        if (track?.Trackable == null || sensor is not IDopplerNotchSensor dopplerSensor)
        {
            return;
        }

        DopplerNotchContact contact = GetOrCreate(dopplerSensor, track.Trackable);
        contact.Tracking = true;
        contact.LastUpdateTime = Time.time;
    }

    public static void TrackReleased(SensorTrack track, ISensor sensor)
    {
        DopplerNotchContact? contact = Find(sensor as IDopplerNotchSensor, track?.Trackable);
        if (contact == null)
        {
            return;
        }

        contact.Tracking = false;
        contact.Locked = false;
        contact.LastUpdateTime = Time.time;
    }

    public static void LockAcquired(SensorTrack track, ISensor sensor)
    {
        if (track?.Trackable == null || sensor is not IDopplerNotchSensor dopplerSensor)
        {
            return;
        }

        DopplerNotchContact contact = GetOrCreate(dopplerSensor, track.Trackable);
        contact.Tracking = true;
        contact.Locked = true;
        contact.LastUpdateTime = Time.time;
    }

    public static void LockReleased(SensorTrack track, ISensor sensor)
    {
        DopplerNotchContact? contact = Find(sensor as IDopplerNotchSensor, track?.Trackable);
        if (contact == null)
        {
            return;
        }

        contact.Locked = false;
        contact.LastUpdateTime = Time.time;
    }

    internal static void RefreshExisting(IDopplerNotchSensor sensor, ISensorTrackable target)
    {
        DopplerNotchContact? contact = Find(sensor, target);
        if (contact?.Tracking == true)
        {
            contact.LastUpdateTime = Time.time;
        }
    }

    public static void GetVisibleContacts(ISensorTrackable target, List<DopplerNotchContact> results)
    {
        results.Clear();
        for (int i = Contacts.Count - 1; i >= 0; i--)
        {
            DopplerNotchContact contact = Contacts[i];
            Component sensorComponent = contact.Sensor.SensorComponent;
            bool targetDestroyed = contact.Target is UnityEngine.Object targetObject && !targetObject;
            bool destroyed = sensorComponent == null || targetDestroyed;
            float persistence = Mathf.Max(0f, contact.Sensor.DopplerSettings.TacViewPersistence);
            bool expired = !contact.Tracking && Time.time - contact.LastUpdateTime > persistence;
            if (destroyed || expired)
            {
                Contacts.RemoveAt(i);
                continue;
            }

            if (ReferenceEquals(contact.Target, target) && contact.Sensor.DopplerSettings.ShowTacViewCone)
            {
                results.Add(contact);
            }
        }
    }

    private static DopplerNotchContact GetOrCreate(IDopplerNotchSensor sensor, ISensorTrackable target)
    {
        DopplerNotchContact? contact = Find(sensor, target);
        if (contact != null)
        {
            return contact;
        }

        contact = new DopplerNotchContact(sensor, target);
        Contacts.Add(contact);
        return contact;
    }

    private static DopplerNotchContact? Find(IDopplerNotchSensor? sensor, ISensorTrackable? target)
    {
        if (sensor == null || target == null)
        {
            return null;
        }

        for (int i = 0; i < Contacts.Count; i++)
        {
            DopplerNotchContact contact = Contacts[i];
            if (ReferenceEquals(contact.Sensor, sensor) && ReferenceEquals(contact.Target, target))
            {
                return contact;
            }
        }

        return null;
    }
}

[HarmonyPatch(typeof(SensorTrack), nameof(SensorTrack.AddSensor))]
internal static class DopplerNotchTrackAddedPatch
{
    private static void Postfix(SensorTrack __instance, ISensor sensor, AcquisitionType acqType)
    {
        if (acqType == AcquisitionType.Active)
        {
            DopplerNotchContactRegistry.TrackAcquired(__instance, sensor);
        }
    }
}

[HarmonyPatch(typeof(SensorTrack), nameof(SensorTrack.RemoveSensor))]
internal static class DopplerNotchTrackRemovedPatch
{
    private static void Postfix(SensorTrack __instance, ISensor sensor, AcquisitionType acqType, bool __result)
    {
        if (__result && acqType == AcquisitionType.Active)
        {
            DopplerNotchContactRegistry.TrackReleased(__instance, sensor);
        }
    }
}

[HarmonyPatch(typeof(SensorTrack), nameof(SensorTrack.AcquireLock))]
internal static class DopplerNotchLockAddedPatch
{
    private static void Postfix(SensorTrack __instance, ISensor sensor)
    {
        DopplerNotchContactRegistry.LockAcquired(__instance, sensor);
    }
}

[HarmonyPatch(typeof(SensorTrack), nameof(SensorTrack.ReleaseLock))]
internal static class DopplerNotchLockRemovedPatch
{
    private static void Postfix(SensorTrack __instance, ISensor sensor)
    {
        DopplerNotchContactRegistry.LockReleased(__instance, sensor);
    }
}

public class DopplerNotchOverlay : ImmediateModeShapeDrawer
{
    private const float DirectionEpsilon = 0.0001f;

    private const float LineThickness = 0.5f;

    private readonly List<DopplerNotchContact> _contacts = new List<DopplerNotchContact>();

    private ShipController? _ship;

    private bool _visible;

    public void SetShip(ShipController ship)
    {
        _ship = ship;
    }

    public void SetVisible(bool visible)
    {
        _visible = visible;
    }

    public override void DrawShapes(Camera cam)
    {
        if (!_visible || _ship == null || _ship.IsEliminated || _ship.Trackable == null)
        {
            return;
        }

        DopplerNotchContactRegistry.GetVisibleContacts(_ship.Trackable, _contacts);
        foreach (DopplerNotchContact contact in _contacts)
        {
            DrawContact(cam, contact);
        }
    }

    private void DrawContact(Camera cam, DopplerNotchContact contact)
    {
        IDopplerNotchSensor sensor = contact.Sensor;
        Component sensorComponent = sensor.SensorComponent;
        DopplerNotchSettings settings = sensor.DopplerSettings;
        if (sensorComponent == null || settings.TacViewConeLength <= 0f)
        {
            return;
        }

        Vector3 targetPosition = contact.Target.Position;
        Vector3 lineOfSight = sensorComponent.transform.position.To(targetPosition);
        if (lineOfSight.sqrMagnitude < DirectionEpsilon)
        {
            return;
        }

        lineOfSight.Normalize();
        Vector3 velocity = DopplerNotchMath.GetTargetVelocity(sensor, contact.Target);
        Vector3 closestNotchDirection = Vector3.ProjectOnPlane(velocity, lineOfSight);
        if (closestNotchDirection.sqrMagnitude < DirectionEpsilon)
        {
            closestNotchDirection = Vector3.ProjectOnPlane(_ship!.transform.forward, lineOfSight);
        }
        if (closestNotchDirection.sqrMagnitude < DirectionEpsilon)
        {
            closestNotchDirection = Vector3.Cross(lineOfSight, Vector3.up);
        }
        if (closestNotchDirection.sqrMagnitude < DirectionEpsilon)
        {
            closestNotchDirection = Vector3.Cross(lineOfSight, Vector3.right);
        }
        closestNotchDirection.Normalize();

        bool notching = DopplerNotchMath.IsNotching(sensor, contact.Target);
        Color color = contact.Locked ? GameColors.Red : notching ? GameColors.Green : GameColors.Yellow;
        float halfAngle = DopplerNotchMath.GetEffectiveBreakAngleDegrees(settings, velocity) * Mathf.Deg2Rad;
        float slantLength = settings.TacViewConeLength;
        float startSlantLength = Mathf.Clamp(settings.TacViewConeStartDistance, 0f, slantLength);
        float axialScale = Mathf.Max(0f, Mathf.Cos(halfAngle));
        float radialScale = Mathf.Sin(halfAngle);
        float startHeight = startSlantLength * axialScale;
        float startRadius = startSlantLength * radialScale;
        float endHeight = slantLength * axialScale;
        float endRadius = slantLength * radialScale;
        Matrix4x4 coneMatrix = Matrix4x4.TRS(
            targetPosition,
            Quaternion.LookRotation(closestNotchDirection),
            Vector3.one);

        using (Draw.Command(cam, CustomPassInjectionPoint.AfterPostProcess))
        {
            Draw.ThicknessSpace = ThicknessSpace.Noots;
            Draw.Thickness = LineThickness;
            Draw.DetailLevel = DetailLevel.Extreme;
            Draw.LineGeometry = LineGeometry.Volumetric3D;
            Draw.Color = color;
            Draw.Matrix = coneMatrix;
            if (startRadius > DirectionEpsilon)
            {
                Draw.Torus(new Vector3(0f, 0f, startHeight), Vector3.forward, startRadius, LineThickness);
            }
            if (endRadius > DirectionEpsilon)
            {
                Draw.Torus(new Vector3(0f, 0f, endHeight), Vector3.forward, endRadius, LineThickness);
            }
        }

        using (Draw.Command(cam, CustomPassInjectionPoint.AfterPostProcess))
        {
            Draw.ThicknessSpace = ThicknessSpace.Noots;
            Draw.Thickness = LineThickness;
            Draw.DetailLevel = DetailLevel.Extreme;
            Draw.LineGeometry = LineGeometry.Volumetric3D;
            Draw.Color = color;
            Vector3 cameraPosition = coneMatrix.inverse * cam.transform.position;
            Draw.Matrix = coneMatrix * Matrix4x4.Rotate(
                MathHelpers.ConstrainedLookRotation(Vector3.zero, cameraPosition, Vector3.forward));
            Draw.Line(
                new Vector3(startRadius, startHeight, 0f),
                new Vector3(endRadius, endHeight, 0f));
            Draw.Line(
                new Vector3(-startRadius, startHeight, 0f),
                new Vector3(-endRadius, endHeight, 0f));
        }
    }
}

[HarmonyPatch(typeof(ShipDetailOverlay), nameof(ShipDetailOverlay.SetShip))]
internal static class DopplerNotchOverlaySetupPatch
{
    private static void Postfix(ShipDetailOverlay __instance, ShipController ship)
    {
        DopplerNotchOverlay overlay = __instance.GetComponent<DopplerNotchOverlay>();
        if (overlay == null)
        {
            overlay = __instance.gameObject.AddComponent<DopplerNotchOverlay>();
        }

        overlay.SetShip(ship);
        DopplerNotchOverlayVisibilityPatch.UpdateOverlay(__instance, overlay);
    }
}

[HarmonyPatch(typeof(ShipDetailOverlay), nameof(ShipDetailOverlay.UpdateVisible))]
internal static class DopplerNotchOverlayVisibilityPatch
{
    private static void Postfix(ShipDetailOverlay __instance)
    {
        DopplerNotchOverlay overlay = __instance.GetComponent<DopplerNotchOverlay>();
        if (overlay != null)
        {
            UpdateOverlay(__instance, overlay);
        }
    }

    internal static void UpdateOverlay(ShipDetailOverlay detailOverlay, DopplerNotchOverlay overlay)
    {
        bool active = Common.GetVal<bool>(detailOverlay, "_active");
        ShipController ship = detailOverlay.ForShip;
        overlay.SetVisible(ship != null && ship.OwnedBy != null && ship.OwnedBy.IsOnLocalPlayerTeam && active);
    }
}

[TestingComponentFactory(Order = 100)]
public sealed class DopplerTestingComponentFactory : ITestingComponentFactory
{
    public void CreateTestingComponents(TestingComponentContext context)
    {
        context.CreateFirst(
            component => component.GetType() == typeof(InternalActiveSensorComponent),
            "AGMLIB/Testing/Doppler Internal Active Sensor",
            builder =>
            {
                string sourceName = builder.Source.ComponentName;
                builder.ReplaceRoot<DopplerInternalActiveSensorComponent>()
                    .SetDisplayName($"[TEST] Doppler {sourceName}")
                    .SetDescription($"Doppler/notch testing version of {builder.Source.SaveKey}.");
            });

        context.CreateFirst(
            component => component.GetComponentsInChildren<ActiveFireControlSensor>(includeInactive: true)
                .Any(sensor => sensor.GetType() == typeof(ActiveFireControlSensor)),
            "AGMLIB/Testing/Doppler Active Fire Control",
            builder =>
            {
                string sourceName = builder.Source.ComponentName;
                int replaced = builder.ReplaceInChildren<ActiveFireControlSensor, DopplerActiveFireControlSensor>();
                if (replaced == 0)
                {
                    throw new InvalidOperationException($"No exact ActiveFireControlSensor was found under {builder.Source.SaveKey}.");
                }

                builder.SetDisplayName($"[TEST] Doppler {sourceName}")
                    .SetDescription($"Doppler/notch fire-control testing version of {builder.Source.SaveKey}; replaced {replaced} active fire-control sensor(s).");
            });
    }
}
