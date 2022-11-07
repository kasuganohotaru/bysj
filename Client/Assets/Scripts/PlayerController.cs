using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    public float speed;
    public float jumpForce;
    private float horizontalMove;

    public LayerMask ground;

    public bool isGround, isJump, isDashing;

    public Transform groundCheckA;
    public Transform groundCheckB;
    private bool jumpPressed;
    private int jumpCount;

    const int PlayerScale = 4;

    private void Awake()
    {
        rb = transform.GetComponent<Rigidbody2D>();
        anim = transform.GetComponent<Animator>();
    }
    void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump") && jumpCount > 0)
        {
            jumpPressed = true;
        }
        print(horizontalMove);
    }
    void FixedUpdate()
    {
        isGround = Physics2D.OverlapCircle(groundCheckA.position, 0.1f, ground) || Physics2D.OverlapCircle(groundCheckB.position, 0.1f, ground);
        GroundMove();
        Jump();
    }

    private void GroundMove()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(horizontalMove * speed, rb.velocity.y);

        if (horizontalMove != 0)
        {
            transform.localScale = new Vector3(-horizontalMove* PlayerScale, 1*PlayerScale, 1);
            anim.SetFloat("run", Math.Abs(horizontalMove));
        }

    }

    void Jump()//ÌøÔ¾
    {
        if (isGround)
        {
            jumpCount = 2;//¿ÉÌøÔ¾ÊýÁ¿
        }
        else
        {
            isJump = true;
        }
        if (jumpPressed && isGround)
        {
            isJump = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount--;
            jumpPressed = false;
        }
        else if (jumpPressed && jumpCount > 0 && isJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount-=2;
            jumpPressed = false;
        }
    }
}
