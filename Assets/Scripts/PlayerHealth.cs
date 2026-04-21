using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    public NetworkVariable<float> health = new NetworkVariable<float>(
        100f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public float maxHealth = 100f;
    public Vector3 respawnPoint;
    private Animator anim;
    private PlayerMovement movement;
    private bool isDead = false;

    private HealthBar healthBar;

    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip healSound;

    void Start()
    {
        anim = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
        healthBar = GetComponentInChildren<HealthBar>();
        // Find the HealthBar component in child Canvas
        healthBar = GetComponentInChildren<HealthBar>();
        UpdateHealthUI();
    }

    void Update()
    {
        UpdateHealthUI();

        if (health.Value <= 0 && !isDead)
        {
            isDead = true;

            if (movement != null) movement.enabled = false;

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.isKinematic = true;
            }

            anim.Play("PlayerDeath", 0, 0f);
            SoundManager.instance.PlaySound(deathSound);

            StartCoroutine(HandleDeath());
        }
    }

    private void UpdateHealthUI()
    {
        if (healthBar != null)
            healthBar.UpdateHealthBar(health.Value, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (!IsServer) return; // only allow server to modify health
        if (isDead) return;
        health.Value -= damage;
        UpdateHealthUI();
        if (health.Value > 0 && anim != null)
        {
            SoundManager.instance.PlaySound(hurtSound);
            anim.SetTrigger("PlayerHurt");
        }
    }
    [ServerRpc]
    public void TakeDamageServerRpc(float damage)
    {
        if (!IsServer) return;
        TakeDamage(damage);
    }

    private IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(1.3f);

        // Hide player visuals during death wait
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        string originalTag = gameObject.tag;
        gameObject.tag = "Untagged";


        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        yield return new WaitForSeconds(5f); // Wait extra before respawn

        // Respawn
        transform.position = respawnPoint;
        health.Value = maxHealth;
        isDead = false;
        gameObject.tag = originalTag;

        if (col != null) col.enabled = true;
        if (sr != null) sr.enabled = true;
        if (movement != null) movement.enabled = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector2.zero;
        }

        // reset Animator
        if (anim != null)
        {
            anim.ResetTrigger("PlayerHurt");
            anim.ResetTrigger("PlayerDeath");
            anim.SetBool("Jump", false);
            anim.SetBool("isDashing", false);
            anim.SetBool("Crouching", false);

            anim.Play("PlayerIdle", 0, 0f);


        }

        UpdateHealthUI();
    }

    public void Heal(float heal)
    {
        if (isDead) return;

        Debug.Log("Heal: " + heal);
        health.Value += heal;
        UpdateHealthUI();

        if (health.Value > 0 && anim != null)
        {
            SoundManager.instance.PlaySound(healSound);
        }
    }


}
