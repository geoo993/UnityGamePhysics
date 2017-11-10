using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Based on https://github.com/suncube/Base-Helicopter-Controller
// https://www.assetstore.unity3d.com/en/#!/content/40107

[RequireComponent(typeof(Rigidbody))]
public class HeloController : MonoBehaviour
{

    /*
        The primary goal of the helicopter physics system is to provide realistic helicopter
        flight in a 3D virtual environment. Another goal is to provide a tool that enables the
        Army Game designers the capability to incorporate helicopter capabilities by simply
        placing navigation/path information into a level. This realism is to be at a fidelity level
        that is believable to the average human (non-helicopter pilot).
    
    */
    public Vector3[] path;


    /*  Helicopter
    
    The HeloController class is implemented to achieve 
    the goals of the remaining three modules (Instruction/Situation, Apply Rules, Act) of the conceptual design
    
    This class employs several functions and routines 
    that address the goals of the instructions/situation, apply rules and act modules.
    
    */

    private Rigidbody rigidBody;
    public HeloRotorController mainRotor;
    public HeloRotorController subRotor;


    /*
        The MOVEMENT methods are critical to the appearance of realism in a virtual helicopter system. 
        The movement sub-module breaks the movement of the helicopter down to two states; 
        move towards and hovering.
        The move towards state is any state in which the helicopter is moving. 
        Whether it is taking off, landing, flying forwards or backwards, 
        if it is changing location, it is in the movement state.
    */
    private enum MoveTowards { TakeOff, Landing, Forward, Backward };
    private MoveTowards movement = MoveTowards.Landing;
    private enum RotateDirection { Left, Right };

    /*
        The ATTITUDE sub-module can be broken down into the traditional components of yaw, pitch, and roll. 
        These same components are applied to aviation vehicles also. 
    */
    private float pitch, yaw, roll, tiltForward, tiltLeft, tiltRight = 0.0f;
    public bool IsGrounded = false;


    // Controlls
    List<KeyCode> keys = new List<KeyCode>{
        KeyCode.W,
        KeyCode.S,
        KeyCode.A,
        KeyCode.D,
        KeyCode.Q,
        KeyCode.E,
        KeyCode.Space,
        KeyCode.C
    };

    // https://en.wikipedia.org/wiki/Sikorsky_UH-60_Black_Hawk
    // http://www.deagel.com/Support-Aircraft/UH-60L-Blackhawk_a000508002.aspx
    private float NeverExceedSpeed = 357000.0f; //357km/h // speed limit
    private float TopSpeed = 100.0f; // at High Altitude //294000.0f; //294km/h // 1219.2
    private float CruiseSpeed = 77.0f; //280000.0f; //280km/h
    private float RateOfClimb = 4.5f; //m/s
    private float Ceiling = 5837.0f; //meter 
    private float TempCeiling = 0.0f;
    private float mass = 158.0f; //W/kg)
    private float power = 287675.7088631541f;  //3,780 shp => kg-m/s 

    private float EmptyWeight = 4819.0f; //(4,819 kg)
    private float MaxTakeoffWeight = 10660.0f; //(10,660 kg)
    private float Payload = 5280f;// kilogram //(11,640 pound) // amount it can lift (like cargos)
    private float DiscArea = Mathf.Pow(210.0f, 2.0f);//(210 m²)
    private float EngineRotationSpeed = 17.132192f;
    private float mainRotorRotationSpeed = 221.0052768f;


    private float _engineForce;
    public float EngineForce
    {
        get { return _engineForce; }
        set
        {
            mainRotor.rotarSpeed = value * mainRotorRotationSpeed;//80.0f;
            subRotor.rotarSpeed = value * (mainRotorRotationSpeed / 2.0f); //40.0f;
            TempCeiling = value + 100.0f;
            _engineForce = value;
        }
    }

    private float TurnForce = 3f;
    private float TurnTiltForce = 30f;
    
    private float ForwardTiltForce = 20f;
    
    private float turnTiltForcePercent = 301.5f;
    private float turnForcePercent = 501.3f;
    
    [SerializeField, Range(1.0f, 2.0f)]
    private float velocityExponent = 1.5f; // the Stokes drag fluid power (U) at low (1.0f) or higher (2.0f) velocity
    private float dragCoefficient = 1.05f; // CD is the drag coefficient – a dimensionless number.
	private float angularDragCoefficient = 4.0f; // CD is the drag coefficient – a dimensionless number.
  
    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();

        rigidBody.mass = mass;
        rigidBody.angularDrag = angularDragCoefficient;
    }
    
    float Mass(){
        return rigidBody.mass;
    }
    
    
    // air density https://en.wikipedia.org/wiki/Density_of_air
    Vector3 WeightVector(){
        return rigidBody.mass * Physics.gravity;
    }

    Vector3 LiftVector()
    {
        if (TempCeiling >= (Ceiling - 100.0f)) { TempCeiling = Ceiling;  } 
        float upForce = 1.0f - Mathf.Clamp(rigidBody.transform.position.y / TempCeiling, 0.0f, 1.0f);
        upForce = Mathf.Lerp(0.0f, EngineForce, upForce) * Mass();
        
        return Vector3.up * upForce;
    }
    
    Vector3 TrustVector()
    {
        return Vector3.forward * Mathf.Max(0.0f, pitch * CruiseSpeed * Mass());
    }
    
    // https://en.wikipedia.org/wiki/Drag_(physics)
    // https://www.lmnoeng.com/Force/DragForce.php
    // https://forum.unity.com/threads/physics-drag-formula.252406/
	//Drag is Resistance From Non physical Objects(e.g air) .
    Vector3 DragVector()
    {
        Vector3 dragV = -rigidBody.velocity * (1.0f - Time.fixedDeltaTime * dragCoefficient);

        Vector3 velocityVector = rigidBody.velocity;// we know how fast the object is traveling through the velocity
        float drag = dragCoefficient * Mathf.Pow(velocityVector.magnitude, velocityExponent);
        Vector3 dragVector = -drag * velocityVector.normalized; // drag in the opposite direction

        return dragVector;
    
    }
    
	//Used To slow Down the Rotation Of Object.
    Vector3 AngularDragVector()
    {
        return -rigidBody.angularVelocity * (1.0f - Time.deltaTime * angularDragCoefficient);
    }
                 
    private void Thrust()
    {
        rigidBody.AddRelativeForce(TrustVector());
    }
    
    private void Lift()
    {
        rigidBody.AddRelativeForce(LiftVector());
    }
    
    private void Drag()
    {
        rigidBody.AddRelativeForce(DragVector());
    }
    
    private void AngularDrag()
    {
        rigidBody.AddRelativeTorque(AngularDragVector());
    }
    
    private void Weight(){
        rigidBody.AddRelativeForce(WeightVector());
    }
    
    private void Pitch(){
        float turn = TurnForce * Mathf.Lerp(roll, roll * (turnTiltForcePercent - Mathf.Abs(pitch)), Mathf.Max(0.0f, pitch));
        tiltForward = Mathf.Lerp(tiltForward, turn, Time.fixedDeltaTime * TurnForce);
        
        // tilt while moving forward
        rigidBody.AddRelativeTorque(0.0f, tiltForward * Mass(), 0.0f);
    }
    
    private void Roll()
    {
        tiltLeft = Mathf.Lerp(tiltLeft, roll * TurnTiltForce, Time.deltaTime);
        tiltRight = Mathf.Lerp(tiltRight, pitch * ForwardTiltForce, Time.deltaTime);
        rigidBody.transform.localRotation = Quaternion.Euler(tiltRight, rigidBody.transform.localEulerAngles.y, -tiltLeft);
    }
    
    private void Yaw(RotateDirection direction){
        yaw = (turnForcePercent - Mathf.Abs(pitch));

        yaw = (direction == RotateDirection.Left) ? -yaw : yaw;
        
        rigidBody.AddRelativeTorque(0.0f, yaw  * Mass(), 0.0f);
    }
    
    
    private void MovementKeyControlls()
    {
        float tempY = 0;
        float tempX = 0;

        // stable forward
        if (pitch > 0.0f)
        {
            tempY = -Time.fixedDeltaTime;
        }
        else
        {
            if (pitch < 0.0f)
            {
                tempY = Time.fixedDeltaTime;
            }
        }

        // stable lurn
        if (roll > 0.0f)
        {
            tempX = -Time.fixedDeltaTime;
        }
        else
        {
            if (roll < 0.0f)
            {
                tempX = Time.fixedDeltaTime;
            }
        } 
        
        foreach (KeyCode key in keys)
        {
            //if (Input.GetKeyDown(key))
            if (Input.GetKey(key))
            {
                 switch (key)
                {
                 case KeyCode.W:
                     //Debug.Log("Forward");

                     if (IsGrounded) { break; }
                     tempY = Time.fixedDeltaTime;
                     movement = MoveTowards.Forward;
                     break;
                 case KeyCode.S:
                     //Debug.Log("Back");
                     
                     if (IsGrounded) { break; }
                     tempY = -Time.fixedDeltaTime;
                     movement = MoveTowards.Backward;
                     break;
                 case KeyCode.A:
                     //Debug.Log("Left");
                     
                     if (IsGrounded) { break; }
                     tempX = -Time.fixedDeltaTime;
                     break;
                 case KeyCode.D:
                     //Debug.Log("Right");
                     
                     if (IsGrounded) { break; }
                     tempX = Time.fixedDeltaTime;
                     break;
                 case KeyCode.Q:
                    {
                        //Debug.Log("TurnLeft");

                        if (IsGrounded) { break; }
                        Yaw(RotateDirection.Left);
                    }
                     break;
                 case KeyCode.E:
                    {
                        //Debug.Log("TurnRight");
                        if (IsGrounded) { break; }
                        Yaw(RotateDirection.Right);
                    }
                     break;
                 default:
                 //   Debug.Log("Default");
                     break;
                }
            }
        }
        
        roll += tempX; // roll 
        roll = Mathf.Clamp(roll, -1.0f, 1.0f);

        pitch += tempY; // pitch
        pitch = Mathf.Clamp(pitch, -1.0f, 1.0f);
    }
    
    private void PowerKeyControlls()
    {
        
        foreach (KeyCode key in keys)
        {
            if (Input.GetKey(key))
            {
                 switch (key)
                {
                 case KeyCode.Space:
                     //Debug.Log("SpeedUp");
                     
                     EngineForce += RateOfClimb;  //0.1f;
                     break;
                 case KeyCode.C:
                     //Debug.Log("SpeedDown");

                     EngineForce -= RateOfClimb; //0.12f;
                     if (EngineForce < 0) { EngineForce = 0; }
                     break;
                 default:
                 //   Debug.Log("Default");
                     break;
                }
            }
        }
      
    }
    
    private void MovementModule(){
        
        Lift();
        
        Thrust();
        Drag();
        
        AngularDrag();
        Weight();
        
    }
    
    private void AttitudeModule(){
        
        Pitch();
        Roll();
    }
    
     /*
        The GetNextTarget function is responsible for processing route information and 
        providing instructions for the helicopter entity to follow
        This functions addresses the goals from the instruction/situation module of the conceptual design.
    */
    void GetNextTarget(){
    
    }
    
    void LookAtTarget(Transform target, bool smooth, float rotationSpeed){
    
        if (target != null)
        {
            if (smooth)
            {
                //rotate to look at the target
                Vector3 relativePos = target.position - transform.position;
                Quaternion look = Quaternion.LookRotation(relativePos);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, rotationSpeed * Time.deltaTime);
                
            }
            else
            {
                transform.LookAt(target);
            }
        }
    
    }
    
    /*
        The Tick function also goals from the instruction/situation module. 
        Tick is basically the simulation engine. 
        It keeps the system flowing from module to module. 
        Tick contains code that provides information to other functions 
        as well as gathers information about the virtual world.
    */
    void Tick(){
    
    }
    
    
    /*
        The Yaw, Pitch and Roll routines are very similar to each other. 
        These routines are combined to determine the attitude of the helicopter entity. 
        The Yaw routine considers the helicopter entity’s current yaw orientation; 
        obtains the current destination location from the GetNextTarget function; 
        and uses the GetDirRot function to determine the desired yaw orientation. 
        Finally, the Yaw routine smoothly interpolates from the current yaw orientation to the desired yaw orientation.
    
    */
    void GetDirectionRotation(){
    
    }
    
    /*
        The Moveto routine is the most often used part of the code. In order for the
        helicopter to move, a direction of movement must be determined as well as the speed of
        the movement.
    */
    void Moveto(Vector3 direction, float speed){
    
    }
    
    private void Move(){
        CruiseSpeed = 77.0f;
    }
    
    /*
        the current velocity of the helicopter would also be a factor in determining
        if the distance would warrant a hover versus a fly through
    */
    void Hover(){
        CruiseSpeed = 0.0f;
    }
    
    
    /*
        the badAngle function is provided to determine if the
        change in direction of the helicopter is at a “bad angle”, i.e. around 180 degrees
        difference
    
    */
    void BadAngle(){
    
    }
    
    
    /*
         the pointDist function is used to calculate the actual distance between
         the locations for the purposes of determining if it is necessary to invoke Hover
    */
    void PointDistance(){
    
    }

    /*
        Lastly, after all the previously described determinations, calculations and
        interpolations are accomplished, the position and orientation of the helicopter entity is
        updated and the process repeats until the entire path has been traversed by the helicopter
        in the virtual world.
    
    */
    
    // FixedUpdate happens more than once per frame if the fixed time step is less than the actual frame update time
    void FixedUpdate()
    {
		MovementKeyControlls();
		PowerKeyControlls();
        MovementModule();
        AttitudeModule();
        
        if (transform.position.y < 100.0f){
            Hover();
        }else{
			Move();
        }
        
        //print("EngineForce  "+ EngineForce + ",  helicopterMove " + helicopterMove + ",   IsGrounded " + IsGrounded);
       
       
    }


    private void OnCollisionEnter(Collision collision)
    {
        movement = MoveTowards.Landing;
		IsGrounded = true;
    }
    
    private void OnCollisionExit(Collision collision)
    {
        movement = MoveTowards.TakeOff;
		IsGrounded = false;
    }

}
