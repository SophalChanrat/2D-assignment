using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;
    
    [Header("Visual Feedback")]
    public float flashDuration = 0.1f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    
    [Header("Death Settings")]
    public float deathDelay = 1f;
    
    private Animator animator;
    private bool isDead = false;
    private DamageSplash damageSplash;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        damageSplash = GetComponent<DamageSplash>();
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        
        // Show damage splash effect
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
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        // Notify WinConditionManager that an enemy was killed
        if (WinConditionManager.Instance != null)
        {
            WinConditionManager.Instance.EnemyKilled();
        }
        
        // Play death animation
        if (animator != null)
        {
            // Check if animator has the die trigger parameter
            bool hasDieTrigger = false;
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.name == "die" && param.type == AnimatorControllerParameterType.Trigger)
                {
                    hasDieTrigger = true;
                    break;
                }
            }
            
            if (hasDieTrigger)
            {
                animator.SetTrigger("die");
            }
        }
        
        // Disable AI and collision
        EnemyAI ai = GetComponent<EnemyAI>();
        if (ai != null)
        {
            ai.enabled = false;
        }
        
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }
        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
        }
        
        // Destroy after delay
        Destroy(gameObject, deathDelay);
    }

    IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    // Public getter for current health
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public bool IsDead()
    {
        return isDead;
    }
}