using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct HelicopterSpecs {
   
    //public float NeverExceedSpeed; //357000.0f ///357km/h // speed limit
    public float TopSpeed;//100.0f // at High Altitude //294000.0f; //294km/h // 1219.2
    public float CruiseSpeed;//77.0f; //280000.0f; //280km/h
    public float RateOfClimb; //m/s
    public float Ceiling; //meter 
    public float Mass; //W/kg)
    public float Power;  //3,780 shp => kg-m/s 
    
    //public float EmptyWeight = 4819.0f; //(4,819 kg) 
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
    
        currentHelicopterSpecs.TopSpeed = 20.0f;
        currentHelicopterSpecs.CruiseSpeed = 10.0f;
        currentHelicopterSpecs.RateOfClimb = 4.5f; 
        currentHelicopterSpecs.Ceiling = 5837.0f;
        currentHelicopterSpecs.Mass = 158.0f;
        currentHelicopterSpecs.Power = 287675.7088631541f;
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
    
    
    void BlackHawkSpecs(){
        // https://en.wikipedia.org/wiki/Sikorsky_UH-60_Black_Hawk
        // http://www.deagel.com/Support-Aircraft/UH-60L-Blackhawk_a000508002.aspx
        // https://www.aircraftcompare.com/helicopter-airplane/sikorsky-uh-60-m-black-hawk/235
        currentHelicopterSpecs.TopSpeed = 100.0f; // 20.0f
        currentHelicopterSpecs.CruiseSpeed = 77.0f; // 10.0f
        currentHelicopterSpecs.RateOfClimb = 4.5f; 
        currentHelicopterSpecs.Ceiling = 5837.0f;
        currentHelicopterSpecs.Mass = 158.0f;
        currentHelicopterSpecs.Power = 287675.7088631541f;
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
