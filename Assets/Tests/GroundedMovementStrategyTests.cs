using NUnit.Framework;
using UnityEngine;

public class GroundedMovementStrategyTests
{
    private PlayerMovementContext ctx;
    private GroundedMovementStrategy strategy;
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
            speedDeceleration = 10f,
            moveDir = Vector3.forward,
        };
        strategy = new GroundedMovementStrategy();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(player);
        Object.DestroyImmediate(playerOrientation);
    }

    [Test]
    public void testApplyDragSetsGroundDraplayernRigidbody()
    {
        strategy.ApplyDrag(ctx);
        Assert.AreEqual(ctx.groundDrag, ctx.rb.linearDamping);
    }

    [Test]
    public void testSpeedControlWhenUnderLimitDoesNotChangeVelocity()
    {
        ctx.rb.linearVelocity = new Vector3(2f, 0f, 0f);
        strategy.SpeedControl(ctx);
        Assert.AreEqual(2f, ctx.rb.linearVelocity.x, 0.01f);
    }

    [Test]
    public void testSpeedControlWhenOverLimitReducesVelocity()
    {
        ctx.rb.linearVelocity = new Vector3(100f, 0f, 0f);
        strategy.SpeedControl(ctx);
        Vector3 flatVel = new Vector3(ctx.rb.linearVelocity.x, 0f, ctx.rb.linearVelocity.z);
        Assert.Less(flatVel.magnitude, 100f);
    }

    [Test]
    public void testSpeedControlPreservesVerticalVelocity()
    {
        ctx.rb.linearVelocity = new Vector3(100f, -5f, 0f);
        strategy.SpeedControl(ctx);
        Assert.AreEqual(-5f, ctx.rb.linearVelocity.y, 0.01f);
    }

    [Test]
    public void testMoveDoesNotThrow()
    {
        Assert.DoesNotThrow(() => strategy.Move(ctx));
    }
}