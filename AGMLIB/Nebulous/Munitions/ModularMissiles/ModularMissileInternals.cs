using Munitions.ModularMissiles.Runtime.Controls;
using Munitions.ModularMissiles.Runtime.Seekers;
using Munitions.ModularMissiles.Runtime;
using HarmonyLib;
using Munitions.ModularMissiles;

public static partial class NativeInternalsExtensions
{
    public static ModularMissileInternals Internals(this ModularMissile missile) => new(missile);
}

public readonly struct ModularMissileInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<ModularMissile, ModularMissile.Stage[]> Stages =
            AccessTools.FieldRefAccess<ModularMissile, ModularMissile.Stage[]>("_stages");
        internal static readonly AccessTools.FieldRef<ModularMissile, int> CurrentStageNumber =
            AccessTools.FieldRefAccess<ModularMissile, int>("_currentStageNumber");
        internal static readonly AccessTools.FieldRef<ModularMissile, string> BodySaveKey =
            AccessTools.FieldRefAccess<ModularMissile, string>("_bodySaveKey");
        internal static readonly AccessTools.FieldRef<ModularMissile, int> DecoySeekerMask =
            AccessTools.FieldRefAccess<ModularMissile, int>("_decoySeekerMask");
        internal static readonly AccessTools.FieldRef<ModularMissile, RuntimeMissileGuidance> GuidanceSystem =
            AccessTools.FieldRefAccess<ModularMissile, RuntimeMissileGuidance>("_guidanceSystem");
        internal static readonly AccessTools.FieldRef<ModularMissile, List<RuntimeMissileBehaviour>> RuntimeScripts =
            AccessTools.FieldRefAccess<ModularMissile, List<RuntimeMissileBehaviour>>("_runtimeScripts");
        internal static readonly AccessTools.FieldRef<ModularMissile, MissileUsageCharacteristics> SeekerUsage =
            AccessTools.FieldRefAccess<ModularMissile, MissileUsageCharacteristics>("_seekerUsage");
        internal static readonly AccessTools.FieldRef<ModularMissile, List<RuntimeMissileSeeker>> TargetingSeekers =
            AccessTools.FieldRefAccess<ModularMissile, List<RuntimeMissileSeeker>>("_targetingSeekers");
        internal static readonly AccessTools.FieldRef<ModularMissile, Guid> TemplateKey =
            AccessTools.FieldRefAccess<ModularMissile, Guid>("_templateKey");
        internal static readonly AccessTools.FieldRef<ModularMissile, List<RuntimeMissileSeeker>> ValidationSeekers =
            AccessTools.FieldRefAccess<ModularMissile, List<RuntimeMissileSeeker>>("_validationSeekers");
        internal static readonly AccessTools.FieldRef<ModularMissile, RuntimeMissileWarhead> Warhead =
            AccessTools.FieldRefAccess<ModularMissile, RuntimeMissileWarhead>("_warhead");
        internal static readonly AccessTools.FieldRef<ModularMissile, BoxCollider> ProxFuze =
            AccessTools.FieldRefAccess<ModularMissile, BoxCollider>("_proxFuze");
    }

    private readonly ModularMissile _missile;
    internal ModularMissileInternals(ModularMissile missile)
    {
        _missile = missile ?? throw new ArgumentNullException(nameof(missile));
    }

    public ref ModularMissile.Stage[] Stages => ref Refs.Stages(_missile);
    public ref int CurrentStageNumber => ref Refs.CurrentStageNumber(_missile);

    public ref ModularMissile.Stage CurrentStage
    {
        get
        {
            ModularMissile.Stage[] stages = Stages;
            int stageNumber = CurrentStageNumber;
            if (stages == null || stageNumber < 0 || stageNumber >= stages.Length)
            {
                throw new InvalidOperationException($"Modular missile '{_missile.name}' has no valid current stage.");
            }

            return ref stages[stageNumber];
        }
    }

    public bool TryGetCurrentStage(out ModularMissile.Stage stage)
    {
        ModularMissile.Stage[] stages = Stages;
        int stageNumber = CurrentStageNumber;
        if (stages == null || stageNumber < 0 || stageNumber >= stages.Length)
        {
            stage = default;
            return false;
        }

        stage = stages[stageNumber];
        return true;
    }

    public ref string BodySaveKey => ref Refs.BodySaveKey(_missile);
    public ref int DecoySeekerMask => ref Refs.DecoySeekerMask(_missile);
    public ref RuntimeMissileGuidance GuidanceSystem => ref Refs.GuidanceSystem(_missile);
    public ref List<RuntimeMissileBehaviour> RuntimeScripts => ref Refs.RuntimeScripts(_missile);
    public ref MissileUsageCharacteristics SeekerUsage => ref Refs.SeekerUsage(_missile);
    public ref List<RuntimeMissileSeeker> TargetingSeekers => ref Refs.TargetingSeekers(_missile);
    public ref Guid TemplateKey => ref Refs.TemplateKey(_missile);
    public ref List<RuntimeMissileSeeker> ValidationSeekers => ref Refs.ValidationSeekers(_missile);
    public ref RuntimeMissileWarhead Warhead => ref Refs.Warhead(_missile);
    public ref BoxCollider ProxFuze => ref Refs.ProxFuze(_missile);
}