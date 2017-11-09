using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddAngularDrag : MonoBehaviour {

	 [Range(1.0f, 2.0f)]
    public float velocityExponent = 1.0f; // the Stokes drag fluid power (U) at low (1.0f) or higher (2.0f) velocity
    public float angularDragConstant = 1.0f; // CD is the drag coefficient – a dimensionless number.
    
    private Rigidbody rigidBody;
    
    void Start () {
        rigidBody = GetComponent<Rigidbody>();
    }
   
    // happens more than once per frame if the fixed time step is less than the actual frame update time
    void FixedUpdate () {

        AngularDrag();
    }
    
    float CalculateAngularDrag(float speed){
        return angularDragConstant * Mathf.Pow(speed, velocityExponent);
    }
    
    void AngularDrag(){
        Vector3 velocityVector = rigidBody.angularVelocity;// we know how fast the object is traveling through the velocity
        float speed = velocityVector.magnitude;
        float drag = CalculateAngularDrag(speed);
        Vector3 angularDragVector = -drag * velocityVector.normalized * Time.deltaTime; // drag in the opposite direction
        
        rigidBody.AddTorque(angularDragVector);
        
        
    }
}
