extern alias agmlib;

using System;
using System.Linq;
using Bundles;
using HarmonyLib;
using Modding;
using Ships;
using Sound;
using UnityEngine;
using Utility;

public sealed class ModEntryPoint : IModEntryPoint
{
    private const string LogPrefix = "[HaloShim]";
    private static Harmony _harmony;

    public void PreLoad()
    {
        _harmony ??= new Harmony("agmlib.compatibility.halo-shim");
        _harmony.PatchAll(typeof(ModEntryPoint).Assembly);

        Debug.Log(
            $"{LogPrefix} PreLoad assembly={typeof(ModEntryPoint).Assembly.FullName} "
                + $"agmlib={ResolveAgmlibVersion()}"
        );
    }

    public void PostLoad()
    {
        (
            int iconRepairs,
            int audioRepairs,
            int driveRepairs,
            int groupedAudioRepairs
        ) =
            NormalizeLegacyHullBindings();
        int materialRepairs = NormalizeLegacyComponentMaterials();
        Type[] façadeTypes = typeof(ModEntryPoint)
            .Assembly
            .GetTypes()
            .Where(
                type =>
                    type != typeof(ModEntryPoint)
                    && !type.IsNested
                    && (typeof(MonoBehaviour).IsAssignableFrom(type)
                        || typeof(ScriptableObject).IsAssignableFrom(type))
            )
            .ToArray();

        Debug.Log(
            $"{LogPrefix} PostLoad legacyFacadeTypes={façadeTypes.Length} "
                + $"iconRepairs={iconRepairs} audioRepairs={audioRepairs} "
                + $"driveRepairs={driveRepairs} "
                + $"groupedAudioRepairs={groupedAudioRepairs} "
                + $"materialRepairs={materialRepairs} "
                + "saveMigration=disabled bundles=unchanged"
        );
    }

    private static (
        int IconRepairs,
        int AudioRepairs,
        int DriveRepairs,
        int GroupedAudioRepairs
    ) NormalizeLegacyHullBindings()
    {
        int iconRepairs = 0;
        int audioRepairs = 0;
        int driveRepairs = 0;
        int groupedAudioRepairs = 0;

        foreach (
            BaseHull hull in Resources.FindObjectsOfTypeAll<BaseHull>()
                .Where(
                    hull =>
                        hull != null
                        && hull.SaveKey != null
                        && hull.SaveKey.StartsWith("Halo/", StringComparison.Ordinal)
                )
        )
        {
            agmlib::BaseHullInternals hullInternals =
                agmlib::NativeInternalsExtensions.Internals(hull);
            Sprite fallback = hull.HullScreenshot;
            if (hullInternals.NormalSilhouette == null && fallback != null)
            {
                hullInternals.NormalSilhouette = fallback;
                iconRepairs++;
            }
            if (
                hullInternals.HudSilhouette == null
                && hullInternals.NormalSilhouette != null
            )
            {
                hullInternals.HudSilhouette = hullInternals.NormalSilhouette;
                iconRepairs++;
            }

            driveRepairs += NormalizeEmbeddedDrives(hull);
            groupedAudioRepairs += NormalizeGroupedAudioSources(hull);

            BookendedAudioPlayer[] players =
                hull.GetComponentsInChildren<BookendedAudioPlayer>(true);
            BookendedSoundEffect survivingEffect =
                ResolveVanillaHullAudio(hull.SaveKey)
                ?? players
                    .Select(
                        player =>
                            agmlib::NativeInternalsExtensions
                                .Internals(player)
                                .SoundEffect
                    )
                    .FirstOrDefault(effect => effect != null);
            if (survivingEffect == null)
                continue;

            foreach (BookendedAudioPlayer player in players)
            {
                agmlib::BookendedAudioPlayerInternals playerInternals =
                    agmlib::NativeInternalsExtensions.Internals(player);
                if (playerInternals.SoundEffect != null)
                    continue;

                playerInternals.SoundEffect = survivingEffect;
                audioRepairs++;
            }
        }

        return (
            iconRepairs,
            audioRepairs,
            driveRepairs,
            groupedAudioRepairs
        );
    }

    private static int NormalizeGroupedAudioSources(BaseHull hull)
    {
        int repairs = 0;
        foreach (
            GroupedAudioSource source in hull.GetComponentsInChildren<GroupedAudioSource>(
                true
            )
        )
        {
            agmlib::GroupedAudioSourceInternals internals =
                agmlib::NativeInternalsExtensions.Internals(source);
            if (
                !internals.HasSimpleSource
                || internals.HasSimpleSoundEffect
            )
            {
                continue;
            }

            // Current vanilla bookended-only groups leave both simple fields null.
            // Legacy Halo kept an AudioSource but no BaseSoundEffect, which causes
            // GroupedAudioSource.CoroutineFadeIn to dereference the missing effect.
            internals.ClearSimpleSource();
            repairs++;
        }

        return repairs;
    }

    private static int NormalizeEmbeddedDrives(BaseHull baseHull)
    {
        if (baseHull is not Hull hull)
            return 0;

        Transform socketRoot =
            agmlib::NativeInternalsExtensions.Internals(hull).SocketRoot;
        if (socketRoot == null)
            return 0;

        int repairs = 0;
        foreach (
            Ships.HullPartDrive drive in hull
                .GetComponentsInChildren<Ships.HullPartDrive>(true)
                .ToArray()
        )
        {
            HullSocket socket =
                drive.GetComponent<HullSocket>()
                ?? drive.gameObject.AddComponent<HullSocket>();
            agmlib::HullSocketInternals socketInternals =
                agmlib::NativeInternalsExtensions.Internals(socket);

            drive.transform.SetParent(socketRoot, true);
            socketInternals.Key = drive.PartKey;
            socketInternals.Size = Vector3Int.one;
            socketInternals.Type = drive.Type;
            socketInternals.Component = drive;
            socketInternals.Hull = hull;

            repairs++;
        }

        return repairs;
    }

    private static int NormalizeLegacyComponentMaterials()
    {
        int repairs = 0;
        foreach (
            HullComponent component in Resources.FindObjectsOfTypeAll<HullComponent>()
                .Where(
                    component =>
                        component != null
                        && component.SaveKey != null
                        && component.SaveKey.StartsWith(
                            "Halo/",
                            StringComparison.Ordinal
                        )
                )
        )
        {
            Material vanillaFallback =
                ResolveVanillaComponentMaterial(component.SaveKey);
            foreach (
                LODGroupSharedMaterial sharedMaterial in component
                    .GetComponentsInChildren<LODGroupSharedMaterial>(true)
            )
            {
                agmlib::LODGroupSharedMaterialInternals internals =
                    agmlib::NativeInternalsExtensions.Internals(sharedMaterial);
                Material[] originalMaterials = internals.OriginalMaterials;
                if (originalMaterials == null || originalMaterials.Length == 0)
                    continue;

                Renderer[] renderers = sharedMaterial
                    .GetComponentsInChildren<MeshRenderer>(true)
                    .Cast<Renderer>()
                    .Concat(
                        sharedMaterial
                            .GetComponentsInChildren<SkinnedMeshRenderer>(true)
                            .Cast<Renderer>()
                    )
                    .ToArray();

                bool repairedGroup = false;
                for (int index = 0; index < originalMaterials.Length; index++)
                {
                    if (originalMaterials[index] != null)
                        continue;

                    Material replacement = renderers
                        .Select(renderer => renderer.sharedMaterials)
                        .Where(materials => index < materials.Length)
                        .Select(materials => materials[index])
                        .FirstOrDefault(material => material != null)
                        ?? vanillaFallback;
                    if (replacement == null)
                        continue;

                    originalMaterials[index] = replacement;
                    repairs++;
                    repairedGroup = true;
                }

                if (repairedGroup)
                {
                    internals.OriginalMaterials = originalMaterials;
                    internals.MaterialInstances = null;
                }
            }
        }

        return repairs;
    }

    private static Material ResolveVanillaComponentMaterial(
        string haloComponentSaveKey
    )
    {
        string sourceKey = haloComponentSaveKey switch
        {
            "Halo/United Rebel Front/MLS-4 Launcher" =>
                "Stock/MLS-3 Launcher",
            "Halo/UNSC/E55.2 'Beacon' Twin Illuminator" =>
                "Stock/E55 'Spotlight' Illuminator",
            "Halo/UNSC/E71.S 'Hush' Spinal Jammer" =>
                "Stock/E71 'Hangup' Jammer",
            "Halo/UNSC/E90.S 'Blindfold' Spinal Jammer" =>
                "Stock/E90 'Blanket' Jammer",
            "Halo/UNSC/VLS-0-43 Launcher" =>
                "Stock/VLS-1-46 Launcher",
            "Halo/UNSC/VLS-1-14 Launcher" =>
                "Stock/VLS-1-23 Launcher",
            "Halo/UNSC/VLS-2 Launcher" => "Stock/VLS-2 Launcher",
            "Halo/UNSC/VLS-4 Launcher" => "Stock/VLS-3 Launcher",
            _ => null,
        };
        if (sourceKey == null)
            return null;

        HullComponent sourceComponent =
            BundleManager.Instance.GetHullComponent(sourceKey);
        if (sourceComponent == null)
            return null;

        foreach (
            LODGroupSharedMaterial sharedMaterial in sourceComponent
                .GetComponentsInChildren<LODGroupSharedMaterial>(true)
        )
        {
            Material[] sourceMaterials =
                agmlib::NativeInternalsExtensions
                    .Internals(sharedMaterial)
                    .OriginalMaterials;
            Material survivingMaterial =
                sourceMaterials?.FirstOrDefault(material => material != null);
            if (survivingMaterial != null)
                return survivingMaterial;
        }

        return null;
    }

    private static BookendedSoundEffect ResolveVanillaHullAudio(
        string haloHullSaveKey
    )
    {
        string sourceKey = haloHullSaveKey switch
        {
            "Halo/UNSC/Gladius Heavy Corvette" => "Stock/Sprinter Corvette",
            "Halo/UNSC/Halberd Light Destroyer" => "Stock/Keystone Destroyer",
            "Halo/UNSC/Paris Heavy Frigate" => "Stock/Raines Frigate",
            "Halo/UNSC/Halcyon Light Cruiser" => "Stock/Vauxhall Light Cruiser",
            "Halo/UNSC/Marathon Heavy Cruiser" => "Stock/Axford Heavy Cruiser",
            "Halo/UNSC/Thanatos Heavy Battleship" => "Stock/Solomon Battleship",
            _ => null,
        };
        if (sourceKey == null)
            return null;

        BaseHull sourceHull = BundleManager.Instance.GetHull(sourceKey);
        BookendedAudioPlayer sourcePlayer =
            sourceHull?.GetComponent<BookendedAudioPlayer>();
        return sourcePlayer == null
            ? null
            : agmlib::NativeInternalsExtensions
                .Internals(sourcePlayer)
                .SoundEffect;
    }

    private static string ResolveAgmlibVersion()
    {
        return AppDomain
                .CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(assembly => assembly.GetName().Name == "AGMLIB")
                ?.GetName()
                .Version
                ?.ToString()
            ?? "not-loaded";
    }
}
