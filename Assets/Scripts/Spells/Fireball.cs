using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 10f; // Speed of the fireball
    public int damage = 10; // Damage dealt by the fireball
    public float lifetime = 3f; // How long the fireball exists
    public ParticleSystem fireballParticles; // Particle effect for the fireball
    public Rigidbody2D rb; // Rigidbody2D component

    private Vector3 direction; // Direction of movement

    void Start()
    {
        // Set up the lifetime and destroy after the set time
        rb = GetComponent<Rigidbody2D>();
        direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        direction.z = 0f;  // We only want movement on the X and Y axes (2D)
        
        // Set velocity based on direction
        rb.velocity = direction * speed;

        Destroy(gameObject, lifetime);
        
        // Play fireball particle effect if available
        if (fireballParticles != null)
        {
            fireballParticles.Play();
        }
    }

    public void Initialize(Vector2 castDirection, int spellDamage)
    {
        direction = castDirection.normalized; // Normalize direction vector
        damage = spellDamage;

        // Set velocity based on new direction
        rb.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            /*var health = collision.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }*/
            Destroy(gameObject); // Destroy on impact
        }
        else if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject); // Destroy on wall impact
        }
    }
}
