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
            if (__instance.CurrentlyFiring || !__instance.Internals().Reloading)
                return;
            if (__instance.GetComponent<DynamicReload>() is not DynamicReload reload)
                return;
            if (!reload.AlwaysUnloaded)
                return;
            Debug.LogError("blocking reload " + __instance.Internals().ReloadAccum);
            float _reloadAccum = __instance.Internals().ReloadAccum;
            IWeaponComponentRPC _weaponRpcProvider = ((WeaponComponent)__instance).Internals().WeaponRpcProvider;
            _reloadAccum -= Time.fixedDeltaTime;
            _reloadAccum = 0;
            _weaponRpcProvider.MarkCycle(__instance, 0f);
            __instance.Internals().ReloadAccum = _reloadAccum;
            Debug.LogError("blocking reload 2 " + __instance.Internals().ReloadAccum);

        }
    }
}
