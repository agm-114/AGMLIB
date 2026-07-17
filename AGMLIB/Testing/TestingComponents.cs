using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Bundles;
using Ships;
using UnityEngine;

namespace Lib.Testing;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class TestingComponentAttribute : Attribute
{
    public TestingComponentAttribute(string sourceSaveKey, string saveKey)
    {
        SourceSaveKey = sourceSaveKey;
        SaveKey = saveKey;
    }

    public string SourceSaveKey { get; }

    public string SaveKey { get; }

    public string DisplayName { get; set; } = "";

    public string Category { get; set; } = "Testing";

    public string Description { get; set; } = "";

    public int PointCost { get; set; }

    public bool UnlockForAllFactions { get; set; } = true;

    public bool DebugBuildOnly { get; set; } = true;

    public int Order { get; set; }
}

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class TestingComponentFactoryAttribute : Attribute
{
    public int Order { get; set; }

    public bool DebugBuildOnly { get; set; } = true;
}

public interface ITestingComponentFactory
{
    void CreateTestingComponents(TestingComponentContext context);
}

public sealed class TestingComponentReport
{
    public int Discovered { get; internal set; }

    public int Created { get; internal set; }

    public int Skipped { get; internal set; }

    public int Failed { get; internal set; }
}

public sealed class TestingComponentMarker : MonoBehaviour
{
    [SerializeField]
    private string _sourceSaveKey = "";

    [SerializeField]
    private string _factory = "";

    public string SourceSaveKey => _sourceSaveKey;

    public string Factory => _factory;

    internal void Initialize(string sourceSaveKey, string factory)
    {
        _sourceSaveKey = sourceSaveKey;
        _factory = factory;
    }
}

public sealed class TestingComponentBuilder
{
    private const BindingFlags SerializedFieldFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    internal TestingComponentBuilder(HullComponent source, GameObject prefab, HullComponent component, string saveKey)
    {
        Source = source;
        Prefab = prefab;
        Component = component;
        SaveKey = saveKey;
        SetDisplayName($"[TEST] {source.ComponentName}");
        SetCategory("Testing");
        SetPointCost(0);
        UnlockForAllFactions();
        SetDescription($"Runtime-generated testing clone of {source.SaveKey}.");
    }

    public HullComponent Source { get; }

    public GameObject Prefab { get; }

    public HullComponent Component { get; private set; }

    public string SaveKey { get; }

    public TestingComponentBuilder ReplaceRoot<TReplacement>() where TReplacement : HullComponent
    {
        ReplaceRoot(typeof(TReplacement));
        return this;
    }

    public TestingComponentBuilder ReplaceRoot(Type replacementType)
    {
        Component = (HullComponent)ReplaceBehaviour(Component, replacementType);
        return this;
    }

    public int ReplaceInChildren<TOriginal, TReplacement>(bool exactTypeOnly = true, Func<TOriginal, bool>? predicate = null)
        where TOriginal : MonoBehaviour
        where TReplacement : TOriginal
    {
        TOriginal[] originals = Prefab.GetComponentsInChildren<TOriginal>(includeInactive: true);
        int replaced = 0;
        foreach (TOriginal original in originals)
        {
            if ((exactTypeOnly && original.GetType() != typeof(TOriginal)) || predicate?.Invoke(original) == false)
            {
                continue;
            }

            ReplaceBehaviour(original, typeof(TReplacement));
            replaced++;
        }

        return replaced;
    }

    public T AddToRoot<T>() where T : Component
    {
        return Prefab.AddComponent<T>();
    }

    public TestingComponentBuilder SetDisplayName(string displayName)
    {
        if (!string.IsNullOrWhiteSpace(displayName))
        {
            Prefab.name = displayName.Trim();
            Common.SetVal(Component, "_shortUIName", Prefab.name);
        }

        return this;
    }

    public TestingComponentBuilder SetCategory(string category)
    {
        Common.SetVal(Component, "_category", category ?? "");
        return this;
    }

    public TestingComponentBuilder SetDescription(string description)
    {
        Common.SetVal(Component, "_shortDescription", "");
        Common.SetVal(Component, "_longDescription", description ?? "");
        return this;
    }

    public TestingComponentBuilder SetPointCost(int pointCost)
    {
        Common.SetVal(Component, "_pointCost", Mathf.Max(0, pointCost));
        Common.SetVal(Component, "_compoundingCost", false);
        Common.SetVal(Component, "_compoundingCostClass", "");
        Common.SetVal(Component, "_firstInstanceFree", false);
        return this;
    }

    public TestingComponentBuilder UnlockForAllFactions()
    {
        Common.SetVal(Component, "_factionKey", "");
        return this;
    }

    internal void FinalizeMetadata()
    {
        Common.SetVal(Component, "_saveKey", SaveKey);
        Component.SourceModId = TestingComponentContext.AgmlibModId;
    }

    private static MonoBehaviour ReplaceBehaviour(MonoBehaviour original, Type replacementType)
    {
        Type originalType = original.GetType();
        if (!originalType.IsAssignableFrom(replacementType))
        {
            throw new InvalidOperationException($"Testing replacement {replacementType.FullName} must derive from {originalType.FullName}.");
        }
        if (replacementType.IsAbstract || !typeof(MonoBehaviour).IsAssignableFrom(replacementType))
        {
            throw new InvalidOperationException($"Testing replacement {replacementType.FullName} must be a concrete MonoBehaviour.");
        }

        List<(FieldInfo Field, object? Value)> serializedValues = CaptureSerializedFields(original, originalType);
        bool enabled = original.enabled;
        GameObject gameObject = original.gameObject;
        UnityEngine.Object.DestroyImmediate(original);

        MonoBehaviour replacement = gameObject.AddComponent(replacementType) as MonoBehaviour
            ?? throw new InvalidOperationException($"Unity could not add testing replacement {replacementType.FullName}.");
        replacement.enabled = enabled;
        foreach ((FieldInfo field, object? value) in serializedValues)
        {
            field.SetValue(replacement, value);
        }

        return replacement;
    }

    private static List<(FieldInfo Field, object? Value)> CaptureSerializedFields(MonoBehaviour original, Type originalType)
    {
        List<(FieldInfo Field, object? Value)> values = new List<(FieldInfo, object?)>();
        for (Type? type = originalType; type != null && type != typeof(MonoBehaviour); type = type.BaseType)
        {
            foreach (FieldInfo field in type.GetFields(SerializedFieldFlags))
            {
                if (!ShouldCopySerializedField(field))
                {
                    continue;
                }

                values.Add((field, field.GetValue(original)));
            }
        }

        return values;
    }

    private static bool ShouldCopySerializedField(FieldInfo field)
    {
        if (field.IsStatic || field.IsLiteral || field.IsInitOnly || field.IsDefined(typeof(NonSerializedAttribute), inherit: false))
        {
            return false;
        }

        return field.IsPublic || field.IsDefined(typeof(SerializeField), inherit: false);
    }
}

public sealed class TestingComponentContext
{
    public const ulong AgmlibModId = 2960504230;

    private static GameObject? _templateRoot;

    private readonly Dictionary<string, HullComponent> _components;

    private readonly IReadOnlyList<HullComponent> _sourceComponents;

    private readonly TestingComponentReport _report;

    private string _currentDeclaration = "unknown";

    internal TestingComponentContext(TestingComponentReport report)
    {
        _report = report;
        _components = Common.GetVal<Dictionary<string, HullComponent>>(BundleManager.Instance, "_components")
            ?? throw new InvalidOperationException("BundleManager component registry is unavailable.");
        _sourceComponents = BundleManager.Instance.AllComponents
            .Where(component => component != null && component.GetComponent<TestingComponentMarker>() == null)
            .OrderBy(component => component.SaveKey, StringComparer.Ordinal)
            .ToList();
    }

    public IReadOnlyList<HullComponent> SourceComponents => _sourceComponents;

    public HullComponent? Create(string sourceSaveKey, string saveKey, Action<TestingComponentBuilder>? configure = null)
    {
        HullComponent? source = _sourceComponents.FirstOrDefault(component => component.SaveKey == sourceSaveKey);
        if (source == null)
        {
            RecordFailure($"source component '{sourceSaveKey}' was not found");
            return null;
        }

        return Create(source, saveKey, configure);
    }

    public HullComponent? CreateFirst(Func<HullComponent, bool> selector, string saveKey, Action<TestingComponentBuilder>? configure = null)
    {
        HullComponent? source = _sourceComponents.FirstOrDefault(selector);
        if (source == null)
        {
            RecordFailure($"no source component matched the selector for '{saveKey}'");
            return null;
        }

        return Create(source, saveKey, configure);
    }

    public IEnumerable<HullComponent> FindSources(Func<HullComponent, bool> selector)
    {
        return _sourceComponents.Where(selector);
    }

    internal void BeginDeclaration(string declaration)
    {
        _currentDeclaration = declaration;
    }

    internal void RecordFactoryFailure(Exception exception)
    {
        _report.Failed++;
        Debug.LogError($"[TestingComponents] {_currentDeclaration} failed: {exception}");
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "One broken test declaration must not abort mod loading.")]
    private HullComponent? Create(HullComponent source, string saveKey, Action<TestingComponentBuilder>? configure)
    {
        if (string.IsNullOrWhiteSpace(saveKey))
        {
            RecordFailure("a testing component declared an empty save key");
            return null;
        }

        saveKey = saveKey.Trim();
        if (_components.TryGetValue(saveKey, out HullComponent existing))
        {
            _report.Skipped++;
            string owner = existing.GetComponent<TestingComponentMarker>()?.Factory ?? "loaded content";
            Debug.LogWarning($"[TestingComponents] Skipped '{saveKey}' from {_currentDeclaration}; the key is already owned by {owner}.");
            return null;
        }

        GameObject prefab = (GameObject)UnityEngine.Object.Instantiate(
            source.gameObject,
            GetTemplateRoot().transform,
            instantiateInWorldSpace: false);
        prefab.name = source.gameObject.name;
        prefab.SetActive(true);

        try
        {
            HullComponent component = prefab.GetComponent<HullComponent>()
                ?? throw new InvalidOperationException($"Cloned source '{source.SaveKey}' has no HullComponent.");
            TestingComponentBuilder builder = new TestingComponentBuilder(source, prefab, component, saveKey);
            configure?.Invoke(builder);
            builder.FinalizeMetadata();

            TestingComponentMarker marker = prefab.GetComponent<TestingComponentMarker>() ?? prefab.AddComponent<TestingComponentMarker>();
            marker.Initialize(source.SaveKey, _currentDeclaration);
            _components.Add(saveKey, builder.Component);
            _report.Created++;
            Debug.Log($"[TestingComponents] Registered '{saveKey}' from '{source.SaveKey}' via {_currentDeclaration}.");
            return builder.Component;
        }
        catch (Exception exception)
        {
            UnityEngine.Object.DestroyImmediate(prefab);
            RecordFactoryFailure(exception);
            return null;
        }
    }

    private void RecordFailure(string reason)
    {
        _report.Failed++;
        Debug.LogError($"[TestingComponents] {_currentDeclaration}: {reason}.");
    }

    private static GameObject GetTemplateRoot()
    {
        if (_templateRoot != null)
        {
            return _templateRoot;
        }

        _templateRoot = new GameObject("AGMLIB Runtime Testing Component Prefabs")
        {
            hideFlags = HideFlags.HideInHierarchy
        };
        _templateRoot.SetActive(false);
        UnityEngine.Object.DontDestroyOnLoad(_templateRoot);
        return _templateRoot;
    }
}

public static class TestingComponentBootstrap
{
#if DEBUG
    private const bool IsDebugBuild = true;
#else
    private const bool IsDebugBuild = false;
#endif
    private static readonly HashSet<Assembly> InitializedAssemblies = new HashSet<Assembly>();

    private sealed class DiscoveryJob
    {
        public int Order;

        public string Name = "";

        public Action<TestingComponentContext> Execute = null!;
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Discovery isolates optional testing declarations from normal mod loading.")]
    public static TestingComponentReport DiscoverAndCreate(Assembly assembly)
    {
        if (assembly == null)
            throw new ArgumentNullException(nameof(assembly));

        TestingComponentReport report = new TestingComponentReport();
        lock (InitializedAssemblies)
        {
            if (InitializedAssemblies.Contains(assembly))
                return report;
        }

        TestingComponentContext context;
        try
        {
            context = new TestingComponentContext(report);
        }
        catch (Exception exception)
        {
            report.Failed++;
            Debug.LogError($"[TestingComponents] Bootstrap failed before discovery: {exception}");
            return report;
        }

        List<DiscoveryJob> jobs = DiscoverJobs(assembly, report);
        foreach (DiscoveryJob job in jobs.OrderBy(job => job.Order).ThenBy(job => job.Name, StringComparer.Ordinal))
        {
            context.BeginDeclaration(job.Name);
            try
            {
                job.Execute(context);
            }
            catch (Exception exception)
            {
                context.RecordFactoryFailure(exception);
            }
        }

        Debug.Log($"[TestingComponents] Discovery complete: discovered={report.Discovered}, created={report.Created}, skipped={report.Skipped}, failed={report.Failed}.");
        if (report.Failed == 0)
        {
            lock (InitializedAssemblies)
            {
                InitializedAssemblies.Add(assembly);
            }
        }

        return report;
    }

    private static List<DiscoveryJob> DiscoverJobs(Assembly assembly, TestingComponentReport report)
    {
        List<DiscoveryJob> jobs = new List<DiscoveryJob>();
        foreach (Type type in GetLoadableTypes(assembly))
        {
            foreach (TestingComponentAttribute attribute in type.GetCustomAttributes<TestingComponentAttribute>(inherit: false))
            {
                report.Discovered++;
                if (attribute.DebugBuildOnly && !IsDebugBuild)
                {
                    report.Skipped++;
                    continue;
                }

                TestingComponentAttribute capturedAttribute = attribute;
                Type capturedType = type;
                jobs.Add(new DiscoveryJob
                {
                    Order = capturedAttribute.Order,
                    Name = capturedType.FullName ?? capturedType.Name,
                    Execute = context => CreateAttributedComponent(context, capturedType, capturedAttribute)
                });
            }

            TestingComponentFactoryAttribute? factoryAttribute = type.GetCustomAttribute<TestingComponentFactoryAttribute>(inherit: false);
            if (factoryAttribute == null)
            {
                continue;
            }

            report.Discovered++;
            if (factoryAttribute.DebugBuildOnly && !IsDebugBuild)
            {
                report.Skipped++;
                continue;
            }

            Type capturedFactoryType = type;
            jobs.Add(new DiscoveryJob
            {
                Order = factoryAttribute.Order,
                Name = capturedFactoryType.FullName ?? capturedFactoryType.Name,
                Execute = context => CreateFromFactory(context, capturedFactoryType)
            });
        }

        return jobs;
    }

    private static void CreateAttributedComponent(TestingComponentContext context, Type componentType, TestingComponentAttribute attribute)
    {
        if (componentType.IsAbstract || !typeof(HullComponent).IsAssignableFrom(componentType))
        {
            throw new InvalidOperationException($"{componentType.FullName} uses TestingComponentAttribute but is not a concrete HullComponent.");
        }

        context.Create(attribute.SourceSaveKey, attribute.SaveKey, builder =>
        {
            builder.ReplaceRoot(componentType);
            if (!string.IsNullOrWhiteSpace(attribute.DisplayName))
            {
                builder.SetDisplayName(attribute.DisplayName);
            }
            if (!string.IsNullOrWhiteSpace(attribute.Category))
            {
                builder.SetCategory(attribute.Category);
            }
            if (!string.IsNullOrWhiteSpace(attribute.Description))
            {
                builder.SetDescription(attribute.Description);
            }
            builder.SetPointCost(attribute.PointCost);
            if (attribute.UnlockForAllFactions)
            {
                builder.UnlockForAllFactions();
            }
        });
    }

    private static void CreateFromFactory(TestingComponentContext context, Type factoryType)
    {
        if (factoryType.IsAbstract || !typeof(ITestingComponentFactory).IsAssignableFrom(factoryType))
        {
            throw new InvalidOperationException($"{factoryType.FullName} uses TestingComponentFactoryAttribute but does not implement ITestingComponentFactory.");
        }

        ITestingComponentFactory factory = Activator.CreateInstance(factoryType) as ITestingComponentFactory
            ?? throw new InvalidOperationException($"Could not construct testing component factory {factoryType.FullName}.");
        factory.CreateTestingComponents(context);
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            foreach (Exception? loaderException in exception.LoaderExceptions)
            {
                if (loaderException != null)
                {
                    Debug.LogWarning($"[TestingComponents] Type discovery loader warning: {loaderException.Message}");
                }
            }

            return exception.Types.Where(type => type != null).Cast<Type>();
        }
    }
}
