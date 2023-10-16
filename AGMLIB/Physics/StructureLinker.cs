using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ships;
using System.Reflection;
using Game.Sensors;
using Game.Intel;
using Game.Map;
using Game;
using Utility;

public class StructureLinker : MonoBehaviour
{
    public List<GameObject> linked;
    bool broken;

    // Start is called before the first frame update
    void Start()
    {
        broken = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.childCount == 0 && broken == false)
        {
            breakjoints();
        }
        else if(broken == false)
        {
            HullComponent strut = transform.GetChild(0).GetComponent<HullComponent>();
            if(strut.IsFunctional == false)
            {
                breakjoints();
            }
        }
    }

    void breakjoints()
    {
        foreach (GameObject child in linked)
        {
            Rigidbody newbody = child.AddComponent<Rigidbody>();
            newbody.useGravity = false;
        }
        broken = true;
    }
}
