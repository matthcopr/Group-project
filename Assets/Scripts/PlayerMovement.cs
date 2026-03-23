using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //in the future we will implement the interface pattern in order to better organize the code, because as you can see much of the code could've been seperate functions, but were instead placed in other functions
    //bloating the overall amount of code present
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    private Vector3 wallNormal;

    public float jumpForce;
    public float jumpCooldown;
    public float wallJumpMultiplier;
    public float airMultiplier;
    public float maxWallSlideSpeed;
    public float turnSpeed;
    public int maxJumps = 2;
    private int jumpsRemaining;
    private bool readyToJump;
    private bool jumpQueued = false;
    private bool wallJumpQueued = false;

    [Header("Coyote Time")]
    public float coyoteTime;
    private float groundCoyoteTimer = 0f;
    private float wallCoyoteTimer = 0f;
    private Vector3 coyoteWallNormal;

    [Header("Input Buffering")]
    public float jumpBufferTime;
    private float jumpBufferTimer = 0f;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode slideKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public float playerRadius;
    public LayerMask whatIsGround;
    private bool grounded;

    private bool touchingWall;

    public Transform orientation;

    private float horizontalInput;
    private float verticalInput;

    [Header("Sliding")]
    public float slideDrag;
    public float slideMultiplier;
    public bool sliding;
    private bool slideQueued = false;
    public float slopeUpBrakingMultiplier;
    public float slideDownSpeedMultiplier;

    private float originalColliderHeight;
    private float originalColliderCenterY;

    [Header("Slide Collider")]
    public float slidingColliderHeight = 1f;
    public float colliderLerpSpeed = 10f;

    [Header("Clamber")]
    public float clamberHeight;
    public float clamberReach;
    public float clamberSpeed;

    private bool clambering;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        touchingWall = false;
        sliding = false;
        maxJumps--;
        jumpsRemaining = maxJumps;
    }

    private void Update()
    {
        grounded = Physics.CheckSphere(transform.position, 0.2f, whatIsGround);

        Vector3 wallCheckPos = transform.position + new Vector3(0f, 0.5f * playerHeight, 0f);
        Collider[] wallHits = Physics.OverlapSphere(wallCheckPos, playerRadius + 0.1f, whatIsGround);
        touchingWall = wallHits.Length > 0;

        if (touchingWall)
        {
            Vector3 closestPoint = wallHits[0].ClosestPoint(wallCheckPos);
            wallNormal = (wallCheckPos - closestPoint).normalized;
        }

        if (grounded)
        {
            groundCoyoteTimer = coyoteTime;
            jumpsRemaining = maxJumps;
        }
        else
        {
            groundCoyoteTimer -= Time.deltaTime;
        }

        if (touchingWall)
        {
            wallCoyoteTimer = coyoteTime;
            coyoteWallNormal = wallNormal;
        }
        else
        {
            wallCoyoteTimer -= Time.deltaTime;
        }

        MyInput();
        SpeedControl();

        if (grounded && !sliding)
            rb.linearDamping = groundDrag;
        else if(sliding)
        {
            rb.linearDamping = slideDrag;
        }
        else
            rb.linearDamping = 0;

        float targetHeight = sliding ? slidingColliderHeight : originalColliderHeight;
        float targetCenterY = sliding ? originalColliderCenterY - (originalColliderHeight - slidingColliderHeight) / 2f : originalColliderCenterY;
    }

    private void FixedUpdate()
    {

        if(grounded)
        {
            Vector3 slopeNormal = GetSlopeNormal();
            float slopeAngle = Vector3.Angle(slopeNormal, Vector3.up);

            if(slopeAngle > 0f && slopeAngle < 50f)
            {
                Vector3 gravityAlongSlope = Vector3.ProjectOnPlane(Physics.gravity, slopeNormal);
                rb.AddForce(-gravityAlongSlope, ForceMode.Acceleration);
            }
        }

        if(touchingWall && !grounded && rb.linearVelocity.y < -maxWallSlideSpeed)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -maxWallSlideSpeed, rb.linearVelocity.z);
        }

        if (jumpQueued)
        {
            Jump();
            jumpQueued = false;
        }
        else if (wallJumpQueued)
        {
            WallJump();
            wallJumpQueued = false;
        }

        if (slideQueued)
        {
            sliding = true;
            slideQueued = false;
        }

        if (sliding && grounded)
        {
            Vector3 slopeNormal = GetSlopeNormal();
            float slopeAngle = Vector3.Angle(slopeNormal, Vector3.up);

            if (slopeAngle > 0f)
            {
                Vector3 gravityAlongSlope = Vector3.ProjectOnPlane(Physics.gravity, slopeNormal);
                rb.AddForce(gravityAlongSlope * slideDownSpeedMultiplier, ForceMode.Acceleration);

                if (rb.linearVelocity.y > 0f)
                    rb.AddForce(-gravityAlongSlope * slopeUpBrakingMultiplier, ForceMode.Acceleration);


                Vector3 slopeVel = Vector3.ProjectOnPlane(rb.linearVelocity, slopeNormal);
                rb.linearVelocity = new Vector3(slopeVel.x, slopeVel.y, slopeVel.z);
            }
        }

        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        bool canGroundJump = groundCoyoteTimer > 0f;
        bool canWallJump = wallCoyoteTimer > 0f;

        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if(Input.GetKeyDown(slideKey) && flatVel.magnitude > 0f && grounded)
        {
            slideQueued = true;
            sliding = true;
        }

        if(Input.GetKeyUp(slideKey))
        {
            sliding = false;
        }

        if(Input.GetKeyDown(jumpKey))
        {
            jumpBufferTimer = jumpBufferTime;
        }

        jumpBufferTimer -= Time.deltaTime;

        //we will implement the strategy pattern for jumping and any future behaviors that can have multiple different actions occur with the press of the same button depending on the situation in order to
        //better organize the code and to reduce the size of myInput
        if(jumpBufferTimer > 0f && readyToJump)
        {
            if(canWallJump && !grounded)
            {
                wallJumpQueued = true;
                wallCoyoteTimer = 0f;
            }
            else if(jumpsRemaining > 0)
            {
                jumpQueued = true;
                jumpsRemaining--;
                groundCoyoteTimer = 0f;
            }
            else
            {
                return;
            }

                readyToJump = false;
            jumpBufferTimer = 0f;
            Invoke(nameof(ResetJump), jumpCooldown);

        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if(sliding)
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            float currentSpeed = flatVel.magnitude;
            
            if(currentSpeed < moveSpeed)
            {
                rb.AddForce(moveDirection.normalized * slideMultiplier * moveSpeed * 10f, ForceMode.Force);
            }

            if(moveDirection != Vector3.zero && currentSpeed > 0f)
            {
                Vector3 targetVel = moveDirection.normalized * currentSpeed;
                Vector3 steeredVel = Vector3.RotateTowards(flatVel, targetVel, turnSpeed * Time.fixedDeltaTime, 0f);
                rb.linearVelocity = new Vector3(steeredVel.x, rb.linearVelocity.y, steeredVel.z);
            }
        }
        else if (grounded)
        {
            Vector3 slopeNormal = GetSlopeNormal();
            Vector3 slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeNormal).normalized;
            rb.AddForce(slopeMoveDirection * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            float currentSpeed = flatVel.magnitude;

            if (currentSpeed < moveSpeed)
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            }

            if (moveDirection != Vector3.zero && currentSpeed > 0f)
            {
                Vector3 targetVel = moveDirection.normalized * currentSpeed;
                Vector3 steeredVel = Vector3.RotateTowards(flatVel, targetVel, turnSpeed * Time.fixedDeltaTime, 0f);
                rb.linearVelocity = new Vector3(steeredVel.x, rb.linearVelocity.y, steeredVel.z);
            }
        }
    }

    private void SpeedControl()
    {
        if (!grounded || sliding) return;

        Vector3 slopeNormal = GetSlopeNormal();
        Vector3 slopeVel = Vector3.ProjectOnPlane(rb.linearVelocity, slopeNormal);

        if (slopeVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = slopeVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void WallJump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        rb.AddForce(coyoteWallNormal * jumpForce * wallJumpMultiplier, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private Vector3 GetSlopeNormal()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.3f, whatIsGround))
            return hit.normal;
        return Vector3.up;
    }
}