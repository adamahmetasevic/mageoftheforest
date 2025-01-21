using UnityEngine;
using System.Collections;

public class TreeBoss : Enemy
{
    // Existing variables
    public int _maxHealth = 100; // Make this private and serialized
    public float chargeSpeed = 10f;
    public float spinDuration = 2f;
    public float attackRange = 1.5f;
    private Vector2 chargeTargetPosition;

    [Header("Phase Transition")]
    public float secondPhaseHealthThreshold = 0.5f;
    public Transform[] minionSpawnPoints;
    public GameObject minionPrefab;
    private bool secondPhaseTriggered = false;

    [Header("UI Slider")]
    public float proximityActivationDistance = 10f;
    private bool sliderActivated = false;

    private Rigidbody2D rb;

    // Delegate and event for TreeBoss death
    public delegate void TreeBossDeathHandler();
    public event TreeBossDeathHandler OnTreeBossDeath;
    private Collider2D myCollider; // To reference the collider

    // Flags
    private bool isCharging = false;
    private bool isInvincible = false;
    private bool shouldStartCharging = false;
    private bool isInitialized = false;
    private bool hasTakenDamageThisCharge = false;

    // Invincibility frames duration
    public float invincibilityDuration = 1f;

    // Health
    protected override void Start()
    {
        Debug.Log($"Initial maxHealth: {_maxHealth}");

        health = _maxHealth;
        Debug.Log($"Setting initial health to: {_maxHealth}");

        base.Start();
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

        isInitialized = true;
        shouldStartCharging = true; // Allow charging to begin
    }

    private void OnEnable()
    {
        if (isInitialized)
        {
            shouldStartCharging = true;
            isCharging = false; // Reset charging state when reactivated
        }
    }

    private void OnDisable()
    {
        // Reset states when disabled
        isCharging = false;
        shouldStartCharging = false;
        hasTakenDamageThisCharge = false;
        isInvincible = false;
    }

    private void Update()
    {
        HandleProximityUI();

        if (shouldStartCharging && !isCharging)
        {
            shouldStartCharging = false;
            StartCoroutine(SpinAndCharge());
        }

        if (!secondPhaseTriggered && health <= (_maxHealth * secondPhaseHealthThreshold)) 
        {
            TriggerSecondPhase();
        }
    }

    private IEnumerator SpinAndCharge()
    {
        if (!gameObject.activeInHierarchy) yield break;

        isCharging = true; // Start charging
        hasTakenDamageThisCharge = false;

        // Capture the player's position at the start of the spin
        Vector2 initialChargeTargetPosition = player != null ? player.position : transform.position;

        // Spin animation
        float spinStartTime = Time.time;
        while (Time.time - spinStartTime < spinDuration)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            yield return new WaitForSeconds(0.2f);
        }

        if (!gameObject.activeInHierarchy) yield break;

        // Charge toward the saved initial position of the player
        if (player != null)
        {
            Vector2 chargeDirection = new Vector2(initialChargeTargetPosition.x - transform.position.x, 0).normalized;
            rb.velocity = chargeDirection * chargeSpeed;

            float chargeDuration = 4f;
            yield return new WaitForSeconds(chargeDuration);
        }

        rb.velocity = Vector2.zero; // Stop the boss after the charge
        StartCoroutine(EnterInvincibilityFrames()); // Activate invincibility frames after charging
        yield return new WaitForSeconds(1f); // Add cooldown between charges

        isCharging = false; // Reset charging state
        yield return new WaitForSeconds(1f);
        
        // Set shouldStartCharging to true to start the next charge
        shouldStartCharging = true;
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
                TakeDamage(25, DamageType.Physical);
                hasTakenDamageThisCharge = true;
            }
        }
    }

    private void TriggerSecondPhase()
{
    if (secondPhaseTriggered) return;

    secondPhaseTriggered = true;
    Debug.Log("Entering second phase!");

    if (minionSpawnPoints.Length > 0 && minionPrefab != null)
    {
        SpawnMinions(); // Spawn minions just once
    }
    else
    {
        Debug.LogWarning("Missing minion spawn points or minion prefab!");
    }
}

private void SpawnMinions()
{
    // Loop through all spawn points and spawn minions once
    foreach (Transform spawnPoint in minionSpawnPoints)
    {
        if (spawnPoint != null)
        {
            GameObject minion = Instantiate(minionPrefab, spawnPoint.position, Quaternion.identity);
            Debug.Log($"Spawned minion at {spawnPoint.position}");
            minion minionobj = FindObjectOfType<minion>();
            if (minionobj != null)
            {
                Collider2D minioncolider = minionobj.GetComponent<Collider2D>();
                if (minioncolider != null)
                {
                    Physics2D.IgnoreCollision(myCollider, minioncolider, true); // Disable collision with minions
                }
            }
        }
    }
}


protected override void Die()
{
    base.Die();
    Debug.Log("TreeBoss has been defeated!");

    // Notify all subscribers (minions)
    OnTreeBossDeath?.Invoke();

    // Deactivate health slider when boss dies
    UIManager.Instance.DeactivateBossHealthSlider();

    // Notify the player of the boss defeat
    Player player = FindObjectOfType<Player>();
    if (player != null)
    {
        player.RecordBossDefeat("TreeBoss");
    }

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
