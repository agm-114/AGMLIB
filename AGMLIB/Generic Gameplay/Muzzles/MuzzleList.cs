using Ships;
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
            return CompatibleAmmoTags.Any((MunitionTags x) => x.Equals(ammo.Tags));
        }

        public void Awake()
        {
            //Common.Hint(this.gameObject, "muzzle");
            foreach (var muzzle in Muzzles)
            {
                //Common.Hint("Spawn Muzzle" + nameof(muzzle));

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
            if (muzzleLists.Count >= 0)
            {
                //Common.Hint("No Lists");

                num = 0;
                return Common.RunFunction;
            }
            if (muzzleLists.Any(mlist => mlist.IsAmmoCompatible(__instance.SelectedAmmoType)))
            {
                __result = muzzleLists.Find(mlist => mlist.IsAmmoCompatible(__instance.SelectedAmmoType)).GetNextMuzzle(out num);
                if (__instance.SelectedAmmoType.SimMethod != __result.SimMethod)
                    Common.Hint($"Possible Sim Method Issue Ammo: {__instance.SelectedAmmoType.SimMethod} Muzzle: {__result.SimMethod}" ) ;


                Common.Hint("Returning Muzzle" + nameof(__result));

                return Common.SkipFunction;
            }
            else
            {
                Common.Hint("Hey none of your muzzle lists are compatible with the ammo you want to use");
                Common.Hint($"Ammo C {__instance.SelectedAmmoType.Tags.Class} S {__instance.SelectedAmmoType.Tags.Subclass}");
                foreach(MuzzleList muzzleList in muzzleLists)
                {
                    Common.Hint("List");
                    foreach(MunitionTags tags in muzzleList.CompatibleAmmoTags)
                    {
                        Common.Hint($"Ammo C {tags.Class} S {tags.Subclass}");
                    }
                }
            }
            num = 0;
            return Common.RunFunction;
        }
    }

    public class DummyMuzzle : Muzzle
    {
        // Dummy simulation method
        public override MunitionSimulationMethod SimMethod => MunitionSimulationMethod.SteppedRaycast;

        public override void Fire()
        {
            // Do nothing
        }

        public override void FireEffect()
        {
            // Do nothing
        }

        public override void GetAccuracyStat(List<(string, string)> rows, float[] distances)
        {
            // No accuracy data to report
        }

        public override void StopFire()
        {
            // Do nothing
        }

        public override void StopFireEffect()
        {
            // Do nothing
        }

        public override void TriggerHitEffect(HitResult hit, Vector3 position, Quaternion rotation)
        {
            // Do nothing
        }
    }

    public class MuzzleState : MonoBehaviour
    {
        public Muzzle[] _muzzles = null;
    }

    [HarmonyPatch(typeof(ContinuousWeaponComponent), "StartFiring")]
    public class ContinuousWeaponComponentStartFiring : MonoBehaviour
    {

        static void Prefix(ContinuousWeaponComponent __instance)
        {
            Common.LogPatch();
            Postfix(__instance);
            List<MuzzleList> muzzleLists = __instance.GetComponentsInChildren<MuzzleList>().ToList() ?? new List<MuzzleList>();
            // Check if the condition to skip the function is met
            if (muzzleLists.Count <= 0)
            {
                //Common.Hint("No Lists");
                return;
            }
            MuzzleState state = __instance.GetComponentInChildren<MuzzleState>() ?? __instance.gameObject.AddComponent<MuzzleState>();
            if (state._muzzles == null)
            {
                state._muzzles = Common.GetVal<Muzzle[]>(__instance, "_muzzles");
            }
            Muzzle[] _muzzles = state._muzzles;
            //Common.SetVal(__instance, "_muzzles", _muzzles);
            //_muzzles = null;
            if (muzzleLists.Any(mlist => mlist.IsAmmoCompatible(__instance.SelectedAmmoType)))
            {
                Common.Hint("Found Good List");

                

                Muzzle[] _tempmuzzles = muzzleLists.Find(mlist => mlist.IsAmmoCompatible(__instance.SelectedAmmoType)).Muzzles;
                if (__instance.SelectedAmmoType.SimMethod != _tempmuzzles[0].SimMethod)
                    Common.Hint($"Possible Sim Method Issue Ammo: {__instance.SelectedAmmoType.SimMethod} Muzzle: {_tempmuzzles[0].SimMethod}");
                var filteredMuzzles = new Muzzle[_muzzles.Length];
                for (int i = 0; i < _muzzles.Length; i++)
                {
                    // Check if the original muzzle exists in the compatible list
                    if (_tempmuzzles.Contains(_muzzles[i]))
                        filteredMuzzles[i] = _muzzles[i];
                    else
                        filteredMuzzles[i] = new DummyMuzzle();
                }
                Common.SetVal<Muzzle[]>(__instance, "_muzzles", _tempmuzzles);
                return;
            }
            else
            {
                Common.Hint("Hey none of your muzzle lists are compatible with the ammo you want to use");
                Common.Hint($"Ammo C {__instance.SelectedAmmoType.Tags.Class} S {__instance.SelectedAmmoType.Tags.Subclass}");
                foreach (MuzzleList muzzleList in muzzleLists)
                {
                    Common.Hint("List");
                    foreach (MunitionTags tags in muzzleList.CompatibleAmmoTags)
                    {
                        Common.Hint($"Ammo C {tags.Class} S {tags.Subclass}");
                    }
                }
            }
        }

        static void Postfix(ContinuousWeaponComponent __instance)
        {
            Common.LogPatch();
 
        }
    }
}
