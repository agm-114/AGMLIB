using Mirror;

namespace Lib.Diagnostics;

internal class SalvoDebugWindow : MonoBehaviour
{
    [Serializable]
    internal class Export(List<SalvoTrace> salvos)
    {
        public int SchemaVersion = 2;
        public string GameVersion = Application.version;
        public string GameAssembly = typeof(ActiveMissileSalvo).Assembly.GetName().Version.ToString();
        public string AgmlibAssembly = typeof(SalvoDebugWindow).Assembly.GetName().Version.ToString();
        public string CapturedUtc = DateTime.UtcNow.ToString("O");
        public List<SalvoTrace> Salvos = salvos;
    }

    public bool Visible;
    internal BeamRayTrace? SelectedRay { get; private set; }
    private readonly List<SalvoTrace> _selection = [];
    private MissileTrace? _missile;
    private Rect _window;
    private Vector2 _historyScroll;
    private Vector2 _missileScroll;
    private Vector2 _rayScroll;
    private Vector2 _detailScroll;
    private int _detailTab;
    private bool _showPaths = true;
    private bool _showInternal = true;
    private bool _showHistory = true;
    private string _message = "";
    private SalvoDebugWorldView? _world;
    private GUIStyle? _text;
    private GUIStyle? _button;
    private GUIStyle? _heading;
    private GUIStyle? _stat;
    private GameObject? _blocker;
    private RectTransform? _blockerRect;
    private IEnumerable<MissileTrace> Missiles => _selection.SelectMany(salvo => salvo.Missiles);
    private IEnumerable<MissileTrace> ShownMissiles => _missile == null ? Missiles : [_missile];

    public void Select(IEnumerable<SalvoTrace> salvos)
    {
        ClearSelection();
        _selection.AddRange(salvos);
        Visible = true;
        _showHistory = _selection.Count == 0;
        _message = _showHistory ? "No trace was retained for this salvo." : "";
    }

    public void ClearSelection()
    {
        _selection.Clear();
        _missile = null;
        SelectedRay = null;
        _detailScroll = _missileScroll = _rayScroll = Vector2.zero;
        _detailTab = 0;
        _message = "";
        _world?.Clear();
    }

    internal void Pin(BeamRayTrace ray)
    {
        if (SelectedRay == ray)
            return;
        SelectedRay = ray;
        _detailScroll = Vector2.zero;
    }

    internal void Inspect(BeamRayTrace? ray)
    {
        if (ray != null)
            Pin(ray);
    }

    internal void FocusImpact()
    {
        if (SelectedRay is not BeamRayTrace ray || FindObjectOfType<global::Cameras.ImprovedOrbitCamera>() is not { } camera)
            return;
        float radius = ray.HasTargetBounds ? Mathf.Max(ray.TargetBounds.extents.magnitude, 1f) : 10f;
        camera.TweenToPosition(ray.End, 0.25f);
        camera.ZoomAbsolute(Mathf.Max(radius * 3f, 5f));
    }

    private void Update()
    {
        if (!Visible)
        {
            _world?.Hide();
            if (_blocker != null)
                _blocker.SetActive(false);
            return;
        }
        _world ??= gameObject.AddComponent<SalvoDebugWorldView>();
        _world.Sync(ShownMissiles.ToList(), SelectedRay, _showPaths, _showInternal);
        if (_blocker == null)
        {
            _blocker = new GameObject("Salvo Debug Input Blocker", typeof(Canvas), typeof(GraphicRaycaster));
            _blocker.transform.SetParent(transform, false);
            Canvas canvas = _blocker.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = short.MaxValue;
            GameObject panel = new("Window", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(_blocker.transform, false);
            panel.GetComponent<Image>().color = Color.clear;
            _blockerRect = panel.GetComponent<RectTransform>();
            _blockerRect.anchorMin = _blockerRect.anchorMax = new Vector2(0, 1);
            _blockerRect.pivot = new Vector2(0, 1);
        }
        _blocker.SetActive(true);
        _blockerRect!.anchoredPosition = new Vector2(_window.x, -_window.y);
        _blockerRect.sizeDelta = _window.size;
    }

    private void OnGUI()
    {
        if (!Visible || SalvoDebugRecorder.Instance == null)
            return;
        _text ??= new GUIStyle(GUI.skin.label) { wordWrap = true, richText = false, fontSize = 12 };
        _button ??= new GUIStyle(GUI.skin.button) { wordWrap = true, richText = false, fontSize = 12 };
        _heading ??= new GUIStyle(_text) { fontSize = 14 };
        _stat ??= new GUIStyle(_text) { fontSize = 18 };
        if (_window.width == 0)
            _window = new Rect(Mathf.Max(0, Screen.width - 500), 20, Mathf.Min(Screen.width - 40, 480), Mathf.Min(Screen.height - 40, 940));
        _window.width = Mathf.Min(_window.width, Screen.width);
        _window.height = Mathf.Min(_window.height, Screen.height);
        _window.x = Mathf.Clamp(_window.x, 0, Mathf.Max(0, Screen.width - _window.width));
        _window.y = Mathf.Clamp(_window.y, 0, Mathf.Max(0, Screen.height - _window.height));
        if (!_window.Contains(Event.current.mousePosition))
            Inspect(_world?.Pick(Event.current.mousePosition));
        _window = GUI.Window(GetInstanceID(), _window, DrawWindow, "AGMLIB / Salvo inspector");
    }

    private void DrawWindow(int id)
    {
        Fill(new Rect(2, 25, _window.width - 4, _window.height - 27), new Color(0.065f, 0.075f, 0.095f));
        SalvoDebugRecorder recorder = SalvoDebugRecorder.Instance!;
        if (GUI.Button(new Rect(_window.width - 32, 2, 28, 22), "X"))
            Visible = false;
        bool capture = GUI.Toggle(new Rect(14, 32, 210, 25), recorder.Recording, "Record local salvos");
        if (capture != recorder.Recording)
            recorder.SetRecording(capture);
        GUI.Label(new Rect(_window.width - 130, 33, 115, 22), NetworkServer.active ? "LOCAL HOST" : "HOST REQUIRED", _text!);
        _showPaths = GUI.Toggle(new Rect(14, 65, 115, 25), _showPaths, "Flight paths");
        _showInternal = GUI.Toggle(new Rect(140, 65, 130, 25), _showInternal, "Internal / spall");
        bool previousEnabled = GUI.enabled;
        GUI.enabled = SelectedRay != null;
        if (GUI.Button(new Rect(_window.width - 122, 62, 108, 28), "Focus hit", _button!))
            FocusImpact();
        GUI.enabled = previousEnabled;
        int page = GUI.Toolbar(new Rect(14, 101, _window.width - 28, 28), _showHistory ? 1 : 0,
            ["Inspect", $"History ({recorder.History.Count})"], _button!);
        _showHistory = page == 1;
        Rect content = new(14, 140, _window.width - 28, _window.height - 210);
        if (_showHistory)
        {
            GUI.Label(new Rect(content.x, content.y, content.width - 120, 25), "RECORDED SALVOS", _heading!);
            if (GUI.Button(new Rect(content.xMax - 108, content.y, 108, 25), "Clear history", _button!))
                recorder.Clear();
            content.yMin += 34;
            DrawHistory(content, recorder);
        }
        else
            DrawDetails(content);
        Fill(new Rect(14, _window.height - 64, _window.width - 28, 1), new Color(0.22f, 0.26f, 0.31f));
        Legend(new Rect(14, _window.height - 56, 105, 18), Color.green, "Damage");
        Legend(new Rect(125, _window.height - 56, 150, 18), new Color(1, 0.65f, 0.2f), "Blocked / zero");
        Legend(new Rect(285, _window.height - 56, 100, 18), Color.gray, "Miss");
        string status = recorder.DroppedMissiles > 0 || (_world?.OmittedLines ?? 0) > 0
            ? $"{recorder.DroppedMissiles} uncaptured missiles / {_world?.OmittedLines ?? 0} hidden lines"
            : "Selection stays pinned / Ctrl+F8 to hide";
        GUI.Label(new Rect(14, _window.height - 33, _window.width - 28, 24), string.IsNullOrEmpty(_message) ? status : _message, _text!);
        GUI.DragWindow(new Rect(0, 0, _window.width - 40, 25));
    }

    private static void Fill(Rect rect, Color color)
    {
        Color previous = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = previous;
    }

    private void Legend(Rect rect, Color color, string text)
    {
        Fill(new Rect(rect.x, rect.y + 5, 7, 7), color);
        GUI.Label(new Rect(rect.x + 14, rect.y, rect.width - 14, rect.height), text, _text!);
    }

    private void DrawHistory(Rect rect, SalvoDebugRecorder recorder)
    {
        GUI.Box(rect, GUIContent.none);
        _historyScroll = GUI.BeginScrollView(rect, _historyScroll, new Rect(0, 0, rect.width - 20, recorder.History.Count * 66 + 30));
        float y = 4;
        for (int i = recorder.History.Count - 1; i >= 0; i--)
        {
            SalvoTrace salvo = recorder.History[i];
            string title = $"{salvo.Title}\n{salvo.Missiles.Count} missiles / {(salvo.Finished ? "finished" : "in flight")} / {salvo.Damage:N1} traced HP";
            if (GUI.Button(new Rect(4, y, rect.width - 28, 62), title, _button!))
                Select([salvo]);
            y += 66;
        }
        GUI.EndScrollView();
    }

    private void DrawDetails(Rect rect)
    {
        if (_selection.Count == 0)
        {
            GUI.Label(new Rect(rect.x + 8, rect.y + 12, rect.width - 16, 90),
                "Choose a recorded salvo from History, or click a finished salvo in the battle HUD.\n\nHover a world-space ray to inspect it; its selection stays when you move away.", _text!);
            return;
        }
        GUI.Label(new Rect(rect.x, rect.y, rect.width, 25),
            _selection.Count == 1 ? _selection[0].Title : $"{_selection.Count} selected salvos", _heading!);
        List<MissileTrace> missiles = Missiles.ToList();
        string[] choices = new[] { $"All ({missiles.Count})" }.Concat(missiles.Select(item => $"M{item.MissileId} / {item.Name}")).ToArray();
        int currentMissile = _missile == null ? 0 : missiles.IndexOf(_missile) + 1;
        float missileHeight = Mathf.Min(Mathf.Ceil(choices.Length / 3f) * 29, 87);
        Rect missileList = new(rect.x, rect.y + 31, rect.width, missileHeight);
        _missileScroll = GUI.BeginScrollView(missileList, _missileScroll,
            new Rect(0, 0, missileList.width - 20, Mathf.Ceil(choices.Length / 3f) * 29));
        int pickedMissile = GUI.SelectionGrid(new Rect(0, 0, missileList.width - 22, Mathf.Ceil(choices.Length / 3f) * 29),
            currentMissile, choices, 3, _button!);
        if (pickedMissile != currentMissile)
        {
            _missile = pickedMissile == 0 ? null : missiles[pickedMissile - 1];
            if (_missile != null && (SelectedRay == null || !_missile.Rays.Contains(SelectedRay)))
                SelectedRay = _missile.Rays.FirstOrDefault();
            _rayScroll = _detailScroll = Vector2.zero;
        }
        GUI.EndScrollView();
        List<BeamRayTrace> rays = ShownMissiles.SelectMany(missile => missile.Rays).ToList();
        float rayHeader = missileList.yMax + 12;
        GUI.Label(new Rect(rect.x, rayHeader, rect.width, 23), $"RAYS / {rays.Count} recorded", _heading!);
        float rayHeight = Mathf.Min(rays.Count * 29 + 3, Mathf.Max(90, rect.height * 0.23f));
        Rect rayList = new(rect.x, rayHeader + 27, rect.width, rayHeight);
        _rayScroll = GUI.BeginScrollView(rayList, _rayScroll, new Rect(0, 0, rayList.width - 20, rays.Count * 29));
        for (int i = 0; i < rays.Count; i++)
        {
            BeamRayTrace item = rays[i];
            Rect row = new(0, i * 29, rayList.width - 22, 27);
            Color previous = GUI.backgroundColor;
            if (item == SelectedRay)
                GUI.backgroundColor = new Color(0.3f, 0.65f, 0.85f);
            if (GUI.Button(row, GUIContent.none, _button!))
                Pin(item);
            GUI.backgroundColor = previous;
            Color color = item.Outcome == RayDamageOutcome.Damage ? Color.green : item.Outcome == RayDamageOutcome.NoDamage ? new Color(1, 0.65f, 0.2f) : Color.gray;
            Fill(new Rect(8, row.y + 10, 7, 7), color);
            GUI.Label(new Rect(24, row.y + 4, 135, 22), $"Ray {item.RayIndex:00} / M{item.MissileId}", _text!);
            GUI.Label(new Rect(160, row.y + 4, 120, 22), item.Hit ? item.Result.ToString() : "Miss", _text!);
            GUI.Label(new Rect(row.width - 105, row.y + 4, 100, 22), $"{item.ReportedDamage:N1} HP", _text!);
        }
        GUI.EndScrollView();
        Rect card = new(rect.x, rayList.yMax + 12, rect.width, 63);
        Fill(card, new Color(0.1f, 0.13f, 0.17f));
        GUI.Label(new Rect(card.x + 10, card.y + 7, card.width - 120, 22),
            SelectedRay is { } selected ? $"SELECTED / M{selected.MissileId} / RAY {selected.RayIndex}" : "HOVER OR SELECT A RAY", _text!);
        GUI.Label(new Rect(card.x + 10, card.y + 28, card.width - 120, 28),
            SelectedRay is { } selectedRay ? $"{selectedRay.ReportedDamage:N1} HP  /  {selectedRay.Result}" : "No ray selected", _stat!);
        if (GUI.Button(new Rect(card.xMax - 108, card.y + 17, 98, 28), "Export JSON", _button!))
            ExportSelection();
        int tab = GUI.Toolbar(new Rect(rect.x, card.yMax + 8, rect.width, 27), _detailTab,
            ["Damage", $"Steps ({SelectedRay?.Steps.Count ?? 0})", "Settings"], _button!);
        if (tab != _detailTab)
        {
            _detailTab = tab;
            _detailScroll = Vector2.zero;
        }
        string details = DetailText();
        Rect detailRect = new(rect.x, card.yMax + 45, rect.width, rect.yMax - card.yMax - 45);
        float textHeight = _text!.CalcHeight(new GUIContent(details), detailRect.width - 22);
        _detailScroll = GUI.BeginScrollView(detailRect, _detailScroll, new Rect(0, 0, detailRect.width - 20, textHeight + 10));
        GUI.Label(new Rect(0, 0, detailRect.width - 22, textHeight), details, _text);
        GUI.EndScrollView();
    }

    private string DetailText()
    {
        if (SelectedRay is not { } ray)
            return "Hover a world-space ray or select a row above. Selection remains when the pointer leaves.";
        if (_detailTab == 1)
            return string.Join("\n\n", ray.Steps.Select((step, i) => $"{i + 1}. {step.Describe()}")) +
                (ray.DroppedSteps > 0 ? $"\n\n{ray.DroppedSteps} steps omitted by the capture limit." : "");
        if (_detailTab == 2)
            return $"Damager\n{ray.Damager}\n\nCollider\n{ray.Collider}\n\n" + ray.Settings.Replace("; ", "\n");
        return $"Target\n{ray.Target}\n\n" +
            $"Range\n{ray.Range * 10f:N1} m / curve input {ray.Range:N3} world units\nSocket weight: {ray.SocketSize:N3}\n\n" +
            $"Component damage curve\n{ray.SizeDamage:N3} x {ray.RangeDamageMultiplier:N4} = {ray.SizeDamage * ray.RangeDamageMultiplier:N3} HP\n\n" +
            $"Penetration curve\n{ray.SizePenetration:N3} x {ray.RangePenetrationMultiplier:N4} = {ray.SizePenetration * ray.RangePenetrationMultiplier:N3} cm\n\n" +
            $"Dealer characteristics\n{ray.ComponentDamage:N3} HP / {ray.ArmorPenetration:N3} cm\n\n" +
            $"Reported damage: {ray.ReportedDamage:N3} HP\nTarget destroyed: {ray.Destroyed}\nCapture: {(ray.Completed ? "Complete" : "Incomplete")}";
    }

    internal string? ExportSelection()
    {
        if (_selection.Count == 0)
        {
            _message = "Select a recorded salvo before exporting.";
            return null;
        }
        try
        {
            string directory = Path.Combine(Application.persistentDataPath, "AGMLIB", "SalvoTraces");
            Directory.CreateDirectory(directory);
            string path = Path.Combine(directory, $"salvo-{DateTime.UtcNow:yyyyMMdd-HHmmss-fff}.json");
            File.WriteAllText(path, SalvoTraceJson.Write(new Export(_selection)));
            _message = $"Saved {Path.GetFileName(path)}";
            Debug.Log($"[AGMLIB Salvo Debug] Exported {path}");
            return path;
        }
        catch (Exception error)
        {
            _message = $"Export failed: {error.Message}";
            Debug.LogException(error);
            return null;
        }
    }
}
