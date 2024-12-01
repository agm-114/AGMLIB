using Bundles;
using FleetEditor;
using Game.Units;
using Munitions;
using RSG;
using Ships;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Utility;
//using HarmonyLib;
public class PaintScheme : MonoBehaviour
{
    // Start is called before the first frame update
    //public string key = "enter key here";
    //private string shaderproperty = "_PaintMask";
    //private static bool baking = false;

    public Texture2D Replacementtexture;
    public bool ValidBaketraget = false;
    public FastNameplateBaker.BakeTarget Baketarget;
    //public Texture2D ospseedtecture;
    private Material replacementmat;
    public string ClassName = "Raines";
    public string SegmentName = "Bow";
    public int Index = -1;
    public string shaderproperty = "_PaintMask";
    private Hull Hull => GetComponentInParent<Hull>();
    //private int count = 0;
    //private Material _originalMaterial;
    private Ship Ship => GetComponentInParent<Ship>();
    //private Material _bakedMaterial
    private int ship = 0;

    private HullSegmentBasic[] hullsegs => Hull.gameObject.GetComponentsInChildren<HullSegmentBasic>();
    private IEnumerable<HullSegmentBasic> validsegs => hullsegs.Where(hullSegment => hullSegment.gameObject.name == SegmentName);
    private HullSegmentBasic targetedhullseg => validsegs.First();
    void Awake()
    {
    }
    void Start()
    {
    }

    void FixedUpdate ()
    {
        if(ClassName == null || SegmentName == null || Replacementtexture == null)
        {
            Common.Hint("Null Value");
            Destroy(this);
            return;
        }
        if (Hull.ClassName != ClassName || Index < 0 || validsegs.Count() <= 0)
        {
            if(Hull.ClassName == ClassName && Index >= 0)
            {
                Common.Hint(SegmentName + " not found in " + hullsegs.Length + " hull segs named " + string.Join(",", hullsegs.ToList().ConvertAll<string>(a => a.gameObject.name)));

            }
            Destroy(this);
            return;
        }
        //Debug.LogError(ClassName);
        //Debug.LogError(SegmentName);
        if (GameSettings.Instance.EnableNameplates && targetedhullseg is HullSegmentFastNameplate fast)
        {
            if (Common.GetVal<ReferenceCounted<Texture2D>>(fast, "_nameplateDecal") == null)
            {
                //Debug.LogError("Waiting For Nameplate Bake");
                return;
            }
        }
        //Common.SetVal(hullSegment, "_bakedMaterials", null);
        //Material[] mats = Common.GetVal<Material[]>(hullSegment, "_bakedMaterials", null);
        foreach (MeshRenderer mesh in targetedhullseg.GetComponentsInChildren<MeshRenderer>())
        {
            mesh.sharedMaterials[Index].SetTexture(shaderproperty, Replacementtexture);
        }
        //hullSegment.BakeNameplateAsync(Ship.ShipName, Ship.HullNumber, Ship.Fleet.FleetBadge);
        Destroy(this);

    }


}


/*
if (invalid)
{
Destroy(this);

return;
}

if (ValidBaketraget)
{
if (GameSettings.Instance.EnableNameplates && targetedhullseg is HullSegmentFastNameplate fast)
{
    FastNameplateBaker.BakeTarget[] _targets = Common.GetVal<FastNameplateBaker.BakeTarget[]>(fast, "_targets");
    _targets[Index] = Baketarget;
    Common.SetVal(fast, "_targets", _targets);
}
}
*/
/*
 * 
 *                     Texture2D copyTextures = new(Replacementtexture.width, Replacementtexture.height);
                copyTextures.SetPixels(Replacementtexture.GetPixels());
                copyTextures.Apply();
                mesh.materials[Index].SetTexture(shaderproperty, copyTextures);
*/

/*
 * 
 * 
 * 
 * 
 * 
 * 
 * 
        //Debug.LogError(Common.GetVal<Material[]>(hullSegment, "_bakedMaterials"));
        Common.SetVal(hullSegment, "_bakedMaterials", null);
        //Debug.LogError(Common.GetVal<Material[]>(hullSegment, "_bakedMaterials"));

        //baking = true;

        Material[] _originalMaterials = Common.GetVal<Material[]>(hullSegment, "_originalMaterials") ?? new Material[0];
        for (int i = 0; i < _originalMaterials.Length; i++)
        {
            Texture2D copyTextures = new(Replacementtexture.width, Replacementtexture.height);
            copyTextures.SetPixels(Replacementtexture.GetPixels());
            copyTextures.Apply();
            _originalMaterials[i].SetTexture(shaderproperty, copyTextures);
        }
        Common.SetVal(hullSegment, "_originalMaterials", _originalMaterials);
        //baking = true;
        replacementmat = Common.GetVal<Material>(hullSegment, "_originalMaterial");
        Texture2D copyTexture = new(Replacementtexture.width, Replacementtexture.height);
        copyTexture.SetPixels(Replacementtexture.GetPixels());
        copyTexture.Apply();
        replacementmat.SetTexture(shaderproperty, copyTexture);
        //Common.SetVal(hullSegment, "_originalMaterial", replacementmat);
        //Common.RunFunc(hullSegment, "MakeBakeMaterial");

        //_EmissionColor
        //Debug.LogError("Slow bake texture");
        hullSegment.BakeNameplateAsync(Ship.ShipName, Ship.HullNumber, Ship.Fleet.FleetBadge);


*/

/*

private IEnumerator CoroutinePaint(HullSegmentBasic hullSegment)
{
    //while(baking)
    //   yield return new WaitForSeconds(0.1f);



}*/

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