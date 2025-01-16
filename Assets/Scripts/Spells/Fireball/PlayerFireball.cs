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
        DestroyFireball(); // Destroy fireball and play explosion
    }
    else if (collision.CompareTag("Wall"))
    {
        DestroyFireball(); // Destroy fireball and play explosion
    }
}

}
