using Game.UI;
using Ships;
using UnityEngine.Scripting.APIUpdating;

/// <summary>
/// Opts a hull into per-component low-Power indicators on the damage-control board.
/// Attach this component to the same prefab GameObject as <see cref="BaseHull"/>.
/// </summary>
[DisallowMultipleComponent]
[MovedFrom(
    autoUpdateAPI: true,
    sourceNamespace: null,
    sourceAssembly: "AGMLIB",
    sourceClassName: "HullLowPowerDcBoardSidecar"
)]
public sealed class DcBoardSettings : MonoBehaviour
{
    private const string PowerResourceName = "Power";

    // Authored hull sidecars remain active while the temporary global rollout is disabled.
    public static bool EnableGlobally = false;

    [SerializeField]
    [Tooltip("Show per-component low-Power indicators on the damage-control board.")]
    private bool _powerDisplayEnabled = true;

    [SerializeField]
    [Tooltip(
        "Optional low-Power indicator sprite. When unset, the native lightning-bolt Power status icon is used."
    )]
    private Sprite? _lowPowerIcon;

    private readonly Dictionary<HullPartResourceConnected, bool> _lowPowerByComponent = new();
    private BaseHull? _hull;
    private HullPartResourceConnected[] _trackedComponents = Array.Empty<HullPartResourceConnected>();
    private bool _componentsCached;

    internal event Action<HullPartResourceConnected, bool>? LowPowerChanged;
    internal event Action? PowerDisplayEnabledChanged;

    /// <summary>
    /// Gets or sets whether this hull displays per-component low-Power indicators.
    /// </summary>
    public bool PowerDisplayEnabled
    {
        get => _powerDisplayEnabled;
        set
        {
            if (_powerDisplayEnabled == value)
            {
                return;
            }

            _powerDisplayEnabled = value;
            PowerDisplayEnabledChanged?.Invoke();
        }
    }

    /// <summary>
    /// Gets or sets the sprite displayed for this hull's low-Power indicator.
    /// A null value uses the native lightning-bolt Power status icon.
    /// </summary>
    public Sprite? LowPowerIcon
    {
        get => _lowPowerIcon;
        set => _lowPowerIcon = value;
    }

    internal IReadOnlyList<HullPartResourceConnected> TrackedComponents
    {
        get
        {
            EnsureComponentsCached();
            return _trackedComponents;
        }
    }

    internal static DcBoardSettings? EnsureAttachedTo(BaseHull? hull)
    {
        if (hull == null)
        {
            return null;
        }

        DcBoardSettings? sidecar = hull.GetComponent<DcBoardSettings>();
        if (sidecar != null || !EnableGlobally)
        {
            return sidecar;
        }

        return hull.gameObject.AddComponent<DcBoardSettings>();
    }

    internal void RefreshAfterResourceTick()
    {
        EnsureComponentsCached();

        foreach (HullPartResourceConnected component in _trackedComponents)
        {
            bool lowPower = CalculateLowPower(component);
            if (_lowPowerByComponent.TryGetValue(component, out bool previous))
            {
                if (previous == lowPower)
                {
                    continue;
                }

                _lowPowerByComponent[component] = lowPower;
                LowPowerChanged?.Invoke(component, lowPower);
                continue;
            }

            _lowPowerByComponent.Add(component, lowPower);
            if (lowPower)
            {
                LowPowerChanged?.Invoke(component, true);
            }
        }
    }

    internal bool IsLowOnPower(HullPartResourceConnected component)
    {
        if (_lowPowerByComponent.TryGetValue(component, out bool lowPower))
        {
            return lowPower;
        }

        return CalculateLowPower(component);
    }

    private void EnsureComponentsCached()
    {
        if (_componentsCached)
        {
            return;
        }

        _hull ??= GetComponent<BaseHull>() ?? GetComponentInParent<BaseHull>();
        if (_hull == null)
        {
            return;
        }

        List<HullPartResourceConnected> components = new();
        foreach (HullPart part in _hull.AllParts.Values)
        {
            if (part is HullPartResourceConnected resourceConnected)
            {
                components.Add(resourceConnected);
            }
        }

        _trackedComponents = components.ToArray();
        _componentsCached = true;
    }

    private static bool CalculateLowPower(HullPartResourceConnected component)
    {
        if (component == null || !component.IsFunctional || component.IsDestroyed)
        {
            return false;
        }

        ResourceValue[]? requiredResources = component.Internals().RequiredResourceValues;
        if (requiredResources == null)
        {
            return false;
        }

        foreach (ResourceValue? resourceValue in requiredResources)
        {
            if (resourceValue?.Resource?.Name == PowerResourceName
                && resourceValue.AmountRequired > 0
                && !resourceValue.HasAll)
            {
                return true;
            }
        }

        return false;
    }

    private void OnDisable()
    {
        foreach (HullPartResourceConnected component in _trackedComponents)
        {
            if (_lowPowerByComponent.TryGetValue(component, out bool lowPower) && lowPower)
            {
                _lowPowerByComponent[component] = false;
                LowPowerChanged?.Invoke(component, false);
            }
        }
    }

    private void OnDestroy()
    {
        LowPowerChanged = null;
        PowerDisplayEnabledChanged = null;
        _lowPowerByComponent.Clear();
        _trackedComponents = Array.Empty<HullPartResourceConnected>();
        _hull = null;
        _componentsCached = false;
    }
}

internal sealed class HullLowPowerDcBoardBinding : MonoBehaviour
{
    private readonly Dictionary<
        HullPartResourceConnected,
        HullLowPowerDcBoardRowBinding
    > _rows = new();

    private DamageControlBoard? _board;
    private DcBoardSettings? _source;

    internal void Bind(DamageControlBoard board, DcBoardSettings source)
    {
        Unbind();

        _board = board;
        _source = source;
        _source.LowPowerChanged += HandleLowPowerChanged;
        _source.PowerDisplayEnabledChanged += HandlePowerDisplayEnabledChanged;
        RebuildRows();
    }

    private void RebuildRows()
    {
        UnbindRows();
        if (_board == null || _source == null || !_source.PowerDisplayEnabled)
        {
            return;
        }

        IReadOnlyList<HullPartResourceConnected> components = _source.TrackedComponents;
        if (components.Count == 0)
        {
            return;
        }

        foreach (HullPartResourceConnected component in components)
        {
            ShipStatusDetailPart? detailPart = _board.GetDetailPart(component);
            if (detailPart == null)
            {
                continue;
            }

            HullLowPowerDcBoardRowBinding row =
                detailPart.GetComponent<HullLowPowerDcBoardRowBinding>()
                ?? detailPart.gameObject.AddComponent<HullLowPowerDcBoardRowBinding>();
            Sprite? customIcon = _source.LowPowerIcon;
            Sprite? indicatorSprite = customIcon ?? FindPowerSprite(_board);
            if (!row.Bind(_source, component, indicatorSprite, customIcon == null))
            {
                continue;
            }

            _rows[component] = row;
        }
    }

    private static Sprite? FindPowerSprite(DamageControlBoard board)
    {
        ShipStatusIconGroup? iconGroup = board.transform.root.GetComponentInChildren<
            ShipStatusIconGroup
        >(includeInactive: true);
        QuantityStatusIcon? powerIcon = iconGroup?.Internals().PowerQuantityIcon;
        if (powerIcon == null)
        {
            return null;
        }

        QuantityStatusIconInternals internals = powerIcon.Internals();
        return internals.NormalIcon ?? internals.IconImage?.sprite;
    }

    internal void Unbind()
    {
        if (_source != null)
        {
            _source.LowPowerChanged -= HandleLowPowerChanged;
            _source.PowerDisplayEnabledChanged -= HandlePowerDisplayEnabledChanged;
        }

        UnbindRows();
        _board = null;
        _source = null;
    }

    private void UnbindRows()
    {
        foreach (HullLowPowerDcBoardRowBinding row in _rows.Values)
        {
            if (row != null)
            {
                row.Unbind();
            }
        }

        _rows.Clear();
    }

    private void HandlePowerDisplayEnabledChanged() => RebuildRows();

    private void HandleLowPowerChanged(HullPartResourceConnected component, bool lowPower)
    {
        if (_rows.TryGetValue(component, out HullLowPowerDcBoardRowBinding? row) && row != null)
        {
            row.SetLowPower(lowPower);
        }
    }

    private void OnDestroy() => Unbind();
}

internal sealed class HullLowPowerDcBoardRowBinding : MonoBehaviour
{
    private GameObject? _iconArea;
    private GameObject? _indicator;
    private bool _lowPower;

    internal bool Bind(
        DcBoardSettings source,
        HullPartResourceConnected component,
        Sprite? indicatorSprite,
        bool tintAsNativePowerIcon
    )
    {
        Unbind();

        ShipStatusDetailPart? detailPart = GetComponent<ShipStatusDetailPart>();
        if (detailPart == null)
        {
            return false;
        }

        ShipStatusDetailPartInternals internals = detailPart.Internals();
        GameObject? iconArea = internals.IconArea;
        Image? fireIcon = internals.FireIcon;
        if (iconArea == null || fireIcon == null)
        {
            return false;
        }

        GameObject indicator = UnityEngine.Object.Instantiate(
            fireIcon.gameObject,
            iconArea.transform,
            worldPositionStays: false
        );
        indicator.name = "AGMLIB Low Power Indicator";
        indicator.transform.SetSiblingIndex(fireIcon.transform.GetSiblingIndex() + 1);

        Image? indicatorImage = indicator.GetComponent<Image>();
        if (indicatorImage == null)
        {
            UnityEngine.Object.Destroy(indicator);
            return false;
        }

        if (indicatorSprite == null)
        {
            UnityEngine.Object.Destroy(indicator);
            return false;
        }

        indicatorImage.sprite = indicatorSprite;
        indicatorImage.color = tintAsNativePowerIcon ? GameColors.Yellow : Color.white;
        indicatorImage.raycastTarget = false;

        _iconArea = iconArea;
        _indicator = indicator;
        SetLowPower(source.IsLowOnPower(component));
        return true;
    }

    internal void SetLowPower(bool lowPower)
    {
        _lowPower = lowPower;
        ReassertVisibility();
    }

    internal void ReassertVisibility()
    {
        if (_indicator == null || _iconArea == null)
        {
            return;
        }

        _indicator.SetActive(_lowPower);
        if (_lowPower)
        {
            _iconArea.SetActive(value: true);
            return;
        }

        bool vanillaIconVisible = false;
        foreach (Transform child in _iconArea.transform)
        {
            if (child != _indicator.transform && child.gameObject.activeSelf)
            {
                vanillaIconVisible = true;
                break;
            }
        }

        _iconArea.SetActive(vanillaIconVisible);
    }

    internal void Unbind()
    {
        if (_indicator != null)
        {
            _indicator.SetActive(value: false);
            _lowPower = false;
            ReassertVisibility();
            UnityEngine.Object.Destroy(_indicator);
        }

        _indicator = null;
        _iconArea = null;
        _lowPower = false;
    }

    private void OnDestroy() => Unbind();
}

[HarmonyPatch(typeof(Ship), nameof(Ship.RunResourceTick))]
internal static class ShipRunResourceTickLowPowerDcBoardPatch
{
    private static void Postfix(Ship __instance)
    {
        DcBoardSettings? sidecar = DcBoardSettings.EnsureAttachedTo(
            __instance.Hull
        );
        if (sidecar != null && sidecar.isActiveAndEnabled)
        {
            sidecar.RefreshAfterResourceTick();
        }
    }
}

[HarmonyPatch(typeof(DamageControlBoard), nameof(DamageControlBoard.LinkShip))]
internal static class DamageControlBoardLinkShipLowPowerPatch
{
    private static void Postfix(DamageControlBoard __instance, ShipController ship)
    {
        HullLowPowerDcBoardBinding? binding = __instance.GetComponent<
            HullLowPowerDcBoardBinding
        >();
        DcBoardSettings? source = DcBoardSettings.EnsureAttachedTo(
            ship?.Ship?.Hull
        );
        if (source == null || !source.isActiveAndEnabled)
        {
            binding?.Unbind();
            return;
        }

        binding ??= __instance.gameObject.AddComponent<HullLowPowerDcBoardBinding>();
        binding.Bind(__instance, source);
    }
}

[HarmonyPatch(typeof(DamageControlBoard), nameof(DamageControlBoard.Clear))]
internal static class DamageControlBoardClearLowPowerPatch
{
    private static void Prefix(DamageControlBoard __instance)
    {
        __instance.GetComponent<HullLowPowerDcBoardBinding>()?.Unbind();
    }
}

[HarmonyPatch(typeof(ShipStatusDetailPart), "UpdateAppearance")]
internal static class ShipStatusDetailPartUpdateAppearanceLowPowerPatch
{
    private static void Postfix(ShipStatusDetailPart __instance)
    {
        __instance.GetComponent<HullLowPowerDcBoardRowBinding>()?.ReassertVisibility();
    }
}
