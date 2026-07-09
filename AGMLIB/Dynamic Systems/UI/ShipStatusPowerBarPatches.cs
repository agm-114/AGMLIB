[HarmonyPatch(typeof(ShipStatusIconGroup), nameof(ShipStatusIconGroup.SetShip))]
class ShipStatusIconGroupSetShipPowerStatusBar
{
    static void Postfix(ShipStatusIconGroup __instance, ShipController ship)
    {
        if (ship == null)
        {
            QuantityStatusIcon powerIcon = Common.GetVal<QuantityStatusIcon>(__instance, "_powerQuantityIcon");
            powerIcon?.GetComponent<ShipStatusPowerBarBinding>()?.ClearAnySource();
            return;
        }

        ShipStatusPowerBar powerBar = ShipStatusPowerBar.EnsureAttachedTo(ship, ShipStatusPowerBar.DisplaySurface.StatusIcon);
        powerBar?.LinkStatusIcons(__instance);
    }
}

[HarmonyPatch(typeof(ShipStatusDisplay), nameof(ShipStatusDisplay.LinkShip))]
class ShipStatusDisplayLinkShipPowerStatusBoard
{
    static void Postfix(ShipStatusDisplay __instance, ShipController ship)
    {
        if (ship == null || __instance == null)
        {
            return;
        }

        ShipStatusPowerBar powerBar = ShipStatusPowerBar.EnsureAttachedTo(ship, ShipStatusPowerBar.DisplaySurface.StatusBoard);
        if (powerBar == null)
        {
            return;
        }

        ShipStatusPowerStatusBoardBinding binding = __instance.GetComponent<ShipStatusPowerStatusBoardBinding>()
            ?? __instance.gameObject.AddComponent<ShipStatusPowerStatusBoardBinding>();
        binding.SetSource(powerBar);
    }
}

[HarmonyPatch(typeof(ShipController), nameof(ShipController.Initialize))]
class ShipControllerInitializePowerStatusBar
{
    static void Postfix(ShipController __instance)
    {
        ShipStatusPowerBar.EnsureDebugAttachedTo(__instance);
    }
}
