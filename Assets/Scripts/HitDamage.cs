using UnityEngine;

public class HitDamage : MonoBehaviour
{
    public float damage;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleDamage(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleDamage(other.gameObject);
    }

    private void HandleDamage(GameObject target)
    {
        if (target.CompareTag("Player"))
        {
            // Try multiplayer health first
            PlayerHealth ph = target.GetComponent<PlayerHealth>();
            if (ph != null && ph.enabled)
            {
                Debug.Log("Physical/Bullet damage triggered (Multiplayer)");
                ph.GetComponent<PlayerHealth>().TakeDamageServerRpc(damage);
                return;
            }

            // Otherwise try singleplayer health
            PlayerHealthSingleplayer sph = target.GetComponent<PlayerHealthSingleplayer>();
            if (sph != null && sph.enabled)
            {
                Debug.Log("Physical/Bullet damage triggered (Singleplayer)");
                sph.TakeDamage(damage);
                return;
            }

            Debug.LogWarning("No working health script found on Player!");
        }
    }

}
