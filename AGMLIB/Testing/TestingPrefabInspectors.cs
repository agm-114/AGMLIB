using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Bundles;
using Munitions.ModularMissiles;
using Ships;
using SmallCraft;
using UnityEngine;

namespace Lib.Testing;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class TestingPrefabInspectorAttribute : Attribute
{
    public int Order { get; set; }
}

public interface ITestingPrefabInspector
{
    void Inspect(TestingPrefabInspectionContext context);
}

public sealed class TestingPrefabInspectionContext
{
    public IReadOnlyCollection<BaseHull> Hulls => BundleManager.Instance.AllHulls;

    public IReadOnlyCollection<Spacecraft> Spaceframes => BundleManager.Instance.AllSpaceframes;

    public IReadOnlyCollection<ModularMissile> MissileBodies => BundleManager.Instance.AllMissileBodies;

    public TestingPrefabColliderReport InspectColliders(
        string category,
        string saveKey,
        Component root,
        Type expectedParentType,
        int interactionLayer,
        bool ownerAddedAtRuntime = false,
        bool logColliderDetails = false)
    {
        Collider[] colliders = root.GetComponentsInChildren<Collider>(includeInactive: true);
        Rigidbody[] rigidbodies = root.GetComponentsInChildren<Rigidbody>(includeInactive: true);
        int physicalCandidates = 0;
        int resolvableCandidates = 0;

        foreach (Collider collider in colliders)
        {
            Component resolved = GetComponentInParentIncludingInactive(collider.transform, expectedParentType);
            bool layersInteract = !Physics.GetIgnoreLayerCollision(interactionLayer, collider.gameObject.layer);
            bool physicalCandidate = collider.enabled && !collider.isTrigger && layersInteract;
            if (physicalCandidate)
            {
                physicalCandidates++;
                if (resolved != null || ownerAddedAtRuntime)
                {
                    resolvableCandidates++;
                }
            }

            if (logColliderDetails)
            {
                Debug.Log(
                    $"[TestingPrefabs] collider='{GetPath(root.transform, collider.transform)}' type={collider.GetType().Name} " +
                    $"layer={collider.gameObject.layer} enabled={collider.enabled} prefabActive={collider.gameObject.activeInHierarchy} " +
                    $"trigger={collider.isTrigger} layer{interactionLayer}Interact={layersInteract} " +
                    $"resolved={resolved?.GetType().Name ?? (ownerAddedAtRuntime ? "RUNTIME" : "NONE")}.");
            }
        }

        bool requiresRuntimeInspection = ownerAddedAtRuntime && physicalCandidates == 0;
        bool passes = physicalCandidates > 0 && resolvableCandidates == physicalCandidates;
        TestingPrefabColliderReport report = new(
            category,
            saveKey,
            colliders.Length,
            rigidbodies.Length,
            physicalCandidates,
            resolvableCandidates,
            ownerAddedAtRuntime,
            requiresRuntimeInspection,
            passes);

        Debug.Log(
            $"[TestingPrefabs] RESULT category={category} key='{saveKey}' root='{root.name}' expected={expectedParentType.Name} " +
            $"colliders={report.ColliderCount} rigidbodies={report.RigidbodyCount} physicalCandidates={report.PhysicalCandidateCount} " +
            $"resolvableCandidates={report.ResolvableCandidateCount} owner={(ownerAddedAtRuntime ? "runtime" : "prefab")} " +
            $"status={(passes ? "PASS" : requiresRuntimeInspection ? "RUNTIME_REQUIRED" : "FAIL")}.");
        return report;
    }

    private static Component GetComponentInParentIncludingInactive(Transform start, Type componentType)
    {
        for (Transform current = start; current != null; current = current.parent)
        {
            Component component = current.GetComponent(componentType);
            if (component != null)
            {
                return component;
            }
        }

        return null;
    }

    private static string GetPath(Transform root, Transform target)
    {
        Stack<string> names = new();
        for (Transform current = target; current != null; current = current.parent)
        {
            names.Push(current.name);
            if (current == root)
            {
                break;
            }
        }

        return string.Join("/", names);
    }
}

public sealed class TestingPrefabColliderReport
{
    internal TestingPrefabColliderReport(
        string category,
        string saveKey,
        int colliderCount,
        int rigidbodyCount,
        int physicalCandidateCount,
        int resolvableCandidateCount,
        bool ownerAddedAtRuntime,
        bool requiresRuntimeInspection,
        bool passes)
    {
        Category = category;
        SaveKey = saveKey;
        ColliderCount = colliderCount;
        RigidbodyCount = rigidbodyCount;
        PhysicalCandidateCount = physicalCandidateCount;
        ResolvableCandidateCount = resolvableCandidateCount;
        OwnerAddedAtRuntime = ownerAddedAtRuntime;
        RequiresRuntimeInspection = requiresRuntimeInspection;
        Passes = passes;
    }

    public string Category { get; }
    public string SaveKey { get; }
    public int ColliderCount { get; }
    public int RigidbodyCount { get; }
    public int PhysicalCandidateCount { get; }
    public int ResolvableCandidateCount { get; }
    public bool OwnerAddedAtRuntime { get; }
    public bool RequiresRuntimeInspection { get; }
    public bool Passes { get; }
}

public static class TestingPrefabInspectorBootstrap
{
    private static readonly HashSet<Assembly> InspectedAssemblies = new();

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "One optional inspector must not abort mod loading.")]
    public static void DiscoverAndInspect(Assembly assembly)
    {
        if (assembly == null)
        {
            throw new ArgumentNullException(nameof(assembly));
        }

        lock (InspectedAssemblies)
        {
            if (!InspectedAssemblies.Add(assembly))
            {
                return;
            }
        }

        TestingPrefabInspectionContext context = new();
        List<(int Order, Type Type)> inspectors = GetLoadableTypes(assembly)
            .Select(type => (Type: type, Attribute: type.GetCustomAttribute<TestingPrefabInspectorAttribute>(inherit: false)))
            .Where(entry => entry.Attribute != null)
            .Select(entry => (entry.Attribute!.Order, entry.Type))
            .OrderBy(entry => entry.Order)
            .ThenBy(entry => entry.Type.FullName, StringComparer.Ordinal)
            .ToList();

        foreach ((int _, Type type) in inspectors)
        {
            try
            {
                if (type.IsAbstract || !typeof(ITestingPrefabInspector).IsAssignableFrom(type))
                {
                    throw new InvalidOperationException($"{type.FullName} must be a concrete {nameof(ITestingPrefabInspector)}.");
                }

                ITestingPrefabInspector inspector = Activator.CreateInstance(type) as ITestingPrefabInspector
                    ?? throw new InvalidOperationException($"Could not construct {type.FullName}.");
                inspector.Inspect(context);
            }
            catch (Exception exception)
            {
                Debug.LogError($"[TestingPrefabs] Inspector {type.FullName} failed: {exception}");
            }
        }
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            return exception.Types.Where(type => type != null).Cast<Type>();
        }
    }
}
