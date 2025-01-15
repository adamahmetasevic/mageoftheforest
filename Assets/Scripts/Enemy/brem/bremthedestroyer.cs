using UnityEngine;
using System.Collections.Generic;

public class BremTheDestroyer : Enemy
{
    public GameObject fireballPrefab;
    public Transform[] firePoints; // Multiple fire points for multi-hand attacks
    public float fireballCooldown = 2f;

    private float nextFireTime = 0f;

    private void Start()
    {
        // Set up resistances and weaknesses
        resistances.Add(DamageType.Fire, 0.5f); // 50% resistance to fire damage
        weaknesses.Add(DamageType.Water, 0.5f); // 50% more damage from water
    }

    private void Update()
    {
        Move();
        if (Time.time >= nextFireTime)
        {
            MultiHandAttack();
            nextFireTime = Time.time + fireballCooldown;
        }
    }

    public override void Move()
    {
        // Specific movement logic for BremTheDestroyer
        transform.Translate(Vector2.left * speed * Time.deltaTime);
        Debug.Log("BremTheDestroyer is gliding forward.");
    }

    public void MultiHandAttack()
{
    foreach (Transform firePoint in firePoints)
    {
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);
        EnemyFireball fireballScript = fireball.GetComponent<EnemyFireball>();

        if (fireballScript != null)
        {
            Vector2 direction = (Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition) - firePoint.position).normalized;
            fireballScript.InitializeEnemyFireball(gameObject, damage);
        }
    }
}


    protected override void Die()
    {
        base.Die();
        Debug.Log("BremTheDestroyer exploded in a blaze of glory!");
    }
}
