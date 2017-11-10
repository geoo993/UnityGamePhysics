﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ShowStats : MonoBehaviour {

    private Rigidbody rigidBody;
    
	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
    
		Debug.Log(name + " inertia tensor "+rigidBody.inertiaTensor + ", inertia tensor magnitude "+ rigidBody.inertiaTensor.magnitude + ", inertia tensor rotation "+ rigidBody.inertiaTensorRotation + ", center of mass "+rigidBody.centerOfMass);
	}
}
