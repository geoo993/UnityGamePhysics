using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsEngine : MonoBehaviour {


    public float mass;                          // [kg - kilograms]
    public Vector3 velocityVector;              // [m/s - meters per second] average velocity this FixedUpdate()
    public Vector3 netForceVector;              // [N - newtons => F = m.a => kg.m/s^2]

    private Vector3 accelerationVector;         // [m/s^2 - meters per seconds squared]

    private const float earthCenter = -6371000f; // based on earth radius, it is 6,371 km => 6371000 meters
    private const float earthMass = 5.972e24f; // 5.972 × 10^24 kg
    
    private List<Vector3> forceVectorList = new List<Vector3>();
    private Vector3 displacementVector;
    
    // code for drawing trail
    public bool showTrails = true;
    public Color startColor = Color.yellow;
    public Color endColor = Color.cyan;
    public float trailWidth = 0.2f;
    private LineRenderer lineRenderer;
    private int numberOfForces;
    
    // call before Start at loading of script
    void Awake()
    {
        
    }
    
    // Use this for initialization
    void Start () {
        SetupThrustTrails();
	}
	
	// Update is called once per frame
	void Update () {
        //displacementVector = velocityVector * Time.deltaTime; // increasing or updating by v every second
	}

    // happens more than once per frame if the fixed time step is less than the actual frame update time
    void FixedUpdate()
    {
		RenderTrails();
        CalculateForcesAndClearList();
        UpdatePosition();
    }
    
    void NewtonsFirstLaw(){
        CalculateForcesAndClearList();
        
        // if there is no net force, then update the position
        if (netForceVector == Vector3.zero)
        {
            Debug.Log("No force or balaced force detected, great!");
            displacementVector = velocityVector * Time.deltaTime;
            transform.position += displacementVector; // update the position (moving at a constant speed)
        }else {
            // if there is a net force (sum of all the forces)
            Debug.Log("Unbalanced force detected, help!");
        }
    }
    
    void NewtonsSecondLaw(){
        CalculateForcesAndClearList();
        UpdatePosition();
        
    }
    
    public void AddForce(Vector3 force){
        forceVectorList.Add(force);
    }
    
    
    void CalculateForcesAndClearList(){
        // start at fresh each time we want to get the net forces
        netForceVector = Vector3.zero;

        foreach (Vector3 force in forceVectorList){
            netForceVector += force;
        }
        
        // clear the list of forces
        forceVectorList = new List<Vector3>();
    }
    
    void UpdatePosition(){
        // change in velocity means we are accelerating
        // the longer you accelerate over time, the more or faster you move due to an increased velocity
        accelerationVector = netForceVector / mass;  
        velocityVector += accelerationVector * Time.deltaTime;
        
        // update the position (moving at a constant speed)
        displacementVector = velocityVector * Time.deltaTime;
        transform.position += displacementVector; 
    }
    
   
    void SetupThrustTrails () {
        forceVectorList = GetComponent<PhysicsEngine>().forceVectorList;

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.useWorldSpace = false;
    }
    
    void RenderTrails () {
        if (showTrails) {
            lineRenderer.enabled = true;
            numberOfForces = forceVectorList.Count;
            lineRenderer.startColor = startColor;
            lineRenderer.endColor = endColor;
            lineRenderer.widthMultiplier = 0.2f;
            lineRenderer.positionCount = numberOfForces * 2;
            
            int i = 0;
            foreach (Vector3 forceVector in forceVectorList) {
                lineRenderer.SetPosition( i, Vector3.zero);
                lineRenderer.SetPosition( i + 1, -forceVector);
                i = i + 2;
            }
        } else {
            lineRenderer.enabled = false;
        }
    }
    
}
