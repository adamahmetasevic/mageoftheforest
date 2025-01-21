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
        GameData data = new GameData
        {
            playerPosition = new SerializableVector3(player.transform.position), // Serialize the position
            unlockedSpells = player.spellCaster.GetUnlockedSpells().Select(spell => spell.name).ToList(),
            defeatedBosses = player.defeatedBosses, // Save defeated bosses
            tutorialTriggers = player.GetTutorialTriggers(),
            playerHealth = player.currentHealth,
            playerMana = player.currentMana
        };

        SaveSystem.SaveGame(data);
    }

    public void LoadGame()
    {
        GameData data = SaveSystem.LoadGame();
        if (data != null)
        {
            player.transform.position = data.playerPosition.ToVector3(); // Convert back to Vector3
            player.spellCaster.SetUnlockedSpells(data.unlockedSpells.Select(name => SpellDatabase.GetSpellByName(name)).ToList());
            player.SetDefeatedBosses(data.defeatedBosses); // Load defeated bosses
            player.SetTutorialTriggers(data.tutorialTriggers);
            player.currentHealth = data.playerHealth;
            player.currentMana = data.playerMana;
        }
    }
}
