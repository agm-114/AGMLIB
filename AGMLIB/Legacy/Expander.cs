using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Expander : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localScale.x < 1)
            transform.localScale *= 1.001f;
    }
}
