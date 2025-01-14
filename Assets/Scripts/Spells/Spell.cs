using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Spell")]
public class Spell : ScriptableObject
{
    public string spellName; // Name of the spell
    public Sprite icon; // Icon for UI
    public int manaCost; // Mana cost
    public float cooldown; // Cooldown time
    public GameObject spellPrefab; // Prefab for the spell
}
