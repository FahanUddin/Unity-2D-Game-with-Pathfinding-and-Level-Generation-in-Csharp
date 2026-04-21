using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Tooltip("Prefab for the host (Player1)")]
    public GameObject player1Prefab;
    [Tooltip("Prefab for the first client (Player2)")]
    public GameObject player2Prefab;
    public Transform player1SpawnPoint;
    public Transform player2SpawnPoint;
    public Camera player1Camera;
    public Camera player2Camera;

    private void OnEnable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += SpawnForClient;
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= SpawnForClient;
    }

    private void Start()
    {
        // Spawn host's player manually because host never triggers OnClientConnectedCallback
        if (NetworkManager.Singleton.IsServer)
        {
            SpawnForClient(NetworkManager.Singleton.LocalClientId);
        }
    }

    private void SpawnForClient(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        GameObject prefab = (clientId == NetworkManager.ServerClientId)
            ? player1Prefab
            : player2Prefab;

        Vector3 spawnPos = (clientId == NetworkManager.ServerClientId)
        ? player1SpawnPoint.position
        : player2SpawnPoint.position;


        Debug.Log($"[Spawner] Spawning {prefab.name} for clientId {clientId}");

        GameObject player = Instantiate(prefab, spawnPos, Quaternion.identity);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

        if (clientId == NetworkManager.ServerClientId && player1Camera != null)
        {
            player1Camera.GetComponent<CameraFollow>().primaryTarget = player.transform;
        }
        else if (clientId != NetworkManager.ServerClientId && player2Camera != null)
        {
            player2Camera.GetComponent<CameraFollow>().primaryTarget = player.transform;
        }
    }
}
