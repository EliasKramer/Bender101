using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    private bool[] allCollisions = new bool[4];
    private System.DateTime _lastTimePressed;
    private float _minDelayBetweenJumpsInMs = 300;
    private Vector2 _inputVector;
    private float _slidingSpeed = -0.5f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0, 0);
        _jumpsLeft = _maxJumps;
        _lastTimePressed = System.DateTime.UtcNow;
        _inputVector = new Vector2(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        //Debug.Log($"amout of jump: {_jumpsLeft}");
        PrintColliders();
    }

    private void PrintColliders()
    {
        Debug.Log($"up:{isCollidingOnSide(Const.CollisionSide.Up)}|down:{isCollidingOnSide(Const.CollisionSide.Down)}|left:{isCollidingOnSide(Const.CollisionSide.Left)}|right:{isCollidingOnSide(Const.CollisionSide.Right)}");
    }

    public bool isCollidingOnSide(Const.CollisionSide side)
    {
        return allCollisions[(int)side];
    }
    public void CollisionAddByChildCollider(bool val, Const.CollisionSide side)
    {
        allCollisions[(int)side] = val;
        if (side == Const.CollisionSide.Down)
        {
            _jumpsLeft = _maxJumps;
        }
        //Debug.Log($"")
    }
    public void CollisionRemByChildCollider(bool val, Const.CollisionSide side)
    {
        allCollisions[(int)side] = val;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        /*
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
                }
                else
                {
                    allCollisions[(int)CollisionSide.Down] = collision;
                    _jumpsLeft = _maxJumps;
                }
            }
            else if (Math.Abs(xdiff) <= allowedDiffX)
            {
                if (xdiff < 0)
                {
                    allCollisions[(int)CollisionSide.Right] = collision;
                }
                else
                {
                    allCollisions[(int)CollisionSide.Left] = collision;
                }
            }
        }*/
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        /*
        for (int i = 0; i < allCollisions.Length; i++)
        {
            if (allCollisions[i] == collision)
            {
                if(i == (int)Const.CollisionSide.Down)
                {
                    //_jumpsLeft--;
                }
                allCollisions[i] = null;
            }
        }*/
    }
    void Move()
    {
        //Debug.Log($"l:{_touchingWallLeft}|r:{_touchingWallRight}|o:{_isCollidingWithHead}|u:{_isOnGround}");
        GetDirection();
        MoveAndJump();
    }

    private void GetDirection()
    {
        //Debug.Log($"dirvec: {_inputVector}");
        //Debug.Log($"dirvec: {Mathf.Round(_inputVector.x)}");
        _inputDirectionFloat = Mathf.Round(_inputVector.x);//Input.GetAxis("Horizontal");
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

        if (_inputDirection == Direction.Right && isCollidingOnSide(Const.CollisionSide.Right))
        {
            movementX = 0;
        }
        if (_inputDirection == Direction.Left && isCollidingOnSide(Const.CollisionSide.Left))
        {
            movementX = 0;
        }
        if ((isCollidingOnSide(Const.CollisionSide.Left) || isCollidingOnSide(Const.CollisionSide.Right)) && rb.velocity.y < 0)
        {
            //new option:
            /*if(rb.velocity.y < _slidingSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, _slidingSpeed);
            }*/
            rb.gravityScale = 0.01f;
        }
        else
        {
            rb.gravityScale = 1f;
        }

        rb.velocity = new Vector2(movementX, rb.velocity.y);

        if ((Mathf.Round(_inputVector.y) == 1) && _jumpsLeft > 0 && HasEnoughDelay())
        {
            Debug.Log($"beforeJump jl: {_jumpsLeft}| afterjump jl: {_jumpsLeft}");
            rb.velocity = new Vector2(0, _jumpForce);
            _jumpsLeft--;
        }
    }
    public void MoveAction(InputAction.CallbackContext context)
    {
        //Debug.Log($"inputthrough new system:{context.ReadValue<Vector2>()}");
        _inputVector = context.ReadValue<Vector2>();
    }
    private bool HasEnoughDelay()
    {
        System.DateTime startTime = System.DateTime.Now;
        if ((startTime - _lastTimePressed).TotalMilliseconds >= _minDelayBetweenJumpsInMs)
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