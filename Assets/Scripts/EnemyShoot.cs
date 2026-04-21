using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    [Header("Projectile")]
    public GameObject bullet;
    public Transform bulletPosition;

    [Header("Firing Settings")]
    public float fireRate = 2f;      // seconds between shots
    public float fireDistance = 7f;  // max range
    public float bulletForce = 10f;  // speed

    [Header("Audio")]
    [SerializeField] private AudioClip bulletFire;

    float timer;
    GameObject currentTarget;
    GameObject previousTarget;

    void Update()
    {
        
        currentTarget = FindClosestPlayerInRange();
        if (currentTarget == null)
            return;

        
        if (currentTarget != previousTarget)
        {
            timer = fireRate;
            previousTarget = currentTarget;
        }

        
        if (!IsFacing(currentTarget))
            return;

       
        timer += Time.deltaTime;
        if (timer < fireRate)
            return;
        timer = 0f;

        
        ShootAt(currentTarget);
    }

    GameObject FindClosestPlayerInRange()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject best = null;
        float bestDist = float.MaxValue;

        foreach (var p in players)
        {
            float d = Vector2.Distance(transform.position, p.transform.position);
            if (d < bestDist && d <= fireDistance)
            {
                bestDist = d;
                best = p;
            }
        }
        return best;
    }

    bool IsFacing(GameObject target)
    {
        Vector2 toTarget = (target.transform.position - transform.position).normalized;
        Vector2 forward = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        return Vector2.Dot(forward, toTarget) > 0f;
    }

    void ShootAt(GameObject target)
    {
        if (bullet == null || bulletPosition == null)
        {
            Debug.LogError("[EnemyShoot] Missing bullet prefab or spawn point!");
            return;
        }
        if (target == null)
        {
            Debug.LogError("[EnemyShoot] Null target in ShootAt");
            return;
        }

        Vector2 spawnPos = bulletPosition.position;
        Vector2 targetPos = target.transform.position;
        Vector2 dir = (targetPos - spawnPos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;


        // draw a debug ray for 2 seconds so you can visually confirm in Scene view
        Debug.DrawRay(spawnPos, dir * 3f, Color.red, 2f);

        // Instantiate
        GameObject b = Instantiate(bullet, spawnPos, Quaternion.Euler(0, 0, angle));
        var rb = b.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;        // clear any existing velocity
            rb.angularVelocity = 0f;           // clear any spin
            rb.gravityScale = 0f;            // disable gravity if needed
            rb.linearVelocity = dir * bulletForce;
        }
        else
        {
            Debug.LogError("[EnemyShoot] Bullet has no Rigidbody2D!");
        }

        if (bulletFire != null)
            SoundManager.instance.PlaySound(bulletFire);

        Debug.Log($"[EnemyShoot] Fired at {target.name} with velocity {dir * bulletForce}");
    }
}
