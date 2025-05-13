using static Ships.WeaponComponent;

namespace Lib.Dynamic_Systems
{
    public class DynamicReload : MonoBehaviour
    {
        public bool AlwaysUnloaded = true;
    }

    [HarmonyPatch(typeof(DiscreteWeaponComponent), "RunTimers")]
    public class DiscreteWeaponComponentRunTimers
    {
        static void Prefix(DiscreteWeaponComponent __instance)
        {
            Common.LogPatch();
            if (__instance.CurrentlyFiring || !Common.GetVal<bool>(__instance, "_reloading"))
                return;
            if (__instance.GetComponent<DynamicReload>() is not DynamicReload reload)
                return;
            if (!reload.AlwaysUnloaded)
                return;
            Debug.LogError("blocking reload " + Common.GetVal<float>(__instance, "_reloadAccum"));
            float _reloadAccum = Common.GetVal<float>(__instance, "_reloadAccum");
            IWeaponComponentRPC _weaponRpcProvider = Common.GetVal<IWeaponComponentRPC>(__instance, "_weaponRpcProvider");
            _reloadAccum -= Time.fixedDeltaTime;
            _reloadAccum = 0;
            _weaponRpcProvider.MarkCycle(__instance, 0f);
            Common.SetVal(__instance, "_reloadAccum", _reloadAccum);
            Debug.LogError("blocking reload 2 " + Common.GetVal<float>(__instance, "_reloadAccum"));

        }
    }
}
