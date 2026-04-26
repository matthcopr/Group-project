using UnityEngine;

public interface IMovementState
{
    void Enter(PlayerMovementContext ctx, IMovementMediator mediator);
    void Exit(PlayerMovementContext ctx);
    void Update(PlayerMovementContext ctx);
    void FixedUpdate(PlayerMovementContext ctx);
}
