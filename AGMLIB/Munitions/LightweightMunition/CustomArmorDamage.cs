using Game.Armor;
using Game.Units;
using HarmonyLib;
using Munitions;
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
        public float ArmorStripPercentage = 1;
    }
    [HarmonyPatch(typeof(ShipController), "ApplyArmorDamage")]
    public class ShipControllerApplyArmorDamage
    {
        static void Prefix(ShipController __instance, MunitionHitInfo hitInfo, IDamageDealer character, bool neverRicochet) {
            if (character is not IModular modular)
                return;
            var test = modular.Modules.OfType<CustomArmorDamage>();
            if (!test.Any())
                return;
            CustomArmorDamage damage = test.First();
            ShipController controller = __instance;
            if (hitInfo.HitCollider is not MeshCollider meshCollider || !hitInfo.HitUV.HasValue)
            {
                return;
            }
            int dynamicArmorIndex = controller.Ship.Hull.GetDynamicArmorIndex(meshCollider.sharedMesh);
            if (dynamicArmorIndex < 0)
            {
                return;
            }
            IArmorSection[] _dynamicArmor = Common.GetVal<IArmorSection[]>(controller, "_dynamicArmor");
            if (dynamicArmorIndex >= _dynamicArmor.Length)
            {
                return;
            }

            _dynamicArmor[dynamicArmorIndex].GetHPAtPosition(hitInfo.HitUV.Value, out var armor, out var heat);
            doarmordamage = true;
            armordamage = armor * damage.ArmorStripPercentage;
        }

        public static bool doarmordamage = false;
        public static float armordamage = 0;
    }

    [HarmonyPatch(typeof(ShipController), "DoArmorDamage")]
    public class ShipControllerDoArmorDamage
    {
        static void Prefix(ShipController __instance, int armorIndex, Vector2 hitUV, ref float damage, float brushSize, float heat)
        {
            if (ShipControllerApplyArmorDamage.doarmordamage)
                damage = ShipControllerApplyArmorDamage.armordamage;
            ShipControllerApplyArmorDamage.doarmordamage = false;
        }

    }

}
