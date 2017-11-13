using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMovement : MonoBehaviour
{
    private Rigidbody rigidBody
    {
        get
        {
            return GetComponent<Rigidbody>();
        }
    }

    private AudioSource audioSource
    {
        get
        {
            return GetComponent<AudioSource>();
        }
    }
    private bool shouldPlayAudio = true;
    
    public RotorController frontLeftRotor;
    public RotorController frontRightRotor;
    public RotorController backLeftRotor;
    public RotorController backRightRotor;

    private float RotorRotationSpeed = 200.0f;

    // lift
    private float _engineForce;
    public float EngineForce
    {
        get { return _engineForce; }
        set
        {
            frontLeftRotor.rotarSpeed = value * RotorRotationSpeed;
            frontRightRotor.rotarSpeed = value * RotorRotationSpeed;
            backLeftRotor.rotarSpeed = value * RotorRotationSpeed;
            backRightRotor.rotarSpeed = value * RotorRotationSpeed;

            if (shouldPlayAudio){
                audioSource.Play();
                shouldPlayAudio = false;
            }
            audioSource.pitch = Mathf.Clamp(value / 30.0f, 0, 2.0f);
            TempCeiling = value + 50.0f;
            _engineForce = value;
        }
    }

    [SerializeField, Range(10.0f, 100.0f) ]
    private float RateOfClimb = 30.0f;
    private float Ceiling = 400.0f;
    private float TempCeiling;
    
    // pitch and trust
    private float ForwardMovementSpeed = 500.0f;
    private float ForwardVelocity;
    private float ForwardPercentage = 20.0f;

    // yaw and rotation
    private float CurrentYaw;
    private float YawVelocity;
    private float YawPercentage = 2.5f;
    
    // roll
    private float RollVelocity;
    private float RollPercentage = 300.0f;
    
    // limit forces
    private Vector3 LimitForcesSmoothDampToZero;
    
    [SerializeField, Range(20.0f, 100.0f) ]
    private float TopSpeed = 60.0f;
    private float SmoothRate = 5.0f; // lower number move drone slower, and higher number moves drone faster
    
    [HideInInspector] public float pitch, yaw, roll;
    
    private Dictionary<string, KeyCode> movementKeyBindings = new Dictionary<string, KeyCode>()
    {
        { "FORWARD", KeyCode.W },
        { "BACKWARD", KeyCode.S },
        { "TILTLEFT", KeyCode.A },
        { "TILTRIGHT", KeyCode.D },
        { "UP", KeyCode.I },
        { "DOWN", KeyCode.K },
        { "LEFT", KeyCode.J },
        { "RIGHT", KeyCode.L }
    };
    
    float Mass(){
        return rigidBody.mass;
    }
    
    void Lift()
    {
     
        if (Input.GetKey(this.movementKeyBindings["UP"])) {
            EngineForce += RateOfClimb * Time.fixedDeltaTime;  //0.1f;
        }
        
        if (Input.GetKey(this.movementKeyBindings["DOWN"]))
        {
            EngineForce -= RateOfClimb * Time.fixedDeltaTime; //0.12f;
            if (EngineForce < 0) { EngineForce = 0; }
        } 
        
        if (TempCeiling >= (Ceiling - 50.0f)) { TempCeiling = Ceiling;  } 
        float upForce = 1.0f - Mathf.Clamp(rigidBody.transform.position.y / TempCeiling, 0.0f, 1.0f);
        upForce = Mathf.Lerp(0.0f, EngineForce, upForce) * Mass();
        
        rigidBody.AddRelativeForce(Vector3.up * upForce);
    }
    
	void Thrust()
	{
		if (Input.GetAxis("Vertical2") > 0.0f || Input.GetAxis("Vertical2") < 0.0f){ // not zero
			rigidBody.AddRelativeForce(Vector3.forward * Input.GetAxis("Vertical2") * ForwardMovementSpeed);
		}
	}
    
    void Movements(){

        Lift();
        Thrust();
    }

    void Pitch(){
        if (Input.GetAxis("Vertical2") > 0.0f || Input.GetAxis("Vertical2") < 0.0f){
            pitch = Mathf.SmoothDamp(pitch, ForwardPercentage * Input.GetAxis("Vertical2"), ref ForwardVelocity, 0.1f );
        }
    }

    void Yaw(){
       
        if(Input.GetKey(this.movementKeyBindings["LEFT"]))
        {
            yaw -= YawPercentage;
        }

        if(Input.GetKey(this.movementKeyBindings["RIGHT"]))
        {
            yaw += YawPercentage;
        }

        CurrentYaw = Mathf.SmoothDamp(CurrentYaw, yaw, ref YawVelocity, 0.25f);
    }
    
	void Roll(){
		if (Mathf.Abs(Input.GetAxis("Horizontal2")) > 0.2f){
			rigidBody.AddRelativeForce(Vector3.right * RollPercentage * Input.GetAxis("Horizontal2") );
			roll = Mathf.SmoothDamp(roll, -ForwardPercentage * Input.GetAxis("Horizontal2"), ref RollVelocity, 0.1f);
		}else{
			roll = Mathf.SmoothDamp(roll, 0.0f, ref RollVelocity, 0.1f);
		}
	}
    
    void Rotations(){
        
        Pitch();
        Yaw();
        Roll();
        
        Vector3 rotation = new Vector3(pitch, yaw, roll);
        rigidBody.rotation = Quaternion.Euler(rotation);
    }
    
    
    void LimitForces(){
        if (Mathf.Abs(Input.GetAxis("Vertical2")) > 0.2f && Mathf.Abs(Input.GetAxis("Horizontal2")) > 0.2f ){
            rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, Mathf.Lerp(rigidBody.velocity.magnitude, TopSpeed, SmoothRate * Time.deltaTime));
        }
        
        if (Mathf.Abs(Input.GetAxis("Vertical2")) > 0.2f && Mathf.Abs(Input.GetAxis("Horizontal2")) < 0.2f ){
            rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, Mathf.Lerp(rigidBody.velocity.magnitude, TopSpeed, SmoothRate * Time.deltaTime));
        }
        
        if (Mathf.Abs(Input.GetAxis("Vertical2")) < 0.2f && Mathf.Abs(Input.GetAxis("Horizontal2")) > 0.2f ){
            rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, Mathf.Lerp(rigidBody.velocity.magnitude, TopSpeed * 0.5f, SmoothRate * Time.deltaTime));
        }
        
        if (Mathf.Abs(Input.GetAxis("Vertical2")) < 0.2f && Mathf.Abs(Input.GetAxis("Horizontal2")) < 0.2f ){
            rigidBody.velocity = Vector3.SmoothDamp(rigidBody.velocity, Vector3.zero, ref LimitForcesSmoothDampToZero, 0.95f );
        }
    }
    
   
    void Start()
    {
        rigidBody.inertiaTensor = new Vector3(Mass(), Mass(), Mass());
        audioSource.Stop();
    }

    public void FixedUpdate()
    {
        Movements();
        Rotations();
        
        LimitForces();
        
    }
    
}