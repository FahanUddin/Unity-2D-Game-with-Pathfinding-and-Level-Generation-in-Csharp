using UnityEngine;

public class RopeSpawnTrigger : MonoBehaviour
{
    public RopeTriggeredRelease ropeTriggeredRelease;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ropeTriggeredRelease.GenerateRope();
            gameObject.SetActive(false); // disable trigger after use
        }
    }
}
