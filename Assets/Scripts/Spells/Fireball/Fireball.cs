using UnityEngine;
using System.Collections.Generic;
using System.Collections;



public class Fireball : MonoBehaviour
{
    public float speed = 10f;               // Speed of the fireball
    public int damage = 10;                 // Damage dealt by the fireball
    public float lifetime = 3f;             // How long the fireball exists
    public ParticleSystem fireballParticles; // Particle effect for the fireball
    public ParticleSystem explosionParticles; // Particle effect for explosion
    public Rigidbody2D rb;                  // Rigidbody2D component
    public LineRenderer lineRenderer;       // Reference to LineRenderer
    public float fadeSpeed = 1f;            // Speed at which the line fades
    public DamageType damageType = DamageType.Fire; // Default damage type

    protected Vector3 direction;            // Direction of movement
    private List<Vector3> linePositions;    // List to store the positions of the line
    private float timeSinceLastUpdate = 0f;  // Timer for updating line positions
    private float lineAlpha = 1f;           // The alpha value of the line (transparency)
    private Color originalColor;            // To store the original color of the line

    public virtual void Initialize(Vector2 castDirection, int spellDamage)
    {
        direction = castDirection.normalized; // Normalize direction vector
        damage = spellDamage;

        // Set velocity based on new direction
        if (rb != null)
        {
            rb.velocity = direction * speed;
        }

        // Update the particle system's velocity again if needed
        if (fireballParticles != null)
        {
            var velocityOverLifetime = fireballParticles.velocityOverLifetime;
            velocityOverLifetime.x = -rb.velocity.x * 2f; // Inverse velocity with a stronger multiplier
            velocityOverLifetime.y = -rb.velocity.y * 2f; // Inverse velocity with a stronger multiplier
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        linePositions = new List<Vector3>();  // Initialize the list for line positions
        originalColor = lineRenderer.startColor;

        lineRenderer.startColor = new Color(originalColor.r, originalColor.g, originalColor.b, 1f); // Fully opaque
        lineRenderer.endColor = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);   // Fully opaque

        if (fireballParticles != null)
        {
            fireballParticles.Play();
        }

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Perform a raycast ahead of the fireball
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, speed * Time.deltaTime);

        // If the raycast hits a collider
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Wall"))
            {
                OnHit(hit.collider);
                Destroy(gameObject); // Destroy fireball upon collision
            }
        }

        // Update line rendering positions
        timeSinceLastUpdate += Time.deltaTime;
        if (timeSinceLastUpdate >= 0.05f)
        {
            timeSinceLastUpdate = 0f;
            linePositions.Add(transform.position);
            lineRenderer.positionCount = linePositions.Count;
            lineRenderer.SetPositions(linePositions.ToArray());
        }

        // Fade the line over time
        if (lineAlpha > 0f)
        {
            lineAlpha -= Time.deltaTime * fadeSpeed;
            lineRenderer.startColor = new Color(originalColor.r, originalColor.g, originalColor.b, lineAlpha);
            lineRenderer.endColor = new Color(originalColor.r, originalColor.g, originalColor.b, lineAlpha);
        }
        else
        {
            lineRenderer.startColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
            lineRenderer.endColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Wall"))
        {
            DestroyFireball();
        }
    }

    protected virtual void OnHit(Collider2D collider)
    {
        // This method can be overridden by PlayerFireball or EnemyFireball to apply damage logic
    }

    protected void DestroyFireball()
{
    if (explosionParticles != null)
    {
        ParticleSystem explosion = Instantiate(explosionParticles, transform.position, Quaternion.identity);
        explosion.Play();

        // Add a small buffer (0.1 seconds) to ensure the explosion completes
        Destroy(explosion.gameObject, explosion.main.duration + 0.6f);
    }
    else
    {
        Debug.LogWarning("Explosion particles not assigned!");
    }
    
    Destroy(gameObject);
}

private IEnumerator DestroyAfterExplosion(ParticleSystem explosion)
{
    // Wait for the explosion's duration to finish
    yield return new WaitForSeconds(explosion.main.duration);

    // After explosion, destroy the fireball
    Destroy(gameObject);
}




}
