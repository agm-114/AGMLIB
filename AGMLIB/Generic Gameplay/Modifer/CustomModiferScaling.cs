public class CustomModiferScaling : MonoBehaviour
{
    public enum ScalingMode
    {
        VanillaPenalty,
        NoPenalty,
        Linear,
        Exponential,
        Power
    }

    [Header("Custom Modifier Scaling")]
    public int Priority = 0;
    public ScalingMode Mode = ScalingMode.VanillaPenalty;
    public bool ScaleModifiers = true;
    public bool ScaleLiterals = true;
    public bool OnlyWhenStatHasStackingPenalty = false;

    [Header("Scaling Values")]
    public float PenaltyFactor = 0f;
    public float LinearStep = 0.25f;
    public float ExponentialFactor = 0.5f;
    public float Power = 2f;
    public float MinimumScale = 0f;
    public float MaximumScale = 1f;

    public float GetScale(int modifierNumber, float vanillaPenaltyFactor)
    {
        modifierNumber = Mathf.Max(1, modifierNumber);
        float scale = Mode switch
        {
            ScalingMode.NoPenalty => 1f,
            ScalingMode.Linear => 1f - ((modifierNumber - 1) * LinearStep),
            ScalingMode.Exponential => Mathf.Pow(ExponentialFactor, modifierNumber - 1),
            ScalingMode.Power => 1f / Mathf.Pow(modifierNumber, Power),
            _ => Mathf.Exp(-Mathf.Pow((modifierNumber - 1) / EffectivePenaltyFactor(vanillaPenaltyFactor), 2f))
        };

        return Mathf.Clamp(scale, MinimumScale, MaximumScale);
    }

    private float EffectivePenaltyFactor(float vanillaPenaltyFactor)
    {
        if (PenaltyFactor > 0f)
            return PenaltyFactor;

        return Mathf.Max(vanillaPenaltyFactor, 0.001f);
    }
}

[HarmonyPatch(typeof(StatValue), "CalculateModifier")]
class StatValueCalculateModifierCustomScaling
{
    static void Postfix(
        StatValue __instance,
        IEnumerable<StatModifier> modifiers,
        ref float totalModifier,
        ref float totalLiteral)
    {
        Dictionary<IModifierSource, StatModifier> modifierSources = Common.GetVal<Dictionary<IModifierSource, StatModifier>>(__instance, "_modifiers");
        if (modifierSources == null || modifiers == null)
            return;

        List<StatModifier> modifierList = modifiers.ToList();
        if (modifierList.Count == 0)
            return;

        CustomModiferScaling scaling = modifierSources
            .Where(pair => modifierList.Contains(pair.Value))
            .Select(pair => GetScaling(pair.Key))
            .Where(scaling => scaling != null)
            .OrderByDescending(scaling => scaling.Priority)
            .FirstOrDefault();

        if (scaling == null)
            return;

        ShipStatAttribute attribute = Common.GetVal<ShipStatAttribute>(__instance, "Attribute");
        if (scaling.OnlyWhenStatHasStackingPenalty && attribute?.StackingPenalty != true)
            return;

        float vanillaPenaltyFactor = attribute?.StackingPenaltyFactor ?? 1f;
        totalModifier = 1f;
        totalLiteral = 0f;

        int modifierNumber = 0;
        foreach (StatModifier modifier in modifierList.OrderByDescending(modifier => modifier.Modifier))
        {
            modifierNumber++;
            float scale = scaling.ScaleModifiers ? scaling.GetScale(modifierNumber, vanillaPenaltyFactor) : 1f;
            totalModifier += modifier.Modifier * scale;
        }

        modifierNumber = 0;
        foreach (StatModifier modifier in modifierList.OrderByDescending(modifier => modifier.Literal))
        {
            modifierNumber++;
            float scale = scaling.ScaleLiterals ? scaling.GetScale(modifierNumber, vanillaPenaltyFactor) : 1f;
            totalLiteral += modifier.Literal * scale;
        }
    }

    private static CustomModiferScaling GetScaling(IModifierSource source)
    {
        if (source is Component component)
            return component.gameObject.GetComponent<CustomModiferScaling>();

        return null;
    }
}