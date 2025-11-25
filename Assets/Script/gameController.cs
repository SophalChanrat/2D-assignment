using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class gameController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpInpulse = 10f;
    public int maxJumps = 2;
    private int jumpCount = 0;

    [Header("Death Settings")]
    public float deathHeight = -10f;
    
    private Vector2 lastSafePosition;
    private Rigidbody2D rb;
    private TouchingDirection touchingDirection;
    private Animator animator;
    private CharacterHealth characterHealth;
    private PlayerAttack playerAttack;
    private PlayerSummon playerSummon;
    private Vector2 moveInput;
    private bool isFacingRight = true;

    public bool isMoving { get; private set; }

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirection = GetComponent<TouchingDirection>();
        animator = GetComponent<Animator>();
        characterHealth = GetComponent<CharacterHealth>();
        playerAttack = GetComponent<PlayerAttack>();
        playerSummon = GetComponent<PlayerSummon>();
    }

    void Update()
    {
        // Check for falling off the map
        if (transform.position.y < deathHeight)
        {
            Die();
        }
        
        // Save last safe position when grounded
        if (touchingDirection.IsGround)
        {
            lastSafePosition = transform.position;
            jumpCount = 0;
        }
    }
    
    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();   

        isMoving = moveInput != Vector2.zero;
        
        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
        }
        
        // Flip the character based on movement direction
        if (moveInput.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveInput.x < 0 && isFacingRight)
        {
            Flip();
        }
    }
    
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // Can jump if grounded, OR if we haven't used all jumps yet
            if (touchingDirection.IsGround || jumpCount < maxJumps)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpInpulse);
                jumpCount++;
            }
        }
    }
    
    public void Onattack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
            
            // Trigger PlayerAttack if it exists
            if (playerAttack != null)
            {
                playerAttack.OnAttack(context);
            }
        }
    }
    
    public void OnSummon(InputAction.CallbackContext context)
    {
        if (context.started && playerSummon != null)
        {
            playerSummon.OnSummon();
        }
    }
    
    public void Die()
    {
        if (animator != null)
        {
            animator.SetTrigger("die");
        }
        
        if (characterHealth != null)
        {
            characterHealth.TakeDamage(1);
        }

        Respawn();
    }
    
    void Respawn()
    {
        float offset = 1f;
        float fallingDirection = transform.position.x - lastSafePosition.x;

        if (fallingDirection > 0)
        {
            lastSafePosition += new Vector2(-offset, 0);
        }
        else
        {
            lastSafePosition += new Vector2(offset, 0);
        }

        transform.position = lastSafePosition;
        rb.velocity = Vector2.zero;
    }
}