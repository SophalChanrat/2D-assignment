using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSplash : MonoBehaviour
{
    [Header("Splash Effect Settings")]
    public GameObject splashPrefab; // Assign a sprite or particle effect
    public Color splashColor = Color.red;
    public float splashDuration = 0.5f;
    public float splashSize = 1f;
    public Vector3 splashOffset = new Vector3(0, 0.5f, 0);
    
    [Header("Alternative: Simple Flash Effect")]
    public bool useFlashInstead = true;
    public int flashCount = 3;
    public float flashSpeed = 0.1f;
    
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    public void ShowDamageSplash()
    {
        if (useFlashInstead)
        {
            // Use rapid flash effect
            StartCoroutine(RapidFlash());
        }
        else if (splashPrefab != null)
        {
            // Spawn splash effect
            SpawnSplashEffect();
        }
        else
        {
            // Fallback to simple flash
            StartCoroutine(RapidFlash());
        }
    }

    void SpawnSplashEffect()
    {
        Vector3 spawnPosition = transform.position + splashOffset;
        GameObject splash = Instantiate(splashPrefab, spawnPosition, Quaternion.identity);
        
        // Set color if it has a SpriteRenderer
        SpriteRenderer splashRenderer = splash.GetComponent<SpriteRenderer>();
        if (splashRenderer != null)
        {
            splashRenderer.color = splashColor;
        }
        
        // Set size
        splash.transform.localScale = Vector3.one * splashSize;
        
        // Animate and destroy
        StartCoroutine(AnimateSplash(splash));
    }

    IEnumerator AnimateSplash(GameObject splash)
    {
        SpriteRenderer splashRenderer = splash.GetComponent<SpriteRenderer>();
        float elapsed = 0f;
        Vector3 startScale = splash.transform.localScale;
        Vector3 endScale = startScale * 1.5f;
        
        while (elapsed < splashDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / splashDuration;
            
            // Scale up
            splash.transform.localScale = Vector3.Lerp(startScale, endScale, progress);
            
            // Fade out
            if (splashRenderer != null)
            {
                Color color = splashRenderer.color;
                color.a = 1f - progress;
                splashRenderer.color = color;
            }
            
            yield return null;
        }
        
        Destroy(splash);
    }

    IEnumerator RapidFlash()
    {
        if (spriteRenderer == null) yield break;
        
        for (int i = 0; i < flashCount; i++)
        {
            // Flash to red
            spriteRenderer.color = splashColor;
            yield return new WaitForSeconds(flashSpeed);
            
            // Back to original
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashSpeed);
        }
        
        // Ensure original color
        spriteRenderer.color = originalColor;
    }
}
