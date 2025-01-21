using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    public int maxHealth = 100;
    public float maxMana = 50f;
    private bool isInvincible = false;

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

    public SpellCaster spellCaster; // Reference to SpellCaster

    private Animator animator;
    private PlayerMovement playerMovement;
    

    // List of defeated bosses
    public List<string> defeatedBosses = new List<string>();

        private TutorialManager tutorialManager;


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

        spellCaster = GetComponent<SpellCaster>(); // Initialize the SpellCaster reference

        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();

        tutorialManager = FindObjectOfType<TutorialManager>();
        if (tutorialManager == null)
        {
            Debug.LogError("TutorialManager not found in the scene!");
        }

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

           if (tutorialManager != null && !tutorialManager.GetTriggerState("Jump"))
        {
            Debug.Log("Tutorial: Press Space to jump.");
            tutorialManager.SetTriggerState("Jump", true);
        }
 if (Input.GetKeyDown(KeyCode.J))
        {
            SaveGame();
        }

        // Call LoadGame when K is pressed
        if (Input.GetKeyDown(KeyCode.K))
        {
            LoadGame();
        }

    }
public void SaveGame()
    {
        // Ensure SaveManager is present in the scene
        SaveManager saveManager = FindObjectOfType<SaveManager>();

        if (saveManager != null)
        {
            saveManager.SaveGame(); // Call the SaveGame method from SaveManager
            Debug.Log("Game Saved!");
        }
        else
        {
            Debug.LogError("SaveManager not found in the scene!");
        }
    }

    // Load the game data
    public void LoadGame()
    {
        // Ensure SaveManager is present in the scene
        SaveManager saveManager = FindObjectOfType<SaveManager>();

        if (saveManager != null)
        {
            saveManager.LoadGame(); // Call the LoadGame method from SaveManager
            Debug.Log("Game Loaded!");
        }
        else
        {
            Debug.LogError("SaveManager not found in the scene!");
        }
    }

    public void MarkBossAsDefeated(string bossName)
    {
        if (!defeatedBosses.Contains(bossName))
        {
            defeatedBosses.Add(bossName); // Mark the boss as defeated
        }
    }

    public bool IsBossDefeated(string bossName)
    {
        return defeatedBosses.Contains(bossName); // Check if the boss is already defeated
    }

    

    public void TakeDamage(int damage, DamageType damageType)
    {
        // Prevent damage if invincible
        if (isInvincible)
        {
            return;
        }

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

    public void SetInvincible(bool invincible)
    {
        isInvincible = invincible;
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

    // Add a boss to the defeated list
    public void RecordBossDefeat(string bossName)
    {
        if (!defeatedBosses.Contains(bossName))
        {
            defeatedBosses.Add(bossName);
            Debug.Log($"Boss defeated: {bossName}");
        }
    }
     public bool GetTutorialTriggers()
    {
        // Replace with actual logic
        return false;
    }

    public void SetTutorialTriggers(bool state)
    {
        // Replace with actual logic
        Debug.Log($"Tutorial triggers set to: {state}");
    }

    // Get the list of defeated bosses
    public List<string> GetDefeatedBosses()
    {
        return new List<string>(defeatedBosses);
    }

    // Set defeated bosses (used when loading data)
    public void SetDefeatedBosses(List<string> bosses)
    {
        defeatedBosses = new List<string>(bosses);
    }
}
