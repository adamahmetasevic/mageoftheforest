using UnityEngine;
using System.Collections;

public class minion : MonoBehaviour
{
    [Header("Stats")]
    public int health = 100;
    public int attackDamage = 10;
    public float moveSpeed = 3f;
    public float attackRange = 1.5f;
    public float chaseRange = 10f;
    public float attackCooldown = 1f;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.2f;
    public float heightThreshold = 2f; // Maximum height difference to chase player

    [Header("Spin Settings")]
    public float spinDuration = 1f;
    public float spinSpeed = 5f;
    private bool isSpinning = false;

    [Header("Particle System")]
    public GameObject explosionParticlePrefab;

    private float nextAttackTime = 0f;
    private Animator animator;
    private Transform player;
    private Rigidbody2D rb;
    private bool isAttacking = false;
    private bool canMove = true;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    private Collider2D myCollider; // To reference the minion's collider

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>(); // Get the collider component
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Ensure the Rigidbody2D is affected by gravity
        rb.gravityScale = 1f;  // Adjust this value as needed (usually 1 or more is good)

        // Lock the rotation to prevent unintended rotation while moving
        rb.freezeRotation = true;

        // Subscribe to the TreeBoss death event
        TreeBoss treeBoss = FindObjectOfType<TreeBoss>();
        if (treeBoss != null)
        {
            treeBoss.OnTreeBossDeath += OnTreeBossDeath;
        }

        // Disable collision with TreeBoss
        TreeBoss treeBossObj = FindObjectOfType<TreeBoss>();
        if (treeBossObj != null)
        {
            Collider2D treeBossCollider = treeBossObj.GetComponent<Collider2D>();
            if (treeBossCollider != null)
            {
                Physics2D.IgnoreCollision(myCollider, treeBossCollider, true); // Disable collision with TreeBoss
            }
        }

        // Start the spawn sequence
        StartCoroutine(SpawnSequence());
    }

    private void Update()
    {
        CheckGrounded();

        if (!isSpinning && canMove && player != null && isGrounded)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            float heightDifference = Mathf.Abs(player.position.y - transform.position.y);

            // Only chase if player is within height threshold and chase range
            if (distanceToPlayer <= chaseRange && heightDifference <= heightThreshold)
            {
                // Check if player is in attack range
                if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
                {
                    StartCoroutine(PerformAttack());
                }
                else if (!isAttacking)
                {
                    ChasePlayer();
                }
            }
            else
            {
                // Stop moving if player is out of range or too high/low
                rb.velocity = new Vector2(0, rb.velocity.y);
                if (animator != null)
                {
                    animator.SetBool("IsMoving", false);
                }
            }
        }
    }

    private void CheckGrounded()
    {
        // Raycast downwards to check if the minion is touching the ground
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null;
    }

    private void ChasePlayer()
    {
        if (!isGrounded) return;

        float direction = Mathf.Sign(player.position.x - transform.position.x);
        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

        // Update sprite direction
        spriteRenderer.flipX = direction < 0;

        if (animator != null)
        {
            animator.SetBool("IsMoving", Mathf.Abs(rb.velocity.x) > 0.1f);
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        canMove = false;
        rb.velocity = new Vector2(0, rb.velocity.y);

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // Deal damage
        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(attackDamage, DamageType.Physical);
            }
        }

        nextAttackTime = Time.time + attackCooldown;

        yield return new WaitForSeconds(0.5f); // Adjust based on animation length

        isAttacking = false;
        canMove = true;
    }

    private IEnumerator SpawnSequence()
    {
        canMove = false;
        yield return new WaitForSeconds(1f);
        canMove = true;
    }

    private IEnumerator SpinBeforeExplosion()
    {
        isSpinning = true;
        canMove = false;
        rb.velocity = Vector2.zero;

        float endTime = Time.time + spinDuration;

        while (Time.time < endTime)
        {
            transform.localScale = new Vector3(
                -transform.localScale.x,
                transform.localScale.y,
                transform.localScale.z
            );

            yield return new WaitForSeconds(1f / spinSpeed);
        }

        isSpinning = false;
        TriggerExplosion();
    }

    private void TriggerExplosion()
    {
        if (explosionParticlePrefab != null)
        {
            GameObject explosion = Instantiate(explosionParticlePrefab, transform.position, Quaternion.identity);
            ParticleSystem particleSystem = explosion.GetComponent<ParticleSystem>();
            
            if (particleSystem != null)
            {
                float duration = particleSystem.main.duration;
                Destroy(explosion, duration);
            }
        }

        // Final damage check
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer <= attackRange)
            {
                Player playerScript = player.GetComponent<Player>();
                if (playerScript != null)
                {
                    playerScript.TakeDamage(attackDamage, DamageType.Physical);
                }
            }
        }

        Destroy(gameObject);
    }

    private void OnTreeBossDeath()
    {
        if (!isSpinning)
        {
            StartCoroutine(SpinBeforeExplosion());
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0 && !isSpinning)
        {
            StartCoroutine(SpinBeforeExplosion());
        }
        else if (animator != null)
        {
            animator.SetTrigger("Hit");
        }
    }

    private void OnDrawGizmos()
    {
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Draw chase range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        // Draw ground check
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}
