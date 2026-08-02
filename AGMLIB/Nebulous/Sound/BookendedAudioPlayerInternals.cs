using HarmonyLib;
using Sound;

public static partial class NativeInternalsExtensions
{
    public static BookendedAudioPlayerInternals Internals(
        this BookendedAudioPlayer player
    ) => new(player);
}

public readonly struct BookendedAudioPlayerInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<
            BookendedAudioPlayer,
            BookendedSoundEffect
        > SoundEffect = AccessTools.FieldRefAccess<
            BookendedAudioPlayer,
            BookendedSoundEffect
        >("_soundEffect");
    }

    private readonly BookendedAudioPlayer _player;

    internal BookendedAudioPlayerInternals(BookendedAudioPlayer player)
    {
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public ref BookendedSoundEffect SoundEffect => ref Refs.SoundEffect(_player);
}

