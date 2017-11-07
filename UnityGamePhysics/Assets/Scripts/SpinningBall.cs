using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningBall : MonoBehaviour {

    public Vector3 velocity;                    // [v]
    public Vector3 angularVelocity;             // [w]
    
    private Rigidbody rigidBody;
    
	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.velocity = velocity;
        rigidBody.angularVelocity = angularVelocity;
	}
	
}
