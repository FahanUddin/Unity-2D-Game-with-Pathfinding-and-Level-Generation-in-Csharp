using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(CameraFollow))]
public class SplitCameraBinder : MonoBehaviour
{
    private CameraFollow follow;

    void Awake()
    {
        follow = GetComponent<CameraFollow>();
    }

    void LateUpdate()
    {

        if (follow.primaryTarget != null && follow.secondaryTarget != null)
            return;

        var nm = NetworkManager.Singleton;
        if (nm == null || nm.SpawnManager == null)
            return;

        foreach (var netObj in nm.SpawnManager.SpawnedObjectsList)
        {

            if (!netObj.TryGetComponent<PlayerMovement>(out _))
                continue;

            if (netObj.OwnerClientId == nm.LocalClientId)
            {
                if (follow.primaryTarget == null)
                {
                    follow.primaryTarget = netObj.transform;
                    Debug.Log($"[SplitCameraBinder] primary ← {netObj.name} (OwnerID {netObj.OwnerClientId})");
                }
            }
            else
            {
                if (follow.secondaryTarget == null)
                {
                    follow.secondaryTarget = netObj.transform;
                    Debug.Log($"[SplitCameraBinder] secondary ← {netObj.name} (OwnerID {netObj.OwnerClientId})");
                }
            }
        }
    }
}
