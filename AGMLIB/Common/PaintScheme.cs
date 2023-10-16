using Bundles;
using FleetEditor;
using Munitions;
using RSG;
using Ships;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Utility;
//using HarmonyLib;


public class PaintScheme : MonoBehaviour
{
    // Start is called before the first frame update
    //public string key = "enter key here";
    //private string shaderproperty = "_PaintMask";
    //private static bool baking = false;

    public Texture2D replacementtexture;
    //public Texture2D ospseedtecture;
    private Material replacementmat;
    public string ClassName = "Raines";
    public string SegmentName = "Bow";
    private readonly string shaderproperty = "_PaintMask";
    private Hull Hull;
    //private int count = 0;
    //private Material _originalMaterial;
    private Ship Ship;
    //private Material _bakedMaterial;



    void Start()
    {

        Hull = GetComponentInParent<Hull>();
        Ship = GetComponentInParent<Ship>();
        if (ClassName != null && Hull.ClassName != ClassName)
            return;

        foreach (HullSegment hullSegment in Hull.gameObject.GetComponentsInChildren<HullSegment>())
        {
            if (SegmentName != null && hullSegment.gameObject.name != SegmentName)
                continue;

            if (replacementtexture == null)
            {
                Debug.LogError("No replacement texture");
                continue;
            }
                


            StartCoroutine(CoroutinePaint(hullSegment));


        }

        
    }

    private IEnumerator CoroutinePaint(HullSegment hullSegment)
    {
        //while(baking)
         //   yield return new WaitForSeconds(0.1f);
        yield return new WaitForSeconds(0f);
        
        //baking = true;
        replacementmat = (Material)GetPrivateField(hullSegment, "_originalMaterial");
        Texture2D copyTexture = new(replacementtexture.width, replacementtexture.height);
        copyTexture.SetPixels(replacementtexture.GetPixels());
        copyTexture.Apply();
        replacementmat.SetTexture(shaderproperty, copyTexture);
        SetPrivateField(hullSegment, "_originalMaterial", replacementmat);

        hullSegment.BakeNameplateAsync(Ship.ShipName, Ship.HullNumber, Ship.Fleet.FleetBadge);
        //baking = false;
        Destroy(this);
    }

    //Debug.LogError("DOING REPLACMENT" + hullSegment.name + replacementtexture.name);


    /*
    Dictionary<string, BaseHull> _hulls = (Dictionary<string, BaseHull>)GetPrivateField(BundleManager.Instance, "_hulls");
    foreach(var h in _hulls)
    {
        Debug.LogError(h.Key);
    }
    */



    /*
    BaseHull hull = BundleManager.Instance.GetHull("Stock/Tugboat");
    HullSegment rend = hull.GetComponentsInChildren<HullSegment>()[0];
    replacementmat = new Material(rend.ArmorMaterial.shader);
    replacementmat.CopyPropertiesFromMaterial(rend.ArmorMaterial);
    replacementmat.SetTexture(shaderproperty, copyTexture);
    replacementmat.SetTexture("_BaseColorMap", replacementmat.GetTexture("_BaseColorMap"));
    replacementmat.SetTexture("_MaskMap", replacementmat.GetTexture("_MaskMap"));
    replacementmat.SetTexture("_NormalMap", replacementmat.GetTexture("_NormalMap"));
    replacementmat.SetColor("_BaseColor", new Color(1f, 0f, 0f));
    replacementmat.SetColor("_BasePaintTint", new Color(0f, 1f, 0f));
    replacementmat.SetColor("_StripePaintTint", new Color(0f, 0f, 1f));
    Debug.LogError(replacementmat.shader.name);
    */

    //private void Awake()
    //{
    /*if(replacementmat != null)
    {
        //SetPrivateField(hullSegment, "_bakedMaterial", null);
        //hullSegment.SetProceduralTextureVariation(new Vector3(), new Color());
        SetPrivateField(hullSegment, "_originalMaterial", replacementmat);
        //SetPrivateField(hullSegment, "_bakedMaterial", null);
        //hullSegment.SetProceduralTextureVariation(new Vector3(), new Color());
        hullSegment.BakeNameplateAsync(name, number, badge);
    }*/
    //hullSegment.bak
    /*
    foreach (PaintableHullSegment segment in Hull.GetComponentsInChildren<PaintableHullSegment>())
    {
        //Debug.LogError("Painter " + painter.gameObject.name + " Key: " + key);
        if (segment.key == key)
        {
            segment.paintScheme = replacementtexture;
            segment.Quickbakepaint();
        }

    }*/
    //FleetCompositionSubmodeController[] uis = FindObjectsOfType<FleetCompositionSubmodeController>();
    //foreach (FleetCompositionSubmodeController ui in uis)
    //{
    //Debug.LogError("BakingPaintScheme");
    //ui.EditShipIdentity();
    //}
    //}
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

    public static void SetPrivateField(object instance, string fieldName, object value)
    {
        static void SetPrivateFieldInternal(object instance, string fieldName, object value, System.Type type)
        {
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null)
            {
                field.SetValue(instance, value);
                return;
            }
            else if (type.BaseType != null)
            {
                SetPrivateFieldInternal(instance, fieldName, value, type.BaseType);
                return;
            }
        }

        SetPrivateFieldInternal(instance, fieldName, value, instance.GetType());
    }

}
//List<Material>  targetmats = new List<Material>();
//MeshRenderer[] targetmeshes = hull.gameObject.GetComponentsInChildren<MeshRenderer>();
//MeshRenderer[] targetmeshes2 = ship.gameObject.GetComponentsInChildren<MeshRenderer>();
//HullSegment[] hullsegs = hull.gameObject.GetComponentsInChildren<HullSegment>();
//ComponentHullPaint[] comps = hull.gameObject.GetComponentsInChildren<ComponentHullPaint>();
//foreach (MeshRenderer targetmesh in targetmeshes2)
//    foreach (Material mat in targetmesh.materials)
//        targetmats.Add(mat);
//foreach (MeshRenderer targetmesh in targetmeshes)
//    foreach (Material mat in targetmesh.materials)
//        targetmats.Add(mat);
//foreach (HullSegment hullSegment in hullsegs)
//    targetmats.Add(hullSegment.ArmorMaterial);
/*
foreach (Material targetmat in targetmats)
{
    if (targetmat.HasProperty(shaderproperty))
    {
        Debug.LogError("Target Material " + targetmat.name + " has texture " + targetmat.GetTexture(shaderproperty).name);
        if (targettexturenames.Contains(targetmat.GetTexture(shaderproperty).name))
        {
            Debug.LogError("Replacing texture " + targetmat.GetTexture(shaderproperty).name + " with " + replacementtexture.name);
            targetmat.SetTexture(shaderproperty, replacementtexture);
            Debug.LogError("Replaced Texture: " + targetmat.GetTexture(shaderproperty).name);
        }

    }
}*/


/*
using Ships;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Utility;
using System;

public class PaintableHullSegment : HullSegment
{
    public string key;
    private Hull Hull;
    private Material OrignalMaterial;
    private const string _paintMaskParam = "_PaintMask";
    //private Material _bakedMaterial;
    public Texture2D paintScheme;
    // Start is called before the first frame update
    void Awake()
    {
        Hull = base.GetComponentInParent<Hull>();
        GetBaseMat();
        foreach (PaintScheme painter in Hull.GetComponentsInChildren<PaintScheme>())
        {
            //Debug.LogError("Painter " + painter.gameObject.name + " Key: " + key);
            if (painter.key == key)
            {
                paintScheme = painter.replacementtexture;
                UpdatePaintScheme();
                Destroy(paintScheme);
            }

        }
    }
    void GetBaseMat()
    {
        OrignalMaterial = (Material)GetPrivateField(this, "_originalMaterial");
    }

    void UpdatePaintScheme()
    {
        OrignalMaterial.SetTexture(_paintMaskParam, paintScheme);
    }

    public void Quickbakepaint()
    {
        foreach (MeshRenderer meshRenderer in base.gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer.material.SetTexture(_paintMaskParam, paintScheme);
        }
        UpdatePaintScheme();
    }

    // Update is called once per frame

    public static object GetPrivateField(object instance, string fieldName)
    {
        static object GetPrivateFieldInternal(object instance, string fieldName, Type type)
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
*/