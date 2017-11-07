using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class GoalBox : MonoBehaviour {

    private MeshRenderer meshRenderer;
    void Start(){
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.color = Color.blue;
    }


    void OnCollisionEnter(Collision collision)
    {
        print("Collision Entered");
    }
    
    void OnTriggerEnter(Collider other)
    {
        print("Trigger Entered");
        Color newColor = new Color( Random.value, Random.value, Random.value, 1.0f );
        meshRenderer.material.color = newColor;
    }

    void OnTriggerExit(Collider other)
    {
    
    }
}
