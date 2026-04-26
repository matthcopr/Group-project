using NUnit.Framework;
using UnityEngine;

public class GroundedStateTests
{
    private PlayerMovementContext ctx;
    private GroundedState state;
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
            moveSpeed = 5f,
            groundDrag = 5f,
            maxJumps = 2,
            grounded = true,
            speedDeceleration = 10f,
        };
        mediator = new MockMediator();
        state = new GroundedState();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(player);
        Object.DestroyImmediate(playerOrientation);
    }

    [Test]
    public void testEnterSetsGroundDrag()
    {
        state.Enter(ctx, mediator);
        Assert.AreEqual(ctx.groundDrag, ctx.rb.linearDamping);
    }

    [Test]
    public void testEnterSetsJumpsRemaining()
    {
        state.Enter(ctx, mediator);
        Assert.AreEqual((int)ctx.maxJumps - 1, ctx.jumpsRemaining);
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
    public void testUpdateWhenSlideHeldRequestsSlidingTransition()
    {
        state.Enter(ctx, mediator);
        ctx.slideHeld = true;
        state.Update(ctx);
        Assert.IsInstanceOf<SlidingState>(mediator.LastRequestedState);
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
    public void testUpdateWhenGroundedAndNoInputNoTransitionRequested()
    {
        state.Enter(ctx, mediator);
        state.Update(ctx);
        Assert.AreEqual(0, mediator.TransitionCount);
    }

    [Test]
    public void testExitDoesNotThrow()
    {
        state.Enter(ctx, mediator);
        Assert.DoesNotThrow(() => state.Exit(ctx));
    }
}