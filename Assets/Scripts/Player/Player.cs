using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    public int maxHealth = 100;
    public int maxMana = 50;
    public int currentHealth;
    public int currentMana;

    [Header("UI Elements")]
    public Slider healthBar;
    public Slider manaBar;
    public Text healthText;
    public Text manaText;

    [Header("Mana Regeneration")]
    public float manaRegenRate = 5f; // Mana points regenerated per second
    private float manaRegenTimer = 0f;

    [Header("Damage Resistance")]
    public float fireResistance = 0f; // Percentage resistance to fire damage
    public float waterResistance = 0f; // Percentage resistance to water damage

    private Animator animator;
    private PlayerMovement playerMovement;

    void Start()
    {
        // Initialize health and mana
        currentHealth = maxHealth;
        currentMana = maxMana;

        // Set UI elements
        UpdateHealthBar();
        UpdateManaBar();

        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // Regenerate mana over time
        if (currentMana < maxMana)
        {
            manaRegenTimer += Time.deltaTime;
            if (manaRegenTimer >= 1f)
            {
                RegenerateMana((int)(manaRegenRate));
                manaRegenTimer = 0f;
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
        currentHealth -= reducedDamage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        UpdateHealthBar();
    }

    public void UseMana(int manaCost)
    {
        if (currentMana >= manaCost)
        {
            currentMana -= manaCost;
            UpdateManaBar();
        }
        else
        {
            Debug.Log("Not enough mana!");
        }
    }

    public void RegenerateMana(int amount)
    {
        currentMana = Mathf.Min(currentMana + amount, maxMana);
        UpdateManaBar();
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;
        }
        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{maxHealth}";
        }
    }

    private void UpdateManaBar()
    {
        if (manaBar != null)
        {
            manaBar.value = (float)currentMana / maxMana;
        }
        if (manaText != null)
        {
            manaText.text = $"{currentMana}/{maxMana}";
        }
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        animator.SetTrigger("Die");
        playerMovement.enabled = false; // Disable movement
        // Add additional game over logic here (e.g., restart level)
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthBar();
    }

    public void RestoreMana(int amount)
    {
        currentMana = Mathf.Min(currentMana + amount, maxMana);
        UpdateManaBar();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Example interaction with healing or mana-restoring items
        if (collision.CompareTag("HealthPotion"))
        {
            Heal(20); // Heal 20 points
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("ManaPotion"))
        {
            RestoreMana(15); // Restore 15 mana points
            Destroy(collision.gameObject);
        }
    }
}
