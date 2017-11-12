using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://gamedevben.wordpress.com/2016/07/11/tutorial-unity3d-c-helicopter-controller/

// Helper classes that runs everything associated with our helicopter
public class ForceApplicator  
{
    protected Rigidbody _rigidbody;
    protected Vector3 _forceAxis;
    public float MaxForce  
    {
        get;
        protected set;
    }
    public ForceApplicator(Rigidbody _newRigidbody, float _newMaxForce, Vector3 _newForceAxis)  
    {
        _rigidbody = _newRigidbody;
        _forceAxis = _newForceAxis;
        MaxForce = _newMaxForce;
    }
    
    public virtual void ApplyForcePercentage(float _percent)  
    {
        // Apply forces from inherited classes.
    }
    
    protected Vector3 CalculateForce(float _percentMaxForce)  
    {
        return _forceAxis * (MaxForce * _percentMaxForce);
    }
}

public class RotationalForceApplicator: ForceApplicator
{
    public float PercentMaxRPM  
    {
        get
        {
            return _rigidbody.angularVelocity.magnitude / _rigidbody.maxAngularVelocity;
        }
    }
    
    // If we want to be able to get the percent of our max rotations-per-minute (RPM)
    public RotationalForceApplicator(Rigidbody _rigidbody, float _maxForce, float _maxRPM, Vector3 _forceAxis) : base(_rigidbody, _maxForce, _forceAxis)
    {
        // This converts _maxRPM to degrees per second, and then to radians per second.
        _rigidbody.maxAngularVelocity = ((_maxRPM / 60F) * 360F) / 57.29577951308F;
    }


    public override void ApplyForcePercentage(float _percent){
        _rigidbody.AddRelativeTorque(CalculateForce(_percent));
    }
    
}

public class LinearForceApplicator : ForceApplicator
{
    public LinearForceApplicator(Rigidbody _rigidbody, float _maxForce, Vector3 _forceAxis) : base(_rigidbody, _maxForce, _forceAxis)
    {
        // Nothin' to see here folks.
    }

    public override void ApplyForcePercentage(float _percent){
        _rigidbody.AddRelativeForce(CalculateForce(_percent));
    }
}

// controls properties
public class HelicopterControls  
{
    public float Pitch
    {
        get;
        protected set;
    }

    public float Roll
    {
        get;
        protected set;
    }    

    public float Yaw
    {
        get;
        protected set;
    }

    public float Collective
    {
        get;
        protected set;
    }

    public float Throttle
    {
        get;
        protected set;
    }
    
    public virtual void HandleInput()  
    {
        // Handle inputs.
    }
}

// Input controls
public class PilotHelicopterControls : HelicopterControls  
{
    public override void HandleInput()
    {
        Pitch = Input.GetAxis("Vertical");   // tile bac and forward
        Roll = Input.GetAxis("Horizontal"); // tilt left right
        Yaw = Mathf.InverseLerp(-1.0f, 1.0f, Input.GetAxis("Horizontal2"));  // horizontal rotation
        Collective = Mathf.InverseLerp(-1.0f, 1.0f, Input.GetAxis("Vertical2")); // lift and drop
        Throttle = 1.0f;
    }
}


// Rotate tail rotor
public class Rotater  
{
    private Transform _transform;
    private Vector3 _rotationAxis;
    private float _maxDegreesPerSecond;

    public Rotater(Transform _newTransform, Vector3 _newRotationAxis, float _maxRPM)
    {
        _transform = _newTransform;
        _rotationAxis = _newRotationAxis;
        _maxDegreesPerSecond = (_maxRPM / 60.0f) * 360.0f;
    }

    public void Rotate(float _percentMaxSpeed)
    {
        _transform.Rotate(_rotationAxis * (Time.deltaTime * (_maxDegreesPerSecond * _percentMaxSpeed)));
    }
}



/*
    We’ll need to create a LinearForceApplicator for our lift and 5 instances of RotationalForceApplicator, one for each of the following:

    Twisting the main rotor
    Twisting the body (remember Newton’s 3rd law)
    Pitching the helicopter
    Rolling the helicopter
    Counter-twisting the body (remember the counter-torque rotor?)

*/
public class HeliController : MonoBehaviour {

    public Rigidbody _mainRotor;
    public Rigidbody _body;
    public Transform _tailRotor;
    
    private LinearForceApplicator _lift;
    private RotationalForceApplicator _mainRotorTorque;
    private RotationalForceApplicator _bodyTorque;
    private RotationalForceApplicator _bodyCounterTorque;
    private RotationalForceApplicator _pitchTorque;
    private RotationalForceApplicator _rollTorque;
    private HelicopterControls _controls;
    private Rotater _tailRotorRotater;
    
    
	// Use this for initialization
	void Start () {
		_lift = new LinearForceApplicator(_body, 100062.0f, Vector3.up);
        _mainRotorTorque = new RotationalForceApplicator(_mainRotor, 503.0f, 240.0f, Vector3.down);
        _bodyTorque = new RotationalForceApplicator(_body, 25000.0f, 120.0f, Vector3.down);
        _bodyCounterTorque = new RotationalForceApplicator(_body, 50000.0f, 120.0f, Vector3.up);
        _pitchTorque = new RotationalForceApplicator(_body, 20000.0f, 100.0f, Vector3.right);
        _rollTorque = new RotationalForceApplicator(_body, 15000.0f, 100.0f, Vector3.back);
        _controls = new PilotHelicopterControls();
        _tailRotorRotater = new Rotater(_tailRotor, Vector3.right, 500.0f);

        // This should account for any weird wobbling caused by misaligned pivots.
        //_body.centerOfMass = Vector3.zero;
        //_mainRotor.centerOfMass = Vector3.zero;
          
                                         //pitch    // yaw     // roll
        _body.inertiaTensor = new Vector3(80000.0f, 60000.0f, 90000.0f);
        _mainRotor.inertiaTensor = new Vector3(_mainRotor.mass, _mainRotor.mass, _mainRotor.mass);
	}
	
	// Update is called once per frame
	void Update () {
    
        // handle controls
    }
    
    // FixedUpdate happens more than once per frame if the fixed time step is less than the actual frame update time
    void FixedUpdate()
    {
		_controls.HandleInput();
        
        
         // Throttle
        _mainRotorTorque.ApplyForcePercentage(_controls.Throttle);
        _bodyTorque.ApplyForcePercentage(_mainRotorTorque.PercentMaxRPM);   
        
        // Yaw
        _tailRotorRotater.Rotate(_mainRotorTorque.PercentMaxRPM);  
        _bodyCounterTorque.ApplyForcePercentage(_mainRotorTorque.PercentMaxRPM * _controls.Yaw ); 
        
        // Collective
        _lift.ApplyForcePercentage(_mainRotorTorque.PercentMaxRPM * _controls.Collective);
        
        // Cyclic
        _pitchTorque.ApplyForcePercentage(_mainRotorTorque.PercentMaxRPM * _controls.Pitch);  
        _rollTorque.ApplyForcePercentage(_mainRotorTorque.PercentMaxRPM * _controls.Roll);


        print("main rotor " + _mainRotor.inertiaTensor + ",   body "+ _body.inertiaTensor);
    }
}
