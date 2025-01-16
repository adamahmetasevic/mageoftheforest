using UnityEngine;

public class PlayerLightningBolt : LightningBoltBase
{
    public int playerDamage = 25;
    public DamageType damageType = DamageType.Lightning; // Add this line to specify damage type

    protected override void DealDamage()
    {
        // Overlap check to hit enemies in the range at the end of the lightning path
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(targetPosition, 0.5f, LayerMask.GetMask("Enemy"));
        
        foreach (var enemy in hitEnemies)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(playerDamage, damageType); // Updated to include damage type
                Debug.Log($"Lightning dealt {playerDamage} damage to {enemy.name}");
            }
        }
    }

    // Optional: Visualize the damage area in the editor
    private void OnDrawGizmosSelected()
    {
        if (targetPosition != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(targetPosition, 0.5f);
        }
    }
}
