using Ships;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utility;

public class DynamicTransform : ActiveSettings
{
    // Start is called before the first frame update
    [Space]
    [Header("Animation Settings")]
    [Space]
    public Transform Start;
    public Transform End;
    public Transform Target;
    public AnimationCurve AnimationCurve = new();

    public float SlerpVlaue => Mathf.Clamp(AnimationCurve.Evaluate(accum), 0, 1);
    public float EndValue => 1 - SlerpVlaue;
    private float accum = 0f;
    //public GameObject target;


    // Update is called once per frame
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if(active)
            accum += Time.deltaTime;
        else
            accum -= Time.deltaTime;
        if(accum < 0f)
            accum = 0f;
        Target.transform.position = Vector3.Slerp(Start.transform.position, End.transform.position, SlerpVlaue);
        Target.transform.rotation = Quaternion.Slerp(Start.transform.rotation, End.transform.rotation, SlerpVlaue);
    }
}
