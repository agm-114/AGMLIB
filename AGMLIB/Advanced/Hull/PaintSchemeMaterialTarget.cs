[RequireComponent(typeof(HullSegmentBasic))]
public sealed class PaintSchemeMaterialTarget : MonoBehaviour
{
    private const string InstanceSuffix = " (Instance)";

    private Texture2D? _paintMask;
    private HullSegmentBasic _segment = null!;
    private string _sourceMaterialName = string.Empty;
    private bool _initialized;

    [Tooltip("Unique key used by paint scheme overrides to address this material target.")]
    public string Key = "default";

    [Tooltip("Source material to identify. The runtime baked material is changed; this asset is not modified.")]
    public Material SourceMaterial = null!;

    [Tooltip("Shader texture property changed on the baked material.")]
    public string PaintMaskProperty = "_PaintMask";

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;
        _segment = GetComponent<HullSegmentBasic>();

        if (_segment == null)
        {
            LogFailure($"Target '{Key}' is not attached to a hull segment.");
        }

        if (SourceMaterial == null)
        {
            LogFailure($"Target '{Key}' on '{name}' has no source material.");
        }
        else
        {
            _sourceMaterialName = NormalizeMaterialName(SourceMaterial.name);
        }

        if (string.IsNullOrWhiteSpace(PaintMaskProperty))
        {
            LogFailure($"Target '{Key}' on '{name}' has no shader texture property configured.");
        }
    }

    public bool TryApply(Texture2D paintMask)
    {
        // Awake is deferred for inactive GameObjects, which overrides also search.
        Initialize();

        if (paintMask == null)
        {
            return LogFailure($"Target '{Key}' on '{name}' was given a null paint mask.");
        }

        _paintMask = paintMask;
        return ApplyPaintMask(logFailure: true);
    }

    private void LateUpdate()
    {
        if (_paintMask != null)
        {
            ApplyPaintMask(logFailure: false);
        }
    }

    private bool ApplyPaintMask(bool logFailure)
    {
        string sourceMaterialDisplayName = SourceMaterial == null ? "<null>" : SourceMaterial.name;
        bool foundMaterial = false;
        bool applied = false;
        foreach (Material material in _segment.SegmentMaterials)
        {
            if (material == null || NormalizeMaterialName(material.name) != _sourceMaterialName)
            {
                continue;
            }

            foundMaterial = true;
            if (!material.HasProperty(PaintMaskProperty))
            {
                continue;
            }

            applied = true;
            if (material.GetTexture(PaintMaskProperty) != _paintMask)
            {
                material.SetTexture(PaintMaskProperty, _paintMask);
            }
        }

        if (logFailure && !foundMaterial)
        {
            LogFailure($"Target '{Key}' could not find baked material '{sourceMaterialDisplayName}' on segment '{name}'.");
        }
        else if (logFailure && !applied)
        {
            LogFailure($"Baked material '{sourceMaterialDisplayName}' on target '{Key}' has no {PaintMaskProperty} property.");
        }

        return applied;
    }

    private static bool LogFailure(string failure)
    {
        Debug.LogError($"[AGMLIB Paint] {failure}");
        return false;
    }

    private static string NormalizeMaterialName(string materialName)
    {
        if (materialName.EndsWith(InstanceSuffix, StringComparison.Ordinal))
        {
            return materialName.Substring(0, materialName.Length - InstanceSuffix.Length);
        }

        return materialName;
    }
}
