using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Based on https://github.com/suncube/Base-Helicopter-Controller
// https://www.assetstore.unity3d.com/en/#!/content/40107

[RequireComponent(typeof(Rigidbody))]
public class HelicopterController : MonoBehaviour
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
    public HelicopterRotorController mainRotor;
    public HelicopterRotorController rearRotor;

    /*
        The MOVEMENT methods are critical to the appearance of realism in a virtual helicopter system. 
        The movement sub-module breaks the movement of the helicopter down to two states; 
        move towards and hovering.
        The move towards state is any state in which the helicopter is moving. 
        Whether it is taking off, landing, flying forwards or backwards, 
        if it is changing location, it is in the movement state.
    */
    private enum MoveTowards { TakeOff, Landing, Forward, Backward, Left, Right };
    private MoveTowards movement = MoveTowards.Landing;
    private enum RotateDirection { Left, Right };

    /*
        The ATTITUDE sub-module can be broken down into the traditional components of yaw, pitch, and roll. 
        These same components are applied to aviation vehicles also. 
    */
    private float side, pitch, yaw, roll, tiltForward, tiltLeft, tiltRight = 0.0f;
    private bool IsGrounded = false;
    private bool ShouldPowerUp = false;
    
    // Controlls
    List<KeyCode> keys = new List<KeyCode>{
        KeyCode.W,
        KeyCode.S,
        KeyCode.A,
        KeyCode.D,
        KeyCode.Q,
        KeyCode.E,
        KeyCode.Z,
        KeyCode.X,
        KeyCode.Space,
        KeyCode.C
    };

    /*
        backward movement
    */
    
    
    // https://en.wikipedia.org/wiki/Sikorsky_UH-60_Black_Hawk
    // http://www.deagel.com/Support-Aircraft/UH-60L-Blackhawk_a000508002.aspx
    // https://www.aircraftcompare.com/helicopter-airplane/sikorsky-uh-60-m-black-hawk/235
    private float NeverExceedSpeed = 357000.0f; //357km/h // speed limit
    private float TopSpeed = 20.0f;//100.0f // at High Altitude //294000.0f; //294km/h // 1219.2
    private float CruiseSpeed = 10.0f;//77.0f; //280000.0f; //280km/h
    private float SpeedAcceleration = 0.0f; //280000.0f; //280km/h
    private float RateOfClimb = 4.5f; //m/s
    private float Ceiling = 5837.0f; //meter 
    private float TempCeiling = 0.0f;
    private float OriginalMass = 158.0f; //W/kg)
    private float Power, OriginalPower = 287675.7088631541f;  //3,780 shp => kg-m/s 
    
    private float EmptyWeight = 4819.0f; //(4,819 kg) 
    private float MaxTakeoffWeight = 10660.0f; //(10,660 kg) // what the pilot is allowed to take off with
    private float Payload = 5280f;// kilogram //(11,640 pound) // amount it can lift (like cargos)
    private float DiscArea = Mathf.Pow(210.0f, 2.0f);//(210 m²)
    private float EngineRotationSpeed = 17.132192f;
    private float MainRotorRotationSpeed = 221.0052768f;

    private float FuelTankCapacity = 1362.74f; // 360.00 gallon  1,362.74 litres
    private float FuelEconomy = 0.32f; // 0.32 km per litre  0.76 NM per gallon
    private float FuelConsumed = 0.0f;
    
    private float TurnForce = 3f;
    private float TurnTiltForce = 30f;
    
    private float ForwardTiltForce = 20f;
   
    private float TurnTiltForcePercent = 1.5f;
    private float TurnForcePercent = 1.3f; 
    
    private float DragCoefficient = 1.05f; // CD is the drag coefficient – a dimensionless number.
    private float AngularDragCoefficient = 6.0f; // CD is the drag coefficient – a dimensionless number.
  
    private float _engineForce;
    public float EngineForce
    {
        get { return _engineForce; }
        set
        {
            mainRotor.rotarSpeed = value * MainRotorRotationSpeed;
            rearRotor.rotarSpeed = value * MainRotorRotationSpeed;
            TempCeiling = value + 100.0f;
            GetComponent<AudioSource>().pitch = Mathf.Clamp(value / 40, 0, 1.2f);
            _engineForce = value;
        }
    }


    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        SetRigidBody(rigidBody);
    }
    
    void SetRigidBody(Rigidbody body){
        body.useGravity = true;
        body.mass = OriginalMass;
        body.drag = DragCoefficient;
        body.angularDrag = AngularDragCoefficient;
        body.inertiaTensor = new Vector3(0.01f, OriginalMass, 0.01f);
    }

    float Mass(){
        return rigidBody.mass;
    }

    float Speed(){
    
        float MaxAcceleration = TopSpeed - CruiseSpeed;
        float Acceleration = Mathf.Clamp(SpeedAcceleration, -MaxAcceleration, MaxAcceleration);
        float CurrentSpeed = (CruiseSpeed + Acceleration);
        if (movement == MoveTowards.Backward)
        {
            return (CurrentSpeed * 0.5f);
        }else if (movement == MoveTowards.Left || movement == MoveTowards.Right){
            return (CurrentSpeed * 0.2f);
        }else {
            return CurrentSpeed;
        }
    }
    
    // Air density https://en.wikipedia.org/wiki/Density_of_air
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
        return Vector3.forward * (pitch * Speed() * Mass());
    }
    
    Vector3 SideTrustVector(){
        return Vector3.right * (side * Speed() * Mass());
    }
    
    // https://en.wikipedia.org/wiki/Drag_(physics)
    // https://www.lmnoeng.com/Force/DragForce.php
    // https://forum.unity.com/threads/physics-drag-formula.252406/
	//Drag is Resistance From Non physical Objects(e.g air) .
    Vector3 DragVector()
    {
        //Vector3 dragV = -rigidBody.velocity * (1.0f - Time.fixedDeltaTime * dragCoefficient);
        
        float velocityExponent = 1.5f; // the Stokes drag fluid power (U) at low (1.0f) or higher (2.0f) velocity
        Vector3 velocityVector = rigidBody.velocity;// we know how fast the object is traveling through the velocity
        float drag = DragCoefficient * Mathf.Pow(velocityVector.magnitude, velocityExponent);
        Vector3 dragVector = -drag * velocityVector.normalized; // drag in the opposite direction

        return dragVector;
    }
    
    Vector3 AngularDragVector()
    {
        return -rigidBody.angularVelocity * (1.0f - Time.deltaTime * AngularDragCoefficient);
    }
                 
    private void ThrustForce()
    {
        rigidBody.AddRelativeForce(TrustVector());
    }
    
    private void SideThrustForce()
    {
        rigidBody.AddRelativeForce(SideTrustVector());
    }
    
    private void LiftForce()
    {
        rigidBody.AddRelativeForce(LiftVector());
    }
    
    private void DragForce()
    {
        rigidBody.AddRelativeForce(DragVector());
    }
    
    private void AngularDragForce()
    {
        rigidBody.AddRelativeTorque(AngularDragVector());
    }
    
    private void WeightForce(){
        rigidBody.AddRelativeForce(WeightVector());
    }
    
    private void Pitch(){
        float turn = TurnForce * Mathf.Lerp(roll, roll * (TurnTiltForcePercent - Mathf.Abs(pitch)), Mathf.Max(0.0f, pitch));
        tiltForward = Mathf.Lerp(tiltForward, turn, Time.fixedDeltaTime * TurnForce);
        rigidBody.AddRelativeTorque(0.0f, tiltForward * Mass(), 0.0f);
    }
    
    private void Roll()
    {
        tiltLeft = Mathf.Lerp(tiltLeft, roll * TurnTiltForce, Time.deltaTime);
        tiltRight = Mathf.Lerp(tiltRight, pitch * ForwardTiltForce, Time.deltaTime);
        rigidBody.transform.localRotation = Quaternion.Euler(tiltRight, rigidBody.transform.localEulerAngles.y, -tiltLeft);
    }
    
    private void Yaw(RotateDirection direction){
        yaw = (TurnForcePercent - Mathf.Abs(pitch));
        yaw = (direction == RotateDirection.Left) ? -yaw : yaw;
        
        rigidBody.AddRelativeTorque(0.0f, yaw  * Mass(), 0.0f);
    }
    
    private void ResetEngine(){
        if (IsGrounded && !ShouldPowerUp){
            EngineForce = Mathf.Lerp(EngineForce, 0.0f, Time.fixedDeltaTime);
            FuelConsumed = 0.0f;
        }
    }
    
    private void MovementKeyControlls()
    {
        float tempY = 0;
        float tempX = 0;
        float tempS = 0;

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

        // stable turn
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
        
        // stable side move
        if (side > 0.0f)
        {
            tempS = -Time.fixedDeltaTime;
        }
        else
        {
            if (side < 0.0f)
            {
                tempS = Time.fixedDeltaTime;
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
                     SpeedAcceleration += Time.fixedDeltaTime;
                     break;
                 case KeyCode.S:
                     //Debug.Log("Back");
                     
                     if (IsGrounded) { break; }
                     tempY = -Time.fixedDeltaTime;
                     movement = MoveTowards.Backward;
                     SpeedAcceleration -= Time.fixedDeltaTime;
                     break;
                 case KeyCode.A:
                     //Debug.Log("Tilt Left");
                     
                     if (IsGrounded) { break; }
                     tempX = -Time.fixedDeltaTime;
                     break;
                 case KeyCode.D:
                     //Debug.Log("Tilt Right");
                     
                     if (IsGrounded) { break; }
                     tempX = Time.fixedDeltaTime;
                     break;
                 case KeyCode.Z:
                     //Debug.Log("Left");
                     
                     if (IsGrounded) { break; }
                     movement = MoveTowards.Left;
				     tempS = -Time.fixedDeltaTime;
                     break;
                 case KeyCode.X:
                     //Debug.Log("Right");
                     if (IsGrounded) { break; }
                     movement = MoveTowards.Right;
					 tempS = Time.fixedDeltaTime;
                     break;
                 case KeyCode.Q:
                     //Debug.Log("TurnLeft");
                     if (IsGrounded) { break; }
                     Yaw(RotateDirection.Left);
                    
                     break;
                 case KeyCode.E:
                    //Debug.Log("TurnRight");
                    if (IsGrounded) { break; }
                    Yaw(RotateDirection.Right);
                    
                     break;
                 default:
                 //   Debug.Log("Default");
                     break;
                }
            }

            if (Input.GetKeyUp(key))
            {
                switch (key)
                {
                    case KeyCode.W:
                        if (IsGrounded) { break; }
                        SpeedAcceleration = 0.0f;
                        break;
                    case KeyCode.S:
                        if (IsGrounded) { break; }
                        SpeedAcceleration = 0.0f;
                        break;
                    default:
                        break;
                }
            }
            
            
        }
        
        roll += tempX; // roll 
        roll = Mathf.Clamp(roll, -1.0f, 1.0f);

        pitch += tempY; // pitch
        pitch = Mathf.Clamp(pitch, -1.0f, 1.0f);

        side += tempS;
        side = Mathf.Clamp(side, -1.0f, 1.0f);
        
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
                     
                     EngineForce += RateOfClimb * Time.fixedDeltaTime;  //0.1f;
                     ShouldPowerUp = true;
                     break;
                 case KeyCode.C:
                     //Debug.Log("SpeedDown");

                     EngineForce -= RateOfClimb * Time.fixedDeltaTime; //0.12f;
                     if (EngineForce < 0) { EngineForce = 0; }
                     ShouldPowerUp = false;
                     break;
                 default:
                 //   Debug.Log("Default");
                     break;
                }
            }
        }
      
    }
    
    private void MovementModule(){
        LiftForce();
        ThrustForce();
        SideThrustForce();
        DragForce();
        AngularDragForce();
        WeightForce();
        ResetEngine();
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
        // calculate direction and avoid BadAngle
    }
    
    /*
        The Moveto routine is the most often used part of the code. In order for the
        helicopter to move, a direction of movement must be determined as well as the speed of
        the movement.
    */
    void Moveto(Vector3 direction, float speed){
        // move to next location
    }
    
    
    /*
        the current velocity of the helicopter would also be a factor in determining
        if the distance would warrant a hover versus a fly through
    */
    void Hover(){
        // hover when you reach location
    }
    
    
    /*
        the badAngle function is provided to determine if the
        change in direction of the helicopter is at a “bad angle”, i.e. around 180 degrees
        difference
    
    */
    void BadAngle(){
        // calcuate angle of heading
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
    
        if (FuelTankCapacity > FuelConsumed) {

            if (IsGrounded == false)
            {
                FuelConsumed += FuelEconomy;
                //float consumed = FuelConsumed / FuelTankCapacity;
            }
            
        }else {
            Debug.LogWarning("Helicopter out of fuel");

            if (IsGrounded == false && rigidBody.mass > 1.0f)
            {
                float consumptionRate = FuelEconomy * 100.0f / FuelTankCapacity;

                float massConsumptionRate = OriginalMass * consumptionRate / 100.0f;
                float powerConsumptionRate = Power * consumptionRate / 100.0f;
                float dragConsumptionRate = DragCoefficient * consumptionRate / 100.0f;
                float angularDragConsumptionRate = AngularDragCoefficient * consumptionRate / 100.0f;
                
                Power -= powerConsumptionRate;
                rigidBody.mass -= massConsumptionRate;
                rigidBody.drag -= dragConsumptionRate;
                rigidBody.angularDrag -= angularDragConsumptionRate;
            }else {
                Debug.LogWarning("Helicopter Dead");
            }
        }
        
        MovementKeyControlls();
        PowerKeyControlls();
        MovementModule();
        AttitudeModule();
        
        
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
