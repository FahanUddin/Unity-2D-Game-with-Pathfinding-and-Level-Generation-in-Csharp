using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour
{
    public GameObject platformPrefab;
    public Transform startPoint;
    public Transform endPoint;

    // Spacing 
    public float minXSpacing = 5f;
    public float maxXSpacing = 9f;

    // Interpret these as the min and max shift from the last Y 
    public float minYOffset = -0.6f;
    public float maxYOffset = 1f;

    // Platform scaling 
    public float minScale = 0.5f;
    public float maxScale = 1f;

    // The next spawn position
    private Vector2 nextPosition;

    void Start()
    {
        // Begin exactly at the start point.
        nextPosition = startPoint.position;

        // Spawn the first platform exactly at the start.
        Instantiate(platformPrefab, nextPosition, Quaternion.identity);

        StartCoroutine(GeneratePlatforms());
    }

    IEnumerator GeneratePlatforms()
    {
        // Continue generating platforms until nextPosition.x reaches or exceeds endPoint.x.
        while (nextPosition.x < endPoint.position.x)
        {
            // Determine a random spacing for X.
            float spacing = Random.Range(minXSpacing, maxXSpacing);
            bool isFinalPlatform = false;

            // If adding the spacing would overshoot the end point, adjust it.
            if (nextPosition.x + spacing > endPoint.position.x)
            {
                spacing = endPoint.position.x - nextPosition.x;
                isFinalPlatform = true;
            }

            // Compute the new X by adding spacing to the last platform's X.
            float newX = nextPosition.x + spacing;
            float newY;

            if (!isFinalPlatform)
            {
                // Constrain the Y offset so each new platform's Y is only slightly above or below the last one.
                float yOffset = Random.Range(minYOffset, maxYOffset);
                newY = nextPosition.y + yOffset;
            }
            else
            {
                // For the final platform, set the Y exactly to the end point's Y.
                newY = endPoint.position.y;
            }

            Vector2 spawnPosition = new Vector2(newX, newY);

            // Instantiate the platform at the computed position.
            GameObject newPlatform = Instantiate(platformPrefab, spawnPosition, Quaternion.identity);

            // Randomize the platform's X scale.
            float scaleX = Random.Range(minScale, maxScale);
            newPlatform.transform.localScale = new Vector2(scaleX, newPlatform.transform.localScale.y);

            // Update nextPosition for the next iteration.
            nextPosition = spawnPosition;

            yield return new WaitForSeconds(0.1f);
        }
    }
}
