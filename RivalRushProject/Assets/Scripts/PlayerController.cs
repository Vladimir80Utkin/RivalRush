using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  [SerializeField] private float walkSpeed = 2f;
  [SerializeField] private float runSpeed;
  [SerializeField] private float jumpForce;

  private float horizontalInput;

  private Vector2 moveVector;
  private Rigidbody2D rb;

  private void Awake()
  {
    rb = GetComponent<Rigidbody2D>();
  }

  private void Update()
  {
    Walk();
    Jump();
    SetDirection();
  }

  private void FixedUpdate()
  {
    
  }

  private void Walk()
  {
    moveVector.x = Input.GetAxis("Horizontal");
    rb.velocity = new Vector2(moveVector.x * walkSpeed, rb.velocity.y);
  }

  private void Jump()
  {
    
  }

  private void Sit()
  {
    
  }

  private void SetDirection()
  {
    
  }
}
