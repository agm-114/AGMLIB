using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Ships;
using System.Reflection;
using Utility;
using static AdvancedPaintScheme;
//using static AdvancedPaintScheme;

[ExecuteAlways]
public class AdvancedPaintScheme : MonoBehaviour
{

    //public Texture2D replacementtexture;
    //public HullScheme[] HullSchemes;


    //public GameObject PaintSchemesChild; 

    public List<HullScheme> HullPaintSchemes = new List<HullScheme>(1);

    public List<string> SerializedClassNames = new List<string>(1);
    public List<string> SerializedSegmentNames = new List<string>(1);
    public List<Texture2D> SerializedTextures = new List<Texture2D>(1);

    //public IDictionary<string, IDictionary<string, Texture2D>> SegmentTextures;
    [Serializable]
    public class HullScheme
    {
        public string ClassName;
        public GameObject Hull;
        //bool preserveshader;
        public List<TextureOverride> HullSegmentTextures;
        //public Dictionary<string, Texture2D> name;
    }
    [Serializable]
    public class TextureOverride
    {

        public string SegmentName;
        public Texture2D ReplacementTexture;
    }



    // Update is called once per frame
    void Update()
    {
        if (Application.isEditor)
        {
            for (int i = 0; i < HullPaintSchemes.Count; i++)
            {
                if (HullPaintSchemes[i].Hull == null)
                    continue;
                
                Hull newhull = HullPaintSchemes[i].Hull.GetComponent<Hull>();
                if (newhull == null)
                    newhull = HullPaintSchemes[i].Hull.GetComponentInChildren<Hull>();
                if (newhull == null)
                    newhull = HullPaintSchemes[i].Hull.GetComponentInParent<Hull>();
                if (newhull == null)
                {
                    Debug.LogError("The linked gameobject has no HULL component");
                    continue;
                }
                HullPaintSchemes[i].ClassName = newhull.ClassName;
                HullSegment[] _paintableMeshes = (HullSegment[])GetPrivateField(newhull, "_paintableMeshes");

                if (HullPaintSchemes[i].HullSegmentTextures == null)
                    HullPaintSchemes[i].HullSegmentTextures = new  List<TextureOverride>(1);
                while (HullPaintSchemes[i].HullSegmentTextures.Count < _paintableMeshes.Length)
                    HullPaintSchemes[i].HullSegmentTextures.Add(new TextureOverride());
                while (HullPaintSchemes[i].HullSegmentTextures.Count > _paintableMeshes.Length)
                    HullPaintSchemes[i].HullSegmentTextures.RemoveAt(HullPaintSchemes[i].HullSegmentTextures.Count - 1);
                for (int j = 0; j < _paintableMeshes.Length; j++)
                {
                    HullPaintSchemes[i].HullSegmentTextures[j].SegmentName = _paintableMeshes[j].gameObject.name;
                }
            }

            SerializedClassNames = new List<string>();
            SerializedSegmentNames = new List<string>();
            SerializedTextures = new List<Texture2D>();

            foreach (HullScheme hullScheme in HullPaintSchemes)
            {
                //Debug.LogError(hullScheme.ClassName);
                foreach (TextureOverride textureOverride in hullScheme.HullSegmentTextures)
                {
                    //SerializedClassNames[counttexutures] = hullScheme.ClassName;
                    //SerializedSegmentNames[counttexutures] = textureOverride.SegmentName;
                    //SerializedTextures[counttexutures] = textureOverride.ReplacementTexture;
                    SerializedClassNames.Add(hullScheme.ClassName);
                    SerializedSegmentNames.Add(textureOverride.SegmentName);
                    SerializedTextures.Add(textureOverride.ReplacementTexture);
                    //counttexutures = 0;
                }
            }

        }
        else
        {
            //if(PaintSchemesChild != null)
            //    Destroy(PaintSchemesChild);
            //Debug.LogError("Component has: " + HullPaintSchemes.Count + " Hull paint schemes");
            for(int i  = 0; i < SerializedTextures.Count; i++)
            {
                if (SerializedTextures[i] == null)
                    continue;
                PaintScheme newpaintscheme = gameObject.AddComponent<PaintScheme>();
                newpaintscheme.ClassName = SerializedClassNames[i];
                newpaintscheme.SegmentName = SerializedSegmentNames[i];
                newpaintscheme.replacementtexture = SerializedTextures[i];
                if (SerializedTextures[i] == null)
                    Debug.LogError("Texture missing in APS");
            }
            Destroy(this);
        }
    }

    public static object GetPrivateField(object instance, string fieldName)
    {
        static object GetPrivateFieldInternal(object instance, string fieldName, System.Type type)
        {
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null)
            {
                return field.GetValue(instance);
            }
            else if (type.BaseType != null)
            {
                return GetPrivateFieldInternal(instance, fieldName, type.BaseType);
            }
            else
            {
                return null;
            }
        }

        return GetPrivateFieldInternal(instance, fieldName, instance.GetType());
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