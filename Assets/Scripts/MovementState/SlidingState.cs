using UnityEngine;

public class SlidingState : IMovementState
{
    private readonly IMovementStrategy movement = new SlidingMovementStrategy();

    public void Enter(PlayerMovementContext ctx) => movement.ApplyDrag(ctx);
    public void Exit(PlayerMovementContext ctx) { }
    public void Update(PlayerMovementContext ctx) { }

    public void FixedUpdate(PlayerMovementContext ctx)
    {
        ApplySlideForces(ctx);
        movement.Move(ctx);
    }

    //takes part of gravity dragging down along slope and multiplies it by slideDownSpeedMultiplier
    private void ApplySlideForces(PlayerMovementContext ctx)
    {
        Rigidbody rb = ctx.rb;
        Vector3 slopeNormal = ctx.GetSlopeNormal();
        float slopeAngle = Vector3.Angle(slopeNormal, Vector3.up);

        if (slopeAngle <= 0f) return;

        Vector3 gravityAlongSlope = Vector3.ProjectOnPlane(Physics.gravity, slopeNormal);
        rb.AddForce(gravityAlongSlope * ctx.slideDownSpeedMultiplier, ForceMode.Acceleration);

        // if sliding up slope apply an extra slopeUpBrakingMultiplier to slow down faster
        if (rb.linearVelocity.y > 0f)
            rb.AddForce(-gravityAlongSlope * ctx.slopeUpBrakingMultiplier, ForceMode.Acceleration);

        //assure velocity is parallel to plane
        Vector3 slopeVel = Vector3.ProjectOnPlane(rb.linearVelocity, slopeNormal);
        rb.linearVelocity = new Vector3(slopeVel.x, slopeVel.y, slopeVel.z);
    }

    public IMovementState GetNextState(PlayerMovementContext ctx)
    {
        if (ctx.jumpQueued) return new AirborneState();
        if (!ctx.slideHeld) return new GroundedState();
        if (!ctx.grounded) return new AirborneState();
        return this;
    }
}