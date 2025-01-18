using UnityEngine;

public class minion : MonoBehaviour
{
    public int health = 100;
    public int attackDamage = 10;
    public float attackRange = 1.5f;
    private float nextAttackTime = 0f;
    public float attackCooldown = 1f;
    private float lastAttackTime;
    private Animator animator;
    private Transform player;

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Subscribe to the TreeBoss death event
        TreeBoss treeBoss = FindObjectOfType<TreeBoss>(); // Assuming there is only one TreeBoss in the scene
        if (treeBoss != null)
        {
            treeBoss.OnTreeBossDeath += OnTreeBossDeath;  // Subscribe to the death event
        }
    }

    // Called when TreeBoss dies
    private void OnTreeBossDeath()
    {
        // Perform actions such as attacking or playing a death animation
        animator.SetTrigger("Die");  // Trigger death animation for the minion

        // Optional: Deal final damage to nearby enemies or the player
        DealMeleeDamage();

        // Destroy the minion after a brief delay to allow the death animation to play
        Destroy(gameObject, 1f);  // Delay destruction to let the death animation finish
    }

    private void DealMeleeDamage()
    {
        if (Time.time < nextAttackTime) return;
        // Perform a final melee attack when the minion dies
        nextAttackTime = Time.time + attackCooldown;

        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(attackDamage, DamageType.Physical);
            }
        }
    }

    // Optional: Can be triggered by another event, like taking damage
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die(); // Call die function when health reaches 0
        }
    }

    // Perform normal death behavior
    public void Die()
    {
        animator.SetTrigger("Die"); // Play death animation
        Destroy(gameObject, 1f);  // Destroy the minion after the animation
    }

    // Optional: Visualization of death radius with debug
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
