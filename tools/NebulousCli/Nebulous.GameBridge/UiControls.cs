using TMPro;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NebulousCli;

internal class UiControls
{
    private static readonly Vector2[] HitOffsets = [Vector2.zero, new(-0.35f, 0), new(0.35f, 0), new(0, -0.35f), new(0, 0.35f)];
    private string? _snapshot;
    private Scene _scene;
    private BaseMenu? _menu;
    private readonly List<(Selectable Control, ControlInfo Info)> _controls = [];
    private readonly List<RaycastResult> _hits = [];

    internal BridgeReply Inspect(GameStatus status)
    {
        Canvas.ForceUpdateCanvases();
        _scene = SceneManager.GetActiveScene();
        _menu = TopMenu(_scene);
        _controls.Clear();
        _snapshot = Guid.NewGuid().ToString("N");

        foreach (var control in UnityEngine.Object.FindObjectsOfType<Selectable>().OrderBy(control => control.GetInstanceID()))
        {
            var info = Describe(control, (uint)_controls.Count + 1, out _);
            if (info != null)
                _controls.Add((control, info));
        }

        return new() { Status = status, Snapshot = _snapshot, Controls = _controls.Select(entry => entry.Info).ToArray() };
    }

    internal BridgeReply Act(BridgeRequest request)
    {
        Canvas.ForceUpdateCanvases();
        if (_snapshot == null || _snapshot != request.Snapshot || _scene != SceneManager.GetActiveScene() ||
            !ReferenceEquals(_menu, TopMenu(_scene)) || request.Id == 0 || request.Id > _controls.Count)
        {
            throw new InvalidOperationException("The control snapshot is stale or the ID is invalid. Run inspect again.");
        }

        var (control, original) = _controls[(int)request.Id - 1];
        var current = Describe(control, request.Id, out var pointer);
        if (current == null || current.Path != original.Path || current.Label != original.Label ||
            current.Value != original.Value || current.Checked != original.Checked)
        {
            throw new InvalidOperationException("The control changed since inspection. Run inspect again.");
        }

        if (pointer == null)
            throw new InvalidOperationException("The control is disabled, read-only or covered by another UI element.");

        switch (request.Command, control)
        {
            case ("click", Button or Toggle):
                ConsumeSnapshot();
                ExecuteEvents.Execute(control.gameObject, pointer, ExecuteEvents.pointerClickHandler);
                return new() { Message = $"Clicked {ControlName(current)}." };
            case ("set-text", TMP_InputField or InputField) when request.Value?.Length <= BridgeProtocol.MaxTextLength:
                ConsumeSnapshot();
                if (control is TMP_InputField tmp)
                    tmp.text = request.Value;
                else
                    ((InputField)control).text = request.Value;

                return new() { Message = $"Updated {ControlName(current)}." };
            default:
                throw new InvalidOperationException("Use click on a button or toggle, or set-text on a text field.");
        }
    }

    private ControlInfo? Describe(Selectable control, uint id, out PointerEventData? pointer)
    {
        pointer = null;
        if (control == null || !control.isActiveAndEnabled || control.transform is not RectTransform rect ||
            rect.rect.width <= 0 || rect.rect.height <= 0)
        {
            return null;
        }

        var canvas = control.GetComponentInParent<Canvas>();
        if (canvas == null || !canvas.isActiveAndEnabled ||
            (control.targetGraphic != null && control.targetGraphic.canvasRenderer.GetInheritedAlpha() <= 0f))
        {
            return null;
        }

        var raycaster = control.GetComponentInParent<GraphicRaycaster>();
        var camera = raycaster != null ? raycaster.eventCamera : null;
        if (!TryGetScreenPoint(rect, camera, Vector2.zero, out _))
            return null;

        var type = control switch
        {
            Button => "button",
            Toggle => "toggle",
            TMP_InputField tmp when tmp.inputType != TMP_InputField.InputType.Password => "input",
            InputField input when input.inputType != InputField.InputType.Password => "input",
            _ => null
        };
        if (type == null)
            return null;

        var label = control switch
        {
            TMP_InputField tmp => tmp.placeholder is TMP_Text placeholder ? ParsedLabel(placeholder) : null,
            InputField input => (input.placeholder as Text)?.text,
            _ => control.GetComponentsInChildren<TMP_Text>().Select(ParsedLabel)
                     .FirstOrDefault(text => text.Length != 0)
                 ?? control.GetComponentsInChildren<Text>().Select(text => text.text.Trim())
                     .FirstOrDefault(text => text.Length != 0)
        };

        var readOnly = control is TMP_InputField { readOnly: true } or InputField { readOnly: true };
        if (!readOnly && control.IsInteractable())
            pointer = FindPointer(control, rect, camera);

        return new()
        {
            Id = id,
            Type = type,
            Path = HierarchyPath(control.transform),
            Label = label ?? "",
            Value = control switch { TMP_InputField tmp => tmp.text, InputField input => input.text, _ => null },
            Checked = control is Toggle toggle ? toggle.isOn : null,
            Interactable = pointer != null
        };
    }

    private bool CanClick(Selectable control, PointerEventData pointer)
    {
        EventSystem.current.RaycastAll(pointer, _hits);
        if (_hits.Count == 0 || ExecuteEvents.GetEventHandler<IPointerClickHandler>(_hits[0].gameObject) != control.gameObject)
            return false;

        pointer.pointerCurrentRaycast = _hits[0];
        return true;
    }

    private PointerEventData? FindPointer(Selectable control, RectTransform rect, Camera? camera)
    {
        var eventSystem = EventSystem.current;
        if (eventSystem == null)
            return null;

        var pointer = new PointerEventData(eventSystem) { button = PointerEventData.InputButton.Left, clickCount = 1 };
        foreach (var offset in HitOffsets)
        {
            if (!TryGetScreenPoint(rect, camera, offset, out var point))
                continue;
            pointer.position = point;
            if (CanClick(control, pointer))
                return pointer;
        }

        return null;
    }

    private static bool TryGetScreenPoint(RectTransform rect, Camera? camera, Vector2 offset, out Vector2 point)
    {
        var world = rect.TransformPoint(rect.rect.center + Vector2.Scale(rect.rect.size, offset));
        var screen = camera != null ? camera.WorldToScreenPoint(world) : world;
        point = screen;
        return screen.z >= 0 && point.x >= 0 && point.y >= 0 && point.x < Screen.width && point.y < Screen.height;
    }

    private static string ParsedLabel(TMP_Text text) => text.GetParsedText().Trim();
    private static string HierarchyPath(Transform transform) => transform.parent == null
        ? transform.name
        : $"{HierarchyPath(transform.parent)}/{transform.name}";

    private static BaseMenu? TopMenu(Scene scene) => MenuController.SceneInstance(scene)?.GetTopMenu();
    private static string ControlName(ControlInfo control) => control.Label.Length != 0 ? control.Label : control.Path;

    private void ConsumeSnapshot()
    {
        _snapshot = null;
        _controls.Clear();
    }
}
