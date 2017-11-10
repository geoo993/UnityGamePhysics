using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterRotorController : MonoBehaviour {

    public enum Axis
    {
        X,
        Y,
        Z,
    }
    
    /*
        the hovering state is simply the state where the helicopter is not changing location. 
        While in the hovering state, the helicopter is allowed to pivot on its axis 
        and still be considered to be hovering.
    */
    
    public Axis rotateAxis;
    
    private float _rotarSpeed;
    public float rotarSpeed
    {
        get { return _rotarSpeed; }
        set { _rotarSpeed = Mathf.Clamp(value, 0, 3000); }
    }

    private float rotateDegree;
    private Vector3 originalRotation; // origin rotation Vector3(0, 0, 0)

    void Start ()
    {
        originalRotation = transform.localEulerAngles;
    }

    void Update ()
    {
        rotateDegree += rotarSpeed * Time.deltaTime;
        rotateDegree = rotateDegree % 360.0f;  // keep rane between 0 and 360 degrees
        
        // rotate along the axis
        switch (rotateAxis)
        {
            case Axis.Y:
                transform.localRotation = Quaternion.Euler(originalRotation.x, rotateDegree, originalRotation.z);
                break;
            case Axis.Z:
                transform.localRotation = Quaternion.Euler(originalRotation.x, originalRotation.y, rotateDegree);
                break;
            default:
                transform.localRotation = Quaternion.Euler(rotateDegree, originalRotation.y, originalRotation.z);
                break;
        }
    }
}