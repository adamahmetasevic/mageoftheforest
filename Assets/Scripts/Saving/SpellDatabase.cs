using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpellDatabase
{
    private static Dictionary<string, Spell> spellDictionary = new Dictionary<string, Spell>();

    public static void RegisterSpell(Spell spell)
    {
        if (!spellDictionary.ContainsKey(spell.name))
        {
            spellDictionary.Add(spell.name, spell);
        }
    }

    public static Spell GetSpellByName(string name)
    {
        return spellDictionary.TryGetValue(name, out var spell) ? spell : null;
    }
}
