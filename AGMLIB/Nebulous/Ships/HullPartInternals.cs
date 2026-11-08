using NativePriority = Ships.Priority;
using Ships;

public static partial class NativeInternalsExtensions
{
    public static HullPartInternals Internals(this HullPart target) => new(target);
}

public readonly struct HullPartInternals(HullPart target)
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<HullPart, HullPart.IHullPartRPC> BaseRpcProvider =
            AccessTools.FieldRefAccess<HullPart, HullPart.IHullPartRPC>("__baseRpcProvider");
        internal static readonly AccessTools.FieldRef<HullPart, DamageControlTeam> AssignedDCTeam =
            AccessTools.FieldRefAccess<HullPart, DamageControlTeam>("_assignedDCTeam");
        internal static readonly AccessTools.FieldRef<HullPart, string> BindTag =
            AccessTools.FieldRefAccess<HullPart, string>("_bindTag");
        internal static readonly AccessTools.FieldRef<HullPart, CasualtyType> CasualtyClass =
            AccessTools.FieldRefAccess<HullPart, CasualtyType>("_casualtyClass");
        internal static readonly AccessTools.FieldRef<HullPart, List<ICrewed>> CrewsInPart =
            AccessTools.FieldRefAccess<HullPart, List<ICrewed>>("_crewsInPart");
        internal static readonly AccessTools.FieldRef<HullPart, float> CurrentHealth =
            AccessTools.FieldRefAccess<HullPart, float>("_currentHealth");
        internal static readonly AccessTools.FieldRef<HullPart, float> DamageThreshold =
            AccessTools.FieldRefAccess<HullPart, float>("_damageThreshold");
        internal static readonly AccessTools.FieldRef<HullPart, NativePriority> DcPriority =
            AccessTools.FieldRefAccess<HullPart, NativePriority>("_dcPriority");
        internal static readonly AccessTools.FieldRef<HullPart, bool> DcVoiceDisabledOnly =
            AccessTools.FieldRefAccess<HullPart, bool>("_dcVoiceDisabledOnly");
        internal static readonly AccessTools.FieldRef<HullPart, DynamicVisibleParticles> DisabledParticles =
            AccessTools.FieldRefAccess<HullPart, DynamicVisibleParticles>("_disabledParticles");
        internal static readonly AccessTools.FieldRef<HullPart, float> FunctioningThreshold =
            AccessTools.FieldRefAccess<HullPart, float>("_functioningThreshold");
        internal static readonly AccessTools.FieldRef<HullPart, bool> IsDestroyed =
            AccessTools.FieldRefAccess<HullPart, bool>("_isDestroyed");
        internal static readonly AccessTools.FieldRef<HullPart, bool> IsDoingWork =
            AccessTools.FieldRefAccess<HullPart, bool>("_isDoingWork");
        internal static readonly AccessTools.FieldRef<HullPart, float> MaxHealth =
            AccessTools.FieldRefAccess<HullPart, float>("_maxHealth");
        internal static readonly AccessTools.FieldRef<HullPart, float> MinHPReached =
            AccessTools.FieldRefAccess<HullPart, float>("_minHPReached");
        internal static readonly AccessTools.FieldRef<HullPart, IPlatform> MyHull =
            AccessTools.FieldRefAccess<HullPart, IPlatform>("_myHull");
        internal static readonly AccessTools.FieldRef<HullPart, DamageFrame.HullPartDamage> PartDamageFrame =
            AccessTools.FieldRefAccess<HullPart, DamageFrame.HullPartDamage>("_partDamageFrame");
        internal static readonly AccessTools.FieldRef<HullPart, string> PartKey =
            AccessTools.FieldRefAccess<HullPart, string>("_partKey");
        internal static readonly AccessTools.FieldRef<HullPart, bool> Reinforced =
            AccessTools.FieldRefAccess<HullPart, bool>("_reinforced");
        internal static readonly AccessTools.FieldRef<HullPart, float> RepairFloor =
            AccessTools.FieldRefAccess<HullPart, float>("_repairFloor");
        internal static readonly AccessTools.FieldRef<HullPart, RestorePartJob> RestoreJob =
            AccessTools.FieldRefAccess<HullPart, RestorePartJob>("_restoreJob");
        internal static readonly AccessTools.FieldRef<HullPart, bool> RpcProviderInitialized =
            AccessTools.FieldRefAccess<HullPart, bool>("_rpcProviderInitialized");
    }

    public ref HullPart.IHullPartRPC BaseRpcProvider => ref Refs.BaseRpcProvider(target);
    public ref DamageControlTeam AssignedDCTeam => ref Refs.AssignedDCTeam(target);
    public ref string BindTag => ref Refs.BindTag(target);
    public ref CasualtyType CasualtyClass => ref Refs.CasualtyClass(target);
    public ref List<ICrewed> CrewsInPart => ref Refs.CrewsInPart(target);
    public ref float CurrentHealth => ref Refs.CurrentHealth(target);
    public ref float DamageThreshold => ref Refs.DamageThreshold(target);
    public ref NativePriority DcPriority => ref Refs.DcPriority(target);
    public ref bool DcVoiceDisabledOnly => ref Refs.DcVoiceDisabledOnly(target);
    public ref DynamicVisibleParticles DisabledParticles => ref Refs.DisabledParticles(target);
    public ref float FunctioningThreshold => ref Refs.FunctioningThreshold(target);
    public ref bool IsDestroyed => ref Refs.IsDestroyed(target);
    public ref bool IsDoingWork => ref Refs.IsDoingWork(target);
    public ref float MaxHealth => ref Refs.MaxHealth(target);
    public ref float MinHPReached => ref Refs.MinHPReached(target);
    public ref IPlatform MyHull => ref Refs.MyHull(target);
    public ref DamageFrame.HullPartDamage PartDamageFrame => ref Refs.PartDamageFrame(target);
    public ref string PartKey => ref Refs.PartKey(target);
    public ref bool Reinforced => ref Refs.Reinforced(target);
    public ref float RepairFloor => ref Refs.RepairFloor(target);
    public ref RestorePartJob RestoreJob => ref Refs.RestoreJob(target);
    public ref bool RpcProviderInitialized => ref Refs.RpcProviderInitialized(target);
}