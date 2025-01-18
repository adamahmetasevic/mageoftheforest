using UnityEngine;
using System.Collections.Generic;

public abstract class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    [Header("Enemy Stats")]
    protected int health;  // Change from public to protected
    public float speed;
    public int damage;

    [Header("Proximity Settings")]
    [SerializeField] private float despawnDistance = 20f;    // Distance to destroy the enemy
    protected Transform player; // Reference to the player's transform

    // Resistances and weaknesses
    public Dictionary<DamageType, float> resistances = new Dictionary<DamageType, float>();
    public Dictionary<DamageType, float> weaknesses = new Dictionary<DamageType, float>();

    public delegate void DeathHandler();
    public event DeathHandler OnDeathTriggered; // Custom event for death

    // For the damage flash effect
    private Renderer enemyRenderer; // The renderer for flashing
    private Color originalColor; // Store the original color of the enemy
    [SerializeField] private Color damageFlashColor = Color.red; // Color of the flash
    [SerializeField] private float flashDuration = 0.2f; // Duration of the flash

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Player not found. Ensure the player has the 'Player' tag assigned.");
            return;
        }

        // Get the renderer component to change its color
        enemyRenderer = GetComponent<Renderer>();
        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color; // Store the original color of the enemy
        }
    }

    private void Update()
    {
        // Call the CheckDespawnDistance method to handle despawn logic in base class
        CheckDespawnDistance();
    }

    // New method to check the distance and despawn if necessary
    protected void CheckDespawnDistance()
    {
        if (player == null) return;

        // Calculate the 2D distance to the player using Vector2.Distance
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Despawn logic: destroy the enemy if it's too far from the player
        if (distanceToPlayer > despawnDistance)
        {
            DestroyEnemy();
        }
    }

    private void DestroyEnemy()
    {
        Debug.Log($"{gameObject.name} is too far from the player and has been destroyed.");
        Destroy(gameObject);
    }

    public virtual void TakeDamage(int amount, DamageType damageType)
    {
        float finalDamage = amount;

        // Apply resistance or weakness modifiers
        if (resistances.ContainsKey(damageType))
        {
            finalDamage *= (1 - resistances[damageType]); // Reduce damage based on resistance
        }
        else if (weaknesses.ContainsKey(damageType))
        {
            finalDamage *= (1 + weaknesses[damageType]); // Increase damage based on weakness
        }

        health -= Mathf.CeilToInt(finalDamage); // Apply damage and round up
        Debug.Log($"{gameObject.name} took {Mathf.CeilToInt(finalDamage)} {damageType} damage. Remaining health: {health}");

        // Flash the enemy when taking damage
        FlashDamage();

        if (health <= 0)
        {
            Die();
        }
    }

    // Flash effect when the enemy takes damage
    private void FlashDamage()
    {
        if (enemyRenderer != null)
        {
            // Change the color to the flash color
            enemyRenderer.material.color = damageFlashColor;

            // Revert the color back after the specified duration
            StartCoroutine(RevertFlashColor());
        }
    }

    private System.Collections.IEnumerator RevertFlashColor()
    {
        yield return new WaitForSeconds(flashDuration); // Wait for the duration of the flash
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = originalColor; // Revert back to original color
        }
    }

    public abstract void Move();

    protected virtual void Die()
    {
        // Trigger the death event
        OnDeathTriggered?.Invoke();
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject);
    }
}
