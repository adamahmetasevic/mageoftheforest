using UnityEngine;

public class EnemyFireball : Fireball
{
    public GameObject creator;
    private Transform playerTransform;

    public void InitializeEnemyFireball(GameObject fireballCreator, int spellDamage)
    {
        creator = fireballCreator;
        damage = spellDamage;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
            Initialize(directionToPlayer, spellDamage);
        }
        else
        {
            Debug.LogWarning("Player object not found in the scene!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage, damageType);
            }
            DestroyFireball();
        }
        else if (collision.CompareTag("Wall"))
        {
            DestroyFireball();
        }
    }
}