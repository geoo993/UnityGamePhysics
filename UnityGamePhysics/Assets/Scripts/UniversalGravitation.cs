using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalGravitation : MonoBehaviour {

    private PhysicsEngine[] physicsEngineArray;
    private const float gravitationalConstant = 6.673e-11f ;  // [N.m2/kg2 (where N is Newtons)  ]
    
    void Start () {
        physicsEngineArray = FindObjectsOfType<PhysicsEngine>();
    }
   	
	// happens more than once per frame if the fixed time step is less than the actual frame update time
    void FixedUpdate()
    {
        CalculateGravity();
    }
    
    void CalculateGravity(){
        // based on https://en.wikipedia.org/wiki/Gravitational_constant
        // were are going to need every pair in every direction to calculate gravity
        // this is to do with newton's third law
        foreach (PhysicsEngine physicsEngineA in physicsEngineArray)
        {
            foreach (PhysicsEngine physicsEngineB in physicsEngineArray)
            {
                if (physicsEngineA && physicsEngineB && physicsEngineA != physicsEngineB && physicsEngineA != this){
                
                    Vector3 distanceOffset = physicsEngineA.transform.position - physicsEngineB.transform.position;
                    float rSquared = Mathf.Pow(distanceOffset.magnitude, 2.0f); // the square of the distance
                    
                    // gravitational constanst formular
                    float gravityMagnitude = gravitationalConstant * physicsEngineA.mass * physicsEngineB.mass / rSquared;
                    
                    Vector3 gravity = gravityMagnitude * distanceOffset.normalized;
                    
                    // calculating a force excerted on A by B
                    // adding a force to the opposite direction of the movement of A
                    physicsEngineA.AddForce(-gravity);
                    
                }
            }
        
        }
        
    
    }
    
    
}
