using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGeneratorObstacles : MonoBehaviour
{
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

    void Start()
    {
        nextPosition = startPoint.position;
        Instantiate(platformPrefab, nextPosition, Quaternion.identity);

        StartCoroutine(GenerateLevel());
    }

    public IEnumerator GenerateLevel()
    {
        while (obstaclesPlaced < totalObstaclesToPlace)
        {
            // Spawn platforms leading to obstacle
            for (int i = 0; i < platformsBeforeObstacle; i++)
            {
                yield return SpawnPlatform();
            }
            // Spawn obstacle
            yield return SpawnObstacle();
        }
        for (int i = 0; i < finalPlatformsCount; i++)
        {
            yield return SpawnPlatform();
        }
        float finalGap = Random.Range(8f, 12f);
        nextPosition += new Vector2(finalGap, 0f);
        yield return new WaitForSeconds(0.2f);
        if (finalPrefab != null)
        {
            // Spawn final flag
            Instantiate(finalPrefab, nextPosition, Quaternion.identity);
        }
    }


    IEnumerator SpawnPlatform()
    {
        float spacing = Random.Range(minXSpacing, maxXSpacing);
        float yOffset = Random.Range(minYOffset, maxYOffset);
        float newX = nextPosition.x + spacing;
        float newY = nextPosition.y + yOffset;
        Vector2 spawnPosition = new Vector2(newX, newY);
        GameObject platform = Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
        float scaleX = Random.Range(minScale, maxScale);
        platform.transform.localScale = new Vector2(scaleX, platform.transform.localScale.y);
        nextPosition = spawnPosition;

        yield return new WaitForSeconds(0.1f);
    }
    IEnumerator SpawnObstacle()
    {
        if (obstaclePrefabs.Count == 0)
        {
            Debug.LogWarning("No obstacle prefabs assigned!");
            yield break;
        }
        //  gap before the obstacle
        float preObstacleGap = Random.Range(8f, 12f);
        nextPosition += new Vector2(preObstacleGap, 0f);

        GameObject randomObstacle = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)];
        Instantiate(randomObstacle, nextPosition, Quaternion.identity);
        //  gap after obstacle
        float postObstacleGap = Random.Range(6f, 8f);
        nextPosition += new Vector2(postObstacleGap, 1.4f);

        obstaclesPlaced++;

        yield return new WaitForSeconds(0.1f);
    }
}
