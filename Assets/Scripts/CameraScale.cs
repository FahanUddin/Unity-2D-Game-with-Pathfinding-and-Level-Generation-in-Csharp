using UnityEngine;

public class FixCameraScale : MonoBehaviour
{
    void LateUpdate()
    {
        // Force the local scale to be 1,1,1 regardless of parent's scale.
        transform.localScale = Vector3.one;
    }
}
