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

    private float _engineForce;
    public float EngineForce
    {
        get { return _engineForce; }
        set
        {
            mainRotor.rotarSpeed = value * 80.0f;
            subRotor.rotarSpeed = value * 40.0f;

            _engineForce = value;
        }
    }
    
    /*  Helicopter
    
    The HeloController class is implemented to achieve 
    the goals of the remaining three modules (Instruction/Situation, Apply Rules, Act) of the conceptual design
    
    This class employs several functions and routines 
    that address the goals of the instructions/situation, apply rules and act modules.
    
    */
    private PhysicsEngine physicsEngine;
    private UniversalGravitation universalGravitation;

    private Rigidbody rigidBody;
    public HeloRotorController mainRotor;
    public HeloRotorController subRotor;


    /*
        The movement methods are critical to the appearance of realism in a virtual helicopter system. 
        The movement sub-module breaks the movement of the helicopter down to two states; 
        move towards and hovering.
        The move towards state is any state in which the helicopter is moving. 
        Whether it is taking off, landing, flying forwards or backwards, 
        if it is changing location, it is in the movement state.
    */
    private enum MoveTowards { TakeOff, Landing, Forward, Backward };
    private enum RotateDirection { Left, Right };
    
    /*
        The ATTITUDE sub-module can be broken down into the traditional components of yaw, pitch, and roll. 
        These same components are applied to aviation vehicles also. 
    */
    private float pitch, yaw, rollPositive, rollNegative = 0.0f;
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
        KeyCode.C,
        KeyCode.UpArrow,
        KeyCode.DownArrow,
        KeyCode.LeftArrow,
        KeyCode.RightArrow,
    };
  
    private float TurnForce = 3f;
    private float TurnTiltForce = 30f;
    
    [Range(1.0f, 50.0f)] public float ForwardForce = 20f;
    private float ForwardTiltForce = 20f;
    
    private float EffectiveHeight = 100f;

    [Range(1.0f, 1000.0f)] public float turnTiltForcePercent = 1.5f;
    [Range(1.0f, 1000.0f)] public float turnForcePercent = 1.3f;
    
    private Vector3 helicopterMove = Vector3.zero;
    
     [Range(1.0f, 2.0f)]
    public float velocityExponent = 1.0f; // the Stokes drag fluid power (U) at low (1.0f) or higher (2.0f) velocity
    public float dragConstant = 1.0f; // CD is the drag coefficient – a dimensionless number.
    public float angularDragConstant = 1.0f;
    
  
    // Use this for initialization
    void Start()
    {
        physicsEngine = GetComponent<PhysicsEngine>();
        universalGravitation = GetComponent<UniversalGravitation>();
        rigidBody = GetComponent<Rigidbody>();
        
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
    
    void Move(){
    
    }

    float Mass(){
        //return physicsEngine.mass;
        return rigidBody.mass;
    }
    
    // air density https://en.wikipedia.org/wiki/Density_of_air
    Vector3 WeightVector(){
        return physicsEngine.mass * universalGravitation.GravitationalForce();
    }

    Vector3 LiftVector()
    {
         float upForce = 1.0f - Mathf.Clamp(rigidBody.transform.position.y / EffectiveHeight, 0.0f, 1.0f); // limit to where it can move
        upForce = Mathf.Lerp(0.0f, EngineForce, upForce) * Mass();
        
        return Vector3.up * upForce;
    }
    
    Vector3 TrustVector()
    {
        return Vector3.forward * Mathf.Max(0.0f, helicopterMove.y * ForwardForce * Mass());
    }
    
   
    private void Drag()
    {
        rigidBody.drag = dragConstant;
        rigidBody.angularDrag = angularDragConstant;
    }
   
    private void Thrust()
    {
        
        rigidBody.AddRelativeForce(TrustVector());
        //physicsEngine.AddTorque(0.0f, helicopterTurn  * Mass(), 0.0f);
        //physicsEngine.AddForce(Vector3.forward * Mathf.Max(0.0f, helicopterMove.y * ForwardForce * Mass()));
    }
    
    private void Lift()
    {
       
        rigidBody.AddRelativeForce(LiftVector());
        //physicsEngine.AddForce(Lift());
        
    }
    
    private void MovementModule(){
		OnKeyPressed();
        Lift();
        Thrust();
        Drag();
    }

    private void Pitch(){
        float turn = TurnForce * Mathf.Lerp(helicopterMove.x, helicopterMove.x * (turnTiltForcePercent - Mathf.Abs(helicopterMove.y)), Mathf.Max(0.0f, helicopterMove.y));
        pitch = Mathf.Lerp(pitch, turn, Time.fixedDeltaTime * TurnForce);
        
        // tilt while moving forward
        rigidBody.AddRelativeTorque(0.0f, pitch * Mass(), 0.0f);
        
    }
    
    private void Roll()
    {
        rollNegative = Mathf.Lerp(rollNegative, helicopterMove.x * TurnTiltForce, Time.deltaTime);
        rollPositive = Mathf.Lerp(rollPositive, helicopterMove.y * ForwardTiltForce, Time.deltaTime);
        rigidBody.transform.localRotation = Quaternion.Euler(rollPositive, rigidBody.transform.localEulerAngles.y, -rollNegative);
    }
    
    private void Yaw(RotateDirection direction){
        yaw = (turnForcePercent - Mathf.Abs(helicopterMove.y));

        yaw = (direction == RotateDirection.Left) ? -yaw : yaw;
        
        rigidBody.AddRelativeTorque(0.0f, yaw  * Mass(), 0.0f);
        //physicsEngine.AddTorque(0.0f, yaw  * Mass(), 0.0f);
    }
    
    void AttitudeModule(){
        Pitch();
        Roll();
    }
    
    /*
        the current velocity of the helicopter would also be a factor in determining
        if the distance would warrant a hover versus a fly through
    */
    void Hover(){
    
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

    private void OnKeyPressed()
    {
        float tempY = 0;
        float tempX = 0;

        // stable forward
        if (helicopterMove.y > 0)
        {
            tempY = -Time.fixedDeltaTime;
        }
        else
        {
            if (helicopterMove.y < 0)
            {
                tempY = Time.fixedDeltaTime;
            }
        }
        
        // stable lurn
        if (helicopterMove.x > 0)
        {
            tempX = -Time.fixedDeltaTime;
        }
        else
        {
            if (helicopterMove.x < 0)
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
                     break;
                 case KeyCode.S:
				     //Debug.Log("Back");
                     
                     if (IsGrounded) { break; }
                     tempY = -Time.fixedDeltaTime;
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
                 case KeyCode.Space:
                     //Debug.Log("SpeedUp");
                     
                     EngineForce += 0.1f;
                     break;
                 case KeyCode.C:
				     //Debug.Log("SpeedDown");
                     
                     EngineForce -= 0.12f;
                     if (EngineForce < 0) { EngineForce = 0; }
                     break;
                 default:
                 //   Debug.Log("Default");
                     break;
                }
            }
        }
        
        helicopterMove.x += tempX; // roll 
        helicopterMove.x = Mathf.Clamp(helicopterMove.x, -1.0f, 1.0f);

        helicopterMove.y += tempY; // pitch
        helicopterMove.y = Mathf.Clamp(helicopterMove.y, -1.0f, 1.0f);
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
        
		
        MovementModule();
        AttitudeModule();

        //print(universalGravitation.GravitationalForce());
        //print("EngineForce  "+ EngineForce + ",  helicopterMove " + helicopterMove + ",   IsGrounded " + IsGrounded);
        
        
    }


    void LateUpdate()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
		IsGrounded = true;
    }
    private void OnCollisionExit(Collision collision)
    {
		IsGrounded = false;
    }

}
