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
        Debug.Log("collision exit" + col.gameObject);
        //currentCollisions.Remove(col);
    }
    private void collHelper(Collision2D collision)
    {
        String output = "";
        //BottomObstacleTag
        //TopObstacleTag
        //LeftObstacleTag
        //RightObstacleTag
        switch (collision.gameObject.tag)
        {
            case "BottomObstacleTag":
                _isCollidingWithHead = true;
                output += "top, ";
                break;
            case "TopObstacleTag":
                _isOnGround = true;
                output += "bottom, ";
                break;
            case "LeftObstacleTag":
                output += "right, ";
                _touchingWallRight = true;
                break;
            case "RightObstacleTag":
                output += "left, ";
                _touchingWallLeft = true;
                break;
        }
        Debug.Log(output + collision.gameObject.name);
        Debug.Log($"nach coll l:{_touchingWallLeft}|r:{_touchingWallRight}|o:{_isCollidingWithHead}|u:{_isOnGround}");
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        collHelper(collision);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        collHelper(collision);

    }
    private void CheckWhereCollisionsAre()
    {

    }
    void Move()
    {
        GetDirection();
        MoveHorizontal();
        SlideDown();
        Debug.Log($"nach move l:{_touchingWallLeft}|r:{_touchingWallRight}|o:{_isCollidingWithHead}|u:{_isOnGround}");

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
    }
    enum Direction
    {
        Right,
        Left
    };
}