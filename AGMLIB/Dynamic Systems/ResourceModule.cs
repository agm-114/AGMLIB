﻿using Game.Units;
using Ships;
using Ships.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Game.CapturableFlag;

public class ResourceModule : ActiveSettings
{
    public enum GenerationScaling
    {
        None,
        ModuleSize,
        ShipSize,
    }
    [Space]
    [Header("Component Specfic Settings")]
    [Space]
    public HullSocketType Type;

    public GenerationScaling scalingmode = GenerationScaling.None;

    public float generation = 1;
    public float scaling = 1;
    public float capacity = 10;

    void Start()
    {
        //Destroy(this);
        if (capacity != 0)
        {
            //Debug.Log("Resource Module modifying capacity");
            base.ResourceComponent.capacity += capacity;
        }
        //Debug.LogError("Module? " + gameObject.name + base.active + " Generating: START");
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        
        if (!base.active)
        {
            //Debug.LogError("Module? " + gameObject.name + " " + base.active + " Generating: UPDATE");
            //Debug.LogError(ShipController.Throttle);
            return;
        }
            
            

        float size;
        if(false && scalingmode == GenerationScaling.ModuleSize)
        {
            foreach (HullComponent part in base.ShipController.gameObject.GetComponentsInChildren<HullComponent>())
            {
                HullSocket partsocket = part.gameObject.transform.parent.gameObject.GetComponent<HullSocket>();
                if(partsocket.Type == Type)
                {
                    //Debug.LogError("Scaling heat based on: " + part.gameObject.name);
                    //size = partsocket.Size.x * partsocket.Size.y * partsocket.Size.y;

                }


            }
        }
        else
        {
            size = 1;
        }
        
        base.ResourceComponent.currentamount = base.ResourceComponent.currentamount += generation * size * scaling * Time.deltaTime;
    }

}
