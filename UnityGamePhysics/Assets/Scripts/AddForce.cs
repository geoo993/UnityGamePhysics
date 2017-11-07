using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhysicsEngine))]
public class AddForce : MonoBehaviour {

    public Vector3 forceVector;
    private PhysicsEngine physicsEngine;
    
	void Start () {
        physicsEngine = GetComponent<PhysicsEngine>();
	}
	
    // happens more than once per frame if the fixed time step is less than the actual frame update time
	void FixedUpdate () {
		physicsEngine.AddForce(forceVector);
	}
}
