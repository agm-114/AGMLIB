using NativeWeaponRole = Utility.WeaponRole;
using HarmonyLib;
using Ships;

public static partial class NativeInternalsExtensions
{
    public static WeaponComponentInternals Internals(this WeaponComponent weapon) => new(weapon);
}

public readonly struct WeaponComponentInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<WeaponComponent, Muzzle[]> Muzzles =
            AccessTools.FieldRefAccess<WeaponComponent, Muzzle[]>("_muzzles");
        internal static readonly AccessTools.FieldRef<WeaponComponent, int> CurrentMuzzle =
            AccessTools.FieldRefAccess<WeaponComponent, int>("_currentMuzzle");
        internal static readonly AccessTools.FieldRef<WeaponComponent, WeaponComponent.IWeaponComponentRPC> RpcProvider =
            AccessTools.FieldRefAccess<WeaponComponent, WeaponComponent.IWeaponComponentRPC>("_weaponRpcProvider");
        internal static readonly Func<WeaponComponent, bool> OnTarget =
            AccessTools.MethodDelegate<Func<WeaponComponent, bool>>(
                AccessTools.PropertyGetter(typeof(WeaponComponent), "_onTarget")
                ?? throw new MissingMemberException(typeof(WeaponComponent).FullName, "_onTarget"));
        internal static readonly Func<WeaponComponent, bool> TargetBlocked =
            AccessTools.MethodDelegate<Func<WeaponComponent, bool>>(
                AccessTools.PropertyGetter(typeof(WeaponComponent), "_targetBlocked")
                ?? throw new MissingMemberException(typeof(WeaponComponent).FullName, "_targetBlocked"));
        internal static readonly AccessTools.FieldRef<WeaponComponent, float[]> AccuracyMeasurementDistances =
            AccessTools.FieldRefAccess<WeaponComponent, float[]>("_accuracyMeasurementDistances");
        internal static readonly AccessTools.FieldRef<WeaponComponent, float> AimCheckRangeMultiplier =
            AccessTools.FieldRefAccess<WeaponComponent, float>("_aimCheckRangeMultiplier");
        internal static readonly AccessTools.FieldRef<WeaponComponent, float> AimpointSelectionTime =
            AccessTools.FieldRefAccess<WeaponComponent, float>("_aimpointSelectionTime");
        internal static readonly AccessTools.FieldRef<WeaponComponent, bool> AllowAmmoSelection =
            AccessTools.FieldRefAccess<WeaponComponent, bool>("_allowAmmoSelection");
        internal static readonly AccessTools.FieldRef<WeaponComponent, bool> AllowGrouping =
            AccessTools.FieldRefAccess<WeaponComponent, bool>("_allowGrouping");
        internal static readonly AccessTools.FieldRef<WeaponComponent, bool> AllowVisualTargeting =
            AccessTools.FieldRefAccess<WeaponComponent, bool>("_allowVisualTargeting");
        internal static readonly Func<WeaponComponent, IMagazine> GetAmmoSource =
            AccessTools.MethodDelegate<Func<WeaponComponent, IMagazine>>(
                AccessTools.PropertyGetter(typeof(WeaponComponent), "_ammoSource")
                ?? throw new MissingMemberException(typeof(WeaponComponent).FullName, "_ammoSource"));
        internal static readonly Action<WeaponComponent, IMagazine> SetAmmoSource =
            AccessTools.MethodDelegate<Action<WeaponComponent, IMagazine>>(
                AccessTools.PropertySetter(typeof(WeaponComponent), "_ammoSource")
                ?? throw new MissingMemberException(typeof(WeaponComponent).FullName, "_ammoSource"));
        internal static readonly AccessTools.FieldRef<WeaponComponent, HashSet<TrackIdentifier>> AssignmentHistory =
            AccessTools.FieldRefAccess<WeaponComponent, HashSet<TrackIdentifier>>("_assignmentHistory");
        internal static readonly AccessTools.FieldRef<WeaponComponent, NativeWeaponRole?> CachedRole =
            AccessTools.FieldRefAccess<WeaponComponent, NativeWeaponRole?>("_cachedRole");
        internal static readonly AccessTools.FieldRef<WeaponComponent, bool> CheckFriendlyFire =
            AccessTools.FieldRefAccess<WeaponComponent, bool>("_checkFriendlyFire");
        internal static readonly AccessTools.FieldRef<WeaponComponent, bool> CheckObstaclesInWay =
            AccessTools.FieldRefAccess<WeaponComponent, bool>("_checkObstaclesInWay");
        internal static readonly AccessTools.FieldRef<WeaponComponent, MunitionTags[]> CompatibleAmmoTags =
            AccessTools.FieldRefAccess<WeaponComponent, MunitionTags[]>("_compatibleAmmoTags");
        internal static readonly AccessTools.FieldRef<WeaponComponent, ShipController> Controller =
            AccessTools.FieldRefAccess<WeaponComponent, ShipController>("_controller");
        internal static readonly AccessTools.FieldRef<WeaponComponent, string> DetailType =
            AccessTools.FieldRefAccess<WeaponComponent, string>("_detailType");
        internal static readonly AccessTools.FieldRef<WeaponComponent, EWarWeaponType> EwType =
            AccessTools.FieldRefAccess<WeaponComponent, EWarWeaponType>("_ewType");
        internal static readonly AccessTools.FieldRef<WeaponComponent, bool> ExecutingPlayerOrder =
            AccessTools.FieldRefAccess<WeaponComponent, bool>("_executingPlayerOrder");
        internal static readonly AccessTools.FieldRef<WeaponComponent, bool> FireLastPositionOnLostTrack =
            AccessTools.FieldRefAccess<WeaponComponent, bool>("_fireLastPositionOnLostTrack");
        internal static readonly AccessTools.FieldRef<WeaponComponent, WeaponGroup> Group =
            AccessTools.FieldRefAccess<WeaponComponent, WeaponGroup>("_group");
        internal static readonly AccessTools.FieldRef<WeaponComponent, string> GroupingName =
            AccessTools.FieldRefAccess<WeaponComponent, string>("_groupingName");
        internal static readonly AccessTools.FieldRef<WeaponComponent, bool> HasFiredOnCurrentTrackTarget =
            AccessTools.FieldRefAccess<WeaponComponent, bool>("_hasFiredOnCurrentTrackTarget");
        internal static readonly AccessTools.FieldRef<WeaponComponent, Vector3?> IdleDirection =
            AccessTools.FieldRefAccess<WeaponComponent, Vector3?>("_idleDirection");
        internal static readonly AccessTools.FieldRef<WeaponComponent, FireControlSensor> IntegratedFireControl =
            AccessTools.FieldRefAccess<WeaponComponent, FireControlSensor>("_integratedFireControl");
        internal static readonly AccessTools.FieldRef<WeaponComponent, int> LeadIterations =
            AccessTools.FieldRefAccess<WeaponComponent, int>("_leadIterations");
        internal static readonly AccessTools.FieldRef<WeaponComponent, bool> LeadTargets =
            AccessTools.FieldRefAccess<WeaponComponent, bool>("_leadTargets");
        internal static readonly AccessTools.FieldRef<WeaponComponent, float> OnTargetAngle =
            AccessTools.FieldRefAccess<WeaponComponent, float>("_onTargetAngle");
        internal static readonly AccessTools.FieldRef<WeaponComponent, WeightClass> OptimalTargetWeight =
            AccessTools.FieldRefAccess<WeaponComponent, WeightClass>("_optimalTargetWeight");
        internal static readonly AccessTools.FieldRef<WeaponComponent, TargetMethod> PdtTargetMethod =
            AccessTools.FieldRefAccess<WeaponComponent, TargetMethod>("_pdtTargetMethod");
        internal static readonly AccessTools.FieldRef<WeaponComponent, bool> PositionTargetIsDirection =
            AccessTools.FieldRefAccess<WeaponComponent, bool>("_positionTargetIsDirection");
        internal static readonly AccessTools.FieldRef<WeaponComponent, float> RandomOffsetSquish =
            AccessTools.FieldRefAccess<WeaponComponent, float>("_randomOffsetSquish");
        internal static readonly AccessTools.FieldRef<WeaponComponent, bool> RequireExternalAmmoFeed =
            AccessTools.FieldRefAccess<WeaponComponent, bool>("_requireExternalAmmoFeed");
        internal static readonly AccessTools.FieldRef<WeaponComponent, NativeWeaponRole> Role =
            AccessTools.FieldRefAccess<WeaponComponent, NativeWeaponRole>("_role");
        internal static readonly AccessTools.FieldRef<WeaponComponent, AmmoFeeder> RoleCachedFromAmmoFeeder =
            AccessTools.FieldRefAccess<WeaponComponent, AmmoFeeder>("_roleCachedFromAmmoFeeder");
        internal static readonly AccessTools.FieldRef<WeaponComponent, bool> ShoulderCamEnabled =
            AccessTools.FieldRefAccess<WeaponComponent, bool>("_shoulderCamEnabled");
        internal static readonly AccessTools.FieldRef<WeaponComponent, Vector3> ShoulderCamOffset =
            AccessTools.FieldRefAccess<WeaponComponent, Vector3>("_shoulderCamOffset");
        internal static readonly AccessTools.FieldRef<WeaponComponent, Vector3> TargetOffset =
            AccessTools.FieldRefAccess<WeaponComponent, Vector3>("_targetOffset");
        internal static readonly AccessTools.FieldRef<WeaponComponent, float> UpdateOffsetAccum =
            AccessTools.FieldRefAccess<WeaponComponent, float>("_updateOffsetAccum");
        internal static readonly AccessTools.FieldRef<WeaponComponent, bool> UseGenericAckCallback =
            AccessTools.FieldRefAccess<WeaponComponent, bool>("_useGenericAckCallback");
        internal static readonly AccessTools.FieldRef<WeaponComponent, UtilityWeaponType> UtilityType =
            AccessTools.FieldRefAccess<WeaponComponent, UtilityWeaponType>("_utilityType");
        internal static readonly AccessTools.FieldRef<WeaponComponent, bool> WaitForFCLock =
            AccessTools.FieldRefAccess<WeaponComponent, bool>("_waitForFCLock");
        internal static readonly AccessTools.FieldRef<WeaponComponent, WeaponChanged> WeaponFunctionalChanged =
            AccessTools.FieldRefAccess<WeaponComponent, WeaponChanged>("_weaponFunctionalChanged");
    }

    private readonly WeaponComponent _weapon;
    internal WeaponComponentInternals(WeaponComponent weapon)
    {
        _weapon = weapon ?? throw new ArgumentNullException(nameof(weapon));
    }

    public ref Muzzle[] Muzzles => ref Refs.Muzzles(_weapon);
    public ref int CurrentMuzzle => ref Refs.CurrentMuzzle(_weapon);
    public WeaponComponent.IWeaponComponentRPC RpcProvider => Refs.RpcProvider(_weapon);
    public bool OnTarget => Refs.OnTarget(_weapon);
    public bool TargetBlocked => Refs.TargetBlocked(_weapon);
    public ref float[] AccuracyMeasurementDistances => ref Refs.AccuracyMeasurementDistances(_weapon);
    public ref float AimCheckRangeMultiplier => ref Refs.AimCheckRangeMultiplier(_weapon);
    public ref float AimpointSelectionTime => ref Refs.AimpointSelectionTime(_weapon);
    public ref bool AllowAmmoSelection => ref Refs.AllowAmmoSelection(_weapon);
    public ref bool AllowGrouping => ref Refs.AllowGrouping(_weapon);
    public ref bool AllowVisualTargeting => ref Refs.AllowVisualTargeting(_weapon);
    public IMagazine AmmoSource { get => Refs.GetAmmoSource(_weapon); set => Refs.SetAmmoSource(_weapon, value); }
    public ref HashSet<TrackIdentifier> AssignmentHistory => ref Refs.AssignmentHistory(_weapon);
    public ref NativeWeaponRole? CachedRole => ref Refs.CachedRole(_weapon);
    public ref bool CheckFriendlyFire => ref Refs.CheckFriendlyFire(_weapon);
    public ref bool CheckObstaclesInWay => ref Refs.CheckObstaclesInWay(_weapon);
    public ref MunitionTags[] CompatibleAmmoTags => ref Refs.CompatibleAmmoTags(_weapon);
    public ref ShipController Controller => ref Refs.Controller(_weapon);
    public ref string DetailType => ref Refs.DetailType(_weapon);
    public ref EWarWeaponType EwType => ref Refs.EwType(_weapon);
    public ref bool ExecutingPlayerOrder => ref Refs.ExecutingPlayerOrder(_weapon);
    public ref bool FireLastPositionOnLostTrack => ref Refs.FireLastPositionOnLostTrack(_weapon);
    public ref WeaponGroup Group => ref Refs.Group(_weapon);
    public ref string GroupingName => ref Refs.GroupingName(_weapon);
    public ref bool HasFiredOnCurrentTrackTarget => ref Refs.HasFiredOnCurrentTrackTarget(_weapon);
    public ref Vector3? IdleDirection => ref Refs.IdleDirection(_weapon);
    public ref FireControlSensor IntegratedFireControl => ref Refs.IntegratedFireControl(_weapon);
    public ref int LeadIterations => ref Refs.LeadIterations(_weapon);
    public ref bool LeadTargets => ref Refs.LeadTargets(_weapon);
    public ref float OnTargetAngle => ref Refs.OnTargetAngle(_weapon);
    public ref WeightClass OptimalTargetWeight => ref Refs.OptimalTargetWeight(_weapon);
    public ref TargetMethod PdtTargetMethod => ref Refs.PdtTargetMethod(_weapon);
    public ref bool PositionTargetIsDirection => ref Refs.PositionTargetIsDirection(_weapon);
    public ref float RandomOffsetSquish => ref Refs.RandomOffsetSquish(_weapon);
    public ref bool RequireExternalAmmoFeed => ref Refs.RequireExternalAmmoFeed(_weapon);
    public ref NativeWeaponRole Role => ref Refs.Role(_weapon);
    public ref AmmoFeeder RoleCachedFromAmmoFeeder => ref Refs.RoleCachedFromAmmoFeeder(_weapon);
    public ref bool ShoulderCamEnabled => ref Refs.ShoulderCamEnabled(_weapon);
    public ref Vector3 ShoulderCamOffset => ref Refs.ShoulderCamOffset(_weapon);
    public ref Vector3 TargetOffset => ref Refs.TargetOffset(_weapon);
    public ref float UpdateOffsetAccum => ref Refs.UpdateOffsetAccum(_weapon);
    public ref bool UseGenericAckCallback => ref Refs.UseGenericAckCallback(_weapon);
    public ref UtilityWeaponType UtilityType => ref Refs.UtilityType(_weapon);
    public ref bool WaitForFCLock => ref Refs.WaitForFCLock(_weapon);
    public ref WeaponChanged WeaponFunctionalChanged => ref Refs.WeaponFunctionalChanged(_weapon);
    public ref WeaponComponent.IWeaponComponentRPC WeaponRpcProvider => ref Refs.RpcProvider(_weapon);
}