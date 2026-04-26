using UnityEngine;

public interface IMovementMediator
{
    void RequestTransition(IMovementState next);
}
