using UnityEngine;

public class CoyoteTimeHandler
{
    public float groundTimer { get; set; }
    public float wallTimer { get; set; }

    public bool canGroundJump => groundTimer > 0f;
    public bool canWallJump => wallTimer > 0f;

    private readonly float coyoteTime;

    public CoyoteTimeHandler(float coyoteTimeGiven)
    {
        coyoteTime = coyoteTimeGiven;
    }

    public void Tick(float deltaTime, bool grounded, bool touchingWall)
    {
        if (grounded) groundTimer = coyoteTime;
        else groundTimer -= deltaTime;

        if (touchingWall) wallTimer = coyoteTime;
        else wallTimer -= deltaTime;
    }
}
 
