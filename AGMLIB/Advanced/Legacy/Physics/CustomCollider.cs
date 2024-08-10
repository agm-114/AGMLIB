
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCollider : MonoBehaviour
{
    public float impulsefactor = 100;
    //float factor = 100;
    // Start is called before the first frame update
    void OnCollisionEnter(Collision collision)
    {
        //Output the Collider's GameObject's name
        //Debug.LogError("Enter" + collision.collider.name);
        //collision.rigidbody.AddForce(collision.impulse * factor * -1, ForceMode.Impulse);
    }

    private void OnCollisionStay(Collision collision)
    {
        //Debug.LogError("Stay " + collision.collider.name);
        //collision.rigidbody.AddForce(collision.impulse * factor * -1, ForceMode.Impulse);
    }

    private void OnCollisionExit(Collision collision)
    {
        //Debug.LogError("Exit " + collision.collider.name);
        //collision.rigidbody.AddForce(collision.impulse * factor * -1, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        //other.attachedRigidbody.
    }

    private void FixedUpdate()
    {
        //GetComponent<Collider>().
    }
}
