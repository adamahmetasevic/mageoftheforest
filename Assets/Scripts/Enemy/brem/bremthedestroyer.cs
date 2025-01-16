using UnityEngine;
using System.Collections.Generic;

public class BremTheDestroyer : Enemy
{
    public GameObject fireballPrefab;
    public Transform[] firePoints; // Multiple fire points for multi-hand attacks
    public float fireballCooldown = 2f;
    public Transform leftPoint;  // Assign this in Unity Inspector
    public Transform rightPoint; // Assign this in Unity Inspector
        private bool movingRight = true;
    public float moveSpeed = 5f;
    private float distanceThreshold = 0.1f; // How close to get to point before turning
    private float nextFireTime = 0f;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        resistances.Add(DamageType.Fire, 0.5f);
        weaknesses.Add(DamageType.Water, 0.5f);
        
        // Configure the Rigidbody2D
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.mass = 10000; // Make the enemy very heavy so it's not easily pushed
            rb.drag = 1000;  // Add high drag to resist pushing forces
        }
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
        if (leftPoint == null || rightPoint == null) return;

        if (movingRight)
        {
            transform.position = Vector2.MoveTowards(transform.position, 
                rightPoint.position, moveSpeed * Time.deltaTime);
            
            if (Vector2.Distance(transform.position, rightPoint.position) < distanceThreshold)
            {
                movingRight = false;
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, 
                leftPoint.position, moveSpeed * Time.deltaTime);
            
            if (Vector2.Distance(transform.position, leftPoint.position) < distanceThreshold)
            {
                movingRight = true;
            }
        }
    }

    public void MultiHandAttack()
{
    GameObject player = GameObject.FindGameObjectWithTag("Player");
    if (player == null) return;

    foreach (Transform firePoint in firePoints)
    {
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);
        EnemyFireball fireballScript = fireball.GetComponent<EnemyFireball>();

        if (fireballScript != null)
        {
            fireballScript.InitializeEnemyFireball(gameObject, damage); // Remove the direction parameter
        }
    }
}


    protected override void Die()
    {
        base.Die();
        Debug.Log("BremTheDestroyer exploded in a blaze of glory!");
    }
}
