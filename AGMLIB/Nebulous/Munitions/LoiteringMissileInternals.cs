using Munitions;

public static partial class NativeInternalsExtensions
{
    public static LoiteringMissileInternals Internals(this LoiteringMissile target) => new(target);
}

public readonly struct LoiteringMissileInternals(LoiteringMissile target)
{
    private static class Refs
    {
        internal static readonly Action<LoiteringMissile, Vector3, bool> TriggerAttack =
            AccessTools.MethodDelegate<Action<LoiteringMissile, Vector3, bool>>(
                AccessTools.Method(typeof(LoiteringMissile), "TriggerAttack", [typeof(Vector3), typeof(bool)])
                ?? throw new MissingMethodException(typeof(LoiteringMissile).FullName, "TriggerAttack"));
        internal static readonly AccessTools.FieldRef<LoiteringMissile, SphereCollider> ActivationTrigger =
            AccessTools.FieldRefAccess<LoiteringMissile, SphereCollider>("_activationTrigger");
        internal static readonly AccessTools.FieldRef<LoiteringMissile, Communicator> Comms =
            AccessTools.FieldRefAccess<LoiteringMissile, Communicator>("_comms");
        internal static readonly AccessTools.FieldRef<LoiteringMissile, AudioSource> PushoffEngineLoop =
            AccessTools.FieldRefAccess<LoiteringMissile, AudioSource>("_pushoffEngineLoop");
        internal static readonly AccessTools.FieldRef<LoiteringMissile, VisualEffect> PushoffEngineParticles =
            AccessTools.FieldRefAccess<LoiteringMissile, VisualEffect>("_pushoffEngineParticles");
    }

    public ref SphereCollider ActivationTrigger => ref Refs.ActivationTrigger(target);
    public ref Communicator Comms => ref Refs.Comms(target);
    public ref AudioSource PushoffEngineLoop => ref Refs.PushoffEngineLoop(target);
    public ref VisualEffect PushoffEngineParticles => ref Refs.PushoffEngineParticles(target);

    public void TriggerAttack(Vector3 targetPosition, bool checkObstruction = true) => Refs.TriggerAttack(target, targetPosition, checkObstruction);
}