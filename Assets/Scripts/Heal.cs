using UnityEngine;

public class Heal : MonoBehaviour
{
    public float heal;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleHeal(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleHeal(other.gameObject);
    }

    private void HandleHeal(GameObject target)
    {
        if (!target.CompareTag("Player")) return;

        // Try multiplayer health
        PlayerHealth ph = target.GetComponent<PlayerHealth>();
        if (ph != null && ph.enabled && ph.health.Value < ph.maxHealth)
        {
            ph.Heal(heal);
            Destroy(gameObject);
            return;
        }

        // Try singleplayer health
        PlayerHealthSingleplayer sph = target.GetComponent<PlayerHealthSingleplayer>();
        if (sph != null && sph.enabled && sph.health < sph.maxHealth)
        {
            sph.Heal(heal);
            Destroy(gameObject);
            return;
        }
    }
}
