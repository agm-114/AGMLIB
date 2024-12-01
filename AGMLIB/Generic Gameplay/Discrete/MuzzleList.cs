namespace AGMLIB.Generic_Gameplay.Discrete
{
    public class MuzzleList : MonoBehaviour
    {
        public WeaponComponent Weapon;

        public MunitionTags[] CompatibleAmmoTags;

        public Muzzle[] Muzzles = new Muzzle[1];

        protected Muzzle[] _muzzles => Common.GetVal< Muzzle[]>(Weapon, "_muzzles");
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


    }

    [HarmonyPatch(typeof(WeaponComponent), "GetNextMuzzle")]
    public class WeaponComponentPatch
    {
        static bool Prefix(WeaponComponent __instance, ref Muzzle __result, out int num)
        {
            List< MuzzleList > muzzleLists = __instance.GetComponentsInChildren<MuzzleList>().ToList() ?? new List< MuzzleList >();
            // Check if the condition to skip the function is met
            if (muzzleLists.Count > 0 && muzzleLists.Any(mlist => mlist.IsAmmoCompatible(__instance.SelectedAmmoType)))
            {
                __result = muzzleLists.Find(mlist => mlist.IsAmmoCompatible(__instance.SelectedAmmoType)).GetNextMuzzle(out num);
                return false; 
            }
            num = 0;
            return true;
        }
    }
}
