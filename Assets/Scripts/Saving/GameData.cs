using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public SerializableVector3 playerPosition; // Updated to use a serializable Vector3
    public List<string> unlockedSpells; // List of unlocked spell names
    public List<string> equippedSpells; // List of equipped spell names
    public List<string> defeatedBosses;
    public bool tutorialTriggers;
    public int playerHealth;
    public float playerMana;
}
