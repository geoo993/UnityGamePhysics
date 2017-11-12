using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gizmo : MonoBehaviour {

    [SerializeField, Range(0.0f, 10.0f)]
    private float gizmoSize;
    
    [SerializeField]
    private Color gizmoColor = Color.yellow;
   
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, gizmoSize);
    }
    
}
