using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AddDrag : MonoBehaviour {

	 [Range(1.0f, 2.0f)]
    public float velocityExponent = 1.0f; // the Stokes drag fluid power (U) at low (1.0f) or higher (2.0f) velocity
    public float dragConstant = 1.0f; // CD is the drag coefficient – a dimensionless number.
    
    private Rigidbody rigidBody;
    
    void Start () {
        rigidBody = GetComponent<Rigidbody>();
    }
   
    // happens more than once per frame if the fixed time step is less than the actual frame update time
    void FixedUpdate () {

        Drag();
    }
    
    float CalculateDrag(float speed){
        return dragConstant * Mathf.Pow(speed, velocityExponent);
    }
    
    void Drag(){
        Vector3 velocityVector = rigidBody.velocity;// we know how fast the object is traveling through the velocity
        float speed = velocityVector.magnitude;
        float drag = CalculateDrag(speed);
        Vector3 dragVector = -drag * velocityVector.normalized * Time.deltaTime; // drag in the opposite direction
        
        //rigidBody.velocity = rigidBody.velocity * ( 1 - Time.deltaTime * drag);
        rigidBody.AddForce(dragVector);
        //print(rigidBody.drag);
        
    }
}
