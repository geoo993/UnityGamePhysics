using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://andrewgotow.files.wordpress.com/2014/08/helicoptertutorial.pdf


public class HelicopterPad : MonoBehaviour {


    public GameObject main_Rotor_GameObject;
    public GameObject tail_Rotor_GameObject1;
    public GameObject tail_Rotor_GameObject2;
    private float max_Rotor_Force = 22241.1081f;
    private float max_Rotor_Velocity = 7200f;
    private float rotor_Velocity = 0.0f;
    private float rotor_Rotation = 0.0f;
    private float max_tail_Rotor_Force = 25000.0f;
    private float max_Tail_Rotor_Velocity = 2200.0f;
    private float tail_Rotor_Velocity = 0.0f;
    private float tail_Rotor_Rotation = 0.0f;
    private float forward_Rotor_Torque_Multiplier = 3.5f;
    private float sideways_Rotor_Torque_Multiplier = 0.5f;
    public bool main_Rotor_Active = true;
    public bool tail_Rotor_Active = true;

    private Rigidbody rigidBody {
        get {
            return GetComponent < Rigidbody>();
        }
    }
    
    private AudioSource audioSource {
        get {
            return GetComponent<AudioSource>();
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
    
        /*
            Now we compute the "controlTorque", or
            the amount of torque applied to the body of the helicopter based on the player input.
            This is done to simulate the varying angle of attack on the helicopter blades, without
            having to do too much extra math.
            The reason the Y value is set to 1.0, is because we want to simulate the torque 
            on the body created by the spinning of the rotors. This is the easiest way to apply 
            that force without adding too much extra code.
        */
        Vector3 torqueValue = Vector3.zero;
        Vector3 controlTorque  = new Vector3(
            Input.GetAxis("Vertical") * forward_Rotor_Torque_Multiplier,
            1.0f,
            -Input.GetAxis( "Horizontal2" ) * sideways_Rotor_Torque_Multiplier
        );
		
        /*
            Now if the main rotor is active, then we wish to apply the net torque to the helicopter
            body as well as the lift force created by the spinning rotors
        
        */
        if ( main_Rotor_Active == true ) {
            torqueValue += (controlTorque * max_Rotor_Force * rotor_Velocity);
            rigidBody.AddRelativeForce( Vector3.up * max_Rotor_Force * rotor_Velocity );
        }

        /*
            you can also add a stabilizing force to the body to make it slowly
            level out. This is important to keep the helicopter level and easy to control. The easiest
            way to do this is by using the Quaterinion.Slerp() function. This function simply
            spherically interpolates the rotation of the helicopter, from its current value to its
            rotation without the X and Z components rotations, essentially leveling it out while still
            keeping the original heading.
        
        */
        bool IsRollLimit = (Vector3.Angle(Vector3.up, transform.up) < 80.0f);
        if ( IsRollLimit ) {
            transform.rotation = Quaternion.Slerp( 
            transform.rotation,  Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y, 0.0f ), 
            Time.deltaTime * rotor_Velocity * 2.0f );
        }

        /*
            Finally, we need to apply the tail rotor's force to the net torque, and apply the torque to
            the helicopter body. So we check if the tail rotor is active, and then subtract the rotor's
            maximum force multiplied by it's throttle from the net torque and apply it relative to the
            body of the helicopter.
            
        */
        if ( tail_Rotor_Active == true ) {
            torqueValue -= (Vector3.up * max_tail_Rotor_Force * tail_Rotor_Velocity);
        }

        rigidBody.AddRelativeTorque(torqueValue);
        

	}


    void Update()
    {
        /*
             we will write all of the responses to control input the user gives. First,
             we can animate the rotors. All this does, is spin the rotor gameObjects 
             based on an ever increasing value.
        */
        
        if ( main_Rotor_Active == true ) {
            main_Rotor_GameObject.transform.rotation = transform.rotation * Quaternion.Euler(-90.0f, 0.0f, rotor_Rotation);
        }
        
        if ( tail_Rotor_Active == true ) {
            tail_Rotor_GameObject1.transform.rotation = transform.rotation * Quaternion.Euler(0.0f, 90.0f, tail_Rotor_Rotation);
            tail_Rotor_GameObject2.transform.rotation = transform.rotation * Quaternion.Euler(0.0f, 90.0f, tail_Rotor_Rotation + 50.0f);
        }
        
        rotor_Rotation += max_Rotor_Velocity * rotor_Velocity * Time.deltaTime;
        tail_Rotor_Rotation += max_Tail_Rotor_Velocity * rotor_Velocity * Time.deltaTime;


        /*
            In order to find the main rotor minimum velocity, we simply do the math to find
            how much force is necessary to counteract the force of gravity, and then divide by the
            maximum force of the rotors. This will result in a value from 0.0 to 1.0, that is the
            minimum throttle needed to keep the helicopter stationary.
        
        */
        float hover_Rotor_Velocity = ( rigidBody.mass * Mathf.Abs( Physics.gravity.y ) / max_Rotor_Force);

        /*
            The tail rotor velocity can also be easily solved for. Because the sole purpose of the
            tail rotor is to counteract the torque of the main rotor, just find the amount of torque
            created by the main rotor, and use that to determine the tail rotor throttle.
        
        */
        float hover_Tail_Rotor_Velocity = (max_Rotor_Force * rotor_Velocity) / max_tail_Rotor_Force;

        /*
            Now, if the player is pressing the key to increase the rotor throttle, then increase the
            throttle of the main rotor, otherwise, slowly interpolate it back to the hover velocity, so
            that it will hover in place. And lastly, set the tail rotor velocity to the minimum velocity,
            and then increase or decrease it by the player input. This will make the tail rotor spin
            slower or faster when the player presses "Left" or "Right", making it apply an
            unbalanced torque and spinning the helicopter in that direction.
            
        */
        
        /*
            Lastly we limit the rotor velocity to a value between 0.0 and 1.0, just to make sure we
            can not possibly apply a force greater than either.
        
        */
        
        if ( Input.GetAxis( "Vertical2" ) > 0.0f || Input.GetAxis( "Vertical2" ) < 0.0f ) {
            rotor_Velocity += Input.GetAxis("Vertical2") * 0.001f;
        }else{
            rotor_Velocity = Mathf.Lerp( rotor_Velocity,
            hover_Rotor_Velocity,
            Time.deltaTime * Time.deltaTime * 5);
        }



        tail_Rotor_Velocity = (hover_Tail_Rotor_Velocity - (Input.GetAxis("Horizontal") * 1.2f) );
        
        if ( rotor_Velocity > 1.0f ) {
            rotor_Velocity = 1.0f;
        }else if ( rotor_Velocity < 0.0f ) {
            rotor_Velocity = 0.0f;
        }

		//print("rotor Velocity "+ rotor_Velocity +", Vertical " + Input.GetAxis("Vertical") + ",   Vertical2 " + Input.GetAxis("Vertical2") + ",  Horizontal " + Input.GetAxis("Horizontal") + ",   Horizontal2 " + Input.GetAxis("Horizontal2"));

        
        /*
            Set the audio clip to whatever sound you wish to make your
            helicopter play, then set play on awake, and loop to true. This will make the audio
            source play the helicopter engine sound constantly throughout the entire game.
            Now simply add a new line in the Update function that says
            and your audio should increase and decrease in pitch depending on the speed at
            which the rotors are spinning.
        */
        audioSource.pitch = rotor_Velocity;

    }
}
