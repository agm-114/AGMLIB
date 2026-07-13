[HarmonyPatch(typeof(ShipStatusIconGroup), nameof(ShipStatusIconGroup.SetShip))]
class ShipStatusIconGroupSetShipPowerStatusBar
{
    static void Postfix(ShipStatusIconGroup __instance, ShipController ship)
    {
        ShipStatusPowerBar.RelinkStatusBoards(__instance, ship);

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
        ShipStatusPowerBar.LinkStatusBoard(__instance, ship);
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
