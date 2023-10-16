using Ships;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class DynamicGlow : MonoBehaviour
{
    // Start is called before the first frame update

    public AnimationCurve[] emissivecurves = new AnimationCurve[4];
    public float thershold = 0.5f;
    List<Material> targetmats = new List<Material>();
    public MeshRenderer[] targetmeshes;
    public DynamicVisibleParticles[] targetparticles;
    private float glow = 0;
    private int i = -500;
    //public GameObject target;
    public enum Mode
    {
        Weight,
        Transparency
    }
    public Mode mode;
    private ResourceComponent ResourceComponent;
    public string[] refrence = { "em0O", "em1O", "em2O", "em3O" };

    void Start()
    {
        foreach (AnimationCurve curve in emissivecurves)
        {
            curve.preWrapMode = WrapMode.Clamp;
            curve.postWrapMode = WrapMode.Clamp;
        }
        ResourceComponent = transform.gameObject.GetComponentInParent<ResourceComponent>();
        Hull hull = gameObject.GetComponentInParent<Hull>();
        Ship ship = gameObject.GetComponentInParent<Ship>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        i++;
        //Debug.LogError("Heat Timer:" + i);
        if (i < 0)
            return;
        //targetmats = new List<Material>();
        //targetmeshes = target.GetComponentsInChildren<MeshRenderer>();

        //targetmats.Add(targetmesh.material);
        if (ResourceComponent != null)
            glow = ResourceComponent.fillpercentage;
        else
            Debug.LogError("Cannot get Glow component");
        float totalglow = 0;
        foreach (DynamicVisibleParticles particle in targetparticles)
        {
            if (glow > thershold)
                particle.Play();
            else
                particle.Stop();
        }

        //Debug.LogError("Setting up glow at time: " + glow);
        foreach (MeshRenderer targetmesh in targetmeshes)
        {
            foreach (Material mat in targetmesh.materials)
            {
                //Debug.LogError("Processing Glow on " + mat.name);
                for (int i = 0; i < refrence.Length; i++)
                {
                    totalglow += emissivecurves[i].Evaluate(glow);
                }
                //Debug.LogError("Total glow: " + totalglow);

                for (int i = 0; i < refrence.Length; i++)
                {
                    if (mode == Mode.Weight)
                    {
                        float weightedglow = emissivecurves[i].Evaluate(glow) / totalglow;
                        mat.SetFloat(refrence[i], weightedglow);
                        //if (mat.HasProperty(refrence[i]))
                        //    Debug.LogError("Setting (W): " + refrence[i] + " to " + weightedglow);
                    }
                    else
                    {
                        mat.SetFloat(refrence[i], emissivecurves[i].Evaluate(glow));
                        //if (mat.HasProperty(refrence[i]))
                        //    Debug.LogError("Setting: " + refrence[i] + " to " + emissivecurves[i].Evaluate(glow));
                    }
                    //if (mat.HasProperty(refrence[i]))
                    //    Debug.LogError("Attribute: " + refrence[i] + " is " + mat.GetFloat(refrence[i]));

                }
            }
        }



    }
}
      