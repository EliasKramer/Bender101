using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerScript : MonoBehaviour
{
    public Rigidbody2D rb;
    private float _speed = 222.0f;
    private float _speedInAirMultp = 0.75f;
    private float _jumpForce = 350f;
    public float _sideJumpForce = 20f;
    public float _sideJumpYForceMult = 0.2f;
    private bool _isOnGround = true;
    public bool _isCollidingWithHead = false;
    private Direction _inputDirection = Direction.Right;
    private float _inputDirectionFloat = 1; //is between -1 and 1: 1 is right -1 left
    public float _visualDirectionFloat = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    private void collHelper(Collision2D collision)
    {
        String currTag = collision.gameObject.tag;
        if (currTag == "Obstacle")
        {
            _isOnGround = true;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        collHelper(collision);
    }
    void Move()
    {
        //Debug.Log($"l:{_touchingWallLeft}|r:{_touchingWallRight}|o:{_isCollidingWithHead}|u:{_isOnGround}");
        GetDirection();
        MoveAndJump();
    }

    private void GetDirection()
    {
        _inputDirectionFloat = Input.GetAxis("Horizontal");

        if (_inputDirectionFloat < 0)
        {
            _inputDirection = Direction.Left;
        }
        else if (_inputDirectionFloat > 0)
        {
            _inputDirection = Direction.Right;
        }

    }
    private void MoveAndJump()
    {
        float movementX = 0;
        movementX = _inputDirectionFloat * _speed * Time.deltaTime;
        if (!_isOnGround)
        {
            movementX *= _speedInAirMultp;
        }
        rb.velocity = new Vector2(movementX, rb.velocity.y);
        if (Input.GetButton("Jump") && _isOnGround)
        {
            rb.AddForce(new Vector2(0, _jumpForce));
            _isOnGround = false;
        }
    }
    enum Direction
    {
        Right,
        Left
    };
}