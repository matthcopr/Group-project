using UnityEngine;

//the point of mock mediator is to have something to call requestTransition on without all the overhead of the actual mediator in the code with playerMovement which has copious variables and values
public class MockMediator : IMovementMediator
{
    public IMovementState LastRequestedState;
    public int TransitionCount;

    public void RequestTransition(IMovementState next)
    {
        LastRequestedState = next;
        TransitionCount++;
    }
}
