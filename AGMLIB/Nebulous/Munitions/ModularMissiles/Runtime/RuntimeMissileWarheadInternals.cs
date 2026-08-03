using HarmonyLib;
using Munitions.ModularMissiles.Descriptors.Warheads;
using Munitions.ModularMissiles.Runtime;

public static partial class NativeInternalsExtensions
{
    public static RuntimeMissileWarheadInternals Internals(this RuntimeMissileWarhead warhead) => new(warhead);
}

public readonly struct RuntimeMissileWarheadInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<RuntimeMissileWarhead, BaseWarheadDescriptor> Descriptor =
            AccessTools.FieldRefAccess<RuntimeMissileWarhead, BaseWarheadDescriptor>("_descriptor");
    }

    private readonly RuntimeMissileWarhead _warhead;

    internal RuntimeMissileWarheadInternals(RuntimeMissileWarhead warhead)
    {
        _warhead = warhead ?? throw new ArgumentNullException(nameof(warhead));
    }

    public ref BaseWarheadDescriptor Descriptor => ref Refs.Descriptor(_warhead);
}
