using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : MonoBehaviour {


    public float thrusterStrength;
    
    private float ceiling = 10.0f;
    private float jumpStrength = 5.0f;
    
    private GameObject parent {
        get{
            return transform.parent.gameObject;
        }
    }
    private Rigidbody rigidBody{
        get
        {
            return parent.GetComponent<Rigidbody>();
        }
    }
    
    private float thrusterDistance{
        get {
            return parent.GetComponent<HoverController>().distanceFromGround;
        }
    }
    
    void FixedUpdate () 
    {
    
        RaycastHit hit;
      
        Vector3 downwardForce;
        float distancePercentage;


        if (Physics.Raycast (transform.position, transform.up * -1.0f, out hit, thrusterDistance)) {

            // the thruster is within thrusterDistance to the ground. How far away?
            distancePercentage = 1.0f - (hit.distance / thrusterDistance);

            //calculate how much force to push:
            downwardForce = transform.up * thrusterStrength * distancePercentage;

            //correct the force for the mass of the car and deltatime:
            downwardForce = rigidBody.mass * downwardForce * Time.deltaTime ;


            //apply the force where the thruster is :
            rigidBody.AddForceAtPosition(downwardForce, transform.position);
            
            
            if(Input.GetButtonDown("Jump")){

                
				
				float upForce = 1.0f - Mathf.Clamp(rigidBody.transform.position.y / ceiling, 0.0f, 1.0f);
				upForce = Mathf.Lerp(0.0f, jumpStrength, upForce) * rigidBody.mass;
				
				Vector3 jumpForce = Physics.gravity * upForce * Time.deltaTime ;
				rigidBody.AddForceAtPosition(- jumpForce, transform.position, ForceMode.Impulse);
				
            }

            
        }else{
        
        }
        

    }

}
