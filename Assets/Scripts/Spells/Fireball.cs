using UnityEngine;
using System.Collections.Generic;

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

    private Vector3 direction;              // Direction of movement
    private List<Vector3> linePositions;    // List to store the positions of the line
    private float timeSinceLastUpdate = 0f;  // Timer for updating line positions
    private float lineAlpha = 1f;           // The alpha value of the line (transparency)
    private Color originalColor;            // To store the original color of the line

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        direction.z = 0f; // Only use X and Y axes in 2D

        // Set velocity based on direction
        rb.velocity = direction * speed;

        // Destroy the fireball after a certain lifetime
        Destroy(gameObject, lifetime);

        // Set up the LineRenderer
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;  // Initially no points in the line
        linePositions = new List<Vector3>(); // Initialize the list for line positions

        // Store the original color of the line (preserves its color)
        originalColor = lineRenderer.startColor;

        // Set the material's initial color (ensure it's fully opaque)
        lineRenderer.startColor = new Color(originalColor.r, originalColor.g, originalColor.b, 1f); // Fully opaque
        lineRenderer.endColor = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);   // Fully opaque

        // Play fireball particle effect if available
        if (fireballParticles != null)
        {
            fireballParticles.Play();

            // Adjust the particle system's velocity to be inverse to the fireball's velocity
            var main = fireballParticles.main;
            var velocityOverLifetime = fireballParticles.velocityOverLifetime;
            
            // Scale the inverse velocity to make the particles trail further
            velocityOverLifetime.x = -rb.velocity.x * 2f; // Increase the multiplier to strengthen the inverse velocity
            velocityOverLifetime.y = -rb.velocity.y * 2f; // Increase the multiplier to strengthen the inverse velocity

            // Optionally, you can modify the start speed of the particles to give them more initial velocity
            main.startSpeed = 3f; // Increase this to make particles move faster
        }
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
                // Handle collision with enemy or wall
                DestroyFireball();
                return; // No need to update position if we collided
            }
        }

        // Increment the timer to update line position periodically
        timeSinceLastUpdate += Time.deltaTime;

        if (timeSinceLastUpdate >= 0.05f)  // Update every 0.05 seconds
        {
            timeSinceLastUpdate = 0f;

            // Add the fireball's current position to the line
            linePositions.Add(transform.position);

            // Update the line renderer with all the points in the list
            lineRenderer.positionCount = linePositions.Count;
            lineRenderer.SetPositions(linePositions.ToArray());
        }

        // Fade the line by reducing the alpha over time
        if (lineAlpha > 0f)
        {
            lineAlpha -= Time.deltaTime * fadeSpeed;  // Reduce alpha based on fade speed
            lineRenderer.startColor = new Color(originalColor.r, originalColor.g, originalColor.b, lineAlpha);
            lineRenderer.endColor = new Color(originalColor.r, originalColor.g, originalColor.b, lineAlpha);
        }
        else
        {
            // Ensure the line is completely invisible when faded
            lineRenderer.startColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
            lineRenderer.endColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        }
    }

    public void Initialize(Vector2 castDirection, int spellDamage)
    {
        direction = castDirection.normalized; // Normalize direction vector
        damage = spellDamage;

        // Set velocity based on new direction
        rb.velocity = direction * speed;

        // Update the particle system's velocity again if needed
        if (fireballParticles != null)
        {
            var velocityOverLifetime = fireballParticles.velocityOverLifetime;
            velocityOverLifetime.x = -rb.velocity.x * 2f; // Inverse velocity with a stronger multiplier
            velocityOverLifetime.y = -rb.velocity.y * 2f; // Inverse velocity with a stronger multiplier
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            DestroyFireball(); // Destroy on impact with enemy
        }
        else if (collision.CompareTag("Wall"))
        {
            DestroyFireball(); // Destroy on impact with wall
        }
    }

    private void DestroyFireball()
    {
        if (explosionParticles != null)
        {
            ParticleSystem explosion = Instantiate(explosionParticles, transform.position, Quaternion.identity);
            explosion.Play();

            // Get the maximum lifetime between particles and light fade
            float particlesDuration = explosion.main.duration + explosion.main.startLifetime.constantMax;
            ExplosionLightFader lightFader = explosion.GetComponentInChildren<ExplosionLightFader>();
            float totalDuration = particlesDuration;

            if (lightFader != null)
            {
                totalDuration = Mathf.Max(particlesDuration, lightFader.fadeDuration);
            }

            // Destroy the explosion after the longest duration
            Destroy(explosion.gameObject, totalDuration);
        }

        // Destroy the fireball immediately
        Destroy(gameObject);
    }
}
