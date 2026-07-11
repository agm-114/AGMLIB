public enum AmmoModeResourceFailurePolicy
{
    Vanilla = 0,
    DisableMode = 1,
    FallbackAmmo = 2,
    FireWithoutAssist = 3
}

[CreateAssetMenu(fileName = "New Ammo Resource Profile", menuName = "Nebulous/Modules/Weapon Ammo Mode/Resource Profile")]
public class AmmoModeResourceProfileModule : ScriptableObject, IOnWeaponAmmoChanged
{
    [SerializeField]
    protected string _resourceName = "Power";

    [SerializeField]
    protected float _amountRequired = 0f;

    [SerializeField]
    protected bool _onlyWhenOperating = true;

    [SerializeField]
    protected AmmoModeResourceFailurePolicy _failurePolicy = AmmoModeResourceFailurePolicy.Vanilla;

    public string ResourceName => _resourceName;
    public float AmountRequired => _amountRequired;
    public bool OnlyWhenOperating => _onlyWhenOperating;
    public AmmoModeResourceFailurePolicy FailurePolicy => _failurePolicy;

    public void OnWeaponAmmoChanged(WeaponComponent weapon, bool wasReloading)
    {
        if (Modular.SelectedAmmoModule<AmmoModeResourceProfileModule>(weapon) == this)
        {
            DynamicReduction.UpdateResources(weapon);
        }
    }
}

public static class AmmoModeResourceProfileRuntime
{
    public static void Apply(HullComponent component)
    {
        if (component is not WeaponComponent weapon)
        {
            return;
        }

        AmmoModeResourceProfileModule profile = Modular.SelectedAmmoModule<AmmoModeResourceProfileModule>(weapon);
        if (profile == null)
        {
            return;
        }

        ResourceType resource = ResourceDefinitions.Instance.GetResource(profile.ResourceName);
        if (resource == null)
        {
            return;
        }

        ResourceValue[] current = Common.GetVal<ResourceValue[]>(component, "_requiredResources") ?? Array.Empty<ResourceValue>();
        int amountRequired = Mathf.Max(0, Mathf.RoundToInt(profile.AmountRequired));
        List<ResourceValue> modified = current.Where(value => value != null).Select(CloneResourceValue).ToList();
        ResourceValue existing = modified.FirstOrDefault(value => value.Resource == resource);

        if (existing != null)
        {
            existing.AmountRequired = amountRequired;
            existing.OnlyWhenOperating = profile.OnlyWhenOperating;
        }
        else
        {
            modified.Add(new ResourceValue(resource, amountRequired, profile.OnlyWhenOperating));
        }

        Common.SetVal(component, "_requiredResources", modified.ToArray());
    }

    public static bool MissingProfileResource(WeaponComponent weapon)
    {
        AmmoModeResourceProfileModule profile = Modular.SelectedAmmoModule<AmmoModeResourceProfileModule>(weapon);
        if (profile == null)
        {
            return false;
        }

        ResourceType resource = ResourceDefinitions.Instance.GetResource(profile.ResourceName);
        ResourceValue[] requiredResources = Common.GetVal<ResourceValue[]>(weapon, "_requiredResources");
        return requiredResources?.Any(value => value != null && value.Resource == resource && !value.HasAll) ?? false;
    }

    private static ResourceValue CloneResourceValue(ResourceValue value)
    {
        return new ResourceValue(value.Resource, value.AmountRequired, value.OnlyWhenOperating)
        {
            AmountReceived = value.AmountReceived,
            Operating = value.Operating
        };
    }
}
