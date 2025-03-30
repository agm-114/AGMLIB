using Steamworks;
using System;

namespace AGMLIB.Generic_Gameplay.Discrete
{
    public class MuzzleList : MonoBehaviour
    {
        public WeaponComponent Weapon;

        public MunitionTags[] CompatibleAmmoTags;

        public Muzzle[] Muzzles = new Muzzle[1];

        protected Muzzle[] _muzzles => Common.GetVal<Muzzle[]>(Weapon, "_muzzles");
        private int _currentMuzzle = 0;
        public Muzzle GetNextMuzzle(out int num)
        {
            Muzzle result = Muzzles[_currentMuzzle];
            _currentMuzzle++;
            if (_currentMuzzle >= _muzzles.Length)
            {
                _currentMuzzle = 0;
            }
            num = Array.FindIndex(Muzzles, muzzle => muzzle == result);
            return result;
        }

        public bool IsAmmoCompatible(IMunition ammo)
        {
            if (ammo == null)
            {
                return true;
            }
            return _muzzles[0].SimMethod == ammo.SimMethod && CompatibleAmmoTags.Any((MunitionTags x) => x.Equals(ammo.Tags));
        }

        public void Awake()
        {
            Common.Hint(this.gameObject, "muzzle");
            foreach (var muzzle in Muzzles)
            {
                Common.Hint("Spawn Muzzle" + nameof(muzzle));

            }
        }


    }

    [HarmonyPatch(typeof(WeaponComponent), "GetNextMuzzle")]
    public class WeaponComponentPatch
    {
        static bool Prefix(WeaponComponent __instance, ref Muzzle __result, out int num)
        {
            Common.LogPatch();
            List<MuzzleList> muzzleLists = __instance.GetComponentsInChildren<MuzzleList>().ToList() ?? new List<MuzzleList>();
            // Check if the condition to skip the function is met
            if (muzzleLists.Count > 0)
            {
                Common.Hint("No Lists");

                num = 0;
                return Common.RunFunction;
            }
            if (muzzleLists.Any(mlist => mlist.IsAmmoCompatible(__instance.SelectedAmmoType)))
            {
                __result = muzzleLists.Find(mlist => mlist.IsAmmoCompatible(__instance.SelectedAmmoType)).GetNextMuzzle(out num);
                Common.Hint("Returning Muzzle" + nameof(__result));

                return Common.SkipFunction;
            }
            else
            {
                //Common.Hint("Hey none of your ammo lists are compatible with the ammo you want to use");
            }
            num = 0;
            return Common.RunFunction;
        }
    }

    [HarmonyPatch(typeof(ContinuousWeaponComponent), "StartFiring")]
    public class ContinuousWeaponComponentStartFiring
    {
        protected static Muzzle[] _muzzles = null;

        static void Prefix(ContinuousWeaponComponent __instance)
        {
            Common.LogPatch();
            List<MuzzleList> muzzleLists = __instance.GetComponentsInChildren<MuzzleList>().ToList() ?? new List<MuzzleList>();
            // Check if the condition to skip the function is met
            if (muzzleLists.Count <= 0)
            {
                //Common.Hint("No Lists");
                ;
                return;
            }
            if (muzzleLists.Any(mlist => mlist.IsAmmoCompatible(__instance.SelectedAmmoType)))
            {
                _muzzles =  Common.GetVal<Muzzle[]>(__instance, "_muzzles");
                Muzzle[] _tempmuzzles = muzzleLists.Find(mlist => mlist.IsAmmoCompatible(__instance.SelectedAmmoType)).Muzzles;
                Common.SetVal<Muzzle[]>(__instance, "_muzzles", _tempmuzzles);
                return;
            }
            else
            {
                Common.Hint("Hey none of your ammo lists are compatible with the ammo you want to use");
            }
        }

        static void Postfix(ContinuousWeaponComponent __instance)
        {
            Common.LogPatch();
            if (_muzzles == null)
                return;
            Common.SetVal(__instance, "_muzzles", _muzzles);
            _muzzles = null;
        }
    }
}
