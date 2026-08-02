using HarmonyLib;
using Sound;
using UnityEngine;

public static partial class NativeInternalsExtensions
{
    public static GroupedAudioSourceInternals Internals(
        this GroupedAudioSource source
    ) => new(source);
}

public readonly struct GroupedAudioSourceInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<
            GroupedAudioSource,
            BaseSoundEffect
        > SimpleSoundEffect = AccessTools.FieldRefAccess<
            GroupedAudioSource,
            BaseSoundEffect
        >("_simpleSoundEffect");

        internal static readonly AccessTools.FieldRef<
            GroupedAudioSource,
            AudioSource
        > SimpleSource = AccessTools.FieldRefAccess<
            GroupedAudioSource,
            AudioSource
        >("_simpleSource");
    }

    private readonly GroupedAudioSource _source;

    internal GroupedAudioSourceInternals(GroupedAudioSource source)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));
    }

    public ref BaseSoundEffect SimpleSoundEffect =>
        ref Refs.SimpleSoundEffect(_source);

    public ref AudioSource SimpleSource => ref Refs.SimpleSource(_source);

    public bool HasSimpleSoundEffect => SimpleSoundEffect != null;

    public bool HasSimpleSource => SimpleSource != null;

    public void ClearSimpleSource()
    {
        SimpleSource = null;
    }
}
