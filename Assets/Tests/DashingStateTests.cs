using NUnit.Framework;
using UnityEngine;

public class DashingStateTests
{
    private PlayerMovementContext ctx;
    private DashingState state;
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
            dashForce = 20f,
            dashDuration = 0.3f,
            grounded = false,
            moveDir = Vector3.forward,
            isDashing = true,
        };
        mediator = new MockMediator();
        state = new DashingState();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(player);
        Object.DestroyImmediate(playerOrientation);
    }

    [Test]
    public void testEnterDisablesGravity()
    {
        state.Enter(ctx, mediator);
        Assert.IsFalse(ctx.rb.useGravity);
    }

    [Test]
    public void testExitRestoresGravity()
    {
        ctx.rb.useGravity = true;
        state.Enter(ctx, mediator);
        state.Exit(ctx);
        Assert.IsTrue(ctx.rb.useGravity);
    }

    [Test]
    public void testExitClearsDashingFlag()
    {
        state.Enter(ctx, mediator);
        state.Exit(ctx);
        Assert.IsFalse(ctx.isDashing);
    }

    [Test]
    public void testUpdateBeforeDurationExpiresNoTransitionRequested()
    {
        state.Enter(ctx, mediator);
        state.Update(ctx);
        Assert.AreEqual(0, mediator.TransitionCount);
    }

    [Test]
    public void testUpdateWhenGroundedAfterDurationRequestsGroundedTransition()
    {
        ctx.grounded = true;
        ctx.dashDuration = 0f;
        state.Enter(ctx, mediator);
        state.Update(ctx);
        Assert.IsInstanceOf<GroundedState>(mediator.LastRequestedState);
    }

    [Test]
    public void testUpdateWhenAirborneAfterDurationRequestsAirborneTransition()
    {
        ctx.grounded = false;
        ctx.dashDuration = 0f;
        state.Enter(ctx, mediator);
        state.Update(ctx);
        Assert.IsInstanceOf<AirborneState>(mediator.LastRequestedState);
    }

    [Test]
    public void testFixedUpdateDoesNotThrow()
    {
        state.Enter(ctx, mediator);
        Assert.DoesNotThrow(() => state.FixedUpdate(ctx));
    }
}