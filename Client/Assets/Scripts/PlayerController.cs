using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    public float speed;
    public float jumpforce;



    private void Awake()
    {
        rb = transform.GetComponent<Rigidbody2D>();
        anim = transform.GetComponent<Animator>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    private void Movement()
    {
        float horizontalmove = Input.GetAxis("Horizontal");
        float facedirection = Input.GetAxisRaw("Horizontal");
        

        //ÒÆ¶¯
        if(horizontalmove != 0)
        {
            rb.velocity = new Vector2 (horizontalmove*speed,rb.velocity.y);
            anim.SetFloat("run",Math.Abs(facedirection));
        }
        if(facedirection != 0)
        {
            transform.localScale = new Vector3(-facedirection*4, 1*4, 1);
        }
        //ÌøÔ¾
        if (Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2 (rb.velocity.x,jumpforce*Time.deltaTime);
        }
    
    }
}
