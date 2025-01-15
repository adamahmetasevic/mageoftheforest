using UnityEngine;

public class EnemyFireball : Fireball
{
    public GameObject creator; // Reference to the enemy that created this fireball
    private Transform playerTransform; // Reference to the player's Transform

    // Initialize the enemy fireball, calculating the direction to the player
    public void InitializeEnemyFireball(GameObject fireballCreator, int spellDamage)
    {
        creator = fireballCreator;
        damage = spellDamage;

        // Find the player object (You can cache this for efficiency if needed)
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;

            // Calculate direction towards the player
            Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;

            // Call the base Initialize method to set direction and damage
            Initialize(directionToPlayer, spellDamage);
        }
        else
        {
            Debug.LogWarning("Player object not found in the scene!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignore collision with the creator or any object with the "Enemy" tag
        if (collision.gameObject == creator || collision.CompareTag("Enemy"))
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage, damageType); // Deal damage to the player
            }
            Destroy(gameObject); // Destroy fireball
        }
        else if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject); // Destroy on impact with wall
        }
    }
}
