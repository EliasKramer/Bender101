using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float slopeCheckDistance;
    [SerializeField]
    private float maxSlopeAngle;
    [SerializeField]
    private LayerMask whatIsGround;
    [SerializeField]
    private PhysicsMaterial2D noFriction;
    [SerializeField]
    private PhysicsMaterial2D fullFriction;

    private float xInput;
    private float slopeDownAngle;
    private float slopeSideAngle;
    private float lastSlopeAngle;
    private float groundCheckRadius;

    private int facingDirection = 1;

    private bool isGrounded;
    private bool isOnSlope;
    private bool isJumping;
    private bool canWalkOnSlope;
    private bool canJump;

    private Vector2 bottomColliderPos;
    private Vector2 newVelocity;
    private Vector2 newForce;
    private Vector2 positionForGroundCheckCollider;
    private Vector2 capsuleColliderSize;

    private Vector2 slopeNormalPerp;

    private Rigidbody2D rb;
    private CapsuleCollider2D cc;

    private Vector2 inputVector;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        inputVector = new Vector2();
        cc = GetComponent<CapsuleCollider2D>();
        groundCheckRadius = cc.bounds.size.x/2;
        capsuleColliderSize = cc.size;
    }

    private void Update()
    {
        CheckInput();
        ApplyMovement();
    }

    private void FixedUpdate()
    {
        Vector2 tempThisPos = cc.bounds.center;
        tempThisPos.y -= cc.bounds.size.y / 2;
        bottomColliderPos = tempThisPos;
        CheckGround();
        SlopeCheck();
    }

    private void CheckInput()
    {

        if (inputVector.x > 0 && facingDirection == -1)
        {
            Flip();
        }
        else if (inputVector.x < 0 && facingDirection == 1)
        {
            Flip();
        }

        

    }
    private void CheckGround()
    {
        //tempThisPos.y += groundCheckRadius;
        positionForGroundCheckCollider = bottomColliderPos;
        positionForGroundCheckCollider.y += groundCheckRadius;
        isGrounded = Physics2D.OverlapCircle(positionForGroundCheckCollider, groundCheckRadius, whatIsGround);

        if (rb.velocity.y <= 0.0f)
        {
            isJumping = false;
        }

        if (isGrounded && !isJumping && slopeDownAngle <= maxSlopeAngle)
        {
            canJump = true;
        }

    }

    private void SlopeCheck()
    {
        //Vector2 checkPos = transform.position - (Vector3)(new Vector2(0.0f, capsuleColliderSize.y / 2));

        SlopeCheckHorizontal();
        SlopeCheckVertical();
    }

    private void SlopeCheckHorizontal()
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(bottomColliderPos, transform.right, slopeCheckDistance, whatIsGround);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(bottomColliderPos, -transform.right, slopeCheckDistance, whatIsGround);

        if (slopeHitFront)
        {
            isOnSlope = true;

            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);

        }
        else if (slopeHitBack)
        {
            isOnSlope = true;

            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            slopeSideAngle = 0.0f;
            isOnSlope = false;
        }

    }

    private void SlopeCheckVertical()
    {
        RaycastHit2D hit = Physics2D.Raycast(bottomColliderPos, Vector2.down, slopeCheckDistance, whatIsGround);
        Debug.DrawLine(bottomColliderPos, bottomColliderPos + (Vector2.down * slopeCheckDistance), Color.black, 1);
        if (hit)
        {

            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;

            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeDownAngle != lastSlopeAngle)
            {
                isOnSlope = true;
            }

            lastSlopeAngle = slopeDownAngle;

            Debug.DrawRay(hit.point, slopeNormalPerp, Color.blue, 2);
            Debug.DrawRay(hit.point, hit.normal, Color.green, 2);

        }

        if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
        {
            canWalkOnSlope = false;
        }
        else
        {
            canWalkOnSlope = true;
        }

        if (isOnSlope && canWalkOnSlope && xInput == 0.0f)
        {
            rb.sharedMaterial = fullFriction;
        }
        else
        {
            rb.sharedMaterial = noFriction;
        }
    }

    private void Jump()
    {
        if (canJump)
        {
            canJump = false;
            isJumping = true;
            newVelocity.Set(0.0f, 0.0f);
            rb.velocity = newVelocity;
            newForce.Set(0.0f, jumpForce);
            rb.AddForce(newForce, ForceMode2D.Impulse);
        }
    }

    private void ApplyMovement()
    {
        int xVector = inputVector.x > 0 ? 1 : inputVector.x < 0 ? -1 : 0;

        if (isGrounded && !isOnSlope && !isJumping) //if not on slope
        {
            Debug.Log("This one");
            newVelocity.Set(movementSpeed * xVector, rb.velocity.y);
            rb.velocity = newVelocity;
        }
        else if (isGrounded && isOnSlope && canWalkOnSlope && !isJumping) //If on slope
        {
            newVelocity.Set(movementSpeed * slopeNormalPerp.x * -xVector, movementSpeed * slopeNormalPerp.y * -xVector);
            rb.velocity = newVelocity;
        }
        else if (!isGrounded) //If in air
        {
            newVelocity.Set(movementSpeed * xVector, rb.velocity.y);
            rb.velocity = newVelocity;
        }
        if (inputVector.y > 0)
        {
            Jump();
        }

    }

    private void Flip()
    {
<<<<<<< HEAD:Assets/player/playerScript.cs
        
=======
        //Debug.Log($"inputthrough new system:{context.ReadValue<Vector2>()}");
        _inputVector = context.ReadValue<Vector2>();
        //Debug.Log($"input vec: {_inputVector}");
>>>>>>> 7f8d121af6825675767b6ca124fcfca443e87284:Assets/player/PlayerMovementScript.cs
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(positionForGroundCheckCollider, groundCheckRadius);
    }

    public void ReadMovementInput(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();
    }
}