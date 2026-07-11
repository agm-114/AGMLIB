public sealed class ShipSignatureDisplayReduction : MonoBehaviour
{
    private const float ChangeTolerance = 0.005f;

    public static bool AutoAttachToAllShips = false;
    public static float NegligibleSignatureThreshold = 0.1f;
    public static float MinimalSignatureThreshold = 0.25f;
    public static float ReducedThreshold = 0.75f;
    public static string ReducedText = "REDUCED";
    public static string MinimalText = "MINIMAL";
    public static string NegligibleText = "NEGLIGIBLE";
    public static GameColors.ColorName ReducedColor = GameColors.ColorName.LightBlue;
    public static GameColors.ColorName MinimalColor = GameColors.ColorName.DarkBlue;
    public static GameColors.ColorName NegligibleColor = GameColors.ColorName.Purple;

    private ShipController? _shipController;
    private float _lastDisplayedMultiplier = float.NaN;

    public static ShipSignatureDisplayReduction? EnsureAttachedTo(ShipController? ship)
    {
        if (ship == null)
        {
            return null;
        }

        ShipSignatureDisplayReduction? display = ship.GetComponent<ShipSignatureDisplayReduction>();
        if (display == null && AutoAttachToAllShips)
        {
            display = ship.gameObject.AddComponent<ShipSignatureDisplayReduction>();
        }

        return display;
    }

    public float GetDisplayedMultiplier()
    {
        if (_shipController == null)
        {
            return 1f;
        }

        float displayedMultiplier = 0f;
        foreach (ISignature signature in _shipController.GetSignatures())
        {
            if (ShouldContributeToDisplay(signature))
            {
                displayedMultiplier = Mathf.Max(displayedMultiplier, signature.ReturnMultiplier);
            }
        }

        return displayedMultiplier;
    }

    public void UpdateDisplay(PercentageStatusText signatureText)
    {
        float displayedMultiplier = GetDisplayedMultiplier();
        signatureText.SetValue(displayedMultiplier);
        if (displayedMultiplier > ReducedThreshold)
        {
            return;
        }

        TextMeshProUGUI text = Common.GetVal<TextMeshProUGUI>(signatureText, "_text");
        if (text != null)
        {
            if (displayedMultiplier <= NegligibleSignatureThreshold)
            {
                text.text = NegligibleText;
                text.color = NegligibleColor.GetColor();
            }
            else if (displayedMultiplier <= MinimalSignatureThreshold)
            {
                text.text = MinimalText;
                text.color = MinimalColor.GetColor();
            }
            else
            {
                text.text = ReducedText;
                text.color = ReducedColor.GetColor();
            }
        }
    }

    private void Awake()
    {
        _shipController = GetComponent<ShipController>();
        _lastDisplayedMultiplier = GetDisplayedMultiplier();
    }

    private void FixedUpdate()
    {
        float displayedMultiplier = GetDisplayedMultiplier();
        if (Mathf.Abs(displayedMultiplier - _lastDisplayedMultiplier) < ChangeTolerance)
        {
            return;
        }

        _lastDisplayedMultiplier = displayedMultiplier;
        NotifySignatureDisplayChanged();
    }

    private static bool ShouldContributeToDisplay(ISignature signature)
    {
        if (signature == null
            || signature.SigType != SignatureType.Radar
            || signature.HideInSigSummary
            || !signature.IsActive)
        {
            return false;
        }

        if (signature is not BaseSignature baseSignature)
        {
            return true;
        }

        DynamicActiveSignature dynamicSignature = baseSignature.GetComponentInChildren<DynamicActiveSignature>();
        return dynamicSignature == null || !dynamicSignature.DisableSignature || !dynamicSignature.active;
    }

    private void NotifySignatureDisplayChanged()
    {
        if (_shipController == null)
        {
            return;
        }

        BaseSignature? signature = _shipController.GetSignatures()
            .OfType<BaseSignature>()
            .FirstOrDefault();
        if (signature != null)
        {
            Common.RunFunc(signature, "FireSignatureChangedEvent", []);
        }
    }
}

[HarmonyPatch(typeof(ShipController), nameof(ShipController.Initialize))]
internal static class ShipControllerInitializeSignatureDisplayReduction
{
    private static void Postfix(ShipController __instance)
    {
        ShipSignatureDisplayReduction.EnsureAttachedTo(__instance);
    }
}

[HarmonyPatch(typeof(ShipInfoBar), "HandleSignatureSizeChanged")]
internal static class ShipInfoBarHandleSignatureSizeChangedReduction
{
    private static bool Prefix(ShipController ship, PercentageStatusText ____signatureText)
    {
        ShipSignatureDisplayReduction? display = ShipSignatureDisplayReduction.EnsureAttachedTo(ship);
        if (display == null)
        {
            return true;
        }

        display.UpdateDisplay(____signatureText);
        return false;
    }
}

[HarmonyPatch(typeof(ShipInfoBar), "Update")]
internal static class ShipInfoBarUpdateSignatureDisplayReduction
{
    private static void Postfix(ShipController? ____primaryShip, PercentageStatusText ____signatureText)
    {
        if (____primaryShip == null || ____signatureText == null)
        {
            return;
        }

        ShipSignatureDisplayReduction? display = ShipSignatureDisplayReduction.EnsureAttachedTo(____primaryShip);
        display?.UpdateDisplay(____signatureText);
    }
}
