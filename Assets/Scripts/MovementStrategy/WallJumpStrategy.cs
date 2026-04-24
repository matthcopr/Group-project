using UnityEngine;

public class WallJumpStrategy : IJumpStrategy
{
    private readonly CoyoteTimeHandler coyoteTime;

    public WallJumpStrategy(CoyoteTimeHandler coyoteTimeGiven)
    {
        coyoteTime = coyoteTimeGiven;
    }

    public bool CanExecute(PlayerMovementContext ctx)
    {
        return coyoteTime.canWallJump && !ctx.grounded;
    }

    // add force along wall normal to bounce away from wall on wall jump
    public void Execute(PlayerMovementContext ctx)
    {
        coyoteTime.wallTimer = 0f;

        Rigidbody rb = ctx.rb;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(ctx.orientation.up * ctx.jumpForce, ForceMode.Impulse);
        rb.AddForce(ctx.coyoteWallNormal * ctx.jumpForce * ctx.wallJumpMultiplier, ForceMode.Impulse);
    }
}
