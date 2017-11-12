using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scooter : MonoBehaviour {

	public GameObject frontRightWheel;
    public WheelCollider frontRightWheelCollider;
    
    public GameObject frontLeftWheel;
    public WheelCollider frontLeftWheelCollider;
    
    public GameObject rearWheel;
    public WheelCollider rearWheelCollider;
    
    public bool reverseTurn;
 
    private Rigidbody rigidBody {
        get {
            return GetComponent<Rigidbody>();
        }
    }
   
    public float maxMotorTorque;
    public float maxSteeringAngle;
 
    // Use this for initialization
    void Start () {

    }

    private void Update()
    {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
        float brakeTorque = Mathf.Abs(Input.GetAxis("Jump"));
        if (brakeTorque > 0.001) {
            brakeTorque = maxMotorTorque;
            motor = 0;
        } else {
            brakeTorque = 0;
        }
        
        
        frontLeftWheelCollider.steerAngle = frontRightWheelCollider.steerAngle = ((reverseTurn) ? -1.0f : 1.0f ) * steering;
            
        frontLeftWheelCollider.motorTorque = frontRightWheelCollider.motorTorque = motor;
        rearWheelCollider.motorTorque = motor;
        
        frontLeftWheelCollider.brakeTorque = frontRightWheelCollider.motorTorque = brakeTorque;
        rearWheelCollider.brakeTorque = brakeTorque;
        
        Quaternion rot;
        Vector3 pos;
        
        frontLeftWheelCollider.GetWorldPose ( out pos, out rot);
        frontLeftWheel.transform.position = pos;
        frontLeftWheel.transform.rotation = rot;
        frontLeftWheel.transform.eulerAngles = new Vector3(frontLeftWheel.transform.eulerAngles.x, frontLeftWheel.transform.eulerAngles.y, frontLeftWheel.transform.eulerAngles.z - 90.0f);
        
        frontRightWheelCollider.GetWorldPose ( out pos, out rot);
        frontRightWheel.transform.position = pos;
        frontRightWheel.transform.rotation = rot;
        frontRightWheel.transform.eulerAngles = new Vector3(frontRightWheel.transform.eulerAngles.x, frontRightWheel.transform.eulerAngles.y, frontRightWheel.transform.eulerAngles.z - 90.0f);
        
        rearWheelCollider.GetWorldPose ( out pos, out rot);
        rearWheel.transform.position = pos;
        rearWheel.transform.rotation = rot;
		rearWheel.transform.eulerAngles = new Vector3(rearWheel.transform.eulerAngles.x, rearWheel.transform.eulerAngles.y, rearWheel.transform.eulerAngles.z - 90.0f);
        
        
        
    }
    
    
    // Update is called once per frame
    void FixedUpdate()
    {

    }
    
    
}
