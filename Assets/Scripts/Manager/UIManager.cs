using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider healthSlider;    // The main green health slider
    public Slider damageSlider;    // The yellow damage slider
    public Slider manaBar;         // The mana bar slider

    private float previousHealth = 100f;
    private Coroutine damageCoroutine;
    public static UIManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }

    void Start()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = 100f;
            healthSlider.value = 100f;
        }

        if (damageSlider != null)
        {
            damageSlider.maxValue = 100f;
            damageSlider.value = 100f;
        }

        if (manaBar != null)
        {
            manaBar.maxValue = 100f;
            manaBar.value = 100f;
        }

        previousHealth = 100f;
    }
    
    // Update health bar logic
    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        if (healthSlider != null && damageSlider != null)
        {
            float healthPercent = (float)currentHealth / maxHealth * 100f;
            healthSlider.value = healthPercent;

            if (healthPercent < previousHealth)
            {
                if (damageCoroutine != null)
                    StopCoroutine(damageCoroutine);

                damageCoroutine = StartCoroutine(AnimateDamage(previousHealth, healthPercent));
            }
            else
            {
                damageSlider.value = healthPercent;
            }

            previousHealth = healthPercent;
        }
    }

    // Update mana bar logic (no color flashing or changes)
    public void UpdateMana(float currentMana, float maxMana)
    {
        if (manaBar != null)
        {
            float manaPercent = (currentMana / maxMana) * 100f;
            manaBar.value = manaPercent;
        }
    }


    // Damage animation (not related to mana, stays as is)
    private IEnumerator AnimateDamage(float fromValue, float toValue)
    {
        yield return new WaitForSeconds(0.2f);  // Hold before animation

        float elapsed = 0f;
        float duration = 0.4f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float currentValue = Mathf.Lerp(fromValue, toValue, elapsed / duration);
            damageSlider.value = currentValue;
            yield return null;
        }

        damageSlider.value = toValue;
    }
}
