using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerScript : MonoBehaviour
{
    public Rigidbody2D rb;
    private float _speed = 222.0f;
    private float _speedInAirMultp = 0.75f;
    private float _jumpForce = 5f;
    public float _sideJumpForce = 20f;
    public float _sideJumpYForceMult = 0.2f;
    private float _maxJumps = 2;
    private float _jumpsLeft;
    public bool _isCollidingWithHead = false;
    private Direction _inputDirection = Direction.Right;
    private float _inputDirectionFloat = 1; //is between -1 and 1: 1 is right -1 left
    public float _visualDirectionFloat = 1;
    private Collider2D internalCollider;
    private Collision2D[] allCollisions = new Collision2D[4];
    private System.DateTime _lastTimePressed;
    private float _minDelayBetweenJumpsInMs = 300;

    void Start()    
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0, 0);
        internalCollider = GetComponent<BoxCollider2D>();
        _jumpsLeft = _maxJumps;
        _lastTimePressed = System.DateTime.UtcNow;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        //Debug.Log($"amout of jump: {_jumpsLeft}");
        //PrintColliders();
    }

    private void PrintColliders()
    {
        Debug.Log($"up:{isCollidingOnSide(CollisionSide.Up)}|down:{isCollidingOnSide(CollisionSide.Down)}|left:{isCollidingOnSide(CollisionSide.Left)}|right:{isCollidingOnSide(CollisionSide.Right)}");
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
                    _jumpsLeft = _maxJumps;
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

            //Debug.Log($"y this{transform.position.y - collision.transform.position.y}|other{sizeOfInternalCollider.y / 2 + sizeOfCollidedITem.y / 2}|diff: {ydiff}");
            //Debug.Log($"x this{transform.position.x - collision.transform.position.x}|other{sizeOfInternalCollider.x / 2 + sizeOfCollidedITem.x / 2}|diff: {xdiff}");

            //{transform.position.y - collision.transform.position.y} ~ {sizeOfInternalCollider.y/2 + sizeOfCollidedITem.y/2} wenn oben oder unten
            //{sizeOfInternalCollider.x/2 + sizeOfCollidedITem.x/2} ~ {transform.position.x-collision.transform.position.x}
        }
        //Debug.Log(name);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        for (int i = 0; i < allCollisions.Length; i++)
        {
            if (allCollisions[i] == collision)
            {
                if(i == (int)CollisionSide.Down)
                {
                    //_jumpsLeft--;
                }
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
        if (_jumpsLeft < _maxJumps)
        {
            movementX *= _speedInAirMultp;
        }

        if(_inputDirection == Direction.Right && isCollidingOnSide(CollisionSide.Right))
        {
            movementX = 0;
        }
        if (_inputDirection == Direction.Left && isCollidingOnSide(CollisionSide.Left))
        {
            movementX = 0;
        }
        //not tested
        if ((isCollidingOnSide(CollisionSide.Left)||isCollidingOnSide(CollisionSide.Right))&&rb.velocity.y < 0)
        {
            rb.gravityScale = 0.01f;
        }
        else
        {
            rb.gravityScale = 1f;
        }

        rb.velocity = new Vector2(movementX, rb.velocity.y);
        if (Input.GetKeyDown("w") && _jumpsLeft > 0 && HasEnoughDelay())
        {
            Debug.Log($"beforeJump jl: {_jumpsLeft}| afterjump jl: {_jumpsLeft}");
            rb.velocity = new Vector2(0, _jumpForce);
            _jumpsLeft--;
        }
    }

    private bool HasEnoughDelay()
    {
        System.DateTime startTime = System.DateTime.Now;
        if((startTime - _lastTimePressed).TotalMilliseconds >= _minDelayBetweenJumpsInMs)
        {
            Debug.Log($"delay = {(startTime - _lastTimePressed).TotalMilliseconds}|jl:{_jumpsLeft}");
            _lastTimePressed = startTime;
            return true;
        }
        return false;
    }

    enum Direction
    {
        Right,
        Left
    };
}