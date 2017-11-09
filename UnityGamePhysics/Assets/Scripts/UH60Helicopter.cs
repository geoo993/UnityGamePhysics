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

// Drone
// https://free3d.com/3d-model/simple-drone-69842.html
// https://free3d.com/3d-model/drone-osprey-42216.html
// https://free3d.com/3d-model/black-ops-2-mq-27-dragonfire-46317.html
// https://free3d.com/3d-model/super-drone-4666.html

public class UH60Helicopter : MonoBehaviour {


	public float pitch = 0.0f;
	public float yaw = 0.0f;
    public float roll = 0.0f;

    public Transform target;

    private float rotationSpeed = 20.0f;
   
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        //rotate to look at the player
        Vector3 relativePos = target.position - transform.position;
        Quaternion look = Quaternion.LookRotation(relativePos);
        transform.rotation = Quaternion.Slerp(transform.rotation, look, rotationSpeed * Time.deltaTime);


        Vector3 angles = GetPitchYawRollDeg(transform.rotation);

        print("angles " + angles);
        //calculateAngles();
        
    }
    
    
    // https://answers.unity.com/questions/756467/getting-object-rotation-on-just-1-axis.html
    // https://gamedev.stackexchange.com/questions/136712/locking-a-rotation-on-an-axisyaw-pitch-roll-based-on-a-parental-transform
    void calculateAngles(){
    
		Vector3 eulerAngles=new Vector3(pitch, yaw, roll);
		transform.rotation = Quaternion.Euler(eulerAngles);
    
    }
    
    
    public static Vector3 GetPitchYawRollRad(Quaternion rotation)
    {
         float roll = Mathf.Atan2(2*rotation.y*rotation.w - 2*rotation.x*rotation.z, 1 - 2*rotation.y*rotation.y - 2*rotation.z*rotation.z);
         float pitch = Mathf.Atan2(2*rotation.x*rotation.w - 2*rotation.y*rotation.z, 1 - 2*rotation.x*rotation.x - 2*rotation.z*rotation.z);
         float yaw = Mathf.Asin(2*rotation.x*rotation.y + 2*rotation.z*rotation.w);
                     
         return new Vector3(pitch, yaw, roll);
    }
                 
    public static Vector3 GetPitchYawRollDeg(Quaternion rotation)
    {
         Vector3 radResult = GetPitchYawRollRad(rotation);
         return new Vector3(radResult.x * Mathf.Rad2Deg, radResult.y * Mathf.Rad2Deg, radResult.z * Mathf.Rad2Deg);
    }
    
}
