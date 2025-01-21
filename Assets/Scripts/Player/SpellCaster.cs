using UnityEngine;
using System.Collections.Generic;

public class SpellCaster : MonoBehaviour
{
    public Spell[] equippedSpells; // Array to store equipped spells
    public List<Spell> unlockedSpells = new List<Spell>(); // List to hold unlocked spells
    public Transform firePoint; // Where spells are cast from
    private float[] cooldownTimers;
    private Player player; // Reference to the Player component


    void Start()
    {
        player = FindObjectOfType<Player>(); // Get the reference to the player

        // Check if the boss has been defeated already
      

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

 public void LoadSpells(List<string> equippedSpellNames, List<string> unlockedSpellNames)
{
    if (equippedSpellNames == null || unlockedSpellNames == null)
    {
        Debug.LogError("Spell data is null. Cannot load spells.");
        return;
    }

    // Clear the current unlocked spells and re-populate them
    unlockedSpells.Clear();
    foreach (string spellName in unlockedSpellNames)
    {
        Spell spell = SpellDatabase.GetSpellByName(spellName);
        if (spell != null)
        {
            unlockedSpells.Add(spell);
        }
    }

    // Initialize all equipped spell slots to null first
    for (int i = 0; i < equippedSpells.Length; i++)
    {
        equippedSpells[i] = null;
    }

    // Only populate slots that had spells saved
    for (int i = 0; i < equippedSpellNames.Count && i < equippedSpells.Length; i++)
    {
        if (!string.IsNullOrEmpty(equippedSpellNames[i]))
        {
            Spell spell = SpellDatabase.GetSpellByName(equippedSpellNames[i]);
            if (spell != null)
            {
                equippedSpells[i] = spell;
                Debug.Log($"Equipped {equippedSpellNames[i]} in slot {i}");
            }
        }
    }

    ResizeCooldownArray();
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
