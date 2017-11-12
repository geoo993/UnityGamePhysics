using UnityEngine;
using System.Collections;

public class HoverController : MonoBehaviour {

	//values that controls the vehicle
	public float acceleration, rotationRate;

	//values for faking a nice turn display;
	public float turnRotationAngle, turnRotationSeekSpeed;

    public float distanceFromGround;

	//reference variable we don't directly use:
	private float rotationVelocity, groundAngleVelocity;

	private Rigidbody rigidBody{
        get{
            return GetComponent<Rigidbody> ();
        }
    }



	void Awake () 
	{
		rigidBody.drag = 1f;
		rigidBody.angularDrag = 1f;

	}


	void FixedUpdate ()
	{


		//check if we are touching the ground:
		if (Physics.Raycast (transform.position, transform.up * -1.0f, distanceFromGround)) {
			////we are on the ground; enable the accelartor and increase drag:
			rigidBody.drag = 1;

			////calculate forward force:
			Vector3 forwardForce = transform.forward * acceleration * Input.GetAxis ("Vertical");

			////correct the force for deltatime and vehical mass:
			forwardForce = forwardForce * Time.deltaTime * rigidBody.mass;

			rigidBody.AddForce (forwardForce);

		} else {
			//we aren't on the ground and dont want to just halt in the mid-air; reduce drag:
			rigidBody.drag = 0;
		}


		////you can turn in the air or on the ground:
		Vector3 turnTorque = Vector3.up * rotationRate * Input.GetAxis ("Horizontal");

		////correct the force for deltatime and vehicle mass:
		turnTorque =turnTorque * Time.deltaTime * rigidBody.mass;
		rigidBody.AddTorque (turnTorque);


		////"Fake" rotate the car when you are turning:
		Vector3 newRotation = transform.eulerAngles;
		float zRotation = Mathf.SmoothDampAngle (
			newRotation.z, 
            Input.GetAxis ("Horizontal") * -turnRotationAngle,
			ref rotationVelocity, 
            turnRotationSeekSpeed
        );
		newRotation = new Vector3 (newRotation.x, newRotation.y, zRotation);
		transform.eulerAngles = newRotation;



	}
}
