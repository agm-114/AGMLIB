using Ships;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    public GameObject body;
    public GameObject barrel;
    public GameObject verticaloffset;
    public GameObject slider;
    public GameObject sholder;
    public GameObject elbow;
    public GameObject wrist;
    public GameObject grabber;
    public TurretController TurretController;
    public TurretedDiscreteWeaponComponent TurretComponent;

    public Rigidbody hammer;
    float liftrate = 0.02f;
    float armrate = 0.1f;
    Quaternion elevation = Quaternion.Euler(Vector3.zero);
    Quaternion azimuth = Quaternion.Euler(Vector3.zero);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.LogError("Melee Check");

        if (TurretComponent.CurrentlyFiring && TurretController.DistanceToTarget < 250)
        {
            if(verticaloffset.transform.localPosition.y > 4 )
            {
                if (UnityEngine.Random.value < .01f && TurretController.DistanceToTarget < 50) // 45% of the time
                {
                    //Debug.LogError("Target within melee range: " + TurretController.DistanceToTarget);
                    elevation = barrel.transform.localRotation * Quaternion.Euler(new Vector3(180 * (UnityEngine.Random.value - 0.5f), 0, 0));
                    azimuth = body.transform.localRotation * Quaternion.Euler(new Vector3(0, 180 * (UnityEngine.Random.value - 0.5f), 0));
                }
                else
                {
                    elevation = Quaternion.RotateTowards(elevation, barrel.transform.localRotation, armrate * 4); //(barrel.transform.localEulerAngles + sholder.transform.localEulerAngles)/2;
                    azimuth = Quaternion.RotateTowards(azimuth, body.transform.localRotation, armrate * 4);
                }
                elbow.transform.localRotation = Quaternion.RotateTowards(elbow.transform.localRotation, Quaternion.Euler(Vector3.right * -179), armrate);
                wrist.transform.localRotation = Quaternion.RotateTowards(wrist.transform.localRotation, Quaternion.Euler(Vector3.right * 179), armrate);
                slider.transform.localRotation = Quaternion.RotateTowards(slider.transform.localRotation, azimuth, armrate * 4);//(body.transform.localEulerAngles + slider.transform.localEulerAngles)/2;
                sholder.transform.localRotation = Quaternion.RotateTowards(sholder.transform.localRotation, elevation, armrate * 4); //(barrel.transform.localEulerAngles + sholder.transform.localEulerAngles)/2;
                grabber.transform.localRotation = Quaternion.RotateTowards(grabber.transform.localRotation, Quaternion.Euler(Vector3.right * -89), armrate);
            }
            else 
            {
                verticaloffset.transform.localPosition = new Vector3(0, verticaloffset.transform.localPosition.y + liftrate, 0);
            }
        }
        else
        {
            if (elbow.transform.localRotation == Quaternion.Euler(Vector3.zero) && verticaloffset.transform.localPosition.y > 2)
            {
                verticaloffset.transform.localPosition = new Vector3(0, verticaloffset.transform.localPosition.y - liftrate, 0);
            }
            //Debug.LogError("Target not within range: " + TurretController.DistanceToTarget);
            //Debug.LogError("Target within range: " + TurretController.DistanceToTarget);
            elbow.transform.localRotation = Quaternion.RotateTowards(elbow.transform.localRotation, Quaternion.Euler(Vector3.zero), armrate);
            wrist.transform.localRotation = Quaternion.RotateTowards(wrist.transform.localRotation, Quaternion.Euler(Vector3.zero), armrate);
            slider.transform.localRotation = Quaternion.RotateTowards(slider.transform.localRotation, Quaternion.Euler(Vector3.zero), armrate * 4);//(body.transform.localEulerAngles + slider.transform.localEulerAngles)/2;
            sholder.transform.localRotation = Quaternion.RotateTowards(sholder.transform.localRotation, Quaternion.Euler(Vector3.zero), armrate * 4); //(barrel.transform.localEulerAngles + sholder.transform.localEulerAngles)/2;
            grabber.transform.localRotation = Quaternion.RotateTowards(grabber.transform.localRotation, Quaternion.Euler(Vector3.zero), armrate);
        }



        //Debug.LogError(elbow.transform.localEulerAngles.x);

        grabber.transform.localPosition = Vector3.zero;
        //grabber.transform.localRotation *= Quaternion.Euler(new Vector3(20, 0, 0));
        hammer.angularVelocity = Vector3.zero;
    }


}
