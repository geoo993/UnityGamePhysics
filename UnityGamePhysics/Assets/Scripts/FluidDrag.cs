using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidDrag : MonoBehaviour {

    [Range(1.0f, 2.0f)]
    public float velocityExponent = 1.0f; // the Stokes drag fluid power (U) at low (1.0f) or higher (2.0f) velocity
    public float dragConstant = 1.0f; // CD is the drag coefficient – a dimensionless number.
    public float angularDragConstant = 1.0f;
    
    private PhysicsEngine physicsEngine;
    
    void Start () {
        physicsEngine = GetComponent<PhysicsEngine>();
    }
   
	// happens more than once per frame if the fixed time step is less than the actual frame update time
	void FixedUpdate () {

        Drag();
        AngularDrag();
        
	}
    
    // https://en.wikipedia.org/wiki/Stokes_flow
    // https://en.wikipedia.org/wiki/Drag_(physics)
    float CalculateDrag(float speed){
        return dragConstant * Mathf.Pow(speed, velocityExponent);
    }
    
    float CalculateAngularDrag(float speed){
        return angularDragConstant * Mathf.Pow(speed, velocityExponent);
    }
    
    void Drag(){
        Vector3 velocityVector = physicsEngine.velocityVector;// we know how fast the object is traveling through the velocity
        float speed = velocityVector.magnitude;
        float drag = CalculateDrag(speed);
        Vector3 dragVector = -drag * velocityVector.normalized; // drag in the opposite direction
        
        physicsEngine.AddForce(dragVector);
    
    }
    
    void AngularDrag(){
        Vector3 angularVelocityVector = physicsEngine.angularVelocityVector;// we know how fast the object is traveling through the velocity
        float speed = angularVelocityVector.magnitude;
        float drag = CalculateAngularDrag(speed);
        Vector3 dragVector = -drag * angularVelocityVector.normalized; // drag in the opposite direction
        
        physicsEngine.AddTorque(dragVector);
        
        //print("angular vel "+ angularVelocityVector + ",     ang dragVector "+dragVector);
    }
    
}
