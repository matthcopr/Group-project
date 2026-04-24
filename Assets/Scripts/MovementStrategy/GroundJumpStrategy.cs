using UnityEngine;

public class GroundJumpStrategy : IJumpStrategy
{
    public bool CanExecute(PlayerMovementContext ctx)
    {
        return ctx.jumpsRemaining > 0;
    }

    //remove drag before jump to make initial jump equal in strength to airjumps because drag is also applied upwards
    public void Execute(PlayerMovementContext ctx)
    {
        ctx.jumpsRemaining--;
        Rigidbody rb = ctx.rb;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.linearDamping = 0f;
        rb.AddForce(Vector3.up * ctx.jumpForce, ForceMode.Impulse);
    }
}
