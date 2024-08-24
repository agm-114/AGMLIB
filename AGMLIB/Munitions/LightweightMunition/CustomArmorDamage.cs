using Game.Armor;
using Game.Units;
using HarmonyLib;
using Munitions;
using Munitions.InstancedDamagers;
using Munitions.ModularMissiles.Descriptors.Support;
using Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AGMLIB.Munitions.LightweightMunition
{
    [CreateAssetMenu(fileName = "New CustomArmorDamage", menuName = "Nebulous/LW Shells/CustomArmorDamage")]
    public class CustomArmorDamage : ScriptableObject
    {
        public bool StripBaseArmor = true;
        public float ArmorStripMultiplier = 1;
        public bool MaxThickness = true;
    }
    [HarmonyPatch(typeof(ShipController), "ApplyArmorDamage")]
    public class ShipControllerApplyArmorDamage
    {
        static void Prefix(ShipController __instance, MunitionHitInfo hitInfo, IDamageDealer character, bool neverRicochet) {
            IModular modular = character as IModular;
            if (modular == null && character is BaseThrowawayDamager damager)
            {
                modular = Common.GetVal<IDamageCharacteristic>(damager, "_damage") as IModular;
                //Debug.LogError("Damager is " + Common.GetVal<IDamageCharacteristic>(damager, "_damage"));

            }
            if (modular == null)
                return;
            var test = modular.Modules.OfType<CustomArmorDamage>();
            if (!test.Any())
                return;
            CustomArmorDamage damage = test.First();
            ShipController controller = __instance;
            IArmorSection[] _dynamicArmor;
            if (damage.MaxThickness)
            {
                _dynamicArmor = Common.GetVal<IArmorSection[]>(controller.Ship.Hull, "_dynamicArmor");
                doarmordamage = true;
                //Debug.LogError("Armor Sections " + _dynamicArmor.FirstOrDefault().Thickness + " % " + damage.ArmorStripPercentage);
                armordamage = _dynamicArmor.FirstOrDefault().Thickness * damage.ArmorStripMultiplier;
                return;
            }
            if (hitInfo.HitCollider is not MeshCollider meshCollider || !hitInfo.HitUV.HasValue)
            {
                return;
            }
            int dynamicArmorIndex = controller.Ship.Hull.GetDynamicArmorIndex(meshCollider.sharedMesh);
            if (dynamicArmorIndex < 0)
            {
                return;
            }
            _dynamicArmor = Common.GetVal<IArmorSection[]>(controller.Ship.Hull, "_dynamicArmor");
            if (dynamicArmorIndex >= _dynamicArmor.Length)
            {
                return;
            }

            _dynamicArmor[dynamicArmorIndex].GetHPAtPosition(hitInfo.HitUV.Value, out var armor, out var heat);
            doarmordamage = true;
            armordamage = armor * damage.ArmorStripMultiplier;
            
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
            //Debug.LogError("Base Armor Damage" + damage);

            if (ShipControllerApplyArmorDamage.doarmordamage)
                damage = damage + ShipControllerApplyArmorDamage.armordamage;
            ShipControllerApplyArmorDamage.doarmordamage = false;
            //Debug.LogError("Custom Armor Damage" + damage);

        }

    }

}
