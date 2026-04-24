using UnityEngine;

// essentially a large data structure that holds all the necessary info for every other class to function
public class PlayerMovementContext
{
    public Rigidbody rb;
    public Transform orientation;
    public float jumpForce;
    public float wallJumpMultiplier;
    public Vector3 wallNormal;
    public Vector3 coyoteWallNormal;
    public bool grounded;
    public bool touchingWall;
    public int jumpsRemaining;
    public float moveSpeed;
    public float groundDrag;
    public float slideDrag;
    public float airMultiplier;
    public float slideMultiplier;
    public float turnSpeed;
    public float maxWallSlideSpeed;
    public float slideDownSpeedMultiplier;
    public float slopeUpBrakingMultiplier;
    public float speedDeceleration;
    public float dashForce;
    public float dashDuration;
    public float maxJumps;
    public LayerMask whatIsGround;
    public float jumpCooldown;

    public bool readyToJump;
    public bool dashQueued;
    public bool isDashing;
    public int currentDashes;

    public Vector3 moveDir;
    public bool jumpQueued;
    public bool wallJumpQueued;
    public bool slideHeld;

    public System.Action<float> invokeResetJump;


    public Vector3 GetSlopeNormal()
    {
        if (Physics.Raycast(orientation.position, Vector3.down, out RaycastHit hit, 1.3f, whatIsGround))
            return hit.normal;
        return Vector3.up;
    }
}
