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
    private bool _touchingWallLeft = false;
    private bool _touchingWallRight = false;
    private bool _isSlidingOnWall = false;
    private Direction _inputDirection = Direction.Right;
    private float _inputDirectionFloat = 1; //is between -1 and 1: 1 is right -1 left
    public float _visualDirectionFloat = 1;
    private bool _isTouchingWall = false;
    //List<Collision2D> currentCollisions = new List<Collision2D>();

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
    private void FixedUpdate()
    {

    }
    /*
    void OnCollisionEnter2D(Collision2D col)
    {
        //currentCollisions.Add(col);
    }
    */

    private void collHelper(Collision2D collision)
    {
        Debug.Log("lol");
        String currTag = collision.gameObject.tag;
        if (currTag == "Environment")
        {
            _isOnGround = true;
        }
        if (currTag == "Obstacle")
        {
            _isTouchingWall = true;
        }

        /*
        String output = "";
        if (collision.gameObject.tag == "Obstacle")
        {
            Vector2 direction = collision.gameObject.transform.position - this.transform.position;
            //direction.x = (float)Math.Round(direction.x);
            //direction.y = (float)Math.Round(direction.y);
            //direction = direction.normalized;
            Debug.Log($"x{direction.x}y{direction.y}");
            //curr.GetContact(0).normal
            if (direction.x > 0) //wall on left side - touching on right side
            {
                output += "right, ";
                _touchingWallRight = true;
            }
            else if (direction.x < 0) //wall on right side - touching on left side
            {
                output += "left, ";
                _touchingWallLeft = true;
            }
            if (direction.y > 0) //wall collider is activated on bottom - touching on top side
            {
                output += "top, ";
                _isOnGround = true;
                //_isCollidingWithHead = true;
            }
            else if (direction.y < 0) //wall collider is activated on top - touching on bottom side
            {
                output += "bottom, ";
                _isOnGround = true;
            }
        }
        else if (collision.gameObject.tag == "Environment")
        {
            _isOnGround = true;
            output += "ground/bottom, ";
        }
        Debug.Log(output + collision.gameObject.name);*/
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        collHelper(collision);
    }
    void Move()
    {
        Debug.Log($"ground:{_isOnGround},touchesWall:{_isTouchingWall}");
        //Debug.Log($"l:{_touchingWallLeft}|r:{_touchingWallRight}|o:{_isCollidingWithHead}|u:{_isOnGround}");
        GetDirection();
        MoveHorizontal();
        SlideDown();
    }

    private void SlideDown()
    {
        if (_isSlidingOnWall && rb.velocity.y <= 0)
        {
            rb.gravityScale = 0.1f;
        }
        else
        {
            rb.gravityScale = 1f;
        }
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

    private void MoveHorizontal()
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
        /*
        if (_touchingWallLeft && _inputDirection == Direction.Left)
        {
            _isSlidingOnWall = true;
        }
        else if (_touchingWallRight && _inputDirection == Direction.Right)
        {
            _isSlidingOnWall = true;
        }
        else
        {
            _isSlidingOnWall = false;
            movementX = _inputDirectionFloat * _speed * Time.deltaTime;
            if (_isOnGround)
            {
                rb.velocity = new Vector2(movementX, rb.velocity.y);
            }
        }

        if (Input.GetButton("Jump") && !_isCollidingWithHead)
        {
            if (_isSlidingOnWall)
            {
                rb.AddForce(new Vector2(_sideJumpForce * (_inputDirectionFloat * -1), _jumpForce * _sideJumpYForceMult));
                _isSlidingOnWall = false;
            }
            else if (_isOnGround)
            {
                rb.AddForce(new Vector2(0, _jumpForce));
                _isOnGround = false;
            }
        */
    }
    enum Direction
    {
        Right,
        Left
    };
}