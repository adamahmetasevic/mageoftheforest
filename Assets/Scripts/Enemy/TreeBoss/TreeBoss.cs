using UnityEngine;
using System.Collections;

public class TreeBoss : Enemy

{

    public int _maxHealth = 100; // Make this private and serialized
 
    [Header("Charge Settings")]
    public float chargeSpeed = 10f;
    public float spinDuration = 2f; // Time to spin back and forth before charging
    public float attackRange = 1.5f;
    private Vector2 chargeTargetPosition;

    [Header("Phase Transition")]
    public float secondPhaseHealthThreshold = 0.5f; // Trigger second phase at 50% health
    public Transform[] minionSpawnPoints; // Points where minions will spawn
    public GameObject minionPrefab;
    private bool secondPhaseTriggered = false;

    [Header("UI Slider")]
    public float proximityActivationDistance = 10f;
    private bool sliderActivated = false;

    private Rigidbody2D rb;

    public delegate void TreeBossDeathHandler();
    public event TreeBossDeathHandler OnTreeBossDeath;  // Event when TreeBoss dies

    private bool isCharging = false;
    private bool isInvincible = false;  // Flag to check if boss is in invincibility state

    // Invincibility frames duration
    public float invincibilityDuration = 1f;
    private bool hasTakenDamageThisCharge = false;

    protected override void Start()
{
    Debug.Log($"Initial maxHealth: {_maxHealth}");  // Debug to check initial maxHealth value
    
    health = _maxHealth;  // Set this first
    Debug.Log($"Setting initial health to: {_maxHealth}");

    base.Start();  // Then call base.Start()
    rb = GetComponent<Rigidbody2D>();
    rb.mass = 100f;

    // Initialize health UI if player is close enough at start
    if (player != null)
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= proximityActivationDistance)
        {
            sliderActivated = true;
            UIManager.Instance.ActivateBossHealthSlider(_maxHealth, _maxHealth);
        }
    }

    Debug.Log($"TreeBoss initialized - _maxHealth: {_maxHealth}, Current Health: {health}");
}


    private void Update()
    {
        HandleProximityUI();

        if (!isCharging)
        {
            StartCoroutine(SpinAndCharge());
        }

        if (!secondPhaseTriggered && health <= (_maxHealth * secondPhaseHealthThreshold))  // Fixed threshold calculation
        {
            TriggerSecondPhase();
        }
    }

    public override void TakeDamage(int damage, DamageType damageType)
{
    Debug.Log("in takedamage");

    if (isInvincible)
    {
        Debug.Log($"Damage blocked - Boss is invincible");
        return;
    }

    if (health <= 0)
    {
        Debug.Log($"Damage blocked - Boss is already dead");
        return;
    }

    // Actually apply the damage
    int previousHealth = health;
    health -= damage;
    health = Mathf.Max(health, 0);

    Debug.Log($"TreeBoss health reduced from {previousHealth} to {health} ({damage} {damageType} damage)");

    if (sliderActivated)
    {
        UIManager.Instance.UpdateBossHealth(health, _maxHealth);
        Debug.Log($"Updated UI health slider to {health}");
    }

    if (health <= 0)
    {
        Debug.Log("TreeBoss health reached 0, calling Die()");
        Die();
    }
}
    private IEnumerator SpinAndCharge()
    {
        isCharging = true;  // Start charging
        hasTakenDamageThisCharge = false;  // Reset flag at the start of the charge

        // Capture the player's position at the start of the spin
        Vector2 initialChargeTargetPosition = player != null ? player.position : transform.position;

        // Spin animation
        float spinStartTime = Time.time;
        while (Time.time - spinStartTime < spinDuration)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            yield return new WaitForSeconds(0.2f);
        }

        // Charge toward the saved initial position of the player
        if (player != null)
        {
            // Calculate the charge direction based on the initial position of the player
            Vector2 chargeDirection = new Vector2(initialChargeTargetPosition.x - transform.position.x, 0).normalized;  // Ignore Y-axis

            // Apply velocity based on the calculated charge direction
            rb.velocity = chargeDirection * chargeSpeed;

            // Increase charge duration for a longer charge
            float chargeDuration = 4f; // You can adjust this as needed
            yield return new WaitForSeconds(chargeDuration);
        }

        rb.velocity = Vector2.zero;  // Stop the boss after the charge
        StartCoroutine(EnterInvincibilityFrames());  // Activate invincibility frames after charging
        yield return new WaitForSeconds(1f); // Add cooldown between charges

        isCharging = false;  // Reset charging state
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator EnterInvincibilityFrames()
    {
        isInvincible = true;
        Debug.Log("TreeBoss is now invincible for a short time!");

        yield return new WaitForSeconds(invincibilityDuration);

        isInvincible = false;
        Debug.Log("TreeBoss is no longer invincible!");
    }

    private void OnCollisionEnter2D(Collision2D collision)
{
    Debug.Log($"Collision detected with {collision.gameObject.name} (Tag: {collision.gameObject.tag})");
    Debug.Log($"Current state - isCharging: {isCharging}, hasTakenDamageThisCharge: {hasTakenDamageThisCharge}, isInvincible: {isInvincible}");

    if (isCharging && !hasTakenDamageThisCharge)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player playerScript = collision.gameObject.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(25, DamageType.Physical);
                hasTakenDamageThisCharge = true;
                Debug.Log("Player damage dealt: 25");
            }
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("TreeBoss hit a wall!");
            Debug.Log("Attempting to deal 25 damage to TreeBoss");
            TakeDamage(25, DamageType.Physical);
            hasTakenDamageThisCharge = true;
        }
    }
}

    private void TriggerSecondPhase()
    {
        if (secondPhaseTriggered) return; // Prevent multiple triggers
        
        secondPhaseTriggered = true;
        Debug.Log("Entering second phase!");
        
        if (minionSpawnPoints.Length > 0 && minionPrefab != null)
        {
            StartCoroutine(SpawnMinions());
        }
        else
        {
            Debug.LogWarning("Missing minion spawn points or minion prefab!");
        }
    }

    private IEnumerator SpawnMinions()
    {
        while (health > 0) // Stop spawning when boss dies
        {
            foreach (Transform spawnPoint in minionSpawnPoints)
            {
                if (spawnPoint != null)
                {
                    GameObject minion = Instantiate(minionPrefab, spawnPoint.position, Quaternion.identity);
                    Debug.Log($"Spawned minion at {spawnPoint.position}");
                }
            }

            yield return new WaitForSeconds(5f);
        }
    }

    protected override void Die()
    {
        base.Die();
        Debug.Log("TreeBoss has been defeated!");

        // Trigger the TreeBoss death event
        OnTreeBossDeath?.Invoke(); // Notify all subscribers (minions)

        // Deactivate health slider when boss dies
        UIManager.Instance.DeactivateBossHealthSlider(); 
    }

    public override void Move()
    {
        // The TreeBoss doesn't have specific movement logic outside of charging.
        // Leave this method empty or add phase-specific movement if required.
    }

     private void HandleProximityUI()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (!sliderActivated && distanceToPlayer <= proximityActivationDistance)
        {
            sliderActivated = true;
            UIManager.Instance.ActivateBossHealthSlider(health, _maxHealth); // Initialize slider with current and max health
        }

        if (sliderActivated)
        {
            UIManager.Instance.UpdateBossHealth(health, _maxHealth);
        }
    }
}
