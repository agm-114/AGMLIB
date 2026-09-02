using Ships;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class CarrierFlightQuartersSpeed : MonoBehaviour
{
    [Min(0f)]
    [Tooltip("Flight Quarters speed while the carrier is outside the permitted green-deck angle. Game speed units are one tenth of the displayed m/s value.")]
    public float FreeGreenDeckSpeed = 0.5f;

    [Min(0f)]
    [Tooltip("Requested maximum Flight Quarters speed while the carrier is inside the permitted green-deck angle. Vanilla still caps this at full speed and floors it at one-third speed plus 0.2. Game speed units are one tenth of the displayed m/s value.")]
    public float MaxGreenDeckSpeed = 1.5f;

    [SerializeField]
    [Tooltip("Optional explicit carrier controller. When empty, the script finds one on the same ship.")]
    private CraftCarrierController? _carrier = null;

    private CraftCarrierController? _activeCarrier;
    private float _originalFreeGreenDeckSpeed;
    private float _originalMaxGreenDeckSpeed;
    private bool _hasOriginalSpeeds;

    private void OnEnable()
    {
        if (!TryBindCarrier())
        {
            enabled = false;
            return;
        }

        Apply();
    }

    private void OnDisable()
    {
        RestoreOriginalSpeeds();
    }

    private void OnValidate()
    {
        NormalizeConfiguredSpeeds(logClamp: false);
        if (Application.isPlaying && isActiveAndEnabled)
        {
            Apply();
        }
    }

    public void Apply()
    {
        if (!TryBindCarrier())
        {
            return;
        }

        NormalizeConfiguredSpeeds(logClamp: true);

        CraftCarrierControllerInternals internals = _activeCarrier!.Internals();
        internals.MinGreenDeckSpeed = FreeGreenDeckSpeed;
        internals.MaxGreenDeckSpeed = MaxGreenDeckSpeed;
    }

    private bool TryBindCarrier()
    {
        CraftCarrierController? resolvedCarrier = ResolveCarrier();
        if (resolvedCarrier == null)
        {
            Debug.LogError($"{nameof(CarrierFlightQuartersSpeed)} on {name} could not find a {nameof(CraftCarrierController)}.", this);
            return false;
        }

        if (_hasOriginalSpeeds && _activeCarrier == resolvedCarrier)
        {
            return true;
        }

        RestoreOriginalSpeeds();
        _activeCarrier = resolvedCarrier;

        CraftCarrierControllerInternals internals = resolvedCarrier.Internals();
        _originalFreeGreenDeckSpeed = internals.MinGreenDeckSpeed;
        _originalMaxGreenDeckSpeed = internals.MaxGreenDeckSpeed;
        _hasOriginalSpeeds = true;
        return true;
    }

    private CraftCarrierController? ResolveCarrier()
    {
        if (_carrier != null)
        {
            return _carrier;
        }

        return GetComponent<CraftCarrierController>()
            ?? GetComponentInParent<CraftCarrierController>()
            ?? transform.root.GetComponentInChildren<CraftCarrierController>(true);
    }

    private void RestoreOriginalSpeeds()
    {
        if (_hasOriginalSpeeds && _activeCarrier != null)
        {
            CraftCarrierControllerInternals internals = _activeCarrier.Internals();
            internals.MinGreenDeckSpeed = _originalFreeGreenDeckSpeed;
            internals.MaxGreenDeckSpeed = _originalMaxGreenDeckSpeed;
        }

        _activeCarrier = null;
        _hasOriginalSpeeds = false;
    }

    private void NormalizeConfiguredSpeeds(bool logClamp)
    {
        FreeGreenDeckSpeed = SanitizeSpeed(FreeGreenDeckSpeed);
        MaxGreenDeckSpeed = SanitizeSpeed(MaxGreenDeckSpeed);
        if (FreeGreenDeckSpeed <= MaxGreenDeckSpeed)
        {
            return;
        }

        if (logClamp)
        {
            Debug.LogWarning(
                $"{nameof(CarrierFlightQuartersSpeed)} on {name} has a free-deck speed greater than its aligned maximum; clamping it to {MaxGreenDeckSpeed}.",
                this);
        }

        FreeGreenDeckSpeed = MaxGreenDeckSpeed;
    }

    private static float SanitizeSpeed(float speed)
    {
        return float.IsNaN(speed) || float.IsInfinity(speed)
            ? 0f
            : Mathf.Max(0f, speed);
    }
}
