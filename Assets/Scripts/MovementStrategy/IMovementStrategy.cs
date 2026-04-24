using UnityEngine;

public interface IMovementStrategy
{
    void Move(PlayerMovementContext ctx);
    void ApplyDrag(PlayerMovementContext ctx);
    void SpeedControl(PlayerMovementContext ctx);
}
