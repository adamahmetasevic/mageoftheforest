using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float blinkDistance = 5f;
    public float blinkCooldown = 1f;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Blinking")]
    public LayerMask blinkLayerMask;

    private Animator animator;
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool canBlink = true;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Handle horizontal movement
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Flip the sprite
        if (moveInput > 0)
            transform.localScale = new Vector3(.5f, .5f, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-.5f, .5f, 1);

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Handle jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // Handle walking animation
        animator.SetBool("isWalking", Mathf.Abs(moveInput) > 0.1f);

        // Handle blink
        if (Input.GetButtonDown("Blink") && canBlink)
        {
            StartCoroutine(Blink());
        }
    }

    private IEnumerator Blink()
    {
        canBlink = false;
        spriteRenderer.enabled = false;

        Vector2 blinkDirection = GetBlinkDirection();
        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = currentPosition + (blinkDirection * blinkDistance);

        // Check for obstacles
        RaycastHit2D hit = Physics2D.Raycast(currentPosition, blinkDirection, blinkDistance, blinkLayerMask);
        
        if (hit.collider != null)
        {
            // If there's an obstacle, set the target position just before the hit point
            targetPosition = hit.point - (blinkDirection * 0.1f);
        }

        // Store current vertical velocity
        float currentYVelocity = rb.velocity.y;

        // Perform the teleport
        transform.position = targetPosition;

        // If blinking horizontally, preserve vertical velocity
        if (blinkDirection.y == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, currentYVelocity);
        }
        else
        {
            // If blinking vertically, reset vertical velocity
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }

        yield return new WaitForSeconds(0.1f);
        spriteRenderer.enabled = true;

        yield return new WaitForSeconds(blinkCooldown);
        canBlink = true;
    }

    private Vector2 GetBlinkDirection()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Normalize the direction to prevent faster diagonal blinking
        Vector2 direction = new Vector2(horizontal, vertical).normalized;

        // If no direction is pressed, blink in the direction the player is facing
        if (direction == Vector2.zero)
        {
            direction = new Vector2(transform.localScale.x > 0 ? 1 : -1, 0);
        }

        return direction;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}