using Codice.CM.Common.Matcher;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    // public modifiable constants
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float wallJumpMultiplier;
    public float airMultiplier;
    public float maxWallSlideSpeed;
    public float turnSpeed;
    public int maxJumps = 2;
    public float speedDeceleration;

    [Header("Coyote Time")]
    public float coyoteTimer;

    [Header("Input Buffering")]
    public float jumpBufferTime;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode slideKey = KeyCode.LeftControl;
    public KeyCode dashKey = KeyCode.LeftShift;

    [Header("Ground / Wall Check")]
    public float playerHeight;
    public float playerRadius;
    public LayerMask whatIsGround;

    [Header("Sliding")]
    public float slideDrag;
    public float slideMultiplier;
    public float slopeUpBrakingMultiplier;
    public float slideDownSpeedMultiplier;

    [Header("Dashing")]
    public int dashes;
    public float dashForce;
    public float dashCooldown;
    public float dashDuration;

    public event System.Action OnSlideStart;
    public event System.Action OnSlideStop;

    public Transform orientation;
    public bool sliding => currentState is SlidingState;

    // private non modifiable variables
    private Rigidbody rb;
    private PlayerMovementContext ctx;
    private IMovementState currentState;
    private CoyoteTimeHandler coyoteTime;
    private IJumpStrategy groundJump;
    private IJumpStrategy wallJump;

    private readonly List<IMovementCommand> commandQueue = new();
    private JumpCommand jumpCommand;
    private DashCommand dashCommand;
    private float dashCooldownTimer;

    private Vector3 wallNormal;

    // ran once on start of game, sets all the variables and classes used in the future
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        coyoteTime = new CoyoteTimeHandler(coyoteTimer);
        groundJump = new GroundJumpStrategy();
        wallJump = new WallJumpStrategy(coyoteTime);

        jumpCommand = new JumpCommand(groundJump, wallJump);
        dashCommand = new DashCommand();

        ctx = new PlayerMovementContext
        {
            rb = rb,
            orientation = orientation,
            jumpForce = jumpForce,
            wallJumpMultiplier = wallJumpMultiplier,
            moveSpeed = moveSpeed,
            groundDrag = groundDrag,
            slideDrag = slideDrag,
            airMultiplier = airMultiplier,
            slideMultiplier = slideMultiplier,
            turnSpeed = turnSpeed,
            maxWallSlideSpeed = maxWallSlideSpeed,
            slideDownSpeedMultiplier = slideDownSpeedMultiplier,
            slopeUpBrakingMultiplier = slopeUpBrakingMultiplier,
            speedDeceleration = speedDeceleration,
            dashForce = dashForce,
            dashDuration = dashDuration,
            maxJumps = maxJumps,
            whatIsGround = whatIsGround,
            readyToJump = true,
            currentDashes = dashes,
            jumpCooldown = jumpCooldown,
            invokeResetJump = (cooldown) => Invoke(nameof(ResetJump), cooldown),
        };

        TransitionTo(new GroundedState());
    }

    //ran once every frame, used to update any changed variables
    private void Update()
    {
        UpdatePhysicsState();
        coyoteTime.Tick(Time.deltaTime, ctx.grounded, ctx.touchingWall);
        HandleInput();
        ProcessCommandQueue();
        HandleDashCooldown();
        currentState.Update(ctx);

        IMovementState next = currentState.GetNextState(ctx);
        if (next != currentState) TransitionTo(next);

        if (ctx.dashQueued)
        {
            ctx.dashQueued = false;
            ctx.isDashing = true;
            TransitionTo(new DashingState());
        }
    }

    //ran once every 1/60th a second, used to update state which determines all movement behaviors such as jumping and dashing
    private void FixedUpdate()
    {
        currentState.FixedUpdate(ctx);
        ctx.jumpQueued = false;
        ctx.wallJumpQueued = false;
    }


    private void TransitionTo(IMovementState next)
    {
        // ? calls only if not null to protect out of bounds on first use of TransitionTo in start
        bool wasSliding = currentState is SlidingState;
        currentState?.Exit(ctx);
        currentState = next;
        currentState.Enter(ctx);
        bool isSliding = currentState is SlidingState;

        if (!wasSliding && isSliding) OnSlideStart?.Invoke();
        if (wasSliding && !isSliding) OnSlideStop?.Invoke();
    }

    // updates the necessary parts of PlayerMovementContext or just ctx with anything that changed such as
    private void UpdatePhysicsState()
    {
        // whether or not grounded
        ctx.grounded = Physics.CheckSphere(transform.position, 0.2f, whatIsGround);

        // whether or not touching wall determined by an array of Colliders of objects touched 
        Vector3 wallCheckPos = transform.position + Vector3.up * (0.5f * playerHeight);
        Collider[] wallHits = Physics.OverlapSphere(wallCheckPos, playerRadius + 0.01f, whatIsGround);
        ctx.touchingWall = wallHits.Length > 0;

        // take the first object's wallnormal from the closest point
        if (ctx.touchingWall)
        {
            Vector3 closest = wallHits[0].ClosestPoint(wallCheckPos);
            wallNormal = (wallCheckPos - closest).normalized;
            ctx.wallNormal = wallNormal;
        }

        // store wall normal to enure coyote time still works with wall jumps
        if (coyoteTime.canWallJump)
            ctx.coyoteWallNormal = wallNormal;

        if (ctx.grounded)
            ctx.jumpsRemaining = maxJumps - 1;
    }

    // get inputs from player and update variables accordingly
    private void HandleInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        ctx.moveDir = orientation.forward * v + orientation.right * h;
        ctx.slideHeld = Input.GetKey(slideKey);

        if (Input.GetKeyDown(jumpKey))
            QueueCommand(jumpCommand);

        if (Input.GetKeyDown(dashKey))
            QueueCommand(dashCommand);
    }

    // god I wonder what it do
    private void QueueCommand(IMovementCommand command)
    {
        command.timeQueued = Time.time;
        commandQueue.Add(command);
    }

    private void ProcessCommandQueue()
    {
        //remove any command that has been in the queue longer than the buffer time
        commandQueue.RemoveAll(c => Time.time - c.timeQueued > c.bufferTime);

        // runs first command in queue that returns true on CanExecute, where each command is adding to the end of the queue
        IMovementCommand next = commandQueue.FirstOrDefault(c => c.CanExecute(ctx));
        if (next == null) return;

        // since it is ran in update only one command is executed each frame
        next.Execute(ctx);
        commandQueue.Remove(next);
    }

    // give more dashes after cooldown, if at max dashes don't do anything
    private void HandleDashCooldown()
    {
        if (ctx.currentDashes >= dashes) return;

        dashCooldownTimer += Time.deltaTime;
        if (dashCooldownTimer >= dashCooldown)
        {
            ctx.currentDashes++;
            if (ctx.currentDashes < dashes)
            {
                dashCooldownTimer = 0f;
            }
            else
            {
                dashCooldownTimer = dashCooldown;
            }
        }
    }

    private void ResetJump() => ctx.readyToJump = true;
}