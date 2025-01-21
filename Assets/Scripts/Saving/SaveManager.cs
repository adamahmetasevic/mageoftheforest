using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    public Player player; // Reference to the player object
    private TutorialManager tutorialManager;

    void Start()
    {
        tutorialManager = FindObjectOfType<TutorialManager>();
    }

    public void SaveGame()
    {
        // Ensure player and spellCaster references are valid
        if (player == null || player.spellCaster == null)
        {
            Debug.LogError("SaveGame failed: Player or SpellCaster reference is missing.");
            return;
        }

        GameData data = new GameData
        {
            playerPosition = new SerializableVector3(player.transform.position), // Serialize the position
            unlockedSpells = player.spellCaster.GetUnlockedSpells().Select(spell => spell.name).ToList(),
            equippedSpells = player.spellCaster.equippedSpells
                .Where(spell => spell != null) // Ignore null spells
                .Select(spell => spell.name).ToList(),
            defeatedBosses = player.defeatedBosses, // Save defeated bosses
            tutorialTriggers = player.GetTutorialTriggers(),
            playerHealth = player.currentHealth,
            playerMana = player.currentMana
        };

        // Pass player.spellCaster to SaveSystem
        SaveSystem.SaveGame(data, player.spellCaster);
    }

    public void LoadGame()
    {
        // Ensure player and spellCaster references are valid
        if (player == null || player.spellCaster == null)
        {
            Debug.LogError("LoadGame failed: Player or SpellCaster reference is missing.");
            return;
        }

        GameData data = SaveSystem.LoadGame(player.spellCaster); // Pass spellCaster
        if (data != null)
        {
            player.transform.position = data.playerPosition.ToVector3(); // Convert back to Vector3
            player.spellCaster.SetUnlockedSpells(
                data.unlockedSpells.Select(name => SpellDatabase.GetSpellByName(name)).ToList()
            );
            player.spellCaster.SetEquippedSpells(
                data.equippedSpells.Select(name => SpellDatabase.GetSpellByName(name)).ToArray()
            );
            player.SetDefeatedBosses(data.defeatedBosses); // Load defeated bosses
            player.SetTutorialTriggers(data.tutorialTriggers);
            player.currentHealth = data.playerHealth;
            player.currentMana = data.playerMana;
        }
    }
}
