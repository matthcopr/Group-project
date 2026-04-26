using NUnit.Framework;
using UnityEngine;

public class DashCommandTests
{
    private PlayerMovementContext ctx;
    private DashCommand dashCommand;
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
            currentDashes = 2,
            isDashing = false,
        };
        dashCommand = new DashCommand();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(player);
        Object.DestroyImmediate(playerOrientation);
    }

    [Test]
    public void testBufferTimeIsZero()
    {
        Assert.AreEqual(0f, dashCommand.bufferTime);
    }

    [Test]
    public void testCanExecuteWhenDashesAvailableAndNotDashingReturnsTrue()
    {
        Assert.IsTrue(dashCommand.CanExecute(ctx));
    }

    [Test]
    public void testCanExecuteWhenNoDashesRemainingReturnsFalse()
    {
        ctx.currentDashes = 0;
        Assert.IsFalse(dashCommand.CanExecute(ctx));
    }

    [Test]
    public void testCanExecuteWhenAlreadyDashingReturnsFalse()
    {
        ctx.isDashing = true;
        Assert.IsFalse(dashCommand.CanExecute(ctx));
    }

    [Test]
    public void testExecuteDecrementsDashCount()
    {
        dashCommand.Execute(ctx);
        Assert.AreEqual(1, ctx.currentDashes);
    }

    [Test]
    public void testExecuteSetsDashQueued()
    {
        dashCommand.Execute(ctx);
        Assert.IsTrue(ctx.dashQueued);
    }

    [Test]
    public void testTimeQueuedCanBeSetAndRead()
    {
        dashCommand.timeQueued = 1.5f;
        Assert.AreEqual(1.5f, dashCommand.timeQueued);
    }
}