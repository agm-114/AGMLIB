using HarmonyLib;
using Munitions;
using UnityEngine;

public static partial class NativeInternalsExtensions
{
    public static AirburstRocketInternals Internals(this AirburstRocket rocket) => new(rocket);
}

public readonly struct AirburstRocketInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<AirburstRocket, GameObject> BurstEffect =
            AccessTools.FieldRefAccess<AirburstRocket, GameObject>("_burstEffect");
    }

    private readonly AirburstRocket _rocket;

    internal AirburstRocketInternals(AirburstRocket rocket)
    {
        _rocket = rocket ?? throw new ArgumentNullException(nameof(rocket));
    }

    public GameObject BurstEffect => Refs.BurstEffect(_rocket);
}
