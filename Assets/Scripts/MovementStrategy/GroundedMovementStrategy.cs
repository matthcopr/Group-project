using UnityEngine;

public class GroundedMovementStrategy : IMovementStrategy
{
    // move player along slope so that move speed remains consistent regardless of slope
    public void Move(PlayerMovementContext ctx)
    {
        Vector3 slopeNormal = ctx.GetSlopeNormal();
        Vector3 slopeMoveDir = Vector3.ProjectOnPlane(ctx.moveDir, slopeNormal).normalized;
        ctx.rb.AddForce(slopeMoveDir * ctx.moveSpeed * 10f, ForceMode.Force);
    }

    public void ApplyDrag(PlayerMovementContext ctx)
    {
        ctx.rb.linearDamping = ctx.groundDrag;
    }

    // speed control for grounded, MoveTowards makes it gradual instead of instant to prevent jerky movement
    public void SpeedControl(PlayerMovementContext ctx)
    {
        Rigidbody rb = ctx.rb;
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > ctx.moveSpeed)
        {
            Vector3 targetVel = flatVel.normalized * ctx.moveSpeed;
            Vector3 smoothed = Vector3.MoveTowards(flatVel, targetVel, ctx.speedDeceleration * Time.deltaTime);
            rb.linearVelocity = new Vector3(smoothed.x, rb.linearVelocity.y, smoothed.z);
        }
    }
}
