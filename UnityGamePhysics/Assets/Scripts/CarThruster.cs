using UnityEngine;
using System.Collections;

public class CarThruster : MonoBehaviour {


	public float thrusterStrength, thrusterDistance, jumpStrength;
	public Transform[] thrusters;

	private Rigidbody rigidBody{
        get
        {
            return GetComponent<Rigidbody>();
        }
	}

	void FixedUpdate () 
	{
	
		RaycastHit hit;
		foreach (Transform thruster in thrusters) 
		{
			Vector3 downwardForce;
			float distancePercentage;


			if (Physics.Raycast (thruster.position, thruster.up * -1.0f, out hit, thrusterDistance)) {


				// the thruster is within thrusterDistance to the ground. How far away?
				distancePercentage = 1.0f - (hit.distance/thrusterDistance);

				//calculate how much force to push:
				downwardForce = transform.up * thrusterStrength * distancePercentage;

				//correct the force for the mass of the car and deltatime:
				downwardForce = rigidBody.mass * downwardForce * Time.deltaTime ;


				//apply the force where the thruster is :
				rigidBody.AddForceAtPosition(downwardForce, thruster.position);
                
                if(Input.GetButtonDown("Jump")){
                    Vector3 jumpForce = Physics.gravity * rigidBody.mass * jumpStrength * Time.deltaTime ;
                    rigidBody.AddForceAtPosition(- jumpForce, thruster.position);
                }

			}
		}

	}




}
