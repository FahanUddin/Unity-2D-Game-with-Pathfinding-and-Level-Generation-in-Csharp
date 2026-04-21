using UnityEngine;

public class ClimbableRope : MonoBehaviour
{
    private Animator anim;
    void Start()
    {

        anim = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object is the player.
        if (collision.CompareTag("Player"))
        {
            // Try to get the PlayerClimb component on the colliding object.
            PlayerClimb playerClimb = collision.GetComponent<PlayerClimb>();
            if (playerClimb != null)
            {
                playerClimb.isClimbing = true;
                anim.Play("PlayerRope", 0, 0f);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // When leaving the climbable area, disable climbing.
        if (collision.CompareTag("Player"))
        {
            PlayerClimb playerClimb = collision.GetComponent<PlayerClimb>();
            if (playerClimb != null)
            {
                playerClimb.isClimbing = false;
            }
        }
    }
}
