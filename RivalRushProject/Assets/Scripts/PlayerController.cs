using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  [SerializeField] private float walkSpeed = 2f;
  [SerializeField] private float runSpeed;
  
  [SerializeField] private float jumpForce = 7f;
  [SerializeField] private bool jumpControl;
  [SerializeField] private int jumpIteration = 0;
  [SerializeField] private int jumpValueIteration = 60;
  

  private float horizontalInput;

  private Vector2 moveVector;
  private Rigidbody2D rb;

  [SerializeField] private Animator anim;

  private bool faceRight = true;
  
  [SerializeField] private bool onGround;
  [SerializeField] private Transform groundCheck;
  [SerializeField] private float checkRadius = 0.5f;
  [SerializeField] private LayerMask Ground;
  

  private void Awake()
  {
    rb = GetComponent<Rigidbody2D>();
    anim = GetComponent<Animator>();
    groundCheck = transform.Find("groundCheck");
  }

  private void Update()
  {
    Walk();
    Jump();
    SetDirection();
    CheckGround();
  }

  private void FixedUpdate()
  {
    
  }

  private void Walk()
  {
    moveVector.x = Input.GetAxis("Horizontal");
    rb.velocity = new Vector2(moveVector.x * walkSpeed, rb.velocity.y);
    
    anim.SetFloat("moveX",Math.Abs(moveVector.x));
  }

  private void Jump()
  {
    if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
    {
      if (onGround) { jumpControl = true; }
    }
    else { jumpControl = false; }

    if (jumpControl) {
      if (jumpIteration++ < jumpValueIteration)
      {
        rb.AddForce(Vector2.up * jumpForce / jumpIteration);
      }
    }
    else {jumpIteration = 0;}
  }

  private void Sit()
  {
    
  }

  private void SetDirection()
  {
    if ((moveVector.x > 0 && !faceRight) || (moveVector.x < 0 && faceRight))
    {
      transform.localScale *= new Vector2(-1, 1);
      faceRight = !faceRight;
    }
  }

  void CheckGround()
  {
    onGround = Physics2D.OverlapCircle(groundCheck.position, checkRadius,Ground);
    anim.SetBool("onGround",onGround);
  }
}
