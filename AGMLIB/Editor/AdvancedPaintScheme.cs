[ExecuteAlways]
[ExecuteInEditMode]
public class AdvancedPaintScheme : MonoBehaviour
{

    //public Texture2D replacementtexture;
    //public HullScheme[] HullSchemes;

    //public GameObject PaintSchemesChild; 

    public enum ShaderProperties
    {
        PaintMask,
        BaseColorMap,
        MaskMap,
        NormalMap
    }

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
            newpaintscheme.Replacementtexture = SerializedTextures[i];
            newpaintscheme.Index = SerializedIndexes[i];
            newpaintscheme.shaderproperty = "_" + shaderproperty.ToString();
            //newpaintscheme.ValidBaketraget = SerializedValidTargets[i];
            //stnewpaintscheme.Baketarget = SerializedTargets[i];
            //if (SerializedTextures[i] == null)
            //    Debug.LogError("Texture missing in APS");
        }
        Destroy(this);

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.LogError("Running APS Tick");
        if (!Application.isEditor)
        {
            return;
        }

        if (Serialize)
        {

            SerializedClassNames = new List<string>();
            SerializedSegmentNames = new List<string>();
            SerializedTextures = new List<Texture2D>();
            SerializedIndexes = new List<int>();
            SerializedValidTargets = new List<bool>();
            SerializedTargets = new List<FastNameplateBaker.BakeTarget>();

            foreach (SegmentOverride textureOverride in HullSegmentTextures)
            {
                //SerializedClassNames[counttexutures] = hullScheme.ClassName;
                //SerializedSegmentNames[counttexutures] = textureOverride.SegmentName;
                //SerializedTextures[counttexutures] = textureOverride.ReplacementTexture;
                void Serialize(int indexval, Texture2D texture, FastNameplateBaker.BakeTarget target, bool validtarget = false)
                {
                    SerializedClassNames.Add(ClassName);
                    SerializedSegmentNames.Add(textureOverride.SegmentName);
                    SerializedTextures.Add(texture);
                    SerializedIndexes.Add(-1);
                    SerializedValidTargets.Add(validtarget);
                    SerializedTargets.Add(target);
                }

                Serialize(-1, textureOverride.ReplacementTexture, new());

                int index = 0;


                foreach (Texture2D texture in textureOverride.TextureOverrides)
                {
                    Serialize(index, texture, new());
                    index++;
                }
                index = 0;

                foreach (FastNameplateBaker.BakeTarget texture in textureOverride.Targets)
                {
                    Serialize(index, null, texture, true);
                    index++;
                }

                //counttexutures = 0;
            }

        }

        if (AutoFill)
        {
            Debug.LogError("Running APS Editor Tick");

            if (Hull == null)
            {
                Debug.LogError("Hull is Null");
                return;

            }

            Hull newhull = (Hull.GetComponent<Hull>() ?? Hull.GetComponentInChildren<Hull>()) ?? Hull.GetComponentInParent<Hull>();
            if (newhull == null)
            {
                Debug.LogError("The linked gameobject has no HULL component");
                return;
            }
            ClassName = newhull.ClassName;
            HullSegmentBasic[] _paintableMeshes = Common.GetVal<HullSegmentBasic[]>(newhull, "_paintableMeshes");
            if (_paintableMeshes == null || _paintableMeshes.Length <= 0)
            {
                Debug.LogError("No Paintable Medshes Detected, falling back on manual detection");
                _paintableMeshes = Hull.GetComponentsInChildren<HullSegmentBasic>();

            }
            if (_paintableMeshes == null || _paintableMeshes.Length <= 0)
            {
                Debug.LogError("No Paintable Medshes Detected, falling back on manual detection");
                return;

            }


            if (HullSegmentTextures == null)
                HullSegmentTextures = new List<SegmentOverride>(1);
            while (HullSegmentTextures.Count < _paintableMeshes.Length)
                HullSegmentTextures.Add(new SegmentOverride());
            while (HullSegmentTextures.Count > _paintableMeshes.Length)
                HullSegmentTextures.RemoveAt(HullSegmentTextures.Count - 1);
            for (int j = 0; j < _paintableMeshes.Length; j++)
            {
                HullSegmentTextures[j].SegmentName = _paintableMeshes[j].gameObject.name;
            }

        }


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