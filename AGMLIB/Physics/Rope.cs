using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public GameObject root;
    public GameObject target;
    public GameObject hinge;
    public GameObject tail;
    public float drag;
    public LineRenderer visualline;
    //public LineRenderer tacviewline;
    private List<RopeSegment> ropeSegments = new List<RopeSegment>();
    private float ropeSegLen;
    public int segments = 10;
    public int Length = 10;
    private float lineWidth = 0.1f;


    // Use this for initialization
    void Start()
    {
        //this.lineRenderer = this.GetComponent<LineRenderer>();
        //Vector3 ropeStartPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 ropeStartPoint = target.transform.position;
        ropeSegLen = Length / segments;
        for (int i = 0; i < segments; i++)
        {
            this.ropeSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.z -= ropeSegLen;
        }
        tail.transform.parent = (new GameObject("Sensor Holder")).transform;
    }

    // Update is called once per frame
    void Update()
    {
        this.DrawRope();
    }

    private void FixedUpdate()
    {
        this.Simulate();
        RopeSegment lastrope = this.ropeSegments[this.segments - 1];

    }

    private void Simulate()
    {
        // SIMULATION
        Vector3 forceGravity = new Vector3(0f, 0f, 0f);

        for (int i = 1; i < this.segments; i++)
        {
            RopeSegment firstSegment = this.ropeSegments[i];
            Vector3 velocity = firstSegment.posNow - firstSegment.posOld;
            velocity = drag * velocity;
            firstSegment.posOld = firstSegment.posNow;
            firstSegment.posNow += velocity;
            firstSegment.posNow += forceGravity * Time.fixedDeltaTime;
            this.ropeSegments[i] = firstSegment;
        }

        //CONSTRAINTS
        for (int i = 0; i < 10; i++)
        {
            this.ApplyConstraint();
        }
    }

    private void ApplyConstraint()
    {
        //Constrant to Mouse
        RopeSegment firstSegment = this.ropeSegments[0];
        //RopeSegment lastsegment = this.ropeSegments[this.segmentLength - 1];
        firstSegment.posNow = target.transform.position;
        //lastsegment.posNow = tail.transform.position;
        this.ropeSegments[0] = firstSegment;
        //this.ropeSegments[this.segmentLength - 1] = lastsegment;

        for (int i = 0; i < this.segments - 1; i++)
        {
            RopeSegment firstSeg = this.ropeSegments[i];
            RopeSegment secondSeg = this.ropeSegments[i + 1];

            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float error = Mathf.Abs(dist - this.ropeSegLen);
            Vector3 changeDir = Vector3.zero;

            if (dist > ropeSegLen)
            {
                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            }
            else if (dist < ropeSegLen)
            {
                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
            }

            Vector3 changeAmount = changeDir * error;
            if (i != 0)
            {
                firstSeg.posNow -= changeAmount * 0.5f;
                this.ropeSegments[i] = firstSeg;
                secondSeg.posNow += changeAmount * 0.5f;
                this.ropeSegments[i + 1] = secondSeg;
            }
            else
            {
                secondSeg.posNow += changeAmount;
                this.ropeSegments[i + 1] = secondSeg;
            }
        }

        firstSegment.posNow = target.transform.position;
        
        //lastsegment.posNow = tail.transform.position;
        //tail.transform.position = lastsegment.posNow;
        this.ropeSegments[0] = firstSegment;
       
        tail.transform.position = this.ropeSegments[this.segments - 1].posNow;
        Quaternion rotation = Quaternion.LookRotation((this.ropeSegments[this.segments - 2].posNow - this.ropeSegments[this.segments - 1].posNow)* -1, tail.transform.TransformDirection(Vector3.up));
        tail.transform.rotation = rotation;//Vector3.ProjectOnPlane(this.ropeSegments[0].posNow - this.ropeSegments[1].posNow, tail.transform.parent.TransformDirection(Vector3.right))
        rotation = Quaternion.LookRotation(-1 * Vector3.ProjectOnPlane(this.ropeSegments[0].posNow - this.ropeSegments[1].posNow, root.transform.TransformDirection(Vector3.right)), tail.transform.TransformDirection(Vector3.up));
        hinge.transform.rotation = rotation;
        //this.ropeSegments[this.segmentLength - 1] = lastsegment;
    }

    private void DrawRope()
    {
        float lineWidth = this.lineWidth;
        visualline.startWidth = lineWidth;
        visualline.endWidth = lineWidth;
        //tacviewline.startWidth = 2;
        //tacviewline.endWidth = 2;

        Vector3[] ropePositions = new Vector3[this.segments];
        for (int i = 0; i < this.segments; i++)
        {
            ropePositions[i] = this.ropeSegments[i].posNow;
        }

        visualline.positionCount = ropePositions.Length;
        visualline.SetPositions(ropePositions);
        //tacviewline.positionCount = ropePositions.Length;
        //tacviewline.SetPositions(ropePositions);
    }

    public struct RopeSegment
    {
        public Vector3 posNow;
        public Vector3 posOld;

        public RopeSegment(Vector3 pos)
        {
            this.posNow = pos;
            this.posOld = pos;
        }
    }
}