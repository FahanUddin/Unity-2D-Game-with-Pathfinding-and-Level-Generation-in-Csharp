using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Camera))]
public class MultiplayerCameraFollow : MonoBehaviour
{
    [Tooltip("Offset from the player position.")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Tooltip("How quickly the camera moves to follow.")]
    [Range(0f, 1f)]
    public float smoothSpeed = 0.125f;

    private Transform target;

    void LateUpdate()
    {
        // If we don’t have an owner yet, try to find it
        if (target == null)
            TryAssignLocalPlayer();

        if (target == null)
            return;

        // Smoothly follow the assigned target
        Vector3 desired = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed);
    }

    private void TryAssignLocalPlayer()
    {
        // Must be running as a client or host
        if (NetworkManager.Singleton == null ||
            !NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost)
            return;

        // Iterate all spawned NetworkObjects
        foreach (var netObj in NetworkManager.Singleton.SpawnManager.SpawnedObjectsList)
        {
            // The local player’s object will have IsOwner == true
            if (netObj.IsOwner)
            {
                target = netObj.transform;
                Debug.Log($"Camera bound to local player: {netObj.name}");
                break;
            }
        }
    }
}
