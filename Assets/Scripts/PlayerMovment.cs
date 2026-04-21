using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
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

    [Header("Dash Settings")]
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

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        // Camera cam = Camera.main;
        // if (cam != null)
        // {
        //     CameraFollow follow = cam.GetComponent<CameraFollow>();
        //     if (follow != null)
        //     {
        //         follow.primaryTarget = this.transform;
        //     }
        // }

        controller = GetComponent<CharacterController2D>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        playerNumber = (int)(NetworkManager.Singleton.LocalClientId == NetworkManager.ServerClientId ? 1 : 2);
    }

    void Start()
    {
        maxJumpCount = (playerNumber == 1) ? 3 : 1; // Player 1 triple jump
    }

    void Update()
    {
        if (!IsOwner) return;

        bool isMultiplayer = NetworkManager.Singleton.ConnectedClientsList.Count > 1;

        if (!isDashing)
        {
            if (playerNumber == 1 || (playerNumber == 2 && isMultiplayer))
            {
                horizontalMove = Input.GetKey(KeyCode.A) ? -runSpeed :
                                 Input.GetKey(KeyCode.D) ? runSpeed : 0f;
            }
            else
            {
                horizontalMove = Input.GetKey(KeyCode.LeftArrow) ? -runSpeed :
                                 Input.GetKey(KeyCode.RightArrow) ? runSpeed : 0f;
            }
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

        bool jumpKey = (playerNumber == 1) ? Input.GetKeyDown(KeyCode.W) :
                        (isMultiplayer ? Input.GetKeyDown(KeyCode.W) : Input.GetKeyDown(KeyCode.UpArrow));

        if (jumpKey && jumpCount < maxJumpCount)
        {
            jump = true;
            isJumping = true;
            jumpCount++;

            animator.SetBool("Jump", true);
            CancelInvoke(nameof(PlayFootstepSound));
            SoundManager.instance.PlaySound(jumpSound);
        }

        bool crouchDown = false;
        bool crouchUp = false;

        if (playerNumber == 1 || (playerNumber == 2 && isMultiplayer))
        {
            crouchDown = Input.GetKeyDown(KeyCode.S);
            crouchUp = Input.GetKeyUp(KeyCode.S);
        }
        else
        {
            crouchDown = Input.GetKeyDown(KeyCode.DownArrow);
            crouchUp = Input.GetKeyUp(KeyCode.DownArrow);
        }

        if (crouchDown) crouch = true;
        else if (crouchUp) crouch = false;

        // Dash input (Player 2 only)
        if (playerNumber == 2)
        {
            bool dashKey = isMultiplayer
                ? Input.GetKeyDown(KeyCode.LeftShift)
                : Input.GetKeyDown(KeyCode.RightShift);

            if (dashKey && canDash)
            {
                StartCoroutine(PerformDash());
            }
        }
    }

    void PlayFootstepSound()
    {
        SoundManager.instance.PlaySound(walkSound);
    }

    public void OnCrouch(bool isCrouching)
    {
        animator.SetBool("Crouching", isCrouching);
    }

    public void OnLanding()
    {
        animator.SetBool("Jump", false);
        isJumping = false;
        jumpCount = 0;
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
            animator.SetBool("isDashing", true); // Start dash animation
            SoundManager.instance.PlaySound(dashSound);
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
            animator.SetBool("isDashing", false); // Stop dash animation
            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        }

        yield return new WaitForSeconds(dashCooldown - dashDuration);
        canDash = true;
    }

}
