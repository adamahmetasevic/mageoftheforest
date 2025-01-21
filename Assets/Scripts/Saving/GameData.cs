using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public SerializableVector3 playerPosition; 
    public List<string> unlockedSpells; // Names of unlocked spells
    public List<string> equippedSpells; // Names of equipped spells
    public List<string> defeatedBosses;
    public bool tutorialTriggers;
    public int playerHealth;
    public float playerMana;
}

