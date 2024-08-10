using Munitions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StrikeCraft : MonoBehaviour
{

    //LEGACY DO NOT USE
    // Start is called before the first frame update
    public MovementTarget target;
    public FormationManager manager;
    public Rigidbody rb;
    //public DumbfireRocket test;

    public Vector3 thrust = new(1, 1, 1);
    public float P = 0.5f;
    public float I = 0.5f;
    public float D = 0.5f;
    public Vector3 rtarget;
    public Vector3 rcurrent;
    public Vector3 deviation;
    public Vector3 torque = new(0,0,0);
    //protected new Guid myGuid => new System.Guid();

    void Start()
    {
    }

    private void FixedUpdate()
    {
        if(rb.velocity.magnitude < 10)
            rb.AddForce( P * (target.transform.position - transform.position) );
        //var rotation = Quaternion.FromToRotation(manager.transform.forward, transform.forward).eulerAngles;
        //if (rb.angularVelocity.magnitude < 1)
        //    rb.AddTorque(rotation);
        float springStrength = 1;
        float damperStrength = 1;
        var springTorque = springStrength * Vector3.Cross(rb.transform.forward, target.transform.forward);
        var dampTorque = damperStrength * -rb.angularVelocity;
        rb.AddTorque(springTorque + dampTorque, ForceMode.Acceleration);

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
