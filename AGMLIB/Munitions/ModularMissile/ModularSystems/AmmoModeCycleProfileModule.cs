public class AmmoModeRuntimeState : MonoBehaviour
{
    public bool CycleCached;
    public int BaseMagazineSize;
    public float BaseRecycleSeconds;
    public float BaseReloadSeconds;
}

[CreateAssetMenu(fileName = "New Ammo Cycle Profile", menuName = "Nebulous/Modules/Weapon Ammo Mode/Cycle Profile")]
public class AmmoModeCycleProfileModule : ScriptableObject, IOnDiscreteWeaponCheckFire, IOnWeaponAmmoChanged
{
    [SerializeField]
    protected float _recycleSeconds = 1f;

    [SerializeField]
    protected float _reloadSeconds = 0f;

    [SerializeField]
    protected int _magazineSize = 1;

    [SerializeField]
    protected bool _reloadOnAmmoChange = true;

    public float RecycleSeconds => _recycleSeconds;
    public float ReloadSeconds => _reloadSeconds;
    public int MagazineSize => _magazineSize;
    public bool ReloadOnAmmoChange => _reloadOnAmmoChange;

    public void OnDiscreteWeaponCheckFire(DiscreteWeaponComponent weapon)
    {
        if (Modular.SelectedAmmoModule<AmmoModeCycleProfileModule>(weapon) == this)
        {
            AmmoModeCycleProfileRuntime.Apply(weapon);
        }
    }

    public void OnWeaponAmmoChanged(WeaponComponent weapon, bool wasReloading)
    {
        if (weapon is not DiscreteWeaponComponent discrete || Modular.SelectedAmmoModule<AmmoModeCycleProfileModule>(weapon) != this)
        {
            return;
        }

        AmmoModeCycleProfileRuntime.Apply(discrete);
        if (!ReloadOnAmmoChange && !wasReloading)
        {
            discrete.Internals().Reloading = false;
            discrete.Internals().ReloadAccum = 0f;
        }
    }
}

public static class AmmoModeCycleProfileRuntime
{
    public static void Apply(DiscreteWeaponComponent weapon)
    {
        if (weapon == null)
        {
            return;
        }

        AmmoModeRuntimeState state = weapon.gameObject.GetOrAddComponent<AmmoModeRuntimeState>();
        StatValue recycleStat = weapon?.Internals().StatRecycleTime;
        StatValue reloadStat = weapon?.Internals().StatReloadTime;
        if (recycleStat == null || reloadStat == null)
        {
            return;
        }

        if (!state.CycleCached)
        {
            state.BaseMagazineSize = (weapon?.Internals().MagazineSizeRef ?? default);
            state.BaseRecycleSeconds = recycleStat.BaseValue;
            state.BaseReloadSeconds = reloadStat.BaseValue;
            state.CycleCached = true;
        }

        AmmoModeCycleProfileModule profile = Modular.SelectedAmmoModule<AmmoModeCycleProfileModule>(weapon);
        int magazineSize = state.BaseMagazineSize;
        float recycleSeconds = state.BaseRecycleSeconds;
        float reloadSeconds = state.BaseReloadSeconds;

        if (profile != null)
        {
            magazineSize = Mathf.Max(1, profile.MagazineSize);
            recycleSeconds = Mathf.Max(0f, profile.RecycleSeconds);
            reloadSeconds = Mathf.Max(0f, profile.ReloadSeconds);
        }

        weapon.Internals().MagazineSizeRef = magazineSize;
        SetStatBaseValue(recycleStat, recycleSeconds);
        SetStatBaseValue(reloadStat, reloadSeconds);

        int fired = (weapon?.Internals().MagazineFired ?? default);
        if (fired > magazineSize)
        {
            weapon.Internals().MagazineFired = magazineSize;
        }
    }

    private static void SetStatBaseValue(StatValue stat, float baseValue)
    {
        stat.Internals().BaseValue = baseValue;
        stat.Internals().Recalculate();
    }
}
