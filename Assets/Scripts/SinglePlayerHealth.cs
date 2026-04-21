using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealthSingleplayer : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;

    [Header("Health UI")]
    public Image healthImage;

    private Animator anim;
    private SinglePlayerMovement movement;
    private bool isDead = false;

    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip healSound;

    [Header("Respawn Settings")]
    public Vector3 respawnPoint;

    void Start()
    {
        anim = GetComponent<Animator>();
        movement = GetComponent<SinglePlayerMovement>();

        if (healthImage == null)
        {
            Debug.LogWarning("[PlayerHealthSingleplayer] HealthImage not assigned in Inspector!");
        }
        else
        {
            UpdateHealthUI();
        }

        // Set initial respawn point
        respawnPoint = transform.position;
    }

    void Update()
    {
        UpdateHealthUI();

        if (health <= 0 && !isDead)
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

        // Debug: Force damage with key
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Take Damage (X key) triggered");
            TakeDamage(20);
        }
    }

    private void UpdateHealthUI()
    {
        if (healthImage != null)
        {
            healthImage.fillAmount = Mathf.Clamp01(health / maxHealth);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        Debug.Log("TakeDamage triggered: " + damage);
        health -= damage;
        UpdateHealthUI();

        if (health > 0 && anim != null)
        {
            SoundManager.instance.PlaySound(hurtSound);
            anim.SetTrigger("PlayerHurt");
        }
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
        health = maxHealth;
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
        health += heal;
        UpdateHealthUI();

        if (health > 0 && anim != null)
        {
            SoundManager.instance.PlaySound(healSound);
        }
    }

}
