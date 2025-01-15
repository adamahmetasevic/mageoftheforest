using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

private Animator animator;

    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
{
    // Handle horizontal movement
    float moveInput = Input.GetAxis("Horizontal");
    rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y); 

    // Flip the sprite using localScale (not rotation)
    if (moveInput > 0)
        transform.localScale = new Vector3(.5f, .5f, 1); // Facing right
    else if (moveInput < 0)
        transform.localScale = new Vector3(-.5f, .5f    , 1); // Facing left

    // Check if the player is grounded
    isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

    // Handle jumping
    if (Input.GetButtonDown("Jump") && isGrounded)
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    // Handle walking animation
    if (moveInput != 0)
        animator.SetBool("isWalking", true);
    else
        animator.SetBool("isWalking", false);
}


    private void OnDrawGizmosSelected()
    {
        // Draw a circle to visualize the ground check area
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
