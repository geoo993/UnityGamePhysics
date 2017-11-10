using UnityEngine;
using System.Collections;

public class LookAtTargetController : MonoBehaviour
{
    public Transform Target;
    public bool smoothLook = true;
    public float damping = 6.0f;

	// Use this for initialization
	void Start () {
	
	}
	
    void LateUpdate()
    {
        LookAtTarget(Target, smoothLook, damping);
    }
    
    
    void LookAtTarget(Transform target, bool smooth, float speed){
    
        if (target != null)
        {
            if (smooth)
            {
                //rotate to look at the target
                Vector3 relativePos = target.position - transform.position;
                Quaternion look = Quaternion.LookRotation(relativePos);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, speed * Time.deltaTime);
                
            }
            else
            {
                transform.LookAt(target);
            }
        }
    
    }
    
    
    
}
