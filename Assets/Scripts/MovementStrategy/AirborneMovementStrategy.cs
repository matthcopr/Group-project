using UnityEngine;

public class AirborneMovementStrategy : IMovementStrategy
{
    public void Move(PlayerMovementContext ctx)
    {
        Rigidbody rb = ctx.rb;
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float currentSpeed = flatVel.magnitude;

        //only can accelerate if below movespeed while in the air
        if (currentSpeed < ctx.moveSpeed)
            rb.AddForce(ctx.moveDir.normalized * ctx.moveSpeed * 10f * ctx.airMultiplier, ForceMode.Force);

        SteerVelocity(ctx, flatVel, currentSpeed);
    }

    // when above move speed you can change the direction of your velocity, but not the magnitude
    private void SteerVelocity(PlayerMovementContext ctx, Vector3 flatVel, float currentSpeed)
    {
        if (ctx.moveDir == Vector3.zero || currentSpeed <= 0f) return;

        Rigidbody rb = ctx.rb;
        Vector3 targetVel = ctx.moveDir.normalized * currentSpeed;
        Vector3 steered = Vector3.RotateTowards(flatVel, targetVel, ctx.turnSpeed * Time.fixedDeltaTime, 0f);
        rb.linearVelocity = new Vector3(steered.x, rb.linearVelocity.y, steered.z);
    }

    //zero drag in air
    public void ApplyDrag(PlayerMovementContext ctx)
    {
        ctx.rb.linearDamping = 0f;
    }

    // not implemented since we want no speed control while in the air
    public void SpeedControl(PlayerMovementContext ctx) { }

}