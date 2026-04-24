using UnityEngine;

public class SlidingMovementStrategy : IMovementStrategy
{
    //lots of sliding is same as air control with some slight changes such as a little bit more drag and different slide/air multiplier which defines how much control you have
    public void Move(PlayerMovementContext ctx)
    {
        Rigidbody rb = ctx.rb;
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float currentSpeed = flatVel.magnitude;

        if (currentSpeed < ctx.moveSpeed)
            rb.AddForce(ctx.moveDir.normalized * ctx.slideMultiplier * ctx.moveSpeed * 10f, ForceMode.Force);

        SteerVelocity(ctx, flatVel, currentSpeed);
    }

    private void SteerVelocity(PlayerMovementContext ctx, Vector3 flatVel, float currentSpeed)
    {
        if (ctx.moveDir == Vector3.zero || currentSpeed <= 0f) return;

        Rigidbody rb = ctx.rb;
        Vector3 targetVel = ctx.moveDir.normalized * currentSpeed;
        Vector3 steered = Vector3.RotateTowards(flatVel, targetVel, ctx.turnSpeed * Time.fixedDeltaTime, 0f);
        rb.linearVelocity = new Vector3(steered.x, rb.linearVelocity.y, steered.z);
    }

    public void ApplyDrag(PlayerMovementContext ctx)
    {
        ctx.rb.linearDamping = ctx.slideDrag;
    }

    // sliding no speed control
    public void SpeedControl(PlayerMovementContext ctx) { }
}