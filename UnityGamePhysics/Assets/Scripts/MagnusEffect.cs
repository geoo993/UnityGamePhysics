using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnusEffect : MonoBehaviour {
    
    [Range(0.01f, 10.0f)]
    public float magnusConstant = 1.0f;
    
	private Rigidbody rigidBody;
    
    // Use this for initialization
    void Start () {
        rigidBody = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 magnusEffect = Vector3.Cross(rigidBody.angularVelocity, rigidBody.velocity);
        rigidBody.AddForce(magnusConstant * magnusEffect);
	}
}
