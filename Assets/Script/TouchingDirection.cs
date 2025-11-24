using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchingDirection : MonoBehaviour
{
    public ContactFilter2D contactFilter;
    public float groundDistance = 0.05f;
    Rigidbody2D rb;
    CapsuleCollider2D capsuleCollider;

    RaycastHit2D[] groundhits = new RaycastHit2D[5];

    private bool _isGrounded;
    public bool IsGround { get { 
           return _isGrounded;
        }private set {
            _isGrounded = value;
        } }

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        IsGround = capsuleCollider.Cast(Vector2.down, contactFilter, groundhits, groundDistance) > 0;
    }
}
