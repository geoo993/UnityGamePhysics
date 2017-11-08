using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour {

    public Vector3 force;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0)){
            GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
        }
        
	}
}
