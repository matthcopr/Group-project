using NUnit.Framework;
using UnityEngine;

public class AirborneStateTests
{
    private PlayerMovementContext ctx;
    private AirborneState state;
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
            airMultiplier = 0.4f,
            maxWallSlideSpeed = 3f,
            grounded = false,
            speedDeceleration = 10f,
        };
        mediator = new MockMediator();
        state = new AirborneState();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(player);
        Object.DestroyImmediate(playerOrientation);
    }

    [Test]
    public void testEnterDoesNotThrow()
    {
        Assert.DoesNotThrow(() => state.Enter(ctx, mediator));
    }

    [Test]
    public void testUpdateWhenGroundedRequestsGroundedTransition()
    {
        state.Enter(ctx, mediator);
        ctx.grounded = true;
        state.Update(ctx);
        Assert.IsInstanceOf<GroundedState>(mediator.LastRequestedState);
    }

    [Test]
    public void testUpdateWhenNotGroundedNoTransitionRequested()
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