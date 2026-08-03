using HarmonyLib;
using Munitions.ModularMissiles;
using Munitions.ModularMissiles.Runtime;

[HarmonyPatch(typeof(LookaheadMunitionBase), "UserCode_RpcRollImpactEffect")]
internal static class AngleWarheadNetworkEffectsPatch
{
    private static void Postfix(
        LookaheadMunitionBase __instance,
        Vector3 position,
        Quaternion rotation)
    {
        if (__instance is not ModularMissile missile)
            return;

        RuntimeMissileWarhead runtime = missile.GetComponent<RuntimeMissileWarhead>();
        if (runtime == null ||
            runtime.Internals().Descriptor is not AngleWarheadDescriptor descriptor)
        {
            return;
        }

        descriptor.SpawnPresentationExplosionEffects(position, rotation, runtime);
    }
}
