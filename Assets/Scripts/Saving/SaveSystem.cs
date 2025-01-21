using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public static class SaveSystem
{
    public static void SaveGame(GameData data, SpellCaster spellCaster)
{
    data.unlockedSpells = new List<string>();
    foreach (var spell in spellCaster.unlockedSpells)
    {
        data.unlockedSpells.Add(spell.name); // Save unlocked spell names
    }

    data.equippedSpells = new List<string>();
    foreach (var spell in spellCaster.equippedSpells)
    {
        if (spell != null)
        {
            data.equippedSpells.Add(spell.name); // Save equipped spell names
        }
        else
        {
            data.equippedSpells.Add(""); // Save an empty slot
        }
    }

    string path = Application.persistentDataPath + "/game.save";
    FileStream stream = new FileStream(path, FileMode.Create);

    BinaryFormatter formatter = new BinaryFormatter();
    formatter.Serialize(stream, data);

    stream.Close();
}


    public static GameData LoadGame(SpellCaster spellCaster)
{
    string path = Application.persistentDataPath + "/game.save";
    if (File.Exists(path))
    {
        FileStream stream = new FileStream(path, FileMode.Open);

        BinaryFormatter formatter = new BinaryFormatter();
        GameData data = (GameData)formatter.Deserialize(stream);

        stream.Close();

        // Load spells into SpellCaster
        if (spellCaster != null)
        {
            spellCaster.LoadSpells(data.equippedSpells, data.unlockedSpells);
        }

        return data;
    }
    else
    {
        Debug.LogError("No save file found!");
        return null;
    }
}


}
