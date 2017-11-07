using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhysicsEngine))]
public class RocketEngine : MonoBehaviour {
    
    public float fuelMass;              // [kg - kilograms]
    public float maxThrust;             // [kN - kilo newtons => F = m.a => kg.m/s^2] 
    
    [Range(0.0f, 1.0f)]
    public float thrustPercent;         // [none]
    
    public Vector3 thrustUnitVector;    // [none] this is for us to tell the engine which direction to thrust in, a directional unit vertor.
    
    private PhysicsEngine physicsEngine;
    private float currentThrust;        // N
    
	void Start () {
        physicsEngine = GetComponent<PhysicsEngine>();
        physicsEngine.mass += fuelMass; 
	}
	
    // happens more than once per frame if the fixed time step is less than the actual frame update time
	void FixedUpdate () {

        // if the fuelmass is greater than the fuel that we are going to use in this update
        if (fuelMass > FuelOnThisUpdate()) {
            // provide we have enough fuel, we can applied the current fuel on this update
            // we reduce the fuel mass
            fuelMass -= FuelOnThisUpdate();
            physicsEngine.mass -= FuelOnThisUpdate();
            ExertForce();
        }else {
            Debug.LogWarning("Out of rocket fuel");
        }
        
    }

    // working out what fuel that is going to be applied in this update
    // based on Rocket engine wiki https://en.wikipedia.org/wiki/Rocket_engine
    float FuelOnThisUpdate(){
        float exhaustGasMassFlow;           // [kg - kilograms] mass per second (ṁ)
        float effectiveExhaustVelocity;     // [m/s - meters per seconds]
        
        // based on https://en.wikipedia.org/wiki/Liquid_rocket_propellant
        effectiveExhaustVelocity = 4462.0f; // [m/s] for a liquid Hydrogen Oxigen engine

        // thrust = massFlow * exhaustVelocity
        // massFlow = currentThrust / exhaustVelocity
        // the currentThrust is already calculated in newtons
        exhaustGasMassFlow = currentThrust / effectiveExhaustVelocity;
        
        return exhaustGasMassFlow * Time.deltaTime;
    }
    
    void ExertForce(){
        
        // this updates our current thrust
        currentThrust = thrustPercent * maxThrust * 1000.0f; // multiply by 1000 to convert to newtons
		Vector3 thrustVector = thrustUnitVector.normalized * currentThrust; // moving at the thrust direction in newtons .
		physicsEngine.AddForce(thrustVector);
    }
    
    
}
