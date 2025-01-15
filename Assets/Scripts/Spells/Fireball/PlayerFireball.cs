using UnityEngine;

public class PlayerFireball : Fireball
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, damageType); // Deal damage to the enemy
            }
            Destroy(gameObject); // Destroy fireball
        }
        else if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject); // Destroy on impact with wall
        }
    }
}
