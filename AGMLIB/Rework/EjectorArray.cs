using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using Bundles;
using Utility;
using Ships;
using Modding;
using Munitions;
//using Game.Ships;






public class EjectorArray : MonoBehaviour
{
    //private int count;
    //public float offset = 1;
    //private float time = 0.0f;
    private int index = 0;

    //public float interpolationPeriod = 1f;
    //public TubeLauncherComponent laucher;
    public List<MissileEjector> linkedejectors;
    public int launcherindex = 0;
    public TubeLauncherComponent tubelauncher;
    public CellLauncherComponent celllauncher;
    public List<MissileEjector> leftejectors;
    public List<MissileEjector> rightejectors;
    // Start is called before the first frame update
    private RotationMonitor rotation;
    private Vector3 dvec;
    private Vector3 lvec;
    void Start()
    {
        
    }
    
    RotationMonitor findmonitor(GameObject searchobject)
    {
        RotationMonitor[] monitors = searchobject.GetComponentsInChildren<RotationMonitor>();
        if(monitors.Length > 0)
            return monitors[0];

        //Debug.Log("No Monitor found");
        return null;
    }

    // Update is called once per frame

    void Update()
    {
        //time += Time.deltaTime;
        //rotation = findmonitor(gorotation);
        //if (ejector = null)
        //    ejector = (MissileEjector)GetPrivateField(launcher, "_ejector");

        if (tubelauncher != null)
            launcherindex = (int)GetPrivateField(tubelauncher, "_currentShot");
        else
            launcherindex = index;

		if (tubelauncher == null)
        {
            if (rotation == null)
                rotation = findmonitor(gameObject.transform.parent.parent.gameObject);
			if (rotation == null)
                return;
            dvec = rotation.angle;
            lvec = gameObject.transform.eulerAngles;
            float dev = ((lvec.y) - (dvec.y)) % 360;

            for (index = 0; index < linkedejectors.Count; index++)
            {
                if (dev < 0)
                {
                    //LogError
                    //Debug.LogError("Right Fire Mission Tube: " + index + " Dev:" + dev + " lvec.y: " + lvec.y + " dvec.y " + dvec.y);
                    linkedejectors[index].gameObject.transform.position = rightejectors[index % rightejectors.Capacity].gameObject.transform.position;
                    linkedejectors[index].gameObject.transform.rotation = rightejectors[index % rightejectors.Capacity].gameObject.transform.rotation;
                }
                else
                {
                    //Debug.LogError("Left Fire Mission Tube: " + index + " Dev:" + dev + " lvec.y: " + lvec.y + " dvec.y " + dvec.y);
                    linkedejectors[index].gameObject.transform.position = leftejectors[index % leftejectors.Capacity].gameObject.transform.position;
                    linkedejectors[index].gameObject.transform.rotation = leftejectors[index % leftejectors.Capacity].gameObject.transform.rotation;
                }
            }



            //transform.position = transform.parent.position;
            //transform.position += new Vector3(0, 0, offset * index);
            //ejector = ejectors[index];
            //base.transform;
            //SetPrivateField(laucher, "_ejector", ejectors[index]);
            index = 0;
        }
        else if (index != launcherindex)
        {
            //index = launcherindex;

            if (rotation == null)
                rotation = findmonitor(gameObject.transform.parent.parent.gameObject);
            dvec = rotation.angle;
            lvec = gameObject.transform.eulerAngles;
            float dev = ((lvec.y) - (dvec.y)) % 360;
            if (dev < 0)
            {
                //LogError
                //Debug.LogError("Right Fire Mission Tube: " + index + " Dev:" + dev + " lvec.y: " + lvec.y + " dvec.y " + dvec.y);
                linkedejectors[index % linkedejectors.Count].gameObject.transform.position = rightejectors[index % rightejectors.Capacity].gameObject.transform.position;
                linkedejectors[index % linkedejectors.Count].gameObject.transform.rotation = rightejectors[index % rightejectors.Capacity].gameObject.transform.rotation;
            }  
            else
            {
                //Debug.LogError("Left Fire Mission Tube: " + index + " Dev:" + dev + " lvec.y: " + lvec.y + " dvec.y " + dvec.y);
                linkedejectors[index % linkedejectors.Count].gameObject.transform.position = leftejectors[index % leftejectors.Capacity].gameObject.transform.position;
                linkedejectors[index % linkedejectors.Count].gameObject.transform.rotation = leftejectors[index % leftejectors.Capacity].gameObject.transform.rotation;
            }


            //transform.position = transform.parent.position;
            //transform.position += new Vector3(0, 0, offset * index);
            //ejector = ejectors[index];
            //base.transform;
            //SetPrivateField(laucher, "_ejector", ejectors[index]);
            index = launcherindex;
        }




    }
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

    public static void SetPrivateField(object instance, string fieldName, object value)
    {
        static void SetPrivateFieldInternal(object instance, string fieldName, object value, Type type)
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
/*

`
time = time - interpolationPeriod;

*/