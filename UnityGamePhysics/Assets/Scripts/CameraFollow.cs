using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public GameObject Target;
    public Vector3 Offset;
    public float Angle;
    public float PositionForce = 5f;
    public float RotationForce = 5f;

    public bool IsDrone = true;

    private Vector3 velocityCameraFollow;
    
    void FixedUpdate()
	{
        if (IsDrone)
        {
            transform.position = Vector3.SmoothDamp(transform.position, Target.transform.TransformPoint(Offset) + Vector3.up * Input.GetAxis("Vertical2"), ref velocityCameraFollow, 0.1f);
            transform.rotation = Quaternion.Euler(Angle, Target.GetComponent<DroneMovement>().yaw, 0.0f);
        }
        else
        {

            var vector = Vector3.forward;
            var dir = Target.transform.rotation * Vector3.forward;
            dir.y = 0f;
            if (dir.magnitude > 0f) vector = dir / dir.magnitude;

            transform.position = Vector3.Lerp(transform.position + Offset, Target.transform.position, PositionForce * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(vector), RotationForce * Time.deltaTime);
        }
        
	}
}

