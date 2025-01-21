using UnityEngine;
using System.Collections.Generic;

public class SpellCaster : MonoBehaviour
{
    public Spell[] equippedSpells; // Array to store equipped spells
    public List<Spell> unlockedSpells = new List<Spell>(); // List to hold unlocked spells
    public Transform firePoint; // Where spells are cast from
    private float[] cooldownTimers;
    private Player player; // Reference to the Player component
        public string bossName; // The name of the boss, used for saving/loading


    void Start()
    {
        player = FindObjectOfType<Player>(); // Get the reference to the player

        // Check if the boss has been defeated already
        if (player.IsBossDefeated(bossName))
        {
            // Disable the boss if it has been defeated
            gameObject.SetActive(false); // Deactivate the boss so it doesn't appear
        }

        cooldownTimers = new float[equippedSpells.Length];
        player = GetComponent<Player>(); // Get reference to Player component
    }

    void Update()
    {
        HandleCooldowns();

        // Cast spells with keys (e.g., Q, W, E, R)
        if (Input.GetKeyDown(KeyCode.Q)) CastSpell(0);
        if (Input.GetKeyDown(KeyCode.E)) CastSpell(1);
    }

    void HandleCooldowns()
    {
        for (int i = 0; i < cooldownTimers.Length; i++)
        {
            if (cooldownTimers[i] > 0)
            {
                cooldownTimers[i] -= Time.deltaTime;
            }
        }
    }

    public void UnlockSpell(Spell spell)
    {
        if (!unlockedSpells.Contains(spell))
        {
            unlockedSpells.Add(spell); // Add the spell to the unlocked list
        }
    }

    void ResizeCooldownArray()
    {
        float[] newCooldownTimers = new float[equippedSpells.Length];
        for (int i = 0; i < cooldownTimers.Length; i++)
        {
            newCooldownTimers[i] = cooldownTimers[i];
        }
        cooldownTimers = newCooldownTimers;
    }

    void CastSpell(int index)
    {
        if (index < 0 || index >= equippedSpells.Length || equippedSpells[index] == null || player == null)
        {
            Debug.Log("Invalid spell index or no spell equipped.");
            return;
        }

        Spell spell = equippedSpells[index];

        if (player.currentMana < spell.manaCost)
        {
            Debug.Log("Not enough mana!");
            return;
        }

        if (cooldownTimers[index] > 0)
        {
            Debug.Log("Spell is on cooldown!");
            return;
        }

        player.UseMana(spell.manaCost);

        cooldownTimers[index] = spell.cooldown;

        GameObject spellObject = Instantiate(spell.spellPrefab, firePoint.position, firePoint.rotation);
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - (Vector2)firePoint.position).normalized;

        Fireball fireball = spellObject.GetComponent<Fireball>();
        if (fireball != null)
        {
            fireball.Initialize(direction, fireball.damage);
        }

        LightningBoltBase lightning = spellObject.GetComponent<LightningBoltBase>();
        if (lightning != null)
        {
            lightning.firePoint = firePoint;
            lightning.Initialize(mousePosition);
        }
    }

    // New Methods for Saving/Loading
    public List<Spell> GetUnlockedSpells()
    {
        List<Spell> unlockedSpells = new List<Spell>();
        foreach (var spell in equippedSpells)
        {
            if (spell != null) unlockedSpells.Add(spell);
        }
        return unlockedSpells;
    }

    public void SetUnlockedSpells(List<Spell> spells)
    {
        unlockedSpells = spells;
    }

    public void SetEquippedSpells(Spell[] spells)
    {
        equippedSpells = spells;
        ResizeCooldownArray(); // Reinitialize the cooldown array based on the equipped spells
    }
}
