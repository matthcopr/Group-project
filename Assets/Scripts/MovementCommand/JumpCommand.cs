using UnityEngine;

public class JumpCommand : IMovementCommand
{
    public float bufferTime => 0.15f;
    public float timeQueued { get; set; }

    private readonly IJumpStrategy groundJump;
    private readonly IJumpStrategy wallJump;

    public JumpCommand(IJumpStrategy IgroundJump, IJumpStrategy IwallJump)
    {
        groundJump = IgroundJump;
        wallJump = IwallJump;
    }
    
    //pretty shrimple just read it
    public bool CanExecute(PlayerMovementContext ctx)
    {
        return ctx.readyToJump && (wallJump.CanExecute(ctx) || groundJump.CanExecute(ctx));
    }

    public void Execute(PlayerMovementContext ctx)
    {
        ctx.readyToJump = false;

        if (wallJump.CanExecute(ctx))
        {
            wallJump.Execute(ctx);
            ctx.wallJumpQueued = true;
        }
        else
        {
            groundJump.Execute(ctx);
            ctx.jumpQueued = true;
        }

        ctx.invokeResetJump?.Invoke(ctx.jumpCooldown);
    }
}
