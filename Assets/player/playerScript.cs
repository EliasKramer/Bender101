using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerScript : MonoBehaviour
{
    public Rigidbody2D rb;
    private float _speed = 222.0f;
    private float _speedInAirMultp = 0.5f;
    private float _jumpForce = 150f;
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
        CheckWhereCollisionsAre();
    }
    private void FixedUpdate()
    {
        _touchingWallLeft = false;
        _touchingWallRight = false;
        _isCollidingWithHead = false;
        _isOnGround = false;
    }
    /*
    void OnCollisionEnter2D(Collision2D col)
    {
        //currentCollisions.Add(col);
    }
    */
    void OnCollisionExit2D(Collision2D col)
    {
        Debug.Log("collision exit" + col.gameObject.name);
        //currentCollisions.Remove(col);
    }
    private void collHelper(Collision2D collision)
    {
        String output = "";
        if (collision.gameObject.tag == "Obstacle")
        {
            Vector2 direction = collision.gameObject.transform.position - this.transform.position;
            direction.x = (float)Math.Round(direction.x);
            direction.y = (float)Math.Round(direction.y);
            //direction = direction.normalized;
            Debug.Log($"x{direction.x}y{direction.y}");
            //curr.GetContact(0).normal
            if (direction.x > 0) //wall on left side - touching on right side
            {
                output += "right, ";
                _touchingWallRight = true;
            }
            if (direction.x < 0) //wall on right side - touching on left side
            {
                output += "left, ";
                _touchingWallLeft = true;
            }
            if (direction.y < 0) //wall collider is activated on bottom - touching on top side
            {
                output += "top, ";
                _isOnGround = true;
                //_isCollidingWithHead = true;
            }
            if (direction.y >= 0) //wall collider is activated on top - touching on bottom side
            {
                output += "bottom, ";
                _isOnGround = true;
            }
        }
        if (collision.gameObject.tag == "Environment")
        {
            _isOnGround = true;
            output += "ground/bottom, ";
        }
        Debug.Log(output + collision.gameObject.name);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        collHelper(collision);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        collHelper(collision);

    }
    /*
    private void OnCollisionExit2D(Collision2D other)
    {
        float ySpeed = rb.velocity.y;
        if (other.gameObject.tag == "Obstacle")
        {
            _touchingWallRight = false;
            _touchingWallRight = false;
            _isCollidingWithHead = false;
        }
        else if (other.gameObject.tag == "Environment" && ySpeed > 0)
        {
            _isOnGround = false;
        }
    }
    */
    private void CheckWhereCollisionsAre()
    {
        /*_touchingWallLeft = false;
        _touchingWallRight = false;
        _isCollidingWithHead = false;
        _isOnGround = false;*/
        /*
        foreach (Collision2D col in currentCollisions)
        {
            Debug.Log(col.gameObject.name);
        }
        foreach (Collision2D curr in currentCollisions)
        {
            
            if (curr.gameObject.tag == "Obstacle")
            {
                Vector2 direction = curr.gameObject.transform.position - this.transform.position;
                direction.x = (float)Math.Round(direction.x);
                direction.y = (float)Math.Round(direction.y);
                //direction = direction.normalized;
                Debug.Log($"x{direction.x}y{direction.y}");
                //curr.GetContact(0).normal
                if (direction.x > 0) //wall on left side - touching on right side
                {
                    Debug.Log("player right");
                    _touchingWallRight = true;
                }
                if (direction.x < 0) //wall on right side - touching on left side
                {
                    Debug.Log("player left");
                    _touchingWallLeft = true;
                }
                if (direction.y < 0) //wall collider is activated on bottom - touching on top side
                {
                    Debug.Log("player top");
                    _isOnGround = true;
                    //_isCollidingWithHead = true;
                }
                if (direction.y >= 0) //wall collider is activated on top - touching on bottom side
                {
                    Debug.Log("player bottom");
                    _isOnGround = true;
                }
            }
            if (curr.gameObject.tag == "Environment")
            {
                _isOnGround = true;
            }
        }*/
    }
    void Move()
    {
        //Debug.Log($"l:{_touchingWallLeft}|r:{_touchingWallRight}|o:{_isCollidingWithHead}|u:{_isOnGround}");
        GetDirection();
        MoveHorizontal();
        Jump();
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
        /*if (_touchingWallLeft)
        {
            _visualDirection = Direction.Right;
            _visualDirectionFloat = 1f;
        }
        else if (_touchingWallRight)
        {
            _visualDirection = Direction.Left;
            _visualDirectionFloat = -1f;
        }
        */
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
        }
        /*

        if (!(_touchingWallLeft && _inputDirection == Direction.Left) && !(_touchingWallRight && _inputDirection == Direction.Right))
        {
            movementX = _inputDirectionFloat * _speed * Time.deltaTime;
        }
        if (!_isOnGround && !(_touchingWallLeft || _touchingWallRight))
        {
            movementX *= _speedInAirMultp;
        }
        rb.velocity = new Vector2(movementX, rb.velocity.y);
        */

        /*if (!_isOnGround)
        {
            moventX *= _speedInAirMultp;

            if (_runningOnWall && !_isOnGround)
            {
                rb.gravityScale = 0.1f;
                moventX = 0;
            }
            else
            {
                rb.gravityScale = 1;
            }
            rb.velocity = new Vector3(moventX, rb.velocity.y, 0); // setting speed is bad for jump
                                                                  //transform.Translate(moventX, 0, 0);
            float directionOnX = Input.GetAxis("Horizontal");
            float moventX = directionOnX * _speed * Time.deltaTime;
            if (moventX < 0)
            {
                _direction = Direction.Left;
                Debug.Log("left");
            }
            else if (moventX > 0)
            {
                _direction = Direction.Right;
                Debug.Log("right");

            }
            */


    }
    private void Jump()
    {
        /*
        float xMovementForce = 0;
        float yMovementForce = 0;
        bool shouldntJump = false;
        if (Input.GetButton("Jump"))
        {
            if (_isOnGround && (_touchingWallLeft || _touchingWallRight))
            {
                yMovementForce = _jumpForce * _sideJumpYForceMult;
                _touchingWallLeft = false;
                _touchingWallRight = false;
            }
            else if (!_isOnGround && (_touchingWallLeft || _touchingWallRight))
            {
                xMovementForce = _sideJumpForce * _visualDirectionFloat;
                yMovementForce = _jumpForce * _sideJumpYForceMult;
                _touchingWallLeft = false;
                _touchingWallRight = false;
            }
            else if (_isOnGround && !(_touchingWallLeft || _touchingWallRight))
            {
                yMovementForce = _jumpForce;
            }
            else if (!_isOnGround && !(_touchingWallLeft || _touchingWallRight))
            {
                shouldntJump = true;
            }
            if (!shouldntJump)
            {
                rb.AddForce(new Vector2(xMovementForce, yMovementForce));
                _isOnGround = false;
            }
            else
            {

            }
        }
        /*
       if (Input.GetButton("Jump") && _isOnGround && !_runningOnWall)
       {
           _isOnGround = false;
           rb.AddForce(new Vector2(0, _jumpForce));
       }
Ü*/
    }
    enum Direction
    {
        Right,
        Left
    };
}