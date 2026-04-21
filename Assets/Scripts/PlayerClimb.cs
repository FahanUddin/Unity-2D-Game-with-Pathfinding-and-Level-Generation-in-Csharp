using UnityEngine;

public class PlayerClimb : MonoBehaviour
{
    private Rigidbody2D rb;
    public bool isClimbing = false;
    public float climbSpeed = 3f;

    private Rigidbody2D ropeRb; // reference to the rope segment's rigidbody

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isClimbing)
        {
            float vertical = Input.GetAxisRaw("Vertical");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, vertical * climbSpeed);

            // Apply sway force to rope segment
            if (ropeRb != null)
            {
                float swayForce = 0.5f;
                ropeRb.AddForce(new Vector2(Random.Range(-swayForce, swayForce), 0f), ForceMode2D.Force);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ClimbableRope"))
        {
            isClimbing = true;
            rb.gravityScale = 0;
            rb.linearVelocity = Vector2.zero;

            // Cache the rope segment's Rigidbody2D
            ropeRb = collision.attachedRigidbody;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ClimbableRope"))
        {
            isClimbing = false;
            rb.gravityScale = 1;

            ropeRb = null;
        }
    }
}
