using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Bundles;
using Factions;
using FleetEditor;
using HarmonyLib;
using Munitions.ModularMissiles;
using Munitions.ModularMissiles.Descriptors;
using Ships;
using UnityEngine;

public enum DefaultMissileSocketFillMode
{
    Cheapest,
    KeepEmpty,
    MostExpensive,
}

[DisallowMultipleComponent]
public sealed class DefaultMissileTemplate : MonoBehaviour
{
    [Tooltip("Automatically add this missile body's saved templates, or its built-in defaults when no template exists, to fleets in the Fleet Editor.")]
    public bool AddToFleetByDefault = true;

    [Tooltip("Apply this component's linked template or empty-socket filling mode when the user creates a new missile from this body.")]
    public bool ApplyDefaultsToNewMissiles = true;

    [Tooltip("Optional exported missile template containing the default socket sizes, installed components, and component settings. Import a copy as an .xml or .txt TextAsset so Unity can link it here.")]
    public TextAsset? TemplateFile;

    [Tooltip("How empty, non-fixed sockets are filled when no template file is linked.")]
    public DefaultMissileSocketFillMode EmptySocketFillMode = DefaultMissileSocketFillMode.Cheapest;
}

internal static class DefaultMissileTemplateLoader
{
    // First-draft behavior is global. Change this to false when default missiles
    // should become opt-in through DefaultMissileTemplate.
    public static bool IncludeUnflaggedBodies { get; set; } = false;

    public static void AddDefaultsToFleet(Fleet fleet)
    {
        if (fleet?.AvailableMunitions == null)
        {
            return;
        }

        AttachToUnflaggedBodies();

        int linkedTemplatesAdded = AddLinkedTemplates(fleet);
        int templatesAdded = AddSavedTemplates(fleet);
        int bodyDefaultsAdded = AddMissingBodyDefaults(fleet);

        if (linkedTemplatesAdded > 0 || templatesAdded > 0 || bodyDefaultsAdded > 0)
        {
            Debug.Log($"Added {linkedTemplatesAdded} linked missile template(s), {templatesAdded} saved missile template(s), and {bodyDefaultsAdded} built-in missile default(s) to fleet {fleet.FleetName}.");
        }
    }

    private static int AddLinkedTemplates(Fleet fleet)
    {
        int added = 0;
        ModularMissile[] bodies = BundleManager.Instance.AllMissileBodies.ToArray();

        foreach (ModularMissile body in bodies)
        {
            if (body == null)
            {
                continue;
            }

            DefaultMissileTemplate? defaultTemplate = GetDefaultTemplate(body);
            if (defaultTemplate == null ||
                !defaultTemplate.AddToFleetByDefault ||
                defaultTemplate.TemplateFile == null ||
                !body.UseableByFaction(fleet.Faction))
            {
                continue;
            }

            SerializedMissileTemplate? template = ReadLinkedTemplate(defaultTemplate.TemplateFile, body);
            if (template == null || fleet.AvailableMunitions.MissileTypeExists(template))
            {
                continue;
            }

            ModularMissile missile = fleet.AvailableMunitions.AddMissileType(template, checkValidity: false);
            if (missile == null)
            {
                Debug.LogError($"Failed to build linked missile template {defaultTemplate.TemplateFile.name} for body {body.BodyKey}.");
                continue;
            }

            missile.EnableStatCalculationTable();
            added++;
        }

        return added;
    }

    private static SerializedMissileTemplate? ReadLinkedTemplate(TextAsset templateFile, ModularMissile body)
    {
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SerializedMissileTemplate));
            using StringReader reader = new StringReader(templateFile.text);
            XmlReaderSettings settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
            };
            using XmlReader xmlReader = XmlReader.Create(reader, settings);
            SerializedMissileTemplate? template = serializer.Deserialize(xmlReader) as SerializedMissileTemplate;

            if (template == null)
            {
                Debug.LogError($"Linked missile template {templateFile.name} did not contain a MissileTemplate document.");
                return null;
            }

            if (template.TemplateKey == Guid.Empty)
            {
                Debug.LogError($"Linked missile template {templateFile.name} has an empty TemplateKey.");
                return null;
            }

            if (template.Sockets == null)
            {
                Debug.LogError($"Linked missile template {templateFile.name} has no socket configuration.");
                return null;
            }

            template.BodyKey = body.BodyKey;
            template.AssociatedTemplate = null;
            template.AssociatedTemplateName = null;

            if (string.IsNullOrEmpty(template.SaveKey))
            {
                template.SaveKey = ModularMissile.MakeSaveKey(template.TemplateKey);
            }

            return template;
        }
        catch (InvalidOperationException exception)
        {
            Debug.LogError($"Failed to read linked missile template {templateFile.name} for body {body.BodyKey}: {exception}");
            return null;
        }
    }

    public static bool TryCreateNewMissileFromLinkedTemplate(
        AvailableMunitionsSet availableMunitions,
        Fleet fleet,
        ModularMissile body,
        out ModularMissile? missile)
    {
        missile = null;
        DefaultMissileTemplate? defaultTemplate = GetDefaultTemplate(body);
        if (defaultTemplate == null ||
            !defaultTemplate.ApplyDefaultsToNewMissiles ||
            defaultTemplate.TemplateFile == null)
        {
            return false;
        }

        SerializedMissileTemplate? template = ReadLinkedTemplate(defaultTemplate.TemplateFile, body);
        if (template == null)
        {
            return false;
        }

        template.TemplateKey = Guid.NewGuid();
        template.SaveKey = null;
        missile = availableMunitions.AddMissileType(template, checkValidity: false);

        if (missile == null)
        {
            Debug.LogError($"Failed to create a new missile from linked template {defaultTemplate.TemplateFile.name} for body {body.BodyKey} in fleet {fleet.FleetName}.");
            return false;
        }

        return true;
    }

    public static void ApplyDefaultsToNewMissile(
        ModularMissile body,
        ModularMissile missile,
        FactionDescription faction)
    {
        DefaultMissileTemplate? defaultTemplate = GetDefaultTemplate(body);
        if (defaultTemplate == null ||
            !defaultTemplate.ApplyDefaultsToNewMissiles ||
            defaultTemplate.TemplateFile != null)
        {
            return;
        }

        FillEmptySockets(missile, faction, defaultTemplate.EmptySocketFillMode);
    }

    private static int AddSavedTemplates(Fleet fleet)
    {
        int added = 0;
        MissileTemplate[] templates = fleet.AvailableMunitions.AllMissileTemplates.ToArray();

        foreach (MissileTemplate template in templates)
        {
            if (template?.MissileBody == null ||
                !ShouldAdd(template.MissileBody) ||
                fleet.AvailableMunitions.MissileTypeExists(template.Missile))
            {
                continue;
            }

            ModularMissile missile = fleet.AvailableMunitions.AddMissileType(template.Missile, checkValidity: false);
            if (missile == null)
            {
                continue;
            }

            missile.EnableStatCalculationTable();
            added++;
        }

        return added;
    }

    private static int AddMissingBodyDefaults(Fleet fleet)
    {
        int added = 0;
        ModularMissile[] bodies = BundleManager.Instance.AllMissileBodies.ToArray();

        foreach (ModularMissile body in bodies)
        {
            if (body == null ||
                !body.UseableByFaction(fleet.Faction) ||
                !ShouldAdd(body) ||
                fleet.AvailableMunitions.AllMissileTypes.Any(missile => missile.BodyKey == body.BodyKey))
            {
                continue;
            }

            try
            {
                ModularMissile missile = fleet.AvailableMunitions.CreateNewMissile(body);
                FillEmptySockets(missile, fleet.Faction, GetSocketFillMode(body));
                missile.EnableStatCalculationTable();
                added++;
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to add the built-in default for missile body {body.BodyKey}: {exception}");
            }
        }

        return added;
    }

    private static DefaultMissileSocketFillMode GetSocketFillMode(ModularMissile body)
    {
        DefaultMissileTemplate? defaultTemplate = GetDefaultTemplate(body);
        if (defaultTemplate?.TemplateFile != null)
        {
            return DefaultMissileSocketFillMode.KeepEmpty;
        }

        return defaultTemplate?.EmptySocketFillMode ?? DefaultMissileSocketFillMode.Cheapest;
    }

    private static void FillEmptySockets(
        ModularMissile missile,
        FactionDescription faction,
        DefaultMissileSocketFillMode fillMode)
    {
        if (fillMode == DefaultMissileSocketFillMode.KeepEmpty)
        {
            return;
        }

        MissileComponentDescriptor[] components = BundleManager.Instance.AllMissileComponents.ToArray();

        foreach (MissileSocket socket in missile.Sockets)
        {
            if (socket.Fixed || socket.Installed != null)
            {
                continue;
            }

            IOrderedEnumerable<MissileComponentDescriptor> rankedComponents =
                fillMode == DefaultMissileSocketFillMode.MostExpensive
                    ? components
                        .Where(component => socket.ComponentPermitted(component, faction))
                        .OrderByDescending(component => component.PointCost)
                    : components
                        .Where(component => socket.ComponentPermitted(component, faction))
                        .OrderBy(component => component.PointCost);

            MissileComponentDescriptor? selected = rankedComponents
                .ThenBy(component => component.SaveKey, StringComparer.Ordinal)
                .FirstOrDefault();

            if (selected != null && !missile.InstallSocketModule(socket, selected))
            {
                Debug.LogWarning($"Failed to install default component {selected.SaveKey} in socket {socket.SocketID} on missile body {missile.BodyKey}.");
            }
        }
    }

    private static bool ShouldAdd(ModularMissile body)
    {
        return GetDefaultTemplate(body)?.AddToFleetByDefault ?? false;
    }

    private static void AttachToUnflaggedBodies()
    {
        if (!IncludeUnflaggedBodies)
        {
            return;
        }

        foreach (ModularMissile body in BundleManager.Instance.AllMissileBodies)
        {
            GetDefaultTemplate(body);
        }
    }

    private static DefaultMissileTemplate? GetDefaultTemplate(ModularMissile body)
    {
        if (body == null)
        {
            return null;
        }

        DefaultMissileTemplate? defaultTemplate = body.GetComponent<DefaultMissileTemplate>();
        if (defaultTemplate == null && IncludeUnflaggedBodies)
        {
            defaultTemplate = body.gameObject.AddComponent<DefaultMissileTemplate>();
        }

        return defaultTemplate;
    }
}

[HarmonyPatch(typeof(AvailableMunitionsSet), nameof(AvailableMunitionsSet.CreateNewMissile))]
internal static class ApplyDefaultMissileTemplateOnCreatePatch
{
    private static bool Prefix(
        AvailableMunitionsSet __instance,
        ModularMissile bodyPrefab,
        Fleet ____fleet,
        ref ModularMissile __result)
    {
        if (DefaultMissileTemplateLoader.TryCreateNewMissileFromLinkedTemplate(
                __instance,
                ____fleet,
                bodyPrefab,
                out ModularMissile? missile))
        {
            __result = missile!;
            return false;
        }

        return true;
    }

    private static void Postfix(
        ModularMissile bodyPrefab,
        Fleet ____fleet,
        ModularMissile __result)
    {
        if (__result != null)
        {
            DefaultMissileTemplateLoader.ApplyDefaultsToNewMissile(
                bodyPrefab,
                __result,
                ____fleet.Faction);
        }
    }
}

[HarmonyPatch(typeof(MissileEditorSubmodeController), nameof(MissileEditorSubmodeController.OnFleetChanged))]
internal static class AddDefaultMissileTemplatesOnFleetChangedPatch
{
    private static void Postfix(Fleet newFleet)
    {
        DefaultMissileTemplateLoader.AddDefaultsToFleet(newFleet);
    }
}
