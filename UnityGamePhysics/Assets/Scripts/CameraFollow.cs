using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform Target;
    public Vector3 offset;
    public float PositionForce = 5f;
    public float RotationForce = 5f;
	
    void FixedUpdate()
	{
        var vector = Vector3.forward;
        var dir = Target.rotation * Vector3.forward;
		dir.y = 0f;
        if (dir.magnitude > 0f) vector = dir / dir.magnitude;

        transform.position = Vector3.Lerp(transform.position + offset, Target.position, PositionForce * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(vector), RotationForce * Time.deltaTime);
	}
}

