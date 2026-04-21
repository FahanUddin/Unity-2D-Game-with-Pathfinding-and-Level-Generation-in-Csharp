using UnityEngine;
using Unity.Netcode;

public class NetworkPrefabSpawner : NetworkBehaviour
{
    [Tooltip("Prefab to spawn over the network.")]
    public GameObject prefabToSpawn;

    [Tooltip("Optional: Spawn position override. If empty, will use this object's position.")]
    public Transform spawnPositionOverride;

    [Tooltip("Automatically spawn on start (server only)?")]
    public bool spawnOnStart = false;

    public override void OnNetworkSpawn()
    {
        if (!IsServer || prefabToSpawn == null) return;

        if (spawnOnStart)
        {
            SpawnPrefabAtPosition(spawnPositionOverride ? spawnPositionOverride.position : transform.position);
        }
    }

    public void SpawnPrefabAtPosition(Vector3 position)
    {
        GameObject obj = Instantiate(prefabToSpawn, position, Quaternion.identity);

        NetworkObject netObj = obj.GetComponent<NetworkObject>();
        if (netObj != null && !netObj.IsSpawned)
        {
            netObj.Spawn(true);
            Debug.Log($"[NetworkPrefabSpawner] Spawned: {prefabToSpawn.name} at {position}");
        }
        else
        {
            Debug.LogWarning("[NetworkPrefabSpawner] Missing NetworkObject or already spawned.");
        }
    }

    [ContextMenu("Spawn Now (Editor)")]
    public void SpawnNowInEditor()
    {
        if (Application.isPlaying && IsServer)
        {
            SpawnPrefabAtPosition(spawnPositionOverride ? spawnPositionOverride.position : transform.position);
        }
        else
        {
            Debug.LogWarning("SpawnNow only works in play mode as server.");
        }
    }
}