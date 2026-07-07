public interface IModular
{
    public List<ScriptableObject> Modules { get; }
}

public interface ILimited
{
    public Dictionary<string, int> RestrictedOptions { get; }
}

public interface IOnWeaponAmmoChanged
{
    void OnWeaponAmmoChanged(WeaponComponent weapon, bool wasReloading);
}

public interface IOnDiscreteWeaponCheckFire
{
    void OnDiscreteWeaponCheckFire(DiscreteWeaponComponent weapon);
}

public interface IOnDamageableImpact
{
    void OnDamageableImpact(LightweightKineticShell shell, LightweightKineticMunitionContainer attachedTo, IDamageable hitObject, MunitionHitInfo hitInfo, bool trigger, HitResult hitRes, float damageDone, bool targetDestroyed);
}

public interface IOnGenericImpact
{
    void OnGenericImpact(LightweightKineticShell shell, LightweightKineticMunitionContainer attachedTo, MunitionHitInfo hitInfo, bool trigger, HitResult hitRes);
}

public static class Modular
{
    public static IEnumerable<T> SelectedAmmoModules<T>(WeaponComponent weapon)
    {
        if (weapon?.SelectedAmmoType is not IModular modular || modular.Modules == null)
        {
            return Enumerable.Empty<T>();
        }

        return modular.Modules.OfType<T>();
    }

    public static T SelectedAmmoModule<T>(WeaponComponent weapon) where T : class
    {
        return SelectedAmmoModules<T>(weapon).FirstOrDefault();
    }

    public static IFilterIndexed FindIndexedFilter(List<IFilterIndexed> filters, int index = -1)
    {
        if (filters == null || filters.Count <= 0)
        {
            return null;
        }

        List<IFilterIndexed> indexedFilters = filters.FindAll(filter => filter.Index == index);
        if (indexedFilters.Count > 0)
        {
            return indexedFilters.First();
        }

        List<IFilterIndexed> fallbackFilters = filters.FindAll(filter => filter.AllIndexes);
        return fallbackFilters.Count > 0 ? fallbackFilters.First() : null;
    }

    public static IFilterIndexed FindIndexedFilter(IEnumerable<IFilterIndexed> filters, int index = -1) => FindIndexedFilter(filters.ToList(), index);

    public static IFilterIndexed Default => ScriptableObject.CreateInstance<ScriptableFilter>();
}

[HarmonyPatch(typeof(WeaponComponent), nameof(WeaponComponent.ChangeAmmoType))]
public class WeaponComponentChangeAmmoTypeModuleCallbacks
{
    static void Prefix(WeaponComponent __instance, out bool __state)
    {
        __state = __instance is DiscreteWeaponComponent discrete && Common.GetVal<bool>(discrete, "_reloading");
    }

    static void Postfix(WeaponComponent __instance, bool __result, bool __state)
    {
        if (!__result || __instance?.SelectedAmmoType is not IModular modular || modular.Modules == null)
        {
            return;
        }

        foreach (IOnWeaponAmmoChanged module in modular.Modules.OfType<IOnWeaponAmmoChanged>())
        {
            module.OnWeaponAmmoChanged(__instance, __state);
        }
    }
}

[HarmonyPatch(typeof(DiscreteWeaponComponent), "CheckFire")]
public class DiscreteWeaponComponentCheckFireModuleCallbacks
{
    static void Prefix(DiscreteWeaponComponent __instance)
    {
        if (__instance?.SelectedAmmoType is not IModular modular || modular.Modules == null)
        {
            return;
        }

        foreach (IOnDiscreteWeaponCheckFire module in modular.Modules.OfType<IOnDiscreteWeaponCheckFire>())
        {
            module.OnDiscreteWeaponCheckFire(__instance);
        }
    }
}

[HarmonyPatch(typeof(LightweightKineticShell), nameof(LightweightKineticShell.DamageableImpact))]
public class LightweightKineticShellDamageableImpactModuleCallbacks
{
    static void Postfix(LightweightKineticShell __instance, LightweightKineticMunitionContainer attachedTo, IDamageable hitObject, MunitionHitInfo hitInfo, bool trigger, HitResult hitRes, float damageDone, bool targetDestroyed)
    {
        if (__instance is not IModular modular || modular.Modules == null)
        {
            return;
        }

        foreach (IOnDamageableImpact module in modular.Modules.OfType<IOnDamageableImpact>())
        {
            module.OnDamageableImpact(__instance, attachedTo, hitObject, hitInfo, trigger, hitRes, damageDone, targetDestroyed);
        }
    }
}

[HarmonyPatch(typeof(LightweightKineticShell), nameof(LightweightKineticShell.GenericImpact))]
public class LightweightKineticShellGenericImpactModuleCallbacks
{
    static void Postfix(LightweightKineticShell __instance, LightweightKineticMunitionContainer attachedTo, MunitionHitInfo hitInfo, bool trigger, HitResult hitRes)
    {
        if (__instance is not IModular modular || modular.Modules == null)
        {
            return;
        }

        foreach (IOnGenericImpact module in modular.Modules.OfType<IOnGenericImpact>())
        {
            module.OnGenericImpact(__instance, attachedTo, hitInfo, trigger, hitRes);
        }
    }
}
