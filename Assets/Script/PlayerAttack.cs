using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public int attackDamage = 1;
    public float attackRange = 1.5f;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    
    [Header("Attack Timing")]
    public float attackCooldown = 0.5f;
    private float lastAttackTime;
    
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    void Attack()
    {
        // Play attack animation
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        
        // Detect enemies in range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        
        // Damage enemies
        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null && !enemyHealth.IsDead())
            {
                enemyHealth.TakeDamage(attackDamage);
            }
        }
    }

    // Visualize attack range in editor
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
