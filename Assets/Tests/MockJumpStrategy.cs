using UnityEngine;

// point of mock jumpstrategy is to make it so that you can test classes dependent upon other classes in isolation with smaller simpler classes that have expected/ guaranteed outputs
public class MockJumpStrategy : IJumpStrategy
{
    public bool canExecute = false;
    public bool wasExecuted = false;

    public bool CanExecute(PlayerMovementContext ctx) => canExecute;
    public void Execute(PlayerMovementContext ctx) => wasExecuted = true;
}
