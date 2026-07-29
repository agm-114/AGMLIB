using System.Collections;
using Game;
using HarmonyLib;
using Modding;
using UnityEngine;

namespace AGMLIB.CI.TestSupport;

public sealed class CiMatchEntryPoint : IModEntryPoint
{
    internal const string ActivationVariable = "AGMLIB_CI_AUTOSTART_MATCH";

    internal static bool IsEnabled =>
        string.Equals(
            Environment.GetEnvironmentVariable(ActivationVariable),
            "1",
            StringComparison.Ordinal);

    public void PreLoad()
    {
        if (!IsEnabled)
        {
            return;
        }

        Debug.Log("[AGMLIB CI] headless-match support enabled");
        new Harmony("agmlib.ci.headless-match").PatchAll(typeof(CiMatchEntryPoint).Assembly);
    }

    public void PostLoad()
    {
    }
}

[HarmonyPatch(typeof(SkirmishLobbyManager), "OnHostStartupCompleted")]
internal static class HeadlessMatchLaunchPatch
{
    private static bool _launchScheduled;

    private static void Postfix(SkirmishLobbyManager __instance)
    {
        if (!CiMatchEntryPoint.IsEnabled || _launchScheduled)
        {
            return;
        }

        if (!__instance.IsDedicatedServer)
        {
            Debug.LogError("[AGMLIB CI] refusing headless match launch outside a dedicated server");
            return;
        }

        _launchScheduled = true;
        __instance.StartCoroutine(LaunchWhenReady(__instance));
    }

    private static IEnumerator LaunchWhenReady(SkirmishLobbyManager lobby)
    {
        const int attempts = 30;
        for (var attempt = 1; attempt <= attempts; attempt++)
        {
            var players = lobby.Players.ToArray();
            var readyToLaunch =
                players.Length >= 2 &&
                players.All(player => player.IsBot && player.IsReady) &&
                players.Select(player => player.TeamId).Distinct().Count() >= 2;

            if (readyToLaunch)
            {
                Debug.Log(
                    $"[AGMLIB CI] launching headless match players={players.Length} bots={players.Count(player => player.IsBot)}");
                lobby.LaunchGame();
                yield break;
            }

            yield return new WaitForSecondsRealtime(1f);
        }

        var finalPlayers = lobby.Players.ToArray();
        Debug.LogError(
            $"[AGMLIB CI] headless match launch timed out players={finalPlayers.Length} " +
            $"bots={finalPlayers.Count(player => player.IsBot)} ready={finalPlayers.Count(player => player.IsReady)} " +
            $"teams={finalPlayers.Select(player => player.TeamId).Distinct().Count()}");
    }
}

[HarmonyPatch(typeof(SkirmishGameHost), nameof(SkirmishGameHost.ReturnToLobbyIfAllDisconnected))]
internal static class KeepHeadlessMatchRunningPatch
{
    private static bool _suppressionLogged;

    private static bool Prefix()
    {
        if (!CiMatchEntryPoint.IsEnabled)
        {
            return true;
        }

        if (!_suppressionLogged)
        {
            _suppressionLogged = true;
            Debug.Log("[AGMLIB CI] suppressing bot-only return to lobby");
        }

        return false;
    }
}

[HarmonyPatch(typeof(SkirmishGameHost), "WaitForPlayersToLoadMap")]
internal static class WaitForDedicatedServerMapPatch
{
    private static bool _waitingLogged;

    private static bool Prefix(
        SkirmishGameManager.ISkirmishManager ____clientManager,
        ref bool __result)
    {
        if (!CiMatchEntryPoint.IsEnabled || ____clientManager.LoadedMap != null)
        {
            return true;
        }

        if (!_waitingLogged)
        {
            _waitingLogged = true;
            Debug.Log("[AGMLIB CI] waiting for dedicated-server map instantiation");
        }

        __result = false;
        return false;
    }
}

[HarmonyPatch(typeof(SkirmishGameHost), "WaitForAllPlayersToComplete")]
internal static class WaitForBotFleetInitializationPatch
{
    private static bool _waitingLogged;

    private static void Postfix(
        WaitingOperationType operation,
        SkirmishGameManager.ISkirmishManager ____clientManager,
        ref bool __result)
    {
        if (!CiMatchEntryPoint.IsEnabled ||
            operation != WaitingOperationType.SpawnFleets ||
            !__result)
        {
            return;
        }

        var botsReady = ____clientManager.Players
            .Where(player => player.IsBot && !player.IsSpectator)
            .Cast<SkirmishPlayer>()
            .All(player => player.FleetSpawned);

        if (!botsReady && !_waitingLogged)
        {
            _waitingLogged = true;
            Debug.Log("[AGMLIB CI] waiting for bot fleet initialization");
        }

        __result = botsReady;
    }
}
