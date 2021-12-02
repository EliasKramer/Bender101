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
    private Collider2D internalCollider;
    private Collision2D[] allCollisions = new Collision2D[4];

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0, 0);
        internalCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    public enum CollisionSide
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3
    };
    public bool isCollidingOnSide(CollisionSide side)
    {
        return allCollisions[(int)side] != null;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        String name = "";
        String currTag = collision.gameObject.tag;
        if (currTag == "Obstacle")
        {
            Vector2 sizeOfCollidedITem = collision.gameObject.GetComponent<Collider2D>().bounds.size;
            Vector2 sizeOfInternalCollider = internalCollider.bounds.size;
            float yVal = transform.position.y - collision.transform.position.y;
            float ydiff = Math.Abs(yVal) - Math.Abs((sizeOfInternalCollider.y / 2 + sizeOfCollidedITem.y / 2));
            float xdiff = Math.Abs((transform.position.x - collision.transform.position.x)) - Math.Abs((sizeOfInternalCollider.x / 2 + sizeOfCollidedITem.x / 2));
            float allowedDiffY = 0.02f;
            float allowedDiffX = 0.05f;

            if (Math.Abs(ydiff) <= allowedDiffY)
            {
                if (yVal < 0)
                {
                    allCollisions[(int)CollisionSide.Up] = collision;
                    name += "overPlayer|";
                }
                else
                {
                    allCollisions[(int)CollisionSide.Down] = collision;
                    name += "underPlayer|";
                    _isOnGround = true;
                }
            }
            else if (Math.Abs(xdiff) <= allowedDiffX)
            {
                if (xdiff < 0)
                {
                    allCollisions[(int)CollisionSide.Right] = collision;
                    name += "rightPlayer|";
                }
                else
                {
                    allCollisions[(int)CollisionSide.Left] = collision;
                    name += "leftPlayer|";
                }
            }
            /*if (dir.x < 0)
            {
                name += "left|";
            }
            if (dir.x > 0)
            {
                name += "right|";
            }
            if (dir.y < 0)
            {
                name += "under|";
            }
            if (dir.y > 0)
            {
                name += "top|";
            }*/
            //Debug.Log($"directionvec x:{dir.x}y:{dir.y}|othersize: {sizeOfCollidedITem}| thissize: {sizeOfInternalCollider} |colision{collision.gameObject.name}");
            //Debug.Log($"thispos x:{transform.position.x} y:{transform.position.y}|otherpos: x:{collision.transform.position.x} y:{collision.transform.position.y}|diff: x:{transform.position.x-collision.transform.position.x}|y:{transform.position.y - collision.transform.position.y}");
            //Debug.Log($"abstand x:{sizeOfInternalCollider.x/2 + sizeOfCollidedITem.x/2}|y:{sizeOfInternalCollider.y/2 + sizeOfCollidedITem.y/2}");

            Debug.Log($"y this{transform.position.y - collision.transform.position.y}|other{sizeOfInternalCollider.y / 2 + sizeOfCollidedITem.y / 2}|diff: {ydiff}");
            Debug.Log($"x this{transform.position.x - collision.transform.position.x}|other{sizeOfInternalCollider.x / 2 + sizeOfCollidedITem.x / 2}|diff: {xdiff}");

            //{transform.position.y - collision.transform.position.y} ~ {sizeOfInternalCollider.y/2 + sizeOfCollidedITem.y/2} wenn oben oder unten
            //{sizeOfInternalCollider.x/2 + sizeOfCollidedITem.x/2} ~ {transform.position.x-collision.transform.position.x}
        }
        Debug.Log(name);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        for (int i = 0; i < allCollisions.Length; i++)
        {
            if (allCollisions[i] == collision)
            {
                allCollisions[i] = null;
            }
        }
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