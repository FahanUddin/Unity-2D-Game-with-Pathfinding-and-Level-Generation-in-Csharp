using UnityEngine;

public class SingelePInstantDeath : MonoBehaviour
{
    [SerializeField] private AudioClip deathSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealthSingleplayer playerHealth = collision.GetComponent<PlayerHealthSingleplayer>();
            if (playerHealth != null)
            {
                // Instantly reduce the player's health to 0
                playerHealth.TakeDamage(playerHealth.health);

                // Play death sound 
                if (SoundManager.instance != null)
                {
                    SoundManager.instance.PlaySound(deathSound);
                }
            }
        }
    }
}
