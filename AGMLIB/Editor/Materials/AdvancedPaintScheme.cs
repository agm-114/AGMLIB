[ExecuteAlways]
[ExecuteInEditMode]
public class AdvancedPaintScheme : MonoBehaviour
{

    //public Texture2D replacementtexture;
    //public HullScheme[] HullSchemes;

    //public GameObject PaintSchemesChild; 



    public ShaderProperties shaderproperty = ShaderProperties.PaintMask;
    public GameObject Hull;
    public string ClassName = "default";
    public bool Serialize = true;
    public bool AutoFill = true;
    //bool preserveshader;
    public List<SegmentOverride> HullSegmentTextures;

    public List<string> SerializedClassNames = new(1);
    public List<string> SerializedSegmentNames = new(1);
    public List<Texture2D> SerializedTextures = new(1);
    public List<int> SerializedIndexes = new(1);
    public List<bool> SerializedValidTargets = new(1);
    public List<FastNameplateBaker.BakeTarget> SerializedTargets = new(1);


    [Serializable]
    public class SegmentOverride
    {

        public string SegmentName = "default";
        public Texture2D ReplacementTexture;
        public List<Texture2D> TextureOverrides = new(0);
        public List<FastNameplateBaker.BakeTarget> Targets = new(0);
    }
    public class MaterialOverrides
    {

        public string SegmentName;
        public Texture2D ReplacementTexture;
        public int MaterialIndex = 0;
        public int NameplateIndex = 0;
    }

    void Start()
    {
        //if(PaintSchemesChild != null)
        //    Destroy(PaintSchemesChild);
        //Debug.LogError("Component has: " + HullPaintSchemes.Count + " Hull paint schemes");
        if (Application.isEditor)
        {
            return;
        }
        for (int i = 0; i < SerializedTextures.Count; i++)
        {
            if (SerializedTextures[i] == null)
                continue;
            PaintScheme newpaintscheme = gameObject.AddComponent<PaintScheme>();
            newpaintscheme.ClassName = SerializedClassNames[i];
            newpaintscheme.SegmentName = SerializedSegmentNames[i];
            newpaintscheme.ReplacementTexture = SerializedTextures[i];
            newpaintscheme.Index = SerializedIndexes[i];
            newpaintscheme.ShaderProperty = shaderproperty;
            //newpaintscheme.ValidBaketraget = SerializedValidTargets[i];
            //stnewpaintscheme.Baketarget = SerializedTargets[i];
            //if (SerializedTextures[i] == null)
            //    Debug.LogError("Texture missing in APS");
        }
        Destroy(this);

    }

    void Update()
    {
        if (!Application.isEditor || (!Serialize && !AutoFill))
        {
            return;
        }

        if (!TryGetHull(out Hull linkedHull))
        {
            return;
        }

        HullSegmentBasic[] paintableMeshes = GetPaintableMeshes(linkedHull);
        if (paintableMeshes.Length == 0)
        {
            Debug.LogError($"AdvancedPaintScheme on '{name}' could not find any paintable hull segments.");
            return;
        }

        if (AutoFill)
        {
            AutoFillSegments(linkedHull, paintableMeshes);
        }

        if (Serialize)
        {
            SerializePaintScheme(paintableMeshes);
        }
    }

    private bool TryGetHull(out Hull linkedHull)
    {
        linkedHull = null;
        if (Hull == null)
        {
            Debug.LogError($"AdvancedPaintScheme on '{name}' has no hull linked.");
            return false;
        }

        linkedHull = Hull.GetComponent<Hull>() ?? Hull.GetComponentInChildren<Hull>() ?? Hull.GetComponentInParent<Hull>();
        if (linkedHull == null)
        {
            Debug.LogError($"AdvancedPaintScheme on '{name}' could not find a Hull component from '{Hull.name}'.");
            return false;
        }

        return true;
    }

    private static HullSegmentBasic[] GetPaintableMeshes(Hull hull)
    {
        HullSegmentBasic[] paintableMeshes = hull.Internals().PaintableMeshes;
        return paintableMeshes == null || paintableMeshes.Length == 0
            ? hull.GetComponentsInChildren<HullSegmentBasic>()
            : paintableMeshes;
    }

    private void AutoFillSegments(Hull hull, HullSegmentBasic[] paintableMeshes)
    {
        ClassName = hull.ClassName;
        HullSegmentTextures ??= new List<SegmentOverride>(paintableMeshes.Length);

        while (HullSegmentTextures.Count < paintableMeshes.Length)
        {
            HullSegmentTextures.Add(new SegmentOverride());
        }
        while (HullSegmentTextures.Count > paintableMeshes.Length)
        {
            HullSegmentTextures.RemoveAt(HullSegmentTextures.Count - 1);
        }
        for (int i = 0; i < paintableMeshes.Length; i++)
        {
            HullSegmentTextures[i].SegmentName = paintableMeshes[i].gameObject.name;
        }
    }

    private void SerializePaintScheme(HullSegmentBasic[] paintableMeshes)
    {
        if (HullSegmentTextures == null)
        {
            Debug.LogError($"AdvancedPaintScheme on '{name}' has no segment overrides to serialize.");
            return;
        }

        List<string> classNames = new();
        List<string> segmentNames = new();
        List<Texture2D> textures = new();
        List<int> indexes = new();
        List<bool> validTargets = new();
        List<FastNameplateBaker.BakeTarget> targets = new();
        string shaderPropertyName = ShaderPropertyName();

        foreach (SegmentOverride textureOverride in HullSegmentTextures)
        {
            void Add(int materialIndex, Texture2D texture, FastNameplateBaker.BakeTarget target, bool validTarget = false)
            {
                classNames.Add(ClassName);
                segmentNames.Add(textureOverride.SegmentName);
                textures.Add(texture);
                indexes.Add(materialIndex);
                validTargets.Add(validTarget);
                targets.Add(target);
            }

            Add(-1, textureOverride.ReplacementTexture, new());

            IReadOnlyList<Texture2D> textureOverrides = textureOverride.TextureOverrides is null
                ? Array.Empty<Texture2D>()
                : textureOverride.TextureOverrides;
            if (textureOverrides.Count > 0)
            {
                HullSegmentBasic[] matchingSegments = paintableMeshes
                    .Where(segment => segment.gameObject.name == textureOverride.SegmentName)
                    .ToArray();
                if (matchingSegments.Length != 1)
                {
                    Debug.LogError($"AdvancedPaintScheme on '{name}' expected one segment named '{textureOverride.SegmentName}', but found {matchingSegments.Length}.");
                    return;
                }
                if (string.IsNullOrWhiteSpace(shaderPropertyName))
                {
                    Debug.LogError($"AdvancedPaintScheme on '{name}' cannot map material overrides without a shader property.");
                    return;
                }

                IReadOnlyList<Material> materials = matchingSegments[0].SegmentMaterials;
                List<int> compatibleIndexes = new();
                for (int materialIndex = 0; materialIndex < materials.Count; materialIndex++)
                {
                    Material material = materials[materialIndex];
                    if (material != null && material.HasProperty(shaderPropertyName))
                    {
                        compatibleIndexes.Add(materialIndex);
                    }
                }

                if (textureOverrides.Count > compatibleIndexes.Count)
                {
                    Debug.LogError($"AdvancedPaintScheme on '{name}' has {textureOverrides.Count} texture overrides for segment '{textureOverride.SegmentName}', but only {compatibleIndexes.Count} materials support '{shaderPropertyName}'.");
                    return;
                }

                for (int i = 0; i < textureOverrides.Count; i++)
                {
                    Add(compatibleIndexes[i], textureOverrides[i], new());
                }
            }

            IReadOnlyList<FastNameplateBaker.BakeTarget> nameplateTargets = textureOverride.Targets is null
                ? Array.Empty<FastNameplateBaker.BakeTarget>()
                : textureOverride.Targets;
            for (int i = 0; i < nameplateTargets.Count; i++)
            {
                Add(i, null, nameplateTargets[i], true);
            }
        }

        SerializedClassNames = classNames;
        SerializedSegmentNames = segmentNames;
        SerializedTextures = textures;
        SerializedIndexes = indexes;
        SerializedValidTargets = validTargets;
        SerializedTargets = targets;
    }

    private string ShaderPropertyName()
    {
        return shaderproperty == ShaderProperties.None ? string.Empty : "_" + shaderproperty;
    }
}

/*
[CustomPropertyDrawer(typeof(TextureOverride))]
public class IngredientDrawerUIE : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Create property container element.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        var keyRect = new Rect(position.x, position.y, position.width / 2f, position.height);
        var texturerect = new Rect(position.x + position.width * (2f / 3f), position.y, position.width / 3f, position.height);

        if (property.FindPropertyRelative("SegmentName").stringValue.Length == 0)
        {
            keyRect = new Rect(position.x + position.width * (1f / 3f), position.y, position.width / 3f, position.height);
            var helprect = new Rect(position.x, position.y, position.width / 3f, position.height);


            EditorGUI.HelpBox(helprect, "Target a hullsegment", MessageType.Warning);
        }
        else
        {
            keyRect = new Rect(position.x + position.width / 6, position.y, position.width / 3f, position.height);
            var labelrect = new Rect(position.x + position.width * (1f / 2f), position.y, position.width / 6f, position.height);
            EditorGUI.PrefixLabel(labelrect, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(" Texture: "));
            labelrect.x = position.x;
            EditorGUI.PrefixLabel(labelrect, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Hull Seg: "));
        }

        // Calculate rects

        //var nameRect = new Rect(position.x + 90, position.y, position.width - 90, position.height);

        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(keyRect, property.FindPropertyRelative("SegmentName"), GUIContent.none);


        EditorGUI.PropertyField(texturerect, property.FindPropertyRelative("ReplacementTexture"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;


        EditorGUI.EndProperty();
    }
}
*/

//int textures = 0;

//PaintScheme[] paintSchemes = PaintSchemesChild.GetComponents<PaintScheme>();

/*
 * 
 * 
*             int counttexutures = 0;
foreach (HullScheme hullScheme in HullPaintSchemes)
{
Debug.LogError(hullScheme.ClassName);
foreach (TextureOverride textureOverride in hullScheme.HullSegmentTextures)
    counttexutures++;
}
if(SerializedClassNames.Count != counttexutures || SerializedSegmentNames.Count != counttexutures || SerializedTextures.Count != counttexutures)
{
//Debug.LogError("Serializing " + counttexutures + " textures");
counttexutures = 0;
}
foreach (HullScheme scheme in HullPaintSchemes)
{
    if (scheme.ClassName.Length == 0)
        continue;

    foreach (TextureOverride textureOverride in scheme.HullSegmentTextures)
    {

        if (textureOverride.SegmentName.Length > 0 && textureOverride.ReplacementTexture != null)
        {
            textures++;
            if (paintSchemes.Length < textures)
            {
                PaintSchemesChild.AddComponent<PaintScheme>();
                paintSchemes = PaintSchemesChild.GetComponents<PaintScheme>();
            }
            paintSchemes[textures - 1].ClassName = scheme.ClassName;
            paintSchemes[textures - 1].SegmentName = textureOverride.SegmentName;
            paintSchemes[textures - 1].replacementtexture = textureOverride.ReplacementTexture;

        }
    }

}

while (paintSchemes.Length > textures)
{
    DestroyImmediate(paintSchemes[paintSchemes.Length - 1]);
    paintSchemes = PaintSchemesChild.GetComponents<PaintScheme>();
}

paintSchemes = gameObject.GetComponents<PaintScheme>();

while (paintSchemes.Length > textures)
{
    DestroyImmediate(paintSchemes[paintSchemes.Length - 1]);
    paintSchemes = gameObject.GetComponents<PaintScheme>();
}
*/

#if UNITY_EDITOR


#endif
