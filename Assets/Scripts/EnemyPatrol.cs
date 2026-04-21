using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public GameObject pointA;
    public GameObject pointB;
    private Rigidbody2D rb;
    private Animator anim;
    private Transform currentPoint;
    public float speed;
    public float switchThreshold = 0.5f;

    // Tracks which direction the enemy is facing; true means facing right.
    private bool facingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentPoint = pointB.transform;
        anim.SetBool("Walking", true);
    }

    void Update()
    {
        // Check if the enemy is close enough to the current target point
        if (Vector2.Distance(transform.position, currentPoint.position) < switchThreshold)
        {
            // Switch target when within threshold distance
            currentPoint = (currentPoint == pointB.transform) ? pointA.transform : pointB.transform;

            // Flip the sprite when changing direction
            Flip();
        }

        // Set velocity based on current target
        if (currentPoint == pointB.transform)
        {
            rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(-speed, rb.linearVelocity.y);
        }
    }

    // Flips the character's sprite by inverting the x scale.
    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.DrawWireSphere(pointA.transform.position, 0.5f);
            Gizmos.DrawWireSphere(pointB.transform.position, 0.5f);
            Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);
        }
    }
}
