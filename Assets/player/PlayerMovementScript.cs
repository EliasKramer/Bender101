using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    /// <summary>
    /// physic simulation 
    /// </summary>
    public Rigidbody2D rb;
    /// <summary>
    /// the speed of the character 
    /// </summary>
    private float _speed = 5f;
    /// <summary>
    /// the amout the speed is multiplied if the character is in the air
    /// </summary>
    private float _speedInAirMultp = 0.75f;
    /// <summary>
    /// the force the player jumps with
    /// </summary>
    private float _jumpForce = 6.5f;
    /// <summary>
    /// the max amount of jumps in the air
    /// </summary>
    private short _maxJumps = 2;
    /// <summary>
    /// the amount of jumps currently left to the player while in air
    /// </summary>
    private short _jumpsLeft;
    /// <summary>
    /// is set to true if something is colliding with the players head
    /// </summary>
    private bool _isCollidingWithHead = false;
    /// <summary>
    /// the direction the player is facing
    /// </summary>
    private Direction _inputDirection = Direction.Right;
    /// <summary>
    /// 
    /// </summary>
    private float _inputDirectionFloat = 1; //is between -1 and 1: 1 is right -1 left
    /// <summary>
    /// array of collisions on all sides
    /// </summary>
    private bool[] allCollisions = new bool[4];
    /// <summary>
    /// is set to the last time something was pressed
    /// </summary>
    private System.DateTime _lastTimePressed;
    /// <summary>
    /// is the mininum of delay 
    /// </summary>
    private float _minDelayBetweenJumpsInMs = 300;
    private Vector2 _inputVector;
    private float _slidingSpeed = -0.5f;


    /// <summary>
    /// gets called when the game is updated
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0, 0);
        _jumpsLeft = _maxJumps;
        _lastTimePressed = System.DateTime.UtcNow;
        _inputVector = new Vector2(0, 0);
    }


    /// <summary>
    ///  update is called to update the current state of the player. It is called once per frame
    /// </summary>
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


    /// <summary>
    /// Checks if the player is colliding with a side
    /// </summary>
    /// <param name="side">the side to be checked</param>
    /// <returns>true if the two objects are colliding</returns>
    public bool isCollidingOnSide(Const.CollisionSide side)
    {
        return allCollisions[(int)side];
    }

    /// <summary>
    /// a method which is called by childcollider to update the current collides
    /// </summary>
    /// <param name="val">if the collision is wanted to be added this should be true else the collision will get removed</param>
    /// <param name="side">the side the collision is on</param>
    public void CollisionUpdateByChildCollider(bool val, Const.CollisionSide side)
    {
        allCollisions[(int)side] = val;
        if (val && side == Const.CollisionSide.Down)
        {
            _jumpsLeft = _maxJumps;
        }
        //Debug.Log($"")
    }

    /// <summary>
    /// a method which allows the player to move
    /// </summary>
    void Move()
    {
        GetDirection();
        MoveAndJump();
    }
    /// <summary>
    /// gets the direction the placer is facing
    /// </summary>
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

    /// <summary>
    /// Does all tge movement thingies
    /// </summary>
    private void MoveAndJump()
    {
        float movementX = 0;
        movementX = _inputDirectionFloat * _speed;
        if (_jumpsLeft < _maxJumps)
        {
            //Debug.Log("airspeedmult active");
            movementX *= _speedInAirMultp;
        }

        if (_inputDirection == Direction.Right && isCollidingOnSide(Const.CollisionSide.Right) && !isCollidingOnSide(Const.CollisionSide.Down))
        {
            movementX = 0;
        }
        if (_inputDirection == Direction.Left && isCollidingOnSide(Const.CollisionSide.Left) &&
            !isCollidingOnSide(Const.CollisionSide.Down))
        {
            movementX = 0;
        }
        if ((isCollidingOnSide(Const.CollisionSide.Left) || isCollidingOnSide(Const.CollisionSide.Right)) && rb.velocity.y < _slidingSpeed)
        {

            rb.velocity = new Vector2(movementX, _slidingSpeed);
        }
        else
        {
            rb.velocity = new Vector2(movementX, rb.velocity.y);
        }
        if ((Mathf.Round(_inputVector.y) == 1) && _jumpsLeft > 0 && HasEnoughDelay())
        {
            //Debug.Log($"beforeJump jl: {_jumpsLeft}| afterjump jl: {_jumpsLeft}");
            rb.velocity = new Vector2(0, _jumpForce);
            _jumpsLeft--;
        }
    }



    public void MoveAction(InputAction.CallbackContext context)
    {
        //Debug.Log($"inputthrough new system:{context.ReadValue<Vector2>()}");
        _inputVector = context.ReadValue<Vector2>();
        //Debug.Log($"input vec: {_inputVector}");
    }


    /// <summary>
    /// Check if enough delay has passed 
    /// </summary>
    /// <returns>True(if enough delay has passed)</returns>
    private bool HasEnoughDelay()
    {
        System.DateTime startTime = System.DateTime.Now;
        if ((startTime - _lastTimePressed).TotalMilliseconds >= _minDelayBetweenJumpsInMs)
        {
            //Debug.Log($"delay = {(startTime - _lastTimePressed).TotalMilliseconds}|jl:{_jumpsLeft}");
            _lastTimePressed = startTime;
            return true;
        }
        return false;
    }

    /// <summary>
    /// an enum for directions
    /// </summary>
    enum Direction
    {
        Right,
        Left
    };
}