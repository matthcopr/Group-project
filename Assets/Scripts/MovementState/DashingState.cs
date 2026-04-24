using UnityEngine;

public class DashingState : IMovementState
{
    private readonly IMovementStrategy movement = new DashingMovementStrategy();
    private float timer;
    private bool wasUsingGravity;

    public void Enter(PlayerMovementContext ctx)
    {
        timer = 0f;
        Rigidbody rb = ctx.rb;
        // store whether or not was using gravity before and restores it to its original state on exit
        wasUsingGravity = rb.useGravity;
        rb.useGravity = false;
        movement.ApplyDrag(ctx);
        // zero out vertical speed
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // picks direction based on movement keys, otherwise dash in the diretion the player is facing
        Vector3 dashDir;
        if (ctx.moveDir.magnitude > 0)
        {
            dashDir = ctx.moveDir.normalized;
        }
        else
        {
            dashDir = ctx.orientation.forward;
        }
        //dash along slope rather than directly horizontal
        if (ctx.grounded)
            dashDir = Vector3.ProjectOnPlane(dashDir, ctx.GetSlopeNormal()).normalized;

        rb.AddForce(dashDir * ctx.dashForce, ForceMode.Impulse);
    }

    public void Exit(PlayerMovementContext ctx)
    {
        ctx.rb.useGravity = wasUsingGravity;
        ctx.isDashing = false;
    }

    public void Update(PlayerMovementContext ctx) => timer += Time.deltaTime;
    public void FixedUpdate(PlayerMovementContext ctx) { }

    // once dash is over change state
    public IMovementState GetNextState(PlayerMovementContext ctx)
    {
        if (timer >= ctx.dashDuration)
        {
            if (ctx.grounded)
            {
                return new GroundedState();
            }
            else
            {
                return new AirborneState();
            }
        }
        return this;
    }
}