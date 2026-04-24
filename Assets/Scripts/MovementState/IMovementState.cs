using UnityEngine;

public interface IMovementState
{
    void Enter(PlayerMovementContext ctx);
    void Exit(PlayerMovementContext ctx);
    void Update(PlayerMovementContext ctx);
    void FixedUpdate(PlayerMovementContext ctx);
    IMovementState GetNextState(PlayerMovementContext ctx);
}
