using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Bundles;
using Modding;
using UnityEngine;

namespace Lib.Testing;

public static class TestingPrefabYamlDumper
{
    private const string DumpEnvironmentVariable = "AGMLIB_PREFAB_DUMP_DIR";
    private const string DumpScheduledAppDomainKey = "AGMLIB_PREFAB_DUMP_SCHEDULED";
    private const string DumpCompletedAppDomainKey = "AGMLIB_PREFAB_DUMP_COMPLETED";
    private const int SchemaVersion = 2;
    private const int MaximumValueDepth = 4;
    private const int MaximumCollectionItems = 256;
    private const BindingFlags InstanceMemberFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private static readonly string[] RegistryCategories =
    [
        "ai-role-definitions",
        "codex",
        "debuffs",
        "factions",
        "hud-themes",
        "hull-components",
        "hulls",
        "maps",
        "missile-bodies",
        "missile-components",
        "mission-sets",
        "munitions",
        "referenced-munitions",
        "scenarios",
        "spaceframes",
    ];

    public static void ScheduleDumpAfterAllModsLoaded()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(DumpEnvironmentVariable)))
        {
            return;
        }

        AppDomain appDomain = AppDomain.CurrentDomain;
        if (appDomain.GetData(DumpScheduledAppDomainKey) is true)
        {
            return;
        }

        appDomain.SetData(DumpScheduledAppDomainKey, true);
        BundleManager.Instance.OnLoadedModsChanged += HandleLoadedModsChanged;
        Debug.Log("[PrefabYamlDump] Scheduled for completion of all enabled mod loads.");
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "A diagnostic dump must not prevent the game from loading.")]
    public static void DumpAllFromEnvironment()
    {
        string? outputPath = Environment.GetEnvironmentVariable(DumpEnvironmentVariable);
        if (string.IsNullOrWhiteSpace(outputPath))
        {
            return;
        }

        try
        {
            DumpAll(Path.GetFullPath(outputPath));
        }
        catch (Exception exception)
        {
            Debug.LogError($"[PrefabYamlDump] Failed: {exception}");
        }
    }

    private static void HandleLoadedModsChanged()
    {
        BundleManager.Instance.OnLoadedModsChanged -= HandleLoadedModsChanged;
        AppDomain appDomain = AppDomain.CurrentDomain;
        if (appDomain.GetData(DumpCompletedAppDomainKey) is true)
        {
            return;
        }

        appDomain.SetData(DumpCompletedAppDomainKey, true);
        DumpAllFromEnvironment();
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "One malformed prefab must not abort the remaining diagnostic snapshot.")]
    public static void DumpAll(string outputPath)
    {
        if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new ArgumentException("A prefab dump output path is required.", nameof(outputPath));
        }

        outputPath = Path.GetFullPath(outputPath);
        string parentPath = Directory.GetParent(outputPath)?.FullName
            ?? throw new InvalidOperationException($"Prefab dump path '{outputPath}' has no parent directory.");
        Directory.CreateDirectory(parentPath);

        string transactionId = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);
        string temporaryPath = outputPath + ".writing-" + transactionId;
        string backupPath = outputPath + ".previous-" + transactionId;
        Directory.CreateDirectory(temporaryPath);

        List<PrefabDumpEntry> entries = CollectEntries();
        List<EnabledModDumpEntry> enabledMods = CollectEnabledMods();
        DateTime generatedUtc = DateTime.UtcNow;
        int errors = 0;
        try
        {
            foreach (PrefabDumpEntry entry in entries)
            {
                string absolutePath = Path.Combine(temporaryPath, entry.RelativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);
                try
                {
                    File.WriteAllText(absolutePath, CreatePrefabYaml(entry), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
                }
                catch (Exception exception)
                {
                    errors++;
                    entry.Error = exception.ToString();
                    File.WriteAllText(absolutePath, CreateErrorYaml(entry), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
                    Debug.LogWarning($"[PrefabYamlDump] Could not serialize {entry.Category} '{entry.SaveKey}': {exception.Message}");
                }
            }

            string modsPath = Path.Combine(temporaryPath, "mods.yaml");
            File.WriteAllText(modsPath, CreateModsYaml(enabledMods, generatedUtc), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            string manifestPath = Path.Combine(temporaryPath, "manifest.yaml");
            File.WriteAllText(manifestPath, CreateManifestYaml(entries, errors, enabledMods, generatedUtc), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            ReplaceDirectoryAtomically(temporaryPath, outputPath, backupPath);
            Debug.Log($"[PrefabYamlDump] Completed path='{outputPath}' prefabs={entries.Count} enabledMods={enabledMods.Count} errors={errors}.");
        }
        catch
        {
            TryDeleteDirectory(temporaryPath);
            throw;
        }
    }

    private static List<PrefabDumpEntry> CollectEntries()
    {
        BundleManager bundleManager = BundleManager.Instance;
        List<PrefabDumpEntry> entries = new();

        AddEntries(entries, "hulls", bundleManager.AllHulls);
        AddEntries(entries, "spaceframes", bundleManager.AllSpaceframes);
        AddEntries(entries, "missile-bodies", bundleManager.AllMissileBodies);
        AddEntries(entries, "hull-components", bundleManager.AllComponents);
        AddEntries(entries, "missile-components", bundleManager.AllMissileComponents);
        AddEntries(entries, "munitions", bundleManager.AllMunitions);
        AddEntries(entries, "factions", bundleManager.AllFactions);
        AddEntries(entries, "maps", bundleManager.SkirmishMaps);
        AddEntries(entries, "mission-sets", bundleManager.MissionSets);
        AddEntries(entries, "scenarios", bundleManager.AllScenarios);
        AddEntries(entries, "codex", bundleManager.CodexEntries);
        AddEntries(entries, "hud-themes", bundleManager.AllHUDThemes);
        AddEntries(entries, "ai-role-definitions", bundleManager.SupplementaryAIRoleDefs);
        AddEntries(
            entries,
            "debuffs",
            bundleManager
                .Internals()
                .Debuffs
                .Values
                .SelectMany(values => values)
                .Distinct());
        AddReferencedSubmunitionEntries(entries);

        return entries
            .OrderBy(entry => entry.Source, StringComparer.OrdinalIgnoreCase)
            .ThenBy(entry => entry.Category, StringComparer.OrdinalIgnoreCase)
            .ThenBy(entry => entry.SaveKey, StringComparer.OrdinalIgnoreCase)
            .ThenBy(entry => entry.ObjectType, StringComparer.Ordinal)
            .ToList();
    }

    private static List<EnabledModDumpEntry> CollectEnabledMods()
    {
        return ModDatabase.Instance.MarkedForLoad
            .Where(record => record?.Info != null)
            .Select(record => new EnabledModDumpEntry(record))
            .OrderBy(record => record.LoadOrder < 0 ? int.MaxValue : record.LoadOrder)
            .ThenBy(record => record.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(record => record.UniqueIdentifier)
            .ToList();
    }

    private static void AddEntries(List<PrefabDumpEntry> entries, string category, IEnumerable values)
    {
        foreach (object value in values)
        {
            if (value is not UnityEngine.Object unityObject || unityObject == null)
            {
                continue;
            }

            string saveKey = ReadIdentity(unityObject);
            string source = ResolveSource(unityObject, saveKey);
            string baseName = SanitizePathPart(string.IsNullOrWhiteSpace(saveKey) ? unityObject.name : saveKey.Replace('/', '_'));
            string identity = string.Join("|", source, category, saveKey, unityObject.GetType().FullName, unityObject.name);
            string fileName = $"{baseName}-{ComputeFnv1a(identity):x8}.yaml";
            string relativePath = Path.Combine(SanitizePathPart(source), SanitizePathPart(category), fileName);
            entries.Add(new PrefabDumpEntry(category, source, saveKey, unityObject, relativePath));
        }
    }

    private static void AddReferencedSubmunitionEntries(List<PrefabDumpEntry> entries)
    {
        HashSet<int> knownGameObjects = entries
            .Select(entry => GetGameObject(entry.RootObject))
            .Where(gameObject => gameObject != null)
            .Select(gameObject => gameObject!.GetInstanceID())
            .ToHashSet();
        Queue<PrefabDumpEntry> pending = new(entries);

        while (pending.Count > 0)
        {
            PrefabDumpEntry parent = pending.Dequeue();
            GameObject? parentObject = GetGameObject(parent.RootObject);
            if (parentObject == null)
            {
                continue;
            }

            foreach (global::Munitions.SubmunitionWarhead warhead in parentObject.GetComponentsInChildren<global::Munitions.SubmunitionWarhead>(includeInactive: true))
            {
                GameObject? submunitionPrefab = ReadMember(warhead, "_submunitionPrefab") as GameObject;
                if (submunitionPrefab == null || !knownGameObjects.Add(submunitionPrefab.GetInstanceID()))
                {
                    continue;
                }

                string saveKey = ReadGameObjectIdentity(submunitionPrefab);
                const string category = "referenced-munitions";
                string baseName = SanitizePathPart(string.IsNullOrWhiteSpace(saveKey) ? submunitionPrefab.name : saveKey.Replace('/', '_'));
                string identity = string.Join("|", parent.Source, category, saveKey, submunitionPrefab.GetType().FullName, submunitionPrefab.name);
                string fileName = $"{baseName}-{ComputeFnv1a(identity):x8}.yaml";
                string relativePath = Path.Combine(SanitizePathPart(parent.Source), category, fileName);
                PrefabDumpEntry entry = new(category, parent.Source, saveKey, submunitionPrefab, relativePath);
                entries.Add(entry);
                pending.Enqueue(entry);
            }
        }
    }

    private static string ReadGameObjectIdentity(GameObject gameObject)
    {
        foreach (Component component in gameObject.GetComponents<Component>().Where(component => component != null))
        {
            object? identity = ReadMember(component, "SaveKey", "FactionKey", "Key", "_saveKey", "_factionKey");
            if (identity is string text && !string.IsNullOrWhiteSpace(text))
            {
                return text;
            }
        }

        return gameObject.name;
    }

    private static string CreatePrefabYaml(PrefabDumpEntry entry)
    {
        StringBuilder builder = new();
        WriteLine(builder, 0, $"schema_version: {SchemaVersion}");
        WriteLine(builder, 0, $"source: {Quote(entry.Source)}");
        WriteLine(builder, 0, $"category: {Quote(entry.Category)}");
        WriteLine(builder, 0, $"save_key: {Quote(entry.SaveKey)}");
        WriteLine(builder, 0, $"name: {Quote(entry.RootObject.name)}");
        WriteLine(builder, 0, $"type: {Quote(entry.ObjectType)}");

        GameObject? gameObject = GetGameObject(entry.RootObject);
        if (gameObject == null)
        {
            WriteLine(builder, 0, "serialized_fields:");
            WriteSerializedFields(builder, 1, entry.RootObject, null, new HashSet<object>(ReferenceComparer.Instance), 0);
            return builder.ToString();
        }

        WriteLine(builder, 0, "hierarchy:");
        foreach (Transform transform in gameObject.GetComponentsInChildren<Transform>(includeInactive: true))
        {
            GameObject child = transform.gameObject;
            WriteLine(builder, 1, $"- path: {Quote(GetTransformPath(gameObject.transform, transform))}");
            WriteLine(builder, 2, $"name: {Quote(child.name)}");
            WriteLine(builder, 2, $"layer: {child.layer}");
            WriteLine(builder, 2, $"tag: {Quote(GetTag(child))}");
            WriteLine(builder, 2, $"active_self: {FormatBoolean(child.activeSelf)}");
            WriteLine(builder, 2, $"active_in_hierarchy: {FormatBoolean(child.activeInHierarchy)}");
            WriteLine(builder, 2, $"local_position: {FormatVector3(transform.localPosition)}");
            WriteLine(builder, 2, $"local_rotation: {FormatQuaternion(transform.localRotation)}");
            WriteLine(builder, 2, $"local_scale: {FormatVector3(transform.localScale)}");
        }

        WriteLine(builder, 0, "components:");
        foreach (Component component in gameObject.GetComponentsInChildren<Component>(includeInactive: true).Where(component => component != null))
        {
            WriteLine(builder, 1, $"- path: {Quote(GetTransformPath(gameObject.transform, component.transform))}");
            WriteLine(builder, 2, $"type: {Quote(component.GetType().FullName ?? component.GetType().Name)}");
            if (component is Behaviour behaviour)
            {
                WriteLine(builder, 2, $"enabled: {FormatBoolean(behaviour.enabled)}");
            }

            WriteComponentFacts(builder, 2, component);
            WriteLine(builder, 2, "serialized_fields:");
            WriteSerializedFields(builder, 3, component, gameObject.transform, new HashSet<object>(ReferenceComparer.Instance), 0);
        }

        return builder.ToString();
    }

    private static void WriteComponentFacts(StringBuilder builder, int indent, Component component)
    {
        if (component is Collider collider)
        {
            WriteLine(builder, indent, "collider:");
            WriteLine(builder, indent + 1, $"is_trigger: {FormatBoolean(collider.isTrigger)}");
            WriteLine(builder, indent + 1, $"enabled: {FormatBoolean(collider.enabled)}");
            WriteLine(builder, indent + 1, $"bounds_center: {FormatVector3(collider.bounds.center)}");
            WriteLine(builder, indent + 1, $"bounds_size: {FormatVector3(collider.bounds.size)}");

            switch (collider)
            {
                case SphereCollider sphere:
                    WriteLine(builder, indent + 1, $"center: {FormatVector3(sphere.center)}");
                    WriteLine(builder, indent + 1, $"radius: {FormatNumber(sphere.radius)}");
                    break;
                case BoxCollider box:
                    WriteLine(builder, indent + 1, $"center: {FormatVector3(box.center)}");
                    WriteLine(builder, indent + 1, $"size: {FormatVector3(box.size)}");
                    break;
                case CapsuleCollider capsule:
                    WriteLine(builder, indent + 1, $"center: {FormatVector3(capsule.center)}");
                    WriteLine(builder, indent + 1, $"radius: {FormatNumber(capsule.radius)}");
                    WriteLine(builder, indent + 1, $"height: {FormatNumber(capsule.height)}");
                    WriteLine(builder, indent + 1, $"direction: {capsule.direction}");
                    break;
                case MeshCollider mesh:
                    WriteLine(builder, indent + 1, $"convex: {FormatBoolean(mesh.convex)}");
                    WriteLine(builder, indent + 1, $"shared_mesh: {FormatUnityReference(mesh.sharedMesh, null)}");
                    break;
            }
        }

        if (component is Rigidbody rigidbody)
        {
            WriteLine(builder, indent, "rigidbody:");
            WriteLine(builder, indent + 1, $"mass: {FormatNumber(rigidbody.mass)}");
            WriteLine(builder, indent + 1, $"use_gravity: {FormatBoolean(rigidbody.useGravity)}");
            WriteLine(builder, indent + 1, $"is_kinematic: {FormatBoolean(rigidbody.isKinematic)}");
            WriteLine(builder, indent + 1, $"collision_detection: {Quote(rigidbody.collisionDetectionMode.ToString())}");
            WriteLine(builder, indent + 1, $"constraints: {Quote(rigidbody.constraints.ToString())}");
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "One unreadable serialized field must not abort its prefab dump.")]
    private static void WriteSerializedFields(
        StringBuilder builder,
        int indent,
        object value,
        Transform? root,
        HashSet<object> visited,
        int depth)
    {
        List<FieldInfo> fields = GetSerializableFields(value.GetType()).ToList();
        if (fields.Count == 0)
        {
            WriteLine(builder, indent, "$empty: true");
            return;
        }

        foreach (FieldInfo field in fields)
        {
            object? fieldValue;
            try
            {
                fieldValue = field.GetValue(value);
            }
            catch (Exception exception)
            {
                WriteLine(builder, indent, $"{QuoteKey(field.Name)}: {Quote("<read failed: " + exception.Message + ">")}");
                continue;
            }

            WriteValue(builder, indent, field.Name, fieldValue, root, visited, depth);
        }
    }

    private static void WriteValue(
        StringBuilder builder,
        int indent,
        string key,
        object? value,
        Transform? root,
        HashSet<object> visited,
        int depth)
    {
        string yamlKey = QuoteKey(key);
        if (value == null)
        {
            WriteLine(builder, indent, $"{yamlKey}: null");
            return;
        }

        if (TryFormatScalar(value, root, out string scalar))
        {
            WriteLine(builder, indent, $"{yamlKey}: {scalar}");
            return;
        }

        if (depth >= MaximumValueDepth)
        {
            WriteLine(builder, indent, $"{yamlKey}: {Quote("<maximum depth: " + value.GetType().FullName + ">")}");
            return;
        }

        if (value is IDictionary dictionary)
        {
            if (dictionary.Count == 0)
            {
                WriteLine(builder, indent, $"{yamlKey}: {{}}");
                return;
            }

            WriteLine(builder, indent, $"{yamlKey}:");
            int index = 0;
            foreach (DictionaryEntry item in dictionary)
            {
                if (index++ >= MaximumCollectionItems)
                {
                    WriteLine(builder, indent + 1, $"{QuoteKey("<truncated>")}: {dictionary.Count - MaximumCollectionItems}");
                    break;
                }

                WriteValue(builder, indent + 1, Convert.ToString(item.Key, CultureInfo.InvariantCulture) ?? "null", item.Value, root, visited, depth + 1);
            }

            return;
        }

        if (value is IEnumerable enumerable && value is not string)
        {
            List<object?> items = enumerable.Cast<object?>().Take(MaximumCollectionItems + 1).ToList();
            if (items.Count == 0)
            {
                WriteLine(builder, indent, $"{yamlKey}: []");
                return;
            }

            WriteLine(builder, indent, $"{yamlKey}:");
            foreach (object? item in items.Take(MaximumCollectionItems))
            {
                if (item == null)
                {
                    WriteLine(builder, indent + 1, "- null");
                }
                else if (TryFormatScalar(item, root, out string itemScalar))
                {
                    WriteLine(builder, indent + 1, "- " + itemScalar);
                }
                else
                {
                    WriteLine(builder, indent + 1, "- value:");
                    WriteNestedObject(builder, indent + 2, item, root, visited, depth + 1);
                }
            }

            if (items.Count > MaximumCollectionItems)
            {
                WriteLine(builder, indent + 1, $"- {Quote("<truncated after " + MaximumCollectionItems.ToString(CultureInfo.InvariantCulture) + " items>")}");
            }

            return;
        }

        WriteLine(builder, indent, $"{yamlKey}:");
        WriteNestedObject(builder, indent + 1, value, root, visited, depth + 1);
    }

    private static void WriteNestedObject(
        StringBuilder builder,
        int indent,
        object value,
        Transform? root,
        HashSet<object> visited,
        int depth)
    {
        bool trackReference = !value.GetType().IsValueType;
        if (trackReference && !visited.Add(value))
        {
            WriteLine(builder, indent, $"$cycle: {Quote(value.GetType().FullName ?? value.GetType().Name)}");
            return;
        }

        WriteLine(builder, indent, $"$type: {Quote(value.GetType().FullName ?? value.GetType().Name)}");
        WriteSerializedFields(builder, indent, value, root, visited, depth);
        if (trackReference)
        {
            visited.Remove(value);
        }
    }

    private static bool TryFormatScalar(object value, Transform? root, out string result)
    {
        switch (value)
        {
            case string text:
                result = Quote(text);
                return true;
            case char character:
                result = Quote(character.ToString());
                return true;
            case bool boolean:
                result = FormatBoolean(boolean);
                return true;
            case Enum enumValue:
                result = Quote(enumValue.ToString());
                return true;
            case byte or sbyte or short or ushort or int or uint or long or ulong or decimal:
                result = Convert.ToString(value, CultureInfo.InvariantCulture) ?? "0";
                return true;
            case float single:
                result = FormatNumber(single);
                return true;
            case double number:
                result = FormatNumber(number);
                return true;
            case Vector2 vector2:
                result = $"[{FormatNumber(vector2.x)}, {FormatNumber(vector2.y)}]";
                return true;
            case Vector3 vector3:
                result = FormatVector3(vector3);
                return true;
            case Vector4 vector4:
                result = $"[{FormatNumber(vector4.x)}, {FormatNumber(vector4.y)}, {FormatNumber(vector4.z)}, {FormatNumber(vector4.w)}]";
                return true;
            case Quaternion quaternion:
                result = FormatQuaternion(quaternion);
                return true;
            case Color color:
                result = $"[{FormatNumber(color.r)}, {FormatNumber(color.g)}, {FormatNumber(color.b)}, {FormatNumber(color.a)}]";
                return true;
            case Rect rect:
                result = $"{{ x: {FormatNumber(rect.x)}, y: {FormatNumber(rect.y)}, width: {FormatNumber(rect.width)}, height: {FormatNumber(rect.height)} }}";
                return true;
            case Bounds bounds:
                result = $"{{ center: {FormatVector3(bounds.center)}, size: {FormatVector3(bounds.size)} }}";
                return true;
            case LayerMask layerMask:
                result = layerMask.value.ToString(CultureInfo.InvariantCulture);
                return true;
            case UnityEngine.Object unityObject:
                result = FormatUnityReference(unityObject, root);
                return true;
            case Type type:
                result = Quote(type.AssemblyQualifiedName ?? type.FullName ?? type.Name);
                return true;
            default:
                result = "";
                return false;
        }
    }

    private static IEnumerable<FieldInfo> GetSerializableFields(Type objectType)
    {
        Stack<Type> hierarchy = new();
        for (Type type = objectType; type != null && type != typeof(object); type = type.BaseType)
        {
            hierarchy.Push(type);
        }

        while (hierarchy.Count > 0)
        {
            Type type = hierarchy.Pop();
            foreach (FieldInfo field in type.GetFields(InstanceMemberFlags).OrderBy(field => field.Name, StringComparer.Ordinal))
            {
                if (field.IsStatic || field.IsLiteral || field.IsInitOnly || field.IsDefined(typeof(NonSerializedAttribute), inherit: false))
                {
                    continue;
                }

                bool serializeReference = field.CustomAttributes.Any(attribute => attribute.AttributeType.FullName == "UnityEngine.SerializeReference");
                if (field.IsPublic || field.IsDefined(typeof(SerializeField), inherit: false) || serializeReference)
                {
                    yield return field;
                }
            }
        }
    }

    private static string CreateManifestYaml(
        IReadOnlyCollection<PrefabDumpEntry> entries,
        int errors,
        IReadOnlyCollection<EnabledModDumpEntry> enabledMods,
        DateTime generatedUtc)
    {
        StringBuilder builder = new();
        WriteLine(builder, 0, $"schema_version: {SchemaVersion}");
        WriteLine(builder, 0, $"generated_utc: {Quote(generatedUtc.ToString("O", CultureInfo.InvariantCulture))}");
        WriteLine(builder, 0, $"game_version: {Quote(Application.version)}");
        WriteLine(builder, 0, $"agmlib_version: {Quote(typeof(TestingPrefabYamlDumper).Assembly.GetName().Version?.ToString() ?? "unknown")}");
        WriteLine(builder, 0, $"prefab_count: {entries.Count}");
        WriteLine(builder, 0, $"error_count: {errors}");
        WriteLine(builder, 0, "mods_file: 'mods.yaml'");
        WriteLine(builder, 0, $"enabled_mod_count: {enabledMods.Count}");
        WriteLine(builder, 0, "enabled_mods:");
        foreach (EnabledModDumpEntry mod in enabledMods)
        {
            WriteLine(builder, 1, $"- name: {Quote(mod.Name)}");
            WriteLine(builder, 2, $"version: {Quote(mod.Version)}");
            WriteLine(builder, 2, $"unique_id: {Quote(mod.UniqueIdentifier.ToString(CultureInfo.InvariantCulture))}");
            WriteLine(builder, 2, $"source: {Quote(mod.Source)}");
            WriteLine(builder, 2, $"content_directory: {Quote(mod.ContentDirectory)}");
            WriteLine(builder, 2, $"load_order: {mod.LoadOrder}");
            WriteLine(builder, 2, $"status: {Quote(mod.Status)}");
        }

        WriteLine(builder, 0, "sources:");
        foreach (IGrouping<string, PrefabDumpEntry> source in entries.GroupBy(entry => entry.Source).OrderBy(group => group.Key, StringComparer.OrdinalIgnoreCase))
        {
            WriteLine(builder, 1, $"- name: {Quote(source.Key)}");
            WriteLine(builder, 2, $"count: {source.Count()}");
        }

        WriteLine(builder, 0, "categories:");
        IReadOnlyDictionary<string, int> categoryCounts = entries
            .GroupBy(entry => entry.Category, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.OrdinalIgnoreCase);
        foreach (string category in RegistryCategories
            .Concat(categoryCounts.Keys)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(value => value, StringComparer.OrdinalIgnoreCase))
        {
            WriteLine(builder, 1, $"- name: {Quote(category)}");
            WriteLine(builder, 2, $"count: {(categoryCounts.TryGetValue(category, out int count) ? count : 0)}");
        }

        WriteLine(builder, 0, "scalar_collections:");
        WriteLine(builder, 1, "- name: 'tips'");
        WriteLine(builder, 2, $"count: {BundleManager.Instance.Internals().Tips.Count}");

        WriteLine(builder, 0, "files:");
        foreach (PrefabDumpEntry entry in entries)
        {
            WriteLine(builder, 1, $"- path: {Quote(entry.RelativePath.Replace('\\', '/'))}");
            WriteLine(builder, 2, $"source: {Quote(entry.Source)}");
            WriteLine(builder, 2, $"category: {Quote(entry.Category)}");
            WriteLine(builder, 2, $"save_key: {Quote(entry.SaveKey)}");
            if (!string.IsNullOrEmpty(entry.Error))
            {
                WriteLine(builder, 2, "status: 'error'");
            }
        }

        return builder.ToString();
    }

    private static string CreateModsYaml(IReadOnlyCollection<EnabledModDumpEntry> enabledMods, DateTime generatedUtc)
    {
        StringBuilder builder = new();
        WriteLine(builder, 0, $"schema_version: {SchemaVersion}");
        WriteLine(builder, 0, $"generated_utc: {Quote(generatedUtc.ToString("O", CultureInfo.InvariantCulture))}");
        WriteLine(builder, 0, $"game_version: {Quote(Application.version)}");
        WriteLine(builder, 0, $"enabled_mod_count: {enabledMods.Count}");
        WriteLine(builder, 0, "mods:");
        foreach (EnabledModDumpEntry mod in enabledMods)
        {
            WriteLine(builder, 1, $"- name: {Quote(mod.Name)}");
            WriteLine(builder, 2, $"version: {Quote(mod.Version)}");
            WriteLine(builder, 2, $"game_version: {Quote(mod.GameVersion)}");
            WriteLine(builder, 2, $"unique_id: {Quote(mod.UniqueIdentifier.ToString(CultureInfo.InvariantCulture))}");
            WriteLine(builder, 2, mod.WorkshopId.HasValue
                ? $"workshop_id: {Quote(mod.WorkshopId.Value.ToString(CultureInfo.InvariantCulture))}"
                : "workshop_id: null");
            WriteLine(builder, 2, $"source: {Quote(mod.Source)}");
            WriteLine(builder, 2, $"mod_info_path: {Quote(mod.ModInfoPath)}");
            WriteLine(builder, 2, $"content_directory: {Quote(mod.ContentDirectory)}");
            WriteLine(builder, 2, $"load_order: {mod.LoadOrder}");
            WriteLine(builder, 2, $"status: {Quote(mod.Status)}");
            WriteLine(builder, 2, $"loaded: {FormatBoolean(mod.Loaded)}");
            WriteLine(builder, 2, $"missing: {FormatBoolean(mod.Missing)}");
            WriteScalarList(builder, 2, "dependencies", mod.Dependencies.Select(value => value.ToString(CultureInfo.InvariantCulture)));
            WriteScalarList(builder, 2, "assemblies", mod.Assemblies);
            WriteScalarList(builder, 2, "asset_bundles", mod.AssetBundles);
            WriteScalarList(builder, 2, "asset_catalogs", mod.AssetCatalogs);
        }

        return builder.ToString();
    }

    private static void WriteScalarList(StringBuilder builder, int indent, string key, IEnumerable<string> values)
    {
        List<string> items = values.Where(value => !string.IsNullOrWhiteSpace(value)).ToList();
        if (items.Count == 0)
        {
            WriteLine(builder, indent, $"{key}: []");
            return;
        }

        WriteLine(builder, indent, $"{key}:");
        foreach (string item in items)
        {
            WriteLine(builder, indent + 1, $"- {Quote(item)}");
        }
    }

    private static string CreateErrorYaml(PrefabDumpEntry entry)
    {
        StringBuilder builder = new();
        WriteLine(builder, 0, $"schema_version: {SchemaVersion}");
        WriteLine(builder, 0, $"source: {Quote(entry.Source)}");
        WriteLine(builder, 0, $"category: {Quote(entry.Category)}");
        WriteLine(builder, 0, $"save_key: {Quote(entry.SaveKey)}");
        WriteLine(builder, 0, $"name: {Quote(entry.RootObject.name)}");
        WriteLine(builder, 0, $"type: {Quote(entry.ObjectType)}");
        WriteLine(builder, 0, $"error: {Quote(entry.Error ?? "unknown")}");
        return builder.ToString();
    }

    private static void ReplaceDirectoryAtomically(string temporaryPath, string outputPath, string backupPath)
    {
        bool movedPrevious = false;
        try
        {
            if (Directory.Exists(outputPath))
            {
                Directory.Move(outputPath, backupPath);
                movedPrevious = true;
            }

            Directory.Move(temporaryPath, outputPath);
        }
        catch
        {
            if (!Directory.Exists(outputPath) && movedPrevious && Directory.Exists(backupPath))
            {
                Directory.Move(backupPath, outputPath);
            }

            throw;
        }

        if (movedPrevious)
        {
            TryDeleteDirectory(backupPath);
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Best-effort cleanup must not invalidate a completed dump.")]
    private static void TryDeleteDirectory(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
            }
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"[PrefabYamlDump] Could not remove temporary directory '{path}': {exception.Message}");
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Optional mod metadata may be unavailable while the fallback remains valid.")]
    private static string ResolveSource(UnityEngine.Object rootObject, string saveKey)
    {
        object? sourceValue = ReadMember(rootObject, "SourceModId", "_sourceModId");
        if (TryConvertToUInt64(sourceValue, out ulong sourceModId) && sourceModId != 0)
        {
            try
            {
                ModRecord? record = ModDatabase.Instance.GetModByID(sourceModId);
                if (record?.Info != null && !string.IsNullOrWhiteSpace(record.Info.ModName))
                {
                    return record.Info.ModName;
                }
            }
            catch (Exception)
            {
                // Fall back to the stable numeric identifier below.
            }

            return "Mod-" + sourceModId.ToString(CultureInfo.InvariantCulture);
        }

        int separator = saveKey.IndexOf('/');
        if (separator > 0)
        {
            string prefix = saveKey.Substring(0, separator);
            return string.Equals(prefix, "Stock", StringComparison.OrdinalIgnoreCase) ? "Vanilla" : prefix;
        }

        return "Vanilla";
    }

    private static string ReadIdentity(UnityEngine.Object rootObject)
    {
        object? identity = ReadMember(rootObject, "SaveKey", "FactionKey", "Key", "_saveKey", "_factionKey");
        string? text = identity as string;
        return !string.IsNullOrWhiteSpace(text) ? text! : rootObject.name ?? rootObject.GetType().Name;
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Reflection probes optional identity members and must continue to fallbacks.")]
    private static object? ReadMember(object target, params string[] names)
    {
        for (Type type = target.GetType(); type != null; type = type.BaseType)
        {
            foreach (string name in names)
            {
                PropertyInfo property = type.GetProperty(name, InstanceMemberFlags);
                if (property != null && property.GetIndexParameters().Length == 0)
                {
                    try
                    {
                        return property.GetValue(target);
                    }
                    catch (Exception)
                    {
                        // Try the next member name.
                    }
                }

                FieldInfo field = type.GetField(name, InstanceMemberFlags);
                if (field != null)
                {
                    try
                    {
                        return field.GetValue(target);
                    }
                    catch (Exception)
                    {
                        // Try the next member name.
                    }
                }
            }
        }

        return null;
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Diagnostic metadata accepts unknown numeric representations and falls back safely.")]
    private static bool TryConvertToUInt64(object? value, out ulong result)
    {
        try
        {
            if (value != null)
            {
                result = Convert.ToUInt64(value, CultureInfo.InvariantCulture);
                return true;
            }
        }
        catch (Exception)
        {
            // The value is not a numeric mod identifier.
        }

        result = 0;
        return false;
    }

    private static GameObject? GetGameObject(UnityEngine.Object rootObject)
    {
        return rootObject switch
        {
            GameObject gameObject => gameObject,
            Component component => component.gameObject,
            _ => null,
        };
    }

    private static string FormatUnityReference(UnityEngine.Object value, Transform? root)
    {
        if (value == null)
        {
            return "null";
        }

        Transform? referencedTransform = value switch
        {
            GameObject gameObject => gameObject.transform,
            Component component => component.transform,
            _ => null,
        };

        if (root != null && referencedTransform != null && (referencedTransform == root || referencedTransform.IsChildOf(root)))
        {
            string componentSuffix = value is Component component ? "#" + (component.GetType().FullName ?? component.GetType().Name) : "";
            return Quote("$ref:" + GetTransformPath(root, referencedTransform) + componentSuffix);
        }

        return Quote("$ref:" + (value.GetType().FullName ?? value.GetType().Name) + ":" + value.name);
    }

    private static string GetTransformPath(Transform root, Transform target)
    {
        Stack<string> segments = new();
        for (Transform? current = target; current != null; current = current.parent)
        {
            int siblingIndex = current.parent == null ? 0 : current.GetSiblingIndex();
            segments.Push(current.name + "[" + siblingIndex.ToString(CultureInfo.InvariantCulture) + "]");
            if (current == root)
            {
                break;
            }
        }

        return string.Join("/", segments);
    }

    private static string GetTag(GameObject gameObject)
    {
        try
        {
            return gameObject.tag;
        }
        catch (UnityException)
        {
            return "<unavailable>";
        }
    }

    private static uint ComputeFnv1a(string value)
    {
        const uint offsetBasis = 2166136261;
        const uint prime = 16777619;
        uint hash = offsetBasis;
        foreach (byte character in Encoding.UTF8.GetBytes(value))
        {
            hash ^= character;
            hash *= prime;
        }

        return hash;
    }

    private static string SanitizePathPart(string value)
    {
        HashSet<char> invalidCharacters = new(Path.GetInvalidFileNameChars());
        StringBuilder builder = new(value.Length);
        foreach (char character in value)
        {
            builder.Append(invalidCharacters.Contains(character) || character is '/' or '\\' ? '_' : character);
        }

        string result = builder.ToString().Trim().TrimEnd('.');
        if (string.IsNullOrWhiteSpace(result))
        {
            return "unnamed";
        }

        return result.Length <= 100 ? result : result.Substring(0, 100);
    }

    private static string Quote(string value)
    {
        StringBuilder builder = new();
        builder.Append('\'');
        foreach (char character in value ?? "")
        {
            switch (character)
            {
                case '\'':
                    builder.Append("''");
                    break;
                case '\r':
                    builder.Append("\\r");
                    break;
                case '\n':
                    builder.Append("\\n");
                    break;
                case '\t':
                    builder.Append("\\t");
                    break;
                default:
                    if (char.IsControl(character))
                    {
                        builder.Append("\\u").Append(((int)character).ToString("x4", CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        builder.Append(character);
                    }

                    break;
            }
        }

        return builder.Append('\'').ToString();
    }

    private static string QuoteKey(string value)
    {
        return Quote(value);
    }

    private static string FormatBoolean(bool value)
    {
        return value ? "true" : "false";
    }

    private static string FormatNumber(float value)
    {
        if (float.IsNaN(value) || float.IsInfinity(value))
        {
            return Quote(value.ToString(CultureInfo.InvariantCulture));
        }

        return value.ToString("R", CultureInfo.InvariantCulture);
    }

    private static string FormatNumber(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            return Quote(value.ToString(CultureInfo.InvariantCulture));
        }

        return value.ToString("R", CultureInfo.InvariantCulture);
    }

    private static string FormatVector3(Vector3 value)
    {
        return $"[{FormatNumber(value.x)}, {FormatNumber(value.y)}, {FormatNumber(value.z)}]";
    }

    private static string FormatQuaternion(Quaternion value)
    {
        return $"[{FormatNumber(value.x)}, {FormatNumber(value.y)}, {FormatNumber(value.z)}, {FormatNumber(value.w)}]";
    }

    private static void WriteLine(StringBuilder builder, int indent, string value)
    {
        builder.Append(' ', indent * 2).AppendLine(value);
    }

    private sealed class PrefabDumpEntry
    {
        public PrefabDumpEntry(string category, string source, string saveKey, UnityEngine.Object rootObject, string relativePath)
        {
            Category = category;
            Source = source;
            SaveKey = saveKey;
            RootObject = rootObject;
            RelativePath = relativePath;
        }

        public string Category { get; }

        public string Source { get; }

        public string SaveKey { get; }

        public UnityEngine.Object RootObject { get; }

        public string RelativePath { get; }

        public string ObjectType => RootObject.GetType().FullName ?? RootObject.GetType().Name;

        public string? Error { get; set; }
    }

    private sealed class EnabledModDumpEntry
    {
        public EnabledModDumpEntry(ModRecord record)
        {
            ModInfo info = record.Info;
            Name = info.ModName ?? "";
            Version = info.ModVer ?? "";
            GameVersion = info.GameVer ?? "";
            UniqueIdentifier = info.UniqueIdentifier;
            WorkshopId = info.DownloadedFromWorkshop ? info.UniqueIdentifier : null;
            Source = info.DownloadedFromWorkshop ? "workshop" : "local";
            ModInfoPath = info.FileLocation.FullPath ?? "";
            ContentDirectory = info.FileLocation.Directory ?? "";
            LoadOrder = record.LoadOrder;
            Status = record.Status.ToString();
            Loaded = record.Loaded;
            Missing = record.Missing;
            Dependencies = info.Dependencies ?? Array.Empty<ulong>();
            Assemblies = info.Assemblies ?? Array.Empty<string>();
            AssetBundles = info.AssetBundles ?? Array.Empty<string>();
            AssetCatalogs = info.AssetCatalogs ?? Array.Empty<string>();
        }

        public string Name { get; }
        public string Version { get; }
        public string GameVersion { get; }
        public ulong UniqueIdentifier { get; }
        public ulong? WorkshopId { get; }
        public string Source { get; }
        public string ModInfoPath { get; }
        public string ContentDirectory { get; }
        public int LoadOrder { get; }
        public string Status { get; }
        public bool Loaded { get; }
        public bool Missing { get; }
        public IReadOnlyCollection<ulong> Dependencies { get; }
        public IReadOnlyCollection<string> Assemblies { get; }
        public IReadOnlyCollection<string> AssetBundles { get; }
        public IReadOnlyCollection<string> AssetCatalogs { get; }
    }

    private sealed class ReferenceComparer : IEqualityComparer<object>
    {
        public static ReferenceComparer Instance { get; } = new();

        public new bool Equals(object? left, object? right)
        {
            return ReferenceEquals(left, right);
        }

        public int GetHashCode(object value)
        {
            return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(value);
        }
    }
}
