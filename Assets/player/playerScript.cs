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
    private Collider2D internalCollider;
    private Collision2D[] allCollisions = new Collision2D[4];
    private System.DateTime _lastTimePressed;
    private float _minDelayBetweenJumpsInMs = 300;
    private Vector2 _inputVector;
    void Start()    
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0, 0);
        internalCollider = GetComponent<BoxCollider2D>();
        _jumpsLeft = _maxJumps;
        _lastTimePressed = System.DateTime.UtcNow;
        _inputVector = new Vector2(0, 0);
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
        }
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

        if(_inputDirection == Direction.Right && isCollidingOnSide(CollisionSide.Right))
        {
            movementX = 0;
        }
        if (_inputDirection == Direction.Left && isCollidingOnSide(CollisionSide.Left))
        {
            movementX = 0;
        }
        if ((isCollidingOnSide(CollisionSide.Left)||isCollidingOnSide(CollisionSide.Right))&&rb.velocity.y < 0)
        {
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