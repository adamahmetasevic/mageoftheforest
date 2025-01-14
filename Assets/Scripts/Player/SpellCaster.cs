using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    public Spell[] equippedSpells; // Array of equipped spells
    public Transform firePoint; // Where spells are cast from
    public int maxMana = 100; // Player's max mana
    private int currentMana;

    private float[] cooldownTimers;

    void Start()
    {
        currentMana = maxMana;
        cooldownTimers = new float[equippedSpells.Length];
    }

    void Update()
    {
        HandleCooldowns();

        // Cast spells with keys (e.g., Q, W, E, R)
        if (Input.GetKeyDown(KeyCode.Q)) CastSpell(0);
        if (Input.GetKeyDown(KeyCode.W)) CastSpell(1);
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

    void CastSpell(int index)
{
    if (index < 0 || index >= equippedSpells.Length || equippedSpells[index] == null)
        return;

    Spell spell = equippedSpells[index];

    // Check mana and cooldown
    if (currentMana < spell.manaCost)
    {
        Debug.Log("Not enough mana!");
        return;
    }
    if (cooldownTimers[index] > 0)
    {
        Debug.Log("Spell is on cooldown!");
        return;
    }

    // Deduct mana and set cooldown
    currentMana -= spell.manaCost;
    cooldownTimers[index] = spell.cooldown;

    // Instantiate the spell prefab
    GameObject spellObject = Instantiate(spell.spellPrefab, firePoint.position, firePoint.rotation);

    // Calculate the direction towards the cursor
    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Get mouse position in world space
    Vector2 direction = (mousePosition - (Vector2)firePoint.position).normalized; // Calculate direction from firepoint to mouse

    // Initialize the fireball (or similar) script
    Fireball fireball = spellObject.GetComponent<Fireball>();
    if (fireball != null)
    {
        fireball.Initialize(direction, fireball.damage);
    }
}


    public void RegenerateMana(int amount)
    {
        currentMana = Mathf.Min(currentMana + amount, maxMana);
    }
}
