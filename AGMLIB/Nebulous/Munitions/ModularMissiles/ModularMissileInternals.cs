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
            if (stages == null ||
                stageNumber < 0 ||
                stageNumber >= stages.Length)
            {
                throw new InvalidOperationException(
                    $"Modular missile '{_missile.name}' has no valid current stage.");
            }

            return ref stages[stageNumber];
        }
    }

    public bool TryGetCurrentStage(out ModularMissile.Stage stage)
    {
        ModularMissile.Stage[] stages = Stages;
        int stageNumber = CurrentStageNumber;
        if (stages == null ||
            stageNumber < 0 ||
            stageNumber >= stages.Length)
        {
            stage = default;
            return false;
        }

        stage = stages[stageNumber];
        return true;
    }
}
