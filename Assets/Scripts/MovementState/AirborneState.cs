using UnityEngine;

public class AirborneState : IMovementState
{
    private readonly IMovementStrategy movement = new AirborneMovementStrategy();
    private IMovementMediator mediator;

    // upon enter change drag
    public void Enter(PlayerMovementContext ctx, IMovementMediator mediator)
    {
        this.mediator = mediator;
        movement.ApplyDrag(ctx);
    }

    public void Exit(PlayerMovementContext ctx) { }

    public void Update(PlayerMovementContext ctx)
    {
        if (ctx.grounded)
            mediator.RequestTransition(new GroundedState());
    }

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
}