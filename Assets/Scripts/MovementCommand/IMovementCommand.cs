using UnityEngine;

public interface IMovementCommand
{
    float bufferTime { get; }
    float timeQueued { get; set; }
    bool CanExecute(PlayerMovementContext ctx);
    void Execute(PlayerMovementContext ctx);
}
