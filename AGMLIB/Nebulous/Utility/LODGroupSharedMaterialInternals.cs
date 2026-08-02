using HarmonyLib;
using UnityEngine;
using Utility;

public static partial class NativeInternalsExtensions
{
    public static LODGroupSharedMaterialInternals Internals(
        this LODGroupSharedMaterial sharedMaterial
    ) => new(sharedMaterial);
}

public readonly struct LODGroupSharedMaterialInternals
{
    private static class Refs
    {
        internal static readonly AccessTools.FieldRef<
            LODGroupSharedMaterial,
            Material[]
        > OriginalMaterials = AccessTools.FieldRefAccess<
            LODGroupSharedMaterial,
            Material[]
        >("_originalMaterials");

        internal static readonly AccessTools.FieldRef<
            LODGroupSharedMaterial,
            Material[]
        > MaterialInstances = AccessTools.FieldRefAccess<
            LODGroupSharedMaterial,
            Material[]
        >("_materialInstances");
    }

    private readonly LODGroupSharedMaterial _sharedMaterial;

    internal LODGroupSharedMaterialInternals(LODGroupSharedMaterial sharedMaterial)
    {
        _sharedMaterial =
            sharedMaterial ?? throw new ArgumentNullException(nameof(sharedMaterial));
    }

    public ref Material[] OriginalMaterials =>
        ref Refs.OriginalMaterials(_sharedMaterial);

    public ref Material[] MaterialInstances =>
        ref Refs.MaterialInstances(_sharedMaterial);
}
