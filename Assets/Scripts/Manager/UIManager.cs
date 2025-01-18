using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider healthSlider;    
    public Slider damageSlider;    
    public Slider manaBar;         
    public Slider bossHealthSlider;
    public Slider bossDamageSlider; // Add this new slider for boss damage animation

    private float previousHealth = 100f;
    private float previousBossHealth = 100f; // Add this for boss health tracking
    private Coroutine damageCoroutine;
    private Coroutine bossDamageCoroutine; // Add this for boss damage animation
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

        if (bossHealthSlider != null)
        {
            bossHealthSlider.gameObject.SetActive(false); // Hide boss health bar initially
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

        public void ActivateBossHealthSlider(float currentHealth, float maxHealth)
{
    if (bossHealthSlider != null && bossDamageSlider != null)
    {
        float healthPercent = (currentHealth / maxHealth) * 100f;
        
        bossHealthSlider.maxValue = 100f;  // Set max value to 100 for percentage
        bossHealthSlider.value = healthPercent;
        bossDamageSlider.maxValue = 100f;  // Set max value to 100 for percentage
        bossDamageSlider.value = healthPercent;
        
        bossHealthSlider.gameObject.SetActive(true);
        bossDamageSlider.gameObject.SetActive(true);
        previousBossHealth = healthPercent;
    }
}

public void UpdateBossHealth(float currentHealth, float maxHealth)
{
    if (bossHealthSlider != null && bossDamageSlider != null)
    {
        float healthPercent = (currentHealth / maxHealth) * 100f;
        bossHealthSlider.value = healthPercent;

        if (healthPercent < previousBossHealth)
        {
            if (bossDamageCoroutine != null)
                StopCoroutine(bossDamageCoroutine);

            bossDamageCoroutine = StartCoroutine(AnimateBossDamage(previousBossHealth, healthPercent));
        }
        else
        {
            bossDamageSlider.value = healthPercent;
        }

        previousBossHealth = healthPercent;
    }
}

    private IEnumerator AnimateBossDamage(float fromValue, float toValue)
    {
        yield return new WaitForSeconds(0.2f);

        float elapsed = 0f;
        float duration = 0.4f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float currentValue = Mathf.Lerp(fromValue, toValue, elapsed / duration);
            bossDamageSlider.value = currentValue;
            yield return null;
        }

        bossDamageSlider.value = toValue;
    }

    public void DeactivateBossHealthSlider()
    {
        if (bossHealthSlider != null)
        {
            bossHealthSlider.gameObject.SetActive(false);
            bossDamageSlider.gameObject.SetActive(false);
        }
    }



}
