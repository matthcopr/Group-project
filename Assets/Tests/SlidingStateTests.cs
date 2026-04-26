using NUnit.Framework;
using UnityEngine;

public class SlidingStateTests
{
    private PlayerMovementContext ctx;
    private SlidingState state;
    private MockMediator mediator;
    private GameObject player;
    private GameObject playerOrientation;

    [SetUp]
    public void SetUp()
    {
        player = new GameObject();
        playerOrientation = new GameObject();
        ctx = new PlayerMovementContext
        {
            rb = player.AddComponent<Rigidbody>(),
            orientation = playerOrientation.transform,
            slideDrag = 2f,
            slideMultiplier = 1.5f,
            slideDownSpeedMultiplier = 2f,
            slopeUpBrakingMultiplier = 1f,
            grounded = true,
            slideHeld = true,
        };
        mediator = new MockMediator();
        state = new SlidingState();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(player);
        Object.DestroyImmediate(playerOrientation);
    }

    [Test]
    public void testEnterSetsSlideDrag()
    {
        state.Enter(ctx, mediator);
        Assert.AreEqual(ctx.slideDrag, ctx.rb.linearDamping);
    }

    [Test]
    public void testUpdateWhenJumpQueuedRequestsAirborneTransition()
    {
        state.Enter(ctx, mediator);
        ctx.jumpQueued = true;
        state.Update(ctx);
        Assert.IsInstanceOf<AirborneState>(mediator.LastRequestedState);
    }

    [Test]
    public void testUpdateWhenSlideReleasedRequestsGroundedTransition()
    {
        state.Enter(ctx, mediator);
        ctx.slideHeld = false;
        state.Update(ctx);
        Assert.IsInstanceOf<GroundedState>(mediator.LastRequestedState);
    }

    [Test]
    public void testUpdateWhenNotGroundedRequestsAirborneTransition()
    {
        state.Enter(ctx, mediator);
        ctx.grounded = false;
        state.Update(ctx);
        Assert.IsInstanceOf<AirborneState>(mediator.LastRequestedState);
    }

    [Test]
    public void testUpdateWhenGroundedAndSlidingNoTransitionRequested()
    {
        state.Enter(ctx, mediator);
        state.Update(ctx);
        Assert.AreEqual(0, mediator.TransitionCount);
    }
}