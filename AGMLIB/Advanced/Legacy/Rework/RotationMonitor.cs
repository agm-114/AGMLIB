using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationMonitor : MonoBehaviour
{
    public GameObject target;
    public Vector3 angle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() => angle = target.transform.eulerAngles;//Debug.LogError("Current Rotation: " + target.transform.eulerAngles);
}
