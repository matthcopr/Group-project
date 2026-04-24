using UnityEngine;

public class AirborneState : IMovementState
{
    private readonly IMovementStrategy movement = new AirborneMovementStrategy();

    // upon enter change drag
    public void Enter(PlayerMovementContext ctx) => movement.ApplyDrag(ctx);
    public void Exit(PlayerMovementContext ctx) { }
    public void Update(PlayerMovementContext ctx) { }

    public void FixedUpdate(PlayerMovementContext ctx)
    {
        ApplyWallSlide(ctx);
        movement.Move(ctx);
    }

    // when touching wall while in the air you cannot fall faster than a specified maxWallSlideSpeed
    private void ApplyWallSlide(PlayerMovementContext ctx)
    {
        Rigidbody rb = ctx.rb;
        if (ctx.touchingWall && rb.linearVelocity.y < -ctx.maxWallSlideSpeed)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -ctx.maxWallSlideSpeed, rb.linearVelocity.z);
    }

    // change state upon being grouneded
    public IMovementState GetNextState(PlayerMovementContext ctx)
    {
        if (ctx.grounded) return new GroundedState();
        return this;
    }
}