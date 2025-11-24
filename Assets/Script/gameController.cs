using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class gameController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector2 lastSafePosition;
    Rigidbody2D rb;
    TouchingDirection touchingDirection;
    Animator animator;
    CharacterHealth characterHealth;
    Vector2 moveInput;
    public float jumpInpulse = 10f;
    public float deathHeight = -10f;

    public bool isMoving { get; private set; }

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirection = GetComponent<TouchingDirection>();
        animator = GetComponent<Animator>();
        characterHealth = GetComponent<CharacterHealth>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < deathHeight)
        {
            Die();
        }
        if(touchingDirection.IsGround)
        {
            lastSafePosition = transform.position;
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
        animator.SetBool("IsMoving", isMoving);
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirection.IsGround)
        {
           rb.velocity = new Vector2(rb.velocity.x, jumpInpulse);
        }
    }
    public void Onattack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetTrigger("Attack");
        }
    }
    public void Die()
    {
        animator.SetTrigger("die");
        characterHealth.TakeDamage(1);

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