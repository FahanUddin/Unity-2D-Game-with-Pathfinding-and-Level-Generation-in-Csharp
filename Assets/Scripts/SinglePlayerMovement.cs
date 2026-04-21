using System.Collections;
using UnityEngine;

public class SinglePlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;

    public float runSpeed = 40f;
    public int playerNumber = 1; // 1 = WASD, 2 = Arrow keys

    float horizontalMove = 0f;
    public int maxJumpCount = 1;
    private int jumpCount = 0;
    bool jump = false;
    bool crouch = false;
    bool isJumping = false;

    [Header("Dash Settings (Player 2 only)")]
    [SerializeField] private float dashForce = 40f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1.5f;
    private bool canDash = true;
    private bool isDashing = false;

    [Header("Audio")]
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip dashSound;
    public float stepSpeed = 0.5f;

    private Rigidbody2D rb;

    void Start()
    {
        controller = GetComponent<CharacterController2D>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        controller.OnLandEvent.AddListener(OnLanding);

        if (playerNumber == 1)
            maxJumpCount = 3; // Triple jump for Player 1
        else
            maxJumpCount = 1; // Normal jump for Player 2
    }

    void Update()
    {
        if (!isDashing)
        {
            HandleMovementInput();
        }

        HandleJumpInput();
        HandleCrouchInput();
        HandleDashInput(); // Player 2 only
    }

    private void HandleMovementInput()
    {
        if (playerNumber == 1)
        {
            horizontalMove = Input.GetKey(KeyCode.A) ? -runSpeed :
                             Input.GetKey(KeyCode.D) ? runSpeed : 0f;
        }
        else if (playerNumber == 2)
        {
            horizontalMove = Input.GetKey(KeyCode.LeftArrow) ? -runSpeed :
                             Input.GetKey(KeyCode.RightArrow) ? runSpeed : 0f;
        }

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Mathf.Abs(horizontalMove) > 0f && !isJumping)
        {
            if (!IsInvoking(nameof(PlayFootstepSound)))
                InvokeRepeating(nameof(PlayFootstepSound), 0f, stepSpeed);
        }
        else
        {
            CancelInvoke(nameof(PlayFootstepSound));
        }
    }

    private void HandleJumpInput()
    {
        bool jumpKeyPressed = (playerNumber == 1) ? Input.GetKeyDown(KeyCode.W) : Input.GetKeyDown(KeyCode.UpArrow);

        if (jumpKeyPressed && jumpCount < maxJumpCount)
        {
            jump = true;
            isJumping = true;
            jumpCount++;

            animator.SetBool("Jump", true);
            CancelInvoke(nameof(PlayFootstepSound));
            SoundManager.instance?.PlaySound(jumpSound);
        }
    }

    private void HandleCrouchInput()
    {
        bool crouchDown = (playerNumber == 1) ? Input.GetKeyDown(KeyCode.S) : Input.GetKeyDown(KeyCode.DownArrow);
        bool crouchUp = (playerNumber == 1) ? Input.GetKeyUp(KeyCode.S) : Input.GetKeyUp(KeyCode.DownArrow);

        if (crouchDown) crouch = true;
        else if (crouchUp) crouch = false;
    }

    private void HandleDashInput()
    {
        if (playerNumber != 2) return; // Only Player 2 can dash

        bool dashPressed = Input.GetKeyDown(KeyCode.RightShift);

        if (dashPressed && canDash)
        {
            StartCoroutine(PerformDash());
        }
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump, jumpCount > 0 && jumpCount < maxJumpCount);
        }
        jump = false;
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;

        if (animator != null)
        {
            animator.SetBool("isDashing", true);
            SoundManager.instance?.PlaySound(dashSound);
        }

        if (rb != null)
        {
            float dashDirection = transform.localScale.x > 0 ? 1f : -1f;
            rb.linearVelocity = new Vector2(dashDirection * dashForce, rb.linearVelocity.y);
        }

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        if (animator != null)
        {
            animator.SetBool("isDashing", false);
            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        }

        yield return new WaitForSeconds(dashCooldown - dashDuration);
        canDash = true;
    }

    void PlayFootstepSound()
    {
        SoundManager.instance?.PlaySound(walkSound);
    }

    public void OnLanding()
    {
        animator.SetBool("Jump", false);
        isJumping = false;
        jumpCount = 0; // Reset jumps when touching the ground
    }

    public void OnCrouch(bool isCrouching)
    {
        animator.SetBool("Crouching", isCrouching);
    }
}
