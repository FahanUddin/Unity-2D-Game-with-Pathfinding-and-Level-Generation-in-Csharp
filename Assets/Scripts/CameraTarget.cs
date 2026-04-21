using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform primaryTarget;
    public Transform secondaryTarget;

    public Vector3 offset = new Vector3(0, 0, -10);
    public float smoothSpeed = 0.125f;

    private Transform currentTarget;

    void LateUpdate()
    {

        if (primaryTarget != null)
        {
            if (currentTarget != primaryTarget)
            {
                currentTarget = primaryTarget;
                Debug.Log("Camera is now following primary target");
            }
        }
        else if (secondaryTarget != null)
        {
            if (currentTarget != secondaryTarget)
            {
                currentTarget = secondaryTarget;
                Debug.Log("Camera is now following secondary target");
            }
        }

        if (currentTarget == null)
            return;

        Vector3 desiredPosition = currentTarget.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
