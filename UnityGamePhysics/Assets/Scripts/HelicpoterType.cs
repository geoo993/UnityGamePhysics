using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Aircrafts
// https://free3d.com/3d-model/world-war-2-aircraft-46520.html
// https://free3d.com/3d-model/russian-ww-2-p-39-airacobra-33185.html
// https://free3d.com/3d-model/airplane-g17sl-30840.html

/// Helicopters
// https://free3d.com/3d-model/uh-60-blackhawk-helicopter-93546.html
// https://free3d.com/3d-model/uh60-helicopter-47194.html
// https://free3d.com/3d-model/mi28-havoc-85395.html
// http://x3dm.com/3D-Model/RAH_66_Comanche_stealth_helicopter_3431.htm
// https://free3d.com/3d-model/helicopter-n916mu-72852.html
// https://free3d.com/3d-model/bell-407-45180.html
// http://www.cadnav.com/3d-models/model-11523.html
// https://free3d.com/3d-model/skycrane-12410.html
// https://free3d.com/3d-model/helicopter-51398.html
// http://www.cadnav.com/3d-models/model-11522.html
// http://www.cadnav.com/3d-models/model-11528.html

// Drone
// https://free3d.com/3d-model/simple-drone-69842.html
// https://free3d.com/3d-model/drone-osprey-42216.html
// https://free3d.com/3d-model/black-ops-2-mq-27-dragonfire-46317.html
// https://free3d.com/3d-model/super-drone-4666.html

public struct HelicopterSpecs {
   
    public float NeverExceedSpeed; ///357km/h => m/s// speed limit
    public float MaximumSpeed;///294km/h // 1219.2
    public float CruiseSpeed;//m/s
    public float RateOfClimb; //m/s
    public float Ceiling; //meter 
    public float Power;  //shp => kw => kg-m/s 
    
    public float EmptyWeight; //(4,819 kg) 
    //public float MaxTakeoffWeight = 10660.0f; //(10,660 kg) // what the pilot is allowed to take off with
    //public float Payload = 5280f;// kilogram //(11,640 pound) // amount it can lift (like cargos)
    //public float DiscArea = Mathf.Pow(210.0f, 2.0f);//(210 m²)
    public float EngineRotationSpeed;
    public float MainRotorRotationSpeed;

    public float FuelTankCapacity; // 360.00 gallon  1,362.74 litres
    public float FuelEconomy; // 0.32 km per litre  0.76 NM per gallon
    
    public float TurnForce;
    public float TurnTiltForce;
    public float ForwardTiltForce;
    public float TurnTiltForcePercent;
    public float TurnForcePercent;
    public float DragCoefficient; // CD is the drag coefficient – a dimensionless number.
    public float AngularDragCoefficient; // CD is the drag coefficient – a dimensionless number.
    
 }


public class HelicpoterType : MonoBehaviour
{
    
    public enum Helicpoter { None, RAH66Comanche, UH60Blackhawk };
    public Helicpoter helicopter = Helicpoter.None;
    
    private Helicpoter selectedHelicopter;
    private HelicopterSpecs currentHelicopterSpecs;
    private GameObject currentHelicopter;


    // Update is called once per frame
    void Update () {
        SelectHelicopter();
    }
    
    void SelectHelicopter(){
		// has the helicopter changed 
        if (helicopter != selectedHelicopter){
            selectedHelicopter = helicopter;
            SwitchHelicopter();
        }
    }
    
    void SwitchHelicopter(){

        if (currentHelicopter)
        {
            currentHelicopter.GetComponent<HelicopterController>().DeActivate();
        }
        
        switch (helicopter)
        {
            case Helicpoter.RAH66Comanche:
                currentHelicopter = GameObject.FindGameObjectWithTag("RAH66Comanche");
                RAH66ComancheSpecs();
                break;
            case Helicpoter.UH60Blackhawk:
                currentHelicopter = GameObject.FindGameObjectWithTag("UH60BlackHawk");
                BlackHawkSpecs();
                break;
            case Helicpoter.None:
                currentHelicopter = null;
                break;
            default:
                break;
        }

        if (currentHelicopter)
        {
            currentHelicopter.GetComponent<HelicopterController>().Run(currentHelicopterSpecs);
            Camera.main.GetComponent<MouseOrbit>().target = currentHelicopter.transform;
        }

            
    }
    
    void RAH66ComancheSpecs(){

        // https://www.aircraftcompare.com/helicopter-airplane/boeing-rah-66-comanche/457
        // http://www.deagel.com/Combat-Aircraft/RAH-66-Comanche_a000518001.aspx
        // https://en.wikipedia.org/wiki/Boeing%E2%80%93Sikorsky_RAH-66_Comanche
        currentHelicopterSpecs.NeverExceedSpeed = 100.0f;
        currentHelicopterSpecs.MaximumSpeed = 89.855f;
        currentHelicopterSpecs.CruiseSpeed = 84.9376f;
        currentHelicopterSpecs.RateOfClimb = 4.55f; 
        currentHelicopterSpecs.Ceiling = 4566.0f;
        currentHelicopterSpecs.EmptyWeight = 4220.0f;
        currentHelicopterSpecs.Power = 118796.93881192534f;
        currentHelicopterSpecs.EngineRotationSpeed = 24.132192f;
        currentHelicopterSpecs.MainRotorRotationSpeed = 261.0052768f;
    
        currentHelicopterSpecs.FuelTankCapacity = 1142.0f; 
        currentHelicopterSpecs.FuelEconomy = 0.42f;
        
        currentHelicopterSpecs.TurnForce = 3.0f;
        currentHelicopterSpecs.TurnTiltForce = 30f;
        currentHelicopterSpecs.ForwardTiltForce = 20.0f;
        currentHelicopterSpecs.TurnTiltForcePercent = 1.5f;
        currentHelicopterSpecs.TurnForcePercent = 1.3f;
        currentHelicopterSpecs.DragCoefficient = 1.05f;
        currentHelicopterSpecs.AngularDragCoefficient = 6.0f; 
    }
    
    
    void BlackHawkSpecs(){
        // https://en.wikipedia.org/wiki/Sikorsky_UH-60_Black_Hawk
        // http://www.deagel.com/Support-Aircraft/UH-60L-Blackhawk_a000508002.aspx
        // https://www.aircraftcompare.com/helicopter-airplane/sikorsky-uh-60-m-black-hawk/235
        currentHelicopterSpecs.NeverExceedSpeed = 99.2429f;
        currentHelicopterSpecs.MaximumSpeed = 81.8083f;
        currentHelicopterSpecs.CruiseSpeed = 75.9968f; 
        currentHelicopterSpecs.RateOfClimb = 4.5f; 
        currentHelicopterSpecs.Ceiling = 5837.0f; 
        currentHelicopterSpecs.EmptyWeight = 5919.0f;
        currentHelicopterSpecs.Power = 143779.9860298839f; 
        currentHelicopterSpecs.EngineRotationSpeed = 17.132192f;
        currentHelicopterSpecs.MainRotorRotationSpeed = 221.0052768f;
    
        currentHelicopterSpecs.FuelTankCapacity = 1362.74f; 
        currentHelicopterSpecs.FuelEconomy = 0.32f;
        
        currentHelicopterSpecs.TurnForce = 3.0f;
        currentHelicopterSpecs.TurnTiltForce = 30f;
        currentHelicopterSpecs.ForwardTiltForce = 20.0f;
        currentHelicopterSpecs.TurnTiltForcePercent = 1.5f;
        currentHelicopterSpecs.TurnForcePercent = 1.3f;
        currentHelicopterSpecs.DragCoefficient = 1.05f;
        currentHelicopterSpecs.AngularDragCoefficient = 6.0f; 
	}
	
}
