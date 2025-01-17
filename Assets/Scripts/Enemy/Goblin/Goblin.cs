using UnityEngine;
using System.Collections;

public class Goblin : Enemy
{
    [Header("Goblin Settings")]
    public float patrolSpeed = 3f;
    public float chaseSpeed = 5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public float detectionRange = 10f;
    public GameObject deathExplosionPrefab;  // Reference to the ParticleSystem prefab

    private float nextAttackTime = 0f;
    private bool movingRight = true;
    private Vector2 patrolStartPosition;
    private bool isChasing = false;  // Track if the goblin has started chasing
    private Vector2 lastKnownPlayerPosition;

    private Animator goblinAnimator;
    private Animator swordAnimator;
    public GameObject sword;

    private void Start()
    {
        base.Start();
        patrolStartPosition = transform.position;

        if (sword != null)
        {
            swordAnimator = sword.GetComponent<Animator>();
        }
        else
        {
            Debug.LogWarning("Sword not assigned to Goblin. Please assign it in the Inspector.");
        }

        goblinAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        bool isWalking = false;

        if (player != null)
        {
            if (CanSeePlayer())
            {
                ChasePlayer();
                isWalking = true;
            }
            else if (isChasing)
            {
                ContinueChase();
                isWalking = true;
            }
            else
            {
                Patrol();
                isWalking = true;
            }

            if (Time.time >= nextAttackTime && Vector2.Distance(transform.position, player.position) <= attackRange)
            {
                MeleeAttack();
            }
        }

        goblinAnimator.SetBool("isWalking", isWalking);
        base.CheckDespawnDistance();
    }

    public override void Move() { }

    private void Patrol()
{
    Vector2 movement = movingRight ? Vector2.right : Vector2.left;
    
    // Check for collisions before moving
    RaycastHit2D hit = Physics2D.Raycast(transform.position, movement, 0.5f, LayerMask.GetMask("Obstacles", "Ground"));

    if (hit.collider != null)
    {
        // If there's an obstacle ahead, turn around
        movingRight = !movingRight;
        Flip();
    }
    else
    {
        // Move only if there's no obstacle
        Vector2 newPosition = (Vector2)transform.position + movement * patrolSpeed * Time.deltaTime;
        
        // Check if the new position is valid (not inside a collider)
        if (!Physics2D.OverlapPoint(newPosition, LayerMask.GetMask("Obstacles", "Ground")))
        {
            transform.position = newPosition;
        }
        else
        {
            // If the new position is invalid, turn around
            movingRight = !movingRight;
            Flip();
        }
    }
}


    private bool CanSeePlayer()
{
    if (Vector2.Distance(transform.position, player.position) <= detectionRange && IsPlayerOnSameLevel())
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        
        // Check if the player is in front of the goblin
        bool playerInFront = (transform.localScale.x > 0 && directionToPlayer.x > 0) || 
                             (transform.localScale.x < 0 && directionToPlayer.x < 0);

        if (playerInFront)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, detectionRange, LayerMask.GetMask("Obstacles"));

            if (hit.collider == null)
            {
                lastKnownPlayerPosition = player.position;
                return true;
            }
        }
    }
    return false;
}
    private void ChasePlayer()
    {
        isChasing = true;
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);

        if (player.position.x < transform.position.x && transform.localScale.x > 0)
        {
            Flip();
        }
        else if (player.position.x > transform.position.x && transform.localScale.x < 0)
        {
            Flip();
        }

        lastKnownPlayerPosition = player.position;
    }

    private void ContinueChase()
    {
        if (Vector2.Distance(transform.position, lastKnownPlayerPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, lastKnownPlayerPosition, chaseSpeed * Time.deltaTime);
        }
        else
        {
            isChasing = false;
        }
    }



    private void Flip()
    {
        Vector3 currentScale = transform.localScale;
        currentScale.x = -currentScale.x;
        transform.localScale = currentScale;
    }

    private bool IsPlayerOnSameLevel()
    {
        return Mathf.Abs(transform.position.y - player.position.y) < 0.5f;
    }

    private void MeleeAttack()
    {
        if (Time.time < nextAttackTime) return;

        if (swordAnimator != null)
        {
            swordAnimator.SetTrigger("Swing");
        }

        Debug.Log($"{gameObject.name} performs a melee attack!");
        nextAttackTime = Time.time + attackCooldown;

        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(damage, DamageType.Physical);
            }
        }
    }

    protected override void Die()
{
    // Instantiate the death explosion prefab at the Goblin's position
    if (deathExplosionPrefab != null)
    {
        GameObject explosion = Instantiate(deathExplosionPrefab, transform.position, Quaternion.identity);
        ParticleSystem particleSystem = explosion.GetComponent<ParticleSystem>();
        Destroy(explosion.gameObject, 1f);
        if (particleSystem != null)
        {
            
        }
        else
        {
            Debug.LogWarning("ParticleSystem component not found on the instantiated prefab.");
            Destroy(explosion.gameObject, 0.6f);
        }
    }
    else
    {
        Debug.LogWarning("Death explosion prefab not assigned to Goblin. Please assign it in the Inspector.");
    }

    base.Die();
    Debug.Log("Goblin has been slain!");
}

private IEnumerator DestroyAfterExplosion(GameObject explosionObject, ParticleSystem explosion)
{
    // Wait for the explosion's duration to finish
    yield return new WaitForSeconds(explosion.main.duration);

    // After explosion, destroy the explosion object
    Destroy(explosionObject);

    // After explosion, destroy the Goblin
    Destroy(gameObject);
}

}
