using UnityEngine;

public class DashCommand : IMovementCommand
{
    public float bufferTime => 0f; // dash is instant no buffering
    public float timeQueued { get; set; }

    //
    public bool CanExecute(PlayerMovementContext ctx)
    {
        return ctx.currentDashes > 0 && !ctx.isDashing;
    }

    public void Execute(PlayerMovementContext ctx)
    {
        ctx.currentDashes--;
        ctx.dashQueued = true;
    }
}
