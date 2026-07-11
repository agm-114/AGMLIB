[CreateAssetMenu(fileName = "New Ammo Fallback Profile", menuName = "Nebulous/Modules/Weapon Ammo Mode/Fallback Profile")]
public class AmmoModeFallbackProfileModule : ScriptableObject, IOnDiscreteWeaponCheckFire
{
    [SerializeField]
    protected bool _fallbackWhenEmpty = true;

    [SerializeField]
    protected bool _fallbackWhenResourceUnavailable = false;

    [SerializeField]
    protected List<string> _preferredFallbackAmmoSaveKeys = new();

    public bool FallbackWhenEmpty => _fallbackWhenEmpty;
    public bool FallbackWhenResourceUnavailable => _fallbackWhenResourceUnavailable;
    public IList<string> PreferredFallbackAmmoSaveKeys => _preferredFallbackAmmoSaveKeys;

    public void OnDiscreteWeaponCheckFire(DiscreteWeaponComponent weapon)
    {
        if (Modular.SelectedAmmoModule<AmmoModeFallbackProfileModule>(weapon) == this)
        {
            AmmoModeFallbackProfileRuntime.TryFallbackAmmo(weapon);
        }
    }
}

public static class AmmoModeFallbackProfileRuntime
{
    public static void TryFallbackAmmo(WeaponComponent weapon)
    {
        if (weapon == null || weapon.SelectedAmmoType == null)
        {
            return;
        }

        AmmoModeFallbackProfileModule profile = Modular.SelectedAmmoModule<AmmoModeFallbackProfileModule>(weapon);
        if (profile == null)
        {
            return;
        }

        bool selectedEmpty = profile.FallbackWhenEmpty && weapon.AmmoQuantityRemaining() <= 0;
        bool missingProfileResource = profile.FallbackWhenResourceUnavailable && AmmoModeResourceProfileRuntime.MissingProfileResource(weapon);
        if (!selectedEmpty && !missingProfileResource)
        {
            return;
        }

        foreach (string saveKey in profile.PreferredFallbackAmmoSaveKeys.Where(key => !string.IsNullOrEmpty(key)))
        {
            IMunition ammo = weapon.Socket?.AmmoFeed?.GetAmmoType(saveKey);
            if (ammo == null || ammo == weapon.SelectedAmmoType || !weapon.IsAmmoCompatible(ammo))
            {
                continue;
            }

            if (weapon.AmmoQuantityRemaining(ammo) <= 0)
            {
                continue;
            }

            if (weapon.ChangeAmmoType(ammo))
            {
                return;
            }
        }
    }
}
