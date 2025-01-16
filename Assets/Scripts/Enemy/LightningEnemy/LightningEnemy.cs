using UnityEngine;

public class LightningEnemy : Enemy
{
    public GameObject lightningBoltPrefab;
    public float attackRange = 10f;
    public float attackCooldown = 2f;
    private float nextAttackTime;
    private Transform playerTransform;

    private void Start()
    {
        // Initialize resistances and weaknesses
        resistances.Add(DamageType.Lightning, 0.5f); // 50% resistance to Lightning
       // weaknesses.Add(DamageType.Earth, 0.5f);      // 50% weakness to Earth

        // Find the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        nextAttackTime = Time.time;
    }

    public override void Move()
    {
        if (playerTransform == null) return;

        // Move towards player if too far, maintain distance if close
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        Vector2 direction = (playerTransform.position - transform.position).normalized;

        if (distanceToPlayer > attackRange)
        {
            // Move towards player
            transform.position += (Vector3)direction * speed * Time.deltaTime;
        }
        else if (distanceToPlayer < attackRange - 2f)
        {
            // Move away from player if too close
            transform.position -= (Vector3)direction * speed * Time.deltaTime;
        }

        // Try to attack if in range and cooldown is ready
        if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
        {
            ShootLightning();
        }
    }

    private void ShootLightning()
    {
        if (playerTransform == null || lightningBoltPrefab == null) return;

        GameObject lightning = Instantiate(lightningBoltPrefab, transform.position, Quaternion.identity);
        EnemyLightningBolt lightningBolt = lightning.GetComponent<EnemyLightningBolt>();
        
        if (lightningBolt != null)
        {
            lightningBolt.InitializeEnemyLightningBolt(gameObject, damage);
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void Update()
    {
        Move();
    }

    protected override void Die()
    {
        // Add any specific death effects for lightning enemy
        // For example, you might want to create a lightning explosion effect
        Debug.Log("Lightning Enemy defeated!");
        base.Die();
    }
}