using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float patrolSpeed = 2f;
    public float patrolDistance = 5f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public Transform wallCheck;
    public float checkRadius = 0.2f;
    
    [Header("Contact Damage")]
    public int contactDamage = 1;
    public float damageCooldown = 1f;
    
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 startPosition;
    private bool movingRight = true;
    private bool isGrounded;
    private bool hitWall;
    private float lastContactDamageTime;
    private float lastFlipTime;
    private const float MIN_FLIP_INTERVAL = 0.5f; // Prevent rapid flipping

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startPosition = transform.position;
        lastFlipTime = -MIN_FLIP_INTERVAL; // Allow immediate first flip
    }

    void Update()
    {
        CheckGround();
        CheckWall();
        
        // Update animator
        if (animator != null)
        {
            animator.SetBool("IsWalking", Mathf.Abs(rb.velocity.x) > 0.1f);
        }
    }

    void FixedUpdate()
    {
        Patrol();
    }

    void Patrol()
    {
        // Check if reached patrol distance from start position
        float distanceFromStart = Mathf.Abs(transform.position.x - startPosition.x);
        
        // Determine if we should flip (with time check to prevent glitching)
        bool shouldFlip = false;
        
        if (Time.time - lastFlipTime >= MIN_FLIP_INTERVAL)
        {
            if (hitWall)
            {
                shouldFlip = true;
            }
            else if (!isGrounded)
            {
                shouldFlip = true;
            }
            else if (distanceFromStart >= patrolDistance)
            {
                shouldFlip = true;
            }
        }
        
        if (shouldFlip)
        {
            Flip();
        }
        
        // Move
        float moveDirection = movingRight ? 1 : -1;
        rb.velocity = new Vector2(moveDirection * patrolSpeed, rb.velocity.y);
    }

    void CheckGround()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        }
        else
        {
            isGrounded = true; // Default to grounded if no check point
        }
    }

    void CheckWall()
    {
        if (wallCheck != null)
        {
            hitWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, groundLayer);
        }
        else
        {
            hitWall = false; // Default to no wall if no check point
        }
    }

    void Flip()
    {
        movingRight = !movingRight;
        lastFlipTime = Time.time;
        
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        
        // Update start position to current position when flipping
        // This prevents the enemy from walking too far in one direction
        startPosition = transform.position;
    }

    // Handle collision damage (player takes damage when touching enemy)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Ensure we have at least one contact point
            if (collision.contacts.Length > 0)
            {
                // Check if player landed on top of enemy
                if (collision.contacts[0].normal.y > 0.5f)
                {
                    // Player jumped on enemy - enemy takes damage
                    EnemyHealth health = GetComponent<EnemyHealth>();
                    if (health != null && !health.IsDead())
                    {
                        health.TakeDamage(1);
                    }
                    
                    // Bounce player up
                    Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                    if (playerRb != null)
                    {
                        playerRb.velocity = new Vector2(playerRb.velocity.x, 10f);
                    }
                }
                else
                {
                    // Player touched enemy from side - player takes damage
                    if (Time.time >= lastContactDamageTime + damageCooldown)
                    {
                        CharacterHealth playerHealth = collision.gameObject.GetComponent<CharacterHealth>();
                        if (playerHealth != null)
                        {
                            playerHealth.TakeDamage(contactDamage);
                            lastContactDamageTime = Time.time;
                        }
                    }
                }
            }
        }
    }

    // Visualization in editor
    private void OnDrawGizmosSelected()
    {
        // Patrol area
        Gizmos.color = Color.green;
        Vector3 startPos = Application.isPlaying ? (Vector3)startPosition : transform.position;
        Gizmos.DrawLine(startPos + Vector3.left * patrolDistance, startPos + Vector3.right * patrolDistance);
        
        // Ground check
        if (groundCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
        
        // Wall check
        if (wallCheck != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(wallCheck.position, checkRadius);
        }
    }
}
