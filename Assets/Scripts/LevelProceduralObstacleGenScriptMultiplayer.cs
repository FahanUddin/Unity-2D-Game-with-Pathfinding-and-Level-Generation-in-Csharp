using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

public class LevelGeneratorObstaclesMultiplayer : NetworkBehaviour
{
    public Gridpath grid;

    [Header("Prefabs")]
    public GameObject platformPrefab;
    public List<GameObject> obstaclePrefabs;
    public GameObject finalPrefab;

    [Header("Settings")]
    public Transform startPoint;
    public float minXSpacing = 5f;
    public float maxXSpacing = 9f;
    public float minYOffset = -0.6f;
    public float maxYOffset = 1f;
    public float minScale = 0.5f;
    public float maxScale = 1f;

    public int platformsBeforeObstacle = 5;
    public int totalObstaclesToPlace = 4;
    public int finalPlatformsCount = 4; // Final platforms before final prefab

    private Vector2 nextPosition;
    private int obstaclesPlaced = 0;

    void Awake()
    {
        //if (!IsServer) return;

        nextPosition = startPoint.position;
        SpawnPlatformObjectServerRpc();

        StartCoroutine(GenerateLevel());
    }

    IEnumerator GenerateLevel()
    {
        while (obstaclesPlaced < totalObstaclesToPlace)
        {
            for (int i = 0; i < platformsBeforeObstacle; i++)
            {
                yield return SpawnPlatform();
            }
            yield return SpawnObstacle();
        }

        for (int i = 0; i < finalPlatformsCount; i++)
        {
            yield return SpawnPlatform();
        }

        float finalGap = Random.Range(8f, 12f);
        nextPosition += new Vector2(finalGap, 0f);

        yield return new WaitForFixedUpdate();
        var gp = FindObjectOfType<Gridpath>();
        gp.CreateGrid();
        Debug.Log("[LevelGenerator] Rebuilt grid with all obstacles in place.");

        if (finalPrefab != null)
        {
            SpawnFinalPrefabObjectServerRpc();
        }
    }

    IEnumerator SpawnPlatform()
    {
        SpawnPlatformObjectServerRpc();
        yield return new WaitForSeconds(0.1f);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlatformObjectServerRpc()
    {
        float spacing = Random.Range(minXSpacing, maxXSpacing);
        float yOffset = Random.Range(minYOffset, maxYOffset);
        float newX = nextPosition.x + spacing;
        float newY = nextPosition.y + yOffset;
        Vector2 spawnPosition = new Vector2(newX, newY);

        GameObject platform = Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
        platform.transform.localScale = new Vector2(Random.Range(minScale, maxScale), platform.transform.localScale.y);

        NetworkObject netObj = platform.GetComponent<NetworkObject>();
        if (netObj != null && !netObj.IsSpawned)
        {
            netObj.Spawn(true);
        }

        nextPosition = spawnPosition;
    }

    IEnumerator SpawnObstacle()
    {
        SpawnObstacleObjectServerRpc();
        yield return new WaitForSeconds(0.1f);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnObstacleObjectServerRpc()
    {
        float preObstacleGap = Random.Range(13f, 15f);
        nextPosition += new Vector2(preObstacleGap, 2f);

        GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)];
        GameObject obstacle = Instantiate(obstaclePrefab, nextPosition, Quaternion.identity);

        NetworkObject netObj = obstacle.GetComponent<NetworkObject>();
        if (netObj != null && !netObj.IsSpawned)
        {
            netObj.Spawn(true);
        }

        float postObstacleGap = Random.Range(5f, 7f);
        nextPosition += new Vector2(postObstacleGap, 1f);
        obstaclesPlaced++;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnFinalPrefabObjectServerRpc()
    {
        GameObject final = Instantiate(finalPrefab, nextPosition, Quaternion.identity);
        NetworkObject netObj = final.GetComponent<NetworkObject>();
        if (netObj != null && !netObj.IsSpawned)
        {
            netObj.Spawn(true);
        }
    }
}
