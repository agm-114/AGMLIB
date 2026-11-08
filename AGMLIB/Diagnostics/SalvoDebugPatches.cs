namespace Lib.Diagnostics;

[HarmonyPatch(typeof(ActiveMissileSalvo), nameof(ActiveMissileSalvo.RegisterMissile))]
internal static class SalvoDebugRegisterPatch
{
    private static void Postfix(ActiveMissileSalvo __instance, IMissile missile) =>
        SalvoDebugRecorder.Instance?.Register(__instance, missile);
}

[HarmonyPatch(typeof(ActiveMissileSalvo), nameof(ActiveMissileSalvo.TerminateMissile))]
internal static class SalvoDebugTerminatePatch
{
    private static void Prefix(ActiveMissileSalvo __instance, IMissile missile, ActiveMissileSalvo.MissileStatus endState) =>
        SalvoDebugRecorder.Instance?.Terminate(__instance, missile, endState);
}

[DisallowMultipleComponent]
internal class SalvoDebugRow : MonoBehaviour
{
    private IActiveMissileSalvo? _salvo;
    private Button? _button;

    public void Bind(FriendlySalvoItem item, IActiveMissileSalvo salvo)
    {
        Unbind();
        _salvo = salvo;
        _button = item.Internals().Button;
        if (_button != null)
            _button.onClick.AddListener(Open);
    }

    public void Unbind()
    {
        if (_button != null)
            _button.onClick.RemoveListener(Open);
        _button = null;
        _salvo = null;
    }

    private void Open()
    {
        if (_salvo != null)
            SalvoDebugRecorder.Instance?.Open(_salvo);
    }

    private void OnDestroy() => Unbind();
}

[HarmonyPatch(typeof(FriendlySalvoItem), nameof(FriendlySalvoItem.SetSalvo))]
internal static class SalvoDebugBindRowPatch
{
    private static void Postfix(FriendlySalvoItem __instance, IActiveMissileSalvo salvo)
    {
        if (SalvoDebugRecorder.Instance != null)
            (__instance.GetComponent<SalvoDebugRow>() ?? __instance.gameObject.AddComponent<SalvoDebugRow>()).Bind(__instance, salvo);
    }
}

[HarmonyPatch(typeof(FriendlySalvoItem), nameof(FriendlySalvoItem.OnRepooled))]
internal static class SalvoDebugUnbindRowPatch
{
    private static void Prefix(FriendlySalvoItem __instance) => __instance.GetComponent<SalvoDebugRow>()?.Unbind();
}

[HarmonyPatch(typeof(BaseHull), nameof(BaseHull.WouldArmorHitPenetrate))]
internal static class SalvoDebugArmorPatch
{
    private static void Postfix(float damage, Vector3 surfaceNormal, Vector3 incidenceNormal, bool ignoreEffectiveThickness,
        float remainingArmor, float damageScaling, bool ricochet, bool __result) =>
        BeamRayCapture.Current?.AddStep(new DamageTraceStep(
            $"Armor test: {damage:N3} cm penetration vs {remainingArmor:N3} cm remaining effective armor; " +
            $"angle {Vector3.Angle(surfaceNormal, incidenceNormal):N1} degrees; ignore slope {ignoreEffectiveThickness}; " +
            $"penetrates {__result}; ricochet candidate {ricochet}; armor damage scale {damageScaling:N3}."));
}

[HarmonyPatch(typeof(ShipController), "Game.IDamageable.ApplyDamageReduction")]
internal static class SalvoDebugReductionPatch
{
    private static void Postfix(float fullDamage, float __result) => BeamRayCapture.Current?.AddStep(
        new DamageTraceStep($"Ship damage reduction: {fullDamage:N3} -> {__result:N3} HP."));
}

[HarmonyPatch(typeof(ShipController), "Game.IDamageable.DoDamage")]
internal static class SalvoDebugDamageScopePatch
{
    private static void Prefix(IDamageDealer damager, out BeamRayCapture? __state) =>
        __state = BeamRayCapture.SuspendUnlessOwned(damager);

    private static void Finalizer(BeamRayCapture? __state) => BeamRayCapture.Restore(__state);
}

[HarmonyPatch(typeof(MunitionsHelpers), nameof(MunitionsHelpers.RaycastComponents),
    [typeof(Ray), typeof(float), typeof(ISubDamageable[])], [ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Ref])]
internal static class SalvoDebugInteriorRayPatch
{
    // Index arguments also work across builds which rename the native parameters.
    private static void Postfix(Ray __0, float __1, ISubDamageable[] __2, int __result) =>
        Record(__0, __1, __2, __result);

    internal static void Record(Ray cast, float distance, ISubDamageable[] hits, int count, float radius = 0)
    {
        BeamRayTrace? ray = BeamRayCapture.Current;
        if (ray == null)
            return;
        BeamRayCapture.RecordInternalRay(new DamageTraceStep($"Internal ray: {distance * 10f:N2} m; radius {radius * 10f:N2} m; {count} intersections before ship filtering.")
        {
            HasSegment = true,
            Start = cast.origin,
            End = cast.GetPoint(distance),
            Intersections = count
        });
        for (int i = 0; i < count && i < hits.Length; i++)
        {
            if (ray.Steps.Count == BeamRayTrace.MaxSteps)
            {
                ray.DroppedSteps += Math.Min(count, hits.Length) - i;
                break;
            }
            ISubDamageable part = hits[i];
            if (part == null)
                continue;
            ray.AddStep(new DamageTraceStep($"Intersection {i + 1}: {PartName(part)}; " +
                $"{part.CurrentHealth:N3} HP available; structural {part.Structural}; already destroyed {part.IsDestroyed}."));
        }
    }

    internal static string PartName(ISubDamageable part) => part is HullPart hullPart ? hullPart.UIName :
        part is Component component ? component.name : part.GetType().Name;
}

[HarmonyPatch(typeof(global::Munitions.InstancedDamagers.BaseThrowawayDamager), "Munitions.IDamageDealer.DamageComponents")]
internal static class SalvoDebugInternalDamagePatch
{
    private static void Prefix(global::Munitions.InstancedDamagers.BaseThrowawayDamager __instance, out DamageTraceStep? __state) =>
        __state = BeamRayCapture.Owns(__instance) ? BeamRayCapture.CurrentInternalRay : null;

    private static void Postfix(float damageDone, HitResult hitRes, DamageTraceStep? __state)
    {
        if (__state == null)
            return;
        __state.DamageEvaluated = true;
        __state.ReportedDamage += damageDone;
        __state.Result = hitRes;
    }
}

[HarmonyPatch(typeof(MunitionsHelpers), nameof(MunitionsHelpers.SpherecastComponents))]
internal static class SalvoDebugInteriorSpherePatch
{
    private static void Postfix(Vector3 __0, Vector3 __1, float __2, float __3, ISubDamageable[] __4, int __result) =>
        SalvoDebugInteriorRayPatch.Record(new Ray(__0, __1), __3, __4, __result, __2);
}

[HarmonyPatch(typeof(HullPart), nameof(HullPart.DoDamage))]
internal static class SalvoDebugComponentPatch
{
    private static void Prefix(HullPart __instance, IDamageCharacteristic damager, out float? __state) =>
        __state = BeamRayCapture.Owns(damager) ? ((ISubDamageable)__instance).CurrentHealth : null;

    private static void Postfix(HullPart __instance, float damageDone, float? __state)
    {
        if (__state is not float before)
            return;
        float after = ((ISubDamageable)__instance).CurrentHealth;
        BeamRayCapture.Current?.AddStep(new DamageTraceStep(
            $"Component {__instance.UIName}: allocated {damageDone:N3} HP; available HP {before:N3} -> {after:N3}; " +
            $"pending HP loss {before - after:N3} (excess allocation cannot add reported HP loss)."));
    }
}

[HarmonyPatch(typeof(HullStructure), "Game.ISubDamageable.DoDamage")]
internal static class SalvoDebugStructurePatch
{
    private static void Prefix(HullStructure __instance, float damage, IDamageCharacteristic character)
    {
        if (BeamRayCapture.Owns(character))
            BeamRayCapture.Current?.AddStep(new DamageTraceStep(
                $"Structure {__instance.name}: allocated {damage:N3} HP; committed health {__instance.CurrentHealth:N3}. " +
                "Structure applies a damage threshold and defers its health update to the damage frame."));
    }
}
