public class Armor : MonoBehaviour
{
}

[HarmonyPatch(typeof(ShipController), "WouldArmorHitPenetrate")]
class ShipControllerWouldArmorHitPenetrate
{
    static void Prefix(ShipController __instance, MunitionHitInfo hitInfo, IDamageDealer character)
    {
        Common.LogPatch();
    }
}

//WouldArmorHitPenetrate
