using UnityEngine;

public class EnemyLightningBolt : LightningBoltBase
{
    public GameObject creator;
    private Transform playerTransform;

    public void InitializeEnemyLightningBolt(GameObject lightningCreator, int spellDamage)
    {
        creator = lightningCreator;
        damage = spellDamage;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            Initialize(playerTransform.position); // Initialize with player's position as target
        }
        else
        {
            Debug.LogWarning("Player object not found in the scene!");
        }
    }

    protected override void DealDamage()
    {
        // Check if there's a player in range when the light reaches the end
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(targetPosition, 0.5f, LayerMask.GetMask("Player"));
        
        foreach (var hitCollider in hitPlayers)
        {
            if (hitCollider.CompareTag("Player"))
            {
                Player player = hitCollider.GetComponent<Player>();
                if (player != null)
                {
                    player.TakeDamage((int)damage, DamageType.Lightning);
                    Debug.Log($"Lightning dealt {damage} damage to player");
                }
            }
        }
    }

    // This will help visualize the damage area in the editor
    private void OnDrawGizmosSelected()
    {
        if (targetPosition != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(targetPosition, 0.5f);
        }
    }
}