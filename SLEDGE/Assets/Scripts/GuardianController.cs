﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianController : MonoBehaviour
{
    private float walk = 0.0f;
    [SerializeField] private float walkSpeed = 5.0f;
    [Range(0, 0.5f)] [SerializeField] private float walkSmooth = 0.05f;
    
    private bool jump = false;
    [SerializeField] private float jumpForce = 100.0f;
    [SerializeField] private float jumpFallMultiplier = 2.0f;

    private bool isGrounded = false;
    [SerializeField] private Transform groundCheck;
    const float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundCheckMask;

    private bool facingRight = true;

    private Rigidbody _rigidbody;
    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        walk = Input.GetAxisRaw("Horizontal") * walkSpeed;

        jump = Input.GetButton("Jump");
    }

    private void FixedUpdate()
    {
        CheckGround();
        Walk();
        Jump();

        if((walk > 0.0f && !facingRight) || (walk < 0.0f && facingRight)) {
            Flip();
        }
    }

    private void CheckGround() {
        isGrounded = false;

        Collider []
        hitColliders = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, groundCheckMask);
        if(hitColliders.Length > 0) {
            isGrounded = true;
        }
    }

    private void Walk()
    {
        Vector3 targetVelocity = new Vector3(walk, _rigidbody.velocity.y, _rigidbody.velocity.z);
        _rigidbody.velocity = Vector3.SmoothDamp(_rigidbody.velocity, targetVelocity, ref velocity, walkSmooth);
    }

    private void Jump()
    {
        // Initial force.
        if(jump && isGrounded) {
            isGrounded = false;
            _rigidbody.AddForce(new Vector3(0.0f, jumpForce, 0.0f));
        }

        // Falling multiplier
        if((!jump && !isGrounded) || velocity.y < 0.0f) {
            _rigidbody.velocity += Vector3.up * Physics.gravity.y * (jumpFallMultiplier - 1) * Time.deltaTime;
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }
}
