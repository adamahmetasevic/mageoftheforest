using UnityEngine;

public class SpellUnlockable : MonoBehaviour
{
    public Spell spellToUnlock; // The spell that will be unlocked
    public ParticleSystem unlockEffect; // Visual effect for unlocking
    private bool isPlayerNearby = false; // Tracks if the player is near the unlockable
    private Vector3 initialPosition; // The initial position of the object
    public float floatSpeed = 1f; // Speed of floating motion
    public float floatAmount = 0.1f; // How much the object moves in the floating effect
    

    private void Start()
    {
        initialPosition = transform.position; // Store the initial position of the object
    }

    private void Update()
    {
        // Make the object float around a tiny area
        FloatAround();

        // Allow unlocking the spell if the player is nearby and presses 'F'
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            UnlockSpell();
        }
    }

    private void FloatAround()
    {
        // Calculate floating motion using sine and cosine for smooth, circular movement
        float xOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        float yOffset = Mathf.Cos(Time.time * floatSpeed) * floatAmount;
        
        transform.position = initialPosition + new Vector3(xOffset, yOffset, 0f);
    }

    private void UnlockSpell()
    {
        Player player = FindObjectOfType<Player>(); // Find the player in the scene
        if (player != null && player.spellCaster != null && spellToUnlock != null)
        {
            // Add the spell to the player's unlocked spells
            player.spellCaster.UnlockSpell(spellToUnlock);

            // Equip the spell automatically (add to equipped spells)
            EquipUnlockedSpell(player);

            // Play the unlock effect
            if (unlockEffect != null)
            {
                // Instantiate the particle system prefab without modifying it
                ParticleSystem explosion = Instantiate(unlockEffect, transform.position, unlockEffect.transform.rotation);
                
                // Destroy the instantiated particle system after its duration
                Destroy(explosion.gameObject, explosion.main.duration + 2f);
            }

            // Feedback
            Debug.Log($"You have unlocked and equipped the spell: {spellToUnlock.name}");

            // Destroy or deactivate the spell unlockable object
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("Player, SpellCaster component, or Spell reference is missing!");
        }
    }

    private void EquipUnlockedSpell(Player player)
    {
        // Check if there's an empty slot in the equipped spells array
        for (int i = 0; i < player.spellCaster.equippedSpells.Length; i++)
        {
            if (player.spellCaster.equippedSpells[i] == null)
            {
                // Equip the unlocked spell in the first available slot
                player.spellCaster.equippedSpells[i] = spellToUnlock;
                Debug.Log($"Equipped {spellToUnlock.name} in slot {i}");
                return;
            }
        }

        // If no empty slot is found, consider replacing the first spell (or implement your own logic here)
        player.spellCaster.equippedSpells[0] = spellToUnlock;
        Debug.Log($"No empty slot found. Replaced first slot with {spellToUnlock.name}");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true; // Player entered the area
            Debug.Log("Press F to unlock the spell.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false; // Player left the area
        }
    }
}
