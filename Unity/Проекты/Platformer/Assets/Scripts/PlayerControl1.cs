﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl1 : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;
    SpriteRenderer sprite;
    [SerializeField]
    int speed;
    [SerializeField]
    int jumpForce;
    [SerializeField]
    Transform groundCheck;
    bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
       if (Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground")))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            if (isGrounded)
                animator.Play("Player_Run");
            sprite.flipX = false;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            if (isGrounded)
                animator.Play("Player_Run");
            sprite.flipX = true;
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            if (isGrounded)
                animator.Play("Player_Idle");
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow))
        {
            if (isGrounded == true)
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.Play("Player_Jump");
        }
    }
}
