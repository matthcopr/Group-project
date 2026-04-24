using UnityEngine;

public interface IJumpStrategy
{
    bool CanExecute(PlayerMovementContext ctx);

    void Execute(PlayerMovementContext ctx);
}
