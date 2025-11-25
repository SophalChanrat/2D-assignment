using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 4;
    public int currentHealth;

    [Header("UI Hearts")]
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    
    [Header("Damage Flash Effect")]
    public float flashDuration = 0.1f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private DamageSplash damageSplash;
    
    void Start()
    {
        currentHealth = maxHealth;
        UpdateHearts();
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        damageSplash = GetComponent<DamageSplash>();
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        UpdateHearts();
        
        // Show damage splash effect (same as enemy)
        if (damageSplash != null)
        {
            damageSplash.ShowDamageSplash();
        }
        else
        {
            // Fallback to simple flash if no DamageSplash component
            if (spriteRenderer != null)
            {
                StartCoroutine(FlashRed());
            }
        }
    }
    
    void Die()
    {
        // Trigger game over
        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.ShowGameOver();
        }
        
        // Disable player controls
        gameController controller = GetComponent<gameController>();
        if (controller != null)
        {
            controller.enabled = false;
        }
        
        // Play death animation
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            // Check if animator has die trigger
            foreach (var param in animator.parameters)
            {
                if (param.name == "die" && param.type == UnityEngine.AnimatorControllerParameterType.Trigger)
                {
                    animator.SetTrigger("die");
                    break;
                }
            }
        }
    }
    
    IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }
    
    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }
}
