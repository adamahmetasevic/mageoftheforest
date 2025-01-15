using UnityEngine;
using System.Collections.Generic;

public abstract class Enemy : MonoBehaviour
{
    public int health;
    public float speed;
    public int damage;

    // Resistances and weaknesses
    public Dictionary<DamageType, float> resistances = new Dictionary<DamageType, float>();
    public Dictionary<DamageType, float> weaknesses = new Dictionary<DamageType, float>();

    public virtual void TakeDamage(int amount, DamageType damageType)
    {
        float finalDamage = amount;

        // Apply resistance or weakness modifiers
        if (resistances.ContainsKey(damageType))
        {
            finalDamage *= (1 - resistances[damageType]); // Reduce damage based on resistance
        }
        else if (weaknesses.ContainsKey(damageType))
        {
            finalDamage *= (1 + weaknesses[damageType]); // Increase damage based on weakness
        }

        health -= Mathf.CeilToInt(finalDamage); // Apply damage and round up
        Debug.Log($"{gameObject.name} took {Mathf.CeilToInt(finalDamage)} {damageType} damage. Remaining health: {health}");

        if (health <= 0)
        {
            Die();
        }
    }
    public abstract void Move();
    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject);
    }
}
