using Ships;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;


public class PaintableHullSegment : HullSegment
{
    public string key;
    private Hull Hull;
    private Material OrignalMaterial;
    private const string _paintMaskParam = "_PaintMask";
    //private Material _bakedMaterial;
    [HideInInspector]
    public Texture2D paintScheme;
    // Start is called before the first frame update
    void Awake()
    {
        Hull = base.GetComponentInParent<Hull>();
        GetBaseMat();
        foreach (PaintScheme painter in Hull.GetComponentsInChildren<PaintScheme>())
        {
            //Debug.LogError("Painter " + painter.gameObject.name + " Key: " + key);
            //if (painter.key == key)
            //{
                //paintScheme = painter.replacementtexture;
                UpdatePaintScheme();
                Destroy(paintScheme);
            //}

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
