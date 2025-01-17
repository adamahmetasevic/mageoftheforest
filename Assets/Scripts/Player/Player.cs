using UnityEngine;

public class Player : MonoBehaviour
{
     [Header("Player Stats")]
    public int maxHealth = 100;
    public float maxMana = 50f;
    public int currentHealth;
    public float currentMana;

    [Header("Mana Regeneration")]
    public float manaRegenRate = 5f;  // Mana per second
    public float manaRegenSmoothness = 5f;  // Higher value = smoother regeneration

    [Header("UIManager Reference")]
    public UIManager uiManager;  // Reference to UIManager

    [Header("Damage Resistance")]
    public float fireResistance = 0f;
    public float waterResistance = 0f;

    private Animator animator;
    private PlayerMovement playerMovement;

    void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;

        if (uiManager == null)
            uiManager = FindObjectOfType<UIManager>();

        if (uiManager != null)
        {
            uiManager.UpdateHealth(currentHealth, maxHealth);
            uiManager.UpdateMana(currentMana, maxMana);
        }

        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // Smooth mana regeneration
        if (currentMana < maxMana)
        {
            float newMana = currentMana + (manaRegenRate * Time.deltaTime);
            currentMana = Mathf.Min(newMana, maxMana);
            
            if (uiManager != null)
            {
                uiManager.UpdateMana(currentMana, maxMana);
            }
        }
    }



    public void TakeDamage(int damage, DamageType damageType)
    {
        // Apply resistances based on damage type
        float resistance = 0f;
        if (damageType == DamageType.Fire)
            resistance = fireResistance;
        else if (damageType == DamageType.Water)
            resistance = waterResistance;

        int reducedDamage = Mathf.Max(1, (int)(damage * (1 - resistance / 100)));
        
        // Store previous health for damage calculation
        int previousHealth = currentHealth;
        
        // Apply damage
        currentHealth = Mathf.Max(0, currentHealth - reducedDamage);

        // Update UI
        if (uiManager != null)
        {
            uiManager.UpdateHealth(currentHealth, maxHealth);
        }

        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }
    }

public void UseMana(float manaCost)
    {
        if (currentMana >= manaCost)
        {
            currentMana -= manaCost;
            if (uiManager != null)
            {
                uiManager.UpdateMana(currentMana, maxMana);
            }
        }
        else
        {
            Debug.Log("Not enough mana!");
        }
    }

    public void RegenerateMana(float amount)
    {
        float previousMana = currentMana;
        currentMana = Mathf.Min(currentMana + amount, maxMana);

        if (currentMana != previousMana && uiManager != null)
        {
            uiManager.UpdateMana(currentMana, maxMana);
        }
    }


    private void Die()
    {
        Debug.Log("Player has died!");
        if (animator != null)
            animator.SetTrigger("Die");
        if (playerMovement != null)
            playerMovement.enabled = false; // Disable movement
        // Add additional game over logic here
    }
}