using Munitions.InstancedDamagers;
namespace AGMLIB.Munitions.LightweightMunition
{
    public class ArmorDamage : ScriptableObject { }
    public class CustomArmorDamage : MultiplierArmorDamage { }
    [CreateAssetMenu(fileName = "New CustomArmorDamage", menuName = "Nebulous/LW Shells/CustomArmorDamage")]
    public class MultiplierArmorDamage : ArmorDamage
    {
        public float ArmorStripMultiplier = 1;
        public bool MaxThickness = true;
    }
    [CreateAssetMenu(fileName = "New CustomArmorDamage", menuName = "Nebulous/LW Shells/FixedArmorDamage")]
    public class FixedArmorDamage : ArmorDamage
    {
        public float ArmorDamageAmount = 1;
        public bool ClampArmorDamage = true;
    }
    [HarmonyPatch(typeof(ShipController), "ApplyArmorDamage")]
    public class ShipControllerApplyArmorDamage
    {
        static bool GetArmorThickness(MunitionHitInfo hitInfo, ShipController controller, out float armor)
        {
            IArmorSection[] _dynamicArmor;
            armor = 0;
            if (hitInfo.HitCollider is not MeshCollider meshCollider || !hitInfo.HitUV.HasValue)
            {
                return true;
            }
            int dynamicArmorIndex = controller.Ship.Hull.GetDynamicArmorIndex(meshCollider.sharedMesh);
            if (dynamicArmorIndex < 0)
            {
                return true;
            }
            _dynamicArmor = Common.GetVal<IArmorSection[]>(controller.Ship.Hull, "_dynamicArmor");
            if (dynamicArmorIndex >= _dynamicArmor.Length)
            {
                return true;
            }

            _dynamicArmor[dynamicArmorIndex].GetHPAtPosition(hitInfo.HitUV.Value, out armor, out var heat);
            return false;
        }

        static void Prefix(ShipController __instance, MunitionHitInfo hitInfo, IDamageDealer character, bool neverRicochet) {
            Common.LogPatch();
            IModular modular = character as IModular;
            if (modular == null && character is BaseThrowawayDamager damager)
            {
                modular = Common.GetVal<IDamageCharacteristic>(damager, "_damage") as IModular;
                //Debug.LogError("Damager is " + Common.GetVal<IDamageCharacteristic>(damager, "_damage"));

            }
            if (modular == null)
                return;
            var test = modular.Modules.OfType<ArmorDamage>();
            if (!test.Any())
                return;
            ArmorDamage model = test.First();
            ShipController controller = __instance;
           
            if(model is MultiplierArmorDamage mult)
            {
                if (mult.MaxThickness)
                {
                    IArmorSection[] _dynamicArmor = Common.GetVal<IArmorSection[]>(controller.Ship.Hull, "_dynamicArmor");
                    doarmordamage = true;
                    //Debug.LogError("Armor Sections " + _dynamicArmor.FirstOrDefault().Thickness + " % " + damage.ArmorStripPercentage);
                    armordamage = _dynamicArmor.FirstOrDefault().Thickness * mult.ArmorStripMultiplier;
                    return;
                }
                if(GetArmorThickness(hitInfo, controller, out float armor))
                    return;
                doarmordamage = true;
                armordamage = armor * mult.ArmorStripMultiplier;
                return;
            }

            if (model is FixedArmorDamage fixeddamage)
            {
                if (fixeddamage.ClampArmorDamage)
                {

                    armordamage = fixeddamage.ArmorDamageAmount;
                    return;
                }
                if (GetArmorThickness(hitInfo, controller, out float armor))
                    return;
                doarmordamage = true;
                armordamage = Mathf.Min(armor, fixeddamage.ArmorDamageAmount);
                return;
            }
            //Debug.LogError("Armor "  + armor + " % " + damage.ArmorStripPercentage + " armordamage " + armordamage);
        }

        public static bool doarmordamage = false;
        public static float armordamage = 0;
    }

    [HarmonyPatch(typeof(ShipController), "DoArmorDamage")]
    public class ShipControllerDoArmorDamage
    {
        static void Prefix(ShipController __instance, int armorIndex, Vector2 hitUV, ref float damage, float brushSize, float heat)
        {
            Common.LogPatch();
            //Debug.LogError("Base Armor Damage" + damage);

            if (ShipControllerApplyArmorDamage.doarmordamage)
                damage = damage + ShipControllerApplyArmorDamage.armordamage;
            ShipControllerApplyArmorDamage.doarmordamage = false;
            //Debug.LogError("Custom Armor Damage" + damage);

        }

    }

}
