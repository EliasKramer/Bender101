using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerScript : MonoBehaviour
{
    public Rigidbody2D rb;
    private float _speed = 300f;
    private float _speedInAirMultp = 0.75f;
    private float _jumpForce = 6.5f;
    private float _maxJumps = 2;
    private float _jumpsLeft;
    public bool _isCollidingWithHead = false;
    private Direction _inputDirection = Direction.Right;
    private float _inputDirectionFloat = 1; //is between -1 and 1: 1 is right -1 left
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
        //PrintColliders();
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
        if ((isCollidingOnSide(Const.CollisionSide.Left) || isCollidingOnSide(Const.CollisionSide.Right)) && rb.velocity.y < _slidingSpeed)
        {

            rb.velocity = new Vector2(rb.velocity.x, _slidingSpeed);

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