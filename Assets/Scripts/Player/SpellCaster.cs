using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    public Spell[] equippedSpells; // Array to store equipped spells (initially empty)
    public Transform firePoint; // Where spells are cast from
    private float[] cooldownTimers;
    private Player player; // Reference to the Player component

    void Start()
    {
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

    public void UnlockSpell(Spell newSpell)
    {
        // Add the new spell to the first empty slot in the equippedSpells array
        for (int i = 0; i < equippedSpells.Length; i++)
        {
            if (equippedSpells[i] == null)
            {
                equippedSpells[i] = newSpell;
                Debug.Log($"You have unlocked the spell: {newSpell.name}");
                ResizeCooldownArray(); // Adjust cooldownTimers size to match equippedSpells
                return;
            }
        }

        Debug.LogWarning("No available slot to equip the unlocked spell!");
    }

    void ResizeCooldownArray()
    {
        // Resize the cooldown array to match the equippedSpells array
        float[] newCooldownTimers = new float[equippedSpells.Length];
        for (int i = 0; i < cooldownTimers.Length; i++)
        {
            newCooldownTimers[i] = cooldownTimers[i]; // Copy existing cooldowns
        }
        cooldownTimers = newCooldownTimers;
    }

    void CastSpell(int index)
    {
        // Ensure the spell is unlocked and the index is valid
        if (index < 0 || index >= equippedSpells.Length || equippedSpells[index] == null || player == null)
        {
            Debug.Log("Invalid spell index or no spell equipped.");
            return;
        }

        Spell spell = equippedSpells[index];

        // Check if player has enough mana
        if (player.currentMana < spell.manaCost)
        {
            Debug.Log("Not enough mana!");
            return;
        }

        // Check cooldown
        if (cooldownTimers[index] > 0)
        {
            Debug.Log("Spell is on cooldown!");
            return;
        }

        // Deduct mana
        player.UseMana(spell.manaCost);

        // Set cooldown
        cooldownTimers[index] = spell.cooldown;

        // Instantiate and initialize the spell
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
}
