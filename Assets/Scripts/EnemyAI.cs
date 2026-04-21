using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;

public enum EnemyState { Patrol, Chase, Attack }

public class EnemyAI : NetworkBehaviour
{
    public EnemyState currentState = EnemyState.Patrol;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;
    public float patrolSpeed = 2f;

    [Header("Chase & Attack Settings")]
    public float chaseDistance = 10f;
    public float attackDistance = 7f; // Used for shooting range
    public float chaseSpeed = 4f;

    private GameObject currentTarget;
    private EnemyPathFFollow pathFollower;
    private Vector2 previousPosition;

    [Header("Shooting")]
    public GameObject bullet;
    public Transform bulletPosition;
    public float fireRate = 2f;
    public float bulletForce = 10f;
    [SerializeField] private AudioClip bulletFire;
    private float fireTimer = 0f;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        pathFollower = GetComponent<EnemyPathFFollow>();
        previousPosition = transform.position;

        Debug.Log("[EnemyAI] OnNetworkSpawn as Server");
    }

    void Update()
    {
        //if (!IsServer) return;

        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                LookForTarget();
                break;
            case EnemyState.Chase:
                Chase();
                CheckAttackRange();
                break;
            case EnemyState.Attack:
                Attack();
                break;
        }

        FlipSprite();
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, patrolSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    void LookForTarget()
    {
        GameObject candidate = FindClosestPlayer();
        // Debug.Log($"[EnemyAI] Target player is {(candidate == null ? "<none>" : candidate.name)}");

        if (candidate != null)
        {
            currentTarget = candidate;
            currentState = EnemyState.Chase;
            if (pathFollower != null)
                pathFollower.SetTarget(currentTarget.transform);
        }
    }

    GameObject FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject closest = null;
        float bestDist = Mathf.Infinity;
        foreach (GameObject p in players)
        {
            float d = Vector2.Distance(transform.position, p.transform.position);
            if (d < bestDist && d < chaseDistance)
            {
                bestDist = d;
                closest = p;
            }
        }
        //Debug.Log($"[EnemyAI] Closest player is {(closest == null ? "<none>" : closest.name)}");
        return closest;
    }

    void Chase()
    {
        if (currentTarget == null)
        {
            currentState = EnemyState.Patrol;
            return;
        }

        bool hasPath = pathFollower != null && pathFollower.HasPath();
        if (!hasPath)
        {
            transform.position = Vector2.MoveTowards(transform.position, currentTarget.transform.position, chaseSpeed * Time.deltaTime);
        }

        if (Vector2.Distance(transform.position, currentTarget.transform.position) > chaseDistance)
        {
            currentTarget = null;
            currentState = EnemyState.Patrol;
        }
    }

    void CheckAttackRange()
    {
        if (currentTarget != null && Vector2.Distance(transform.position, currentTarget.transform.position) <= attackDistance)
            currentState = EnemyState.Attack;
    }

    void Attack()
    {
        if (currentTarget == null)
        {
            currentState = EnemyState.Patrol;
            return;
        }
        float distance = Vector2.Distance(transform.position, currentTarget.transform.position);
        fireTimer += Time.deltaTime;

        if (distance <= attackDistance && IsFacingTarget())
        {
            if (fireTimer >= fireRate)
            {
                fireTimer = 0f;
                Shoot();
            }
        }
        if (distance > attackDistance)
        {
            currentState = EnemyState.Chase;
        }
    }
    void Shoot()
    {
        if (bullet == null || bulletPosition == null || currentTarget == null) return;

        GameObject b = Instantiate(bullet, bulletPosition.position, Quaternion.identity);
        SoundManager.instance.PlaySound(bulletFire);

        Vector2 direction = (currentTarget.transform.position - bulletPosition.position).normalized;
        Rigidbody2D rb = b.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = direction * bulletForce;
        float rot = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        b.transform.rotation = Quaternion.Euler(0, 0, rot);
        Debug.DrawRay(bulletPosition.position, direction * 3f, Color.red, 2f);
    }

    bool IsFacingTarget()
    {
        if (currentTarget == null) return false;
        bool facingRight = transform.localScale.x > 0;
        return facingRight
            ? currentTarget.transform.position.x > transform.position.x
            : currentTarget.transform.position.x < transform.position.x;
    }

    void FlipSprite()
    {
        float deltaX = transform.position.x - previousPosition.x;
        if (deltaX > 0.01f)
        {
            Vector3 s = transform.localScale;
            s.x = Mathf.Abs(s.x);
            transform.localScale = s;
        }
        else if (deltaX < -0.01f)
        {
            Vector3 s = transform.localScale;
            s.x = -Mathf.Abs(s.x);
            transform.localScale = s;
        }
        previousPosition = transform.position;
    }
}
