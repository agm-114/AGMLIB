using UnityEngine.Rendering;

namespace Lib.Diagnostics;

internal class SalvoDebugWorldView : MonoBehaviour
{
    private const int MaxLines = 1024;
    private class WorldLine(LineRenderer renderer)
    {
        public LineRenderer Renderer = renderer;
        public BeamRayTrace? Ray;
        public bool Internal;
        public int Color;
        public Vector3 Center;
        public Vector3 Start;
        public Vector3 End;
    }
    private readonly List<WorldLine> _lines = [];
    private readonly List<(Vector3 Start, Vector3 End, BeamRayTrace Ray)> _segments = [];
    private readonly List<Material> _materials = [];
    private GameObject? _root;
    private int _revision;
    private int _used;
    public int OmittedLines { get; private set; }

    public void Hide()
    {
        if (_root != null)
            _root.SetActive(false);
    }

    public void Clear()
    {
        Hide();
        _revision = 0;
        _segments.Clear();
    }

    private void OnDestroy() => _materials.ForEach(Destroy);

    public void Sync(List<MissileTrace> missiles, BeamRayTrace? focus, bool paths, bool internals)
    {
        int revision = paths ? 17 : 31;
        foreach (MissileTrace missile in missiles)
        {
            revision = unchecked(revision * 397 ^ missile.GetHashCode() ^ missile.Path.Count ^ missile.Rays.Count);
            foreach (BeamRayTrace ray in missile.Rays)
                revision = unchecked(revision * 397 ^ ray.Steps.Count);
        }
        revision = unchecked(revision * 397 ^ (internals ? 1 : 0));
        revision = unchecked(revision * 397 ^ (focus?.GetHashCode() ?? 0));
        if (_root == null)
        {
            _root = new GameObject("Recorded salvo world geometry");
            _root.transform.SetParent(transform, false);
            Shader shader = Shader.Find("HDRP/Unlit") ?? Shader.Find("Sprites/Default");
            foreach (Color color in new[] { Color.cyan, Color.green, new Color(1, 0.65f, 0.2f), Color.gray, Color.white })
            {
                Material material = new(shader) { hideFlags = HideFlags.DontSave, renderQueue = (int)RenderQueue.Transparent };
                material.SetColor("_UnlitColor", color);
                material.SetColor("_Color", color);
                material.SetFloat("_SurfaceType", 1);
                material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                material.SetFloat("_ZWrite", 0);
                material.SetFloat("_ZTestTransparent", (float)CompareFunction.Always);
                material.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
                material.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                _materials.Add(material);
            }
        }
        _root.SetActive(true);
        if (revision != _revision)
        {
            _revision = revision;
            _used = OmittedLines = 0;
            _segments.Clear();
            foreach (MissileTrace missile in missiles.OrderBy(item => focus != null && item.Rays.Contains(focus) ? 0 : 1))
            {
                if (paths && missile.Path.Count > 1)
                    AddLine(missile.Path.Select(point => point.Position).ToArray(), null, false, 0);
                foreach (BeamRayTrace ray in missile.Rays.OrderBy(item => item == focus ? 0 : 1))
                {
                    AddLine([ray.Origin, ray.End], ray, false, ColorIndex(ray.Outcome));
                    if (internals)
                        foreach (DamageTraceStep step in ray.Steps.Where(step => step.HasSegment))
                            AddLine([step.Start, step.End], ray, true, ColorIndex(step.Outcome));
                }
            }
            for (int i = _used; i < _lines.Count; i++)
                _lines[i].Renderer.gameObject.SetActive(false);
        }
        Camera camera = Camera.main;
        for (int i = 0; i < _used; i++)
        {
            WorldLine line = _lines[i];
            bool selected = line.Ray != null && line.Ray == focus;
            line.Renderer.sharedMaterial = _materials[selected && !line.Internal ? 4 : line.Color];
            float distance = camera != null ? Mathf.Min(Vector3.Distance(camera.transform.position, line.Center),
                Mathf.Min(Vector3.Distance(camera.transform.position, line.Start), Vector3.Distance(camera.transform.position, line.End))) : 50;
            float width = Mathf.Clamp(distance * 0.001f, 0.025f, 2f);
            line.Renderer.startWidth = line.Renderer.endWidth = selected ? width * 1.75f : width;
        }
    }

    private static int ColorIndex(RayDamageOutcome outcome) => outcome switch
    {
        RayDamageOutcome.Damage => 1,
        RayDamageOutcome.NoDamage => 2,
        _ => 3
    };

    private void AddLine(Vector3[] points, BeamRayTrace? ray, bool internalRay, int color)
    {
        if (_used == MaxLines)
        {
            OmittedLines++;
            return;
        }
        if (_used == _lines.Count)
        {
            GameObject item = new("Recorded damage line");
            item.transform.SetParent(_root!.transform, false);
            LineRenderer renderer = item.AddComponent<LineRenderer>();
            renderer.useWorldSpace = true;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.numCapVertices = 2;
            renderer.startColor = renderer.endColor = Color.white;
            _lines.Add(new WorldLine(renderer));
        }
        WorldLine line = _lines[_used++];
        line.Ray = ray;
        line.Internal = internalRay;
        line.Color = color;
        line.Center = (points[0] + points[points.Length - 1]) / 2;
        line.Start = points[0];
        line.End = points[points.Length - 1];
        line.Renderer.gameObject.SetActive(true);
        line.Renderer.positionCount = points.Length;
        line.Renderer.SetPositions(points);
        if (ray != null)
            _segments.Add((points[0], points[points.Length - 1], ray));
    }

    public BeamRayTrace? Pick(Vector2 pointer)
    {
        Camera camera = Camera.main;
        if (camera == null)
            return null;
        BeamRayTrace? nearestRay = null;
        float nearest = 8;
        foreach (var segment in _segments)
        {
            Vector3 start = camera.WorldToScreenPoint(segment.Start);
            Vector3 end = camera.WorldToScreenPoint(segment.End);
            float near = camera.nearClipPlane + 0.001f;
            if (start.z <= near && end.z <= near)
                continue;
            if (start.z < near)
                start = camera.WorldToScreenPoint(Vector3.Lerp(segment.Start, segment.End, (near - start.z) / (end.z - start.z)));
            else if (end.z < near)
                end = camera.WorldToScreenPoint(Vector3.Lerp(segment.Start, segment.End, (near - start.z) / (end.z - start.z)));
            float distance = SalvoTraceGeometry.DistanceToSegment(pointer,
                new Vector2(start.x, Screen.height - start.y), new Vector2(end.x, Screen.height - end.y));
            if (distance < nearest)
            {
                nearest = distance;
                nearestRay = segment.Ray;
            }
        }
        return nearestRay;
    }
}
