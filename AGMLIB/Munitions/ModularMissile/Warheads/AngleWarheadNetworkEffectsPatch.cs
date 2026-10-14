using HarmonyLib;
using Munitions.ModularMissiles;
using Munitions.ModularMissiles.Runtime;

[HarmonyPatch(typeof(LookaheadMunitionBase), "UserCode_RpcRollImpactEffect")]
internal static class AngleWarheadNetworkEffectsPatch
{
    // Reserved effect sets in the native impact RPC distinguish custom presentation
    // from ordinary impacts. Beam events carry the exact length in surfaceSetIndex.
    internal const int ExplosionEffect = -1;
    internal const int BeamEffect = -2;

    private static bool Prefix(LookaheadMunitionBase __instance, HitResult hitRes, int effectSet, int surfaceSetIndex,
        Vector3 position, Quaternion rotation, Mirror.NetworkIdentity attachTo)
    {
        if (effectSet is not (ExplosionEffect or BeamEffect) || attachTo != null || __instance is not ModularMissile missile)
            return true;
        RuntimeMissileWarhead runtime = missile.GetComponent<RuntimeMissileWarhead>();
        if (runtime == null || runtime.Internals().Descriptor is not AngleWarheadDescriptor descriptor)
            return true;
        if (effectSet == ExplosionEffect)
            descriptor.SpawnPresentationExplosionEffects(position, rotation, runtime);
        else if (descriptor is BeamWarheadDescriptor beam)
            beam.SpawnPresentationBeam(position, rotation, BitConverter.ToSingle(BitConverter.GetBytes(surfaceSetIndex), 0),
                hitRes != HitResult.None, runtime);
        return false;
    }
}

[HarmonyPatch(typeof(ModularMissile), "GenericImpact")]
internal static class AngleWarheadSceneryEffectsPatch
{
    private static void Postfix(ModularMissile __instance, MunitionHitInfo hitInfo)
    {
        if (!__instance.isServer || __instance.GetComponent<RuntimeMissileWarhead>() is not { } runtime ||
            runtime.Internals().Descriptor is not AngleWarheadDescriptor)
            return;

        // Native scenery impacts bypass the warhead. Preserve their custom visual
        // without repeating damage or treating ordinary ship-impact RPCs as detonations.
        ((LookaheadMunitionBase)__instance).Internals().RpcRollImpactEffect(HitResult.None,
            AngleWarheadNetworkEffectsPatch.ExplosionEffect, 0, hitInfo.Point, __instance.transform.rotation, null!);
    }
}
