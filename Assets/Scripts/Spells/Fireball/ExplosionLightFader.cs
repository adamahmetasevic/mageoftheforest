// ExplosionLightFader.cs
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class ExplosionLightFader : MonoBehaviour
{
    private Light2D explosionLight;
    private bool isFading = false;
    public float fadeDuration = 0.5f; // Customize this in the inspector

    void Start()
    {
        explosionLight = GetComponent<Light2D>();
        FadeOutLight(); // Start fading immediately when created
    }

    public void FadeOutLight()
    {
        if (!isFading && explosionLight != null)
        {
            isFading = true;
            StartCoroutine(FadeOutCoroutine());
        }
    }

    private IEnumerator FadeOutCoroutine()
    {
        float startIntensity = explosionLight.intensity;
        float startRadius = explosionLight.pointLightOuterRadius;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float lerpFactor = elapsedTime / fadeDuration;
            explosionLight.intensity = Mathf.Lerp(startIntensity, 0f, lerpFactor);
            explosionLight.pointLightOuterRadius = Mathf.Lerp(startRadius, 0f, lerpFactor);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        explosionLight.intensity = 0f;
        explosionLight.pointLightOuterRadius = 0f;
    }
}