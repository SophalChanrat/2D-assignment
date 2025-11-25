using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSummon : MonoBehaviour
{
    [Header("Summon Settings")]
    public int summonDamage = 1;
    public float summonRange = 3f;
    public Transform summonPoint; // Point where summon appears
    public LayerMask enemyLayers;
    
    [Header("Summon Timing")]
    public float summonCooldown = 2f;
    private float lastSummonTime = -999f;
    public float summonDuration = 1f; // How long the summon stays
    
    [Header("Summon Visuals")]
    public GameObject summonPrefab; // The visual summon object (your aHit/Summon sprite)
    public Sprite summonIconSprite; // UI icon for the skill
    public AudioClip summonAudioClip; // Audio clip for summon sound
    public float summonVerticalOffset = 0.5f; // How high above enemy the summon appears
    
    [Header("UI Elements")]
    public Image summonCooldownImage; // UI image to show cooldown
    public KeyCode summonKey = KeyCode.E; // For UI display
    
    private Animator playerAnimator;
    private bool isFacingRight = true;
    private GameObject currentSummon;
    private AudioSource audioSource;

    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        
        // Add AudioSource if it doesn't exist
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Initialize cooldown UI
        if (summonCooldownImage != null)
        {
            summonCooldownImage.fillAmount = 0;
        }
    }

    void Update()
    {
        // Update facing direction based on player scale
        isFacingRight = transform.localScale.x > 0;
        
        // Update cooldown UI
        UpdateCooldownUI();
    }

    public void OnSummon()
    {
        // Check if cooldown is ready
        if (Time.time >= lastSummonTime + summonCooldown)
        {
            CastSummon();
        }
    }

    void CastSummon()
    {
        lastSummonTime = Time.time;
        
        // Play summon sound
        if (audioSource != null && summonAudioClip != null)
        {
            audioSource.PlayOneShot(summonAudioClip);
        }
        
        // Play summon animation if it exists
        if (playerAnimator != null)
        {
            // Check if animator has summon trigger
            foreach (var param in playerAnimator.parameters)
            {
                if (param.name == "Summon" && param.type == UnityEngine.AnimatorControllerParameterType.Trigger)
                {
                    playerAnimator.SetTrigger("Summon");
                    break;
                }
            }
        }
        
        // Find and damage enemies, spawning summon on each
        SpawnSummonsOnEnemies();
    }

    void SpawnSummonsOnEnemies()
    {
        Vector2 summonPosition = summonPoint != null ? summonPoint.position : transform.position;
        
        // Adjust position based on facing direction
        float directionMultiplier = isFacingRight ? 1f : -1f;
        summonPosition += new Vector2(directionMultiplier * 1f, 0);
        
        // Detect enemies in range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(summonPosition, summonRange, enemyLayers);
        
        // Spawn summon on each enemy and damage them
        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null && !enemyHealth.IsDead())
            {
                // Deal damage
                enemyHealth.TakeDamage(summonDamage);
                
                // Spawn summon visual on enemy
                if (summonPrefab != null)
                {
                    // Position summon on enemy (slightly above center)
                    Vector3 enemyPosition = enemy.transform.position;
                    Vector3 spawnPosition = enemyPosition + new Vector3(0, summonVerticalOffset, 0);
                    
                    GameObject summon = Instantiate(summonPrefab, spawnPosition, Quaternion.identity);
                    
                    // Add follow script to make it stick to enemy
                    SummonFollowTarget followScript = summon.AddComponent<SummonFollowTarget>();
                    followScript.SetTarget(enemy.transform);
                    
                    // Flip summon sprite if needed (match enemy facing)
                    if (enemy.transform.localScale.x < 0)
                    {
                        Vector3 scale = summon.transform.localScale;
                        scale.x *= -1;
                        summon.transform.localScale = scale;
                    }
                    
                    // Destroy summon after duration
                    Destroy(summon, summonDuration);
                }
            }
        }
    }

    void UpdateCooldownUI()
    {
        if (summonCooldownImage != null)
        {
            float cooldownPercent = (Time.time - lastSummonTime) / summonCooldown;
            
            if (cooldownPercent >= 1f)
            {
                summonCooldownImage.fillAmount = 0; // Ready to use
            }
            else
            {
                summonCooldownImage.fillAmount = 1f - cooldownPercent; // Still on cooldown
            }
        }
    }

    public bool IsOnCooldown()
    {
        return Time.time < lastSummonTime + summonCooldown;
    }

    public float GetCooldownTimeRemaining()
    {
        float timeRemaining = (lastSummonTime + summonCooldown) - Time.time;
        return Mathf.Max(0, timeRemaining);
    }

    // Visualize summon range in editor
    private void OnDrawGizmosSelected()
    {
        Vector3 summonPosition = summonPoint != null ? summonPoint.position : transform.position;
        
        // Draw summon range
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(summonPosition, summonRange);
    }
}
