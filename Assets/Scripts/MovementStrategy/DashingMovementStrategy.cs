using UnityEngine;

public class DashingMovementStrategy : IMovementStrategy
{
    // you do nothing during dash because you want zero control during dash, why? I don't know that's how every other game does it and it feels right
    public void Move(PlayerMovementContext ctx) { }
    public void ApplyDrag(PlayerMovementContext ctx) => ctx.rb.linearDamping = 0f;
    public void SpeedControl(PlayerMovementContext ctx) { }
}
