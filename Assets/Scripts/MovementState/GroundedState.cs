using UnityEngine;

public class GroundedState : IMovementState
{
    private readonly IMovementStrategy movement = new GroundedMovementStrategy();

    // reset jumps upon being grounded
    public void Enter(PlayerMovementContext ctx)
    {
        movement.ApplyDrag(ctx);
        ctx.jumpsRemaining = (int)ctx.maxJumps - 1;
    }

    public void Exit(PlayerMovementContext ctx) { }
    public void Update(PlayerMovementContext ctx) { }

    //when grounded and not sliding you cannot go above movespeed
    public void FixedUpdate(PlayerMovementContext ctx)
    {
        ApplySlopeGravity(ctx);
        movement.Move(ctx);
        movement.SpeedControl(ctx);
    }

    //calculates gravity force that would pull you down slope and counteracts it
    private void ApplySlopeGravity(PlayerMovementContext ctx)
    {
        if (ctx.rb.linearVelocity.y > 0f) return;

        Vector3 slopeNormal = ctx.GetSlopeNormal();
        float slopeAngle = Vector3.Angle(slopeNormal, Vector3.up);

        if (slopeAngle > 0f && slopeAngle < 50f)
        {
            Vector3 gravityAlongSlope = Vector3.ProjectOnPlane(Physics.gravity, slopeNormal);
            ctx.rb.AddForce(-gravityAlongSlope, ForceMode.Acceleration);
        }
    }

    public IMovementState GetNextState(PlayerMovementContext ctx)
    {
        if (ctx.jumpQueued || ctx.wallJumpQueued) return new AirborneState();
        if (ctx.slideHeld) return new SlidingState();
        if (!ctx.grounded) return new AirborneState();
        return this;
    }
}