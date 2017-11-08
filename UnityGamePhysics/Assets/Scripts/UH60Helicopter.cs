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

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += transform.forward * 2.0f;
	}
}
