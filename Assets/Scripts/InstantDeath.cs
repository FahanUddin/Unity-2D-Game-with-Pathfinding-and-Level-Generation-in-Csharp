using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class InstantDeath : NetworkBehaviour
{
    [SerializeField] private AudioClip deathSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return; // Only the server handles death logic

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // Instantly reduce the player's health to 0 on the server
                playerHealth.TakeDamage(playerHealth.health.Value);
            }
        }
    }


}
