using NUnit.Framework;
using UnityEngine;

public class JumpCommandTests
{
    private PlayerMovementContext ctx;
    private JumpCommand jumpCommand;
    private MockJumpStrategy mockGroundJump;
    private MockJumpStrategy mockWallJump;
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
            readyToJump = true,
            jumpCooldown = 0.25f,
            invokeResetJump = (_) => { },
        };
        mockGroundJump = new MockJumpStrategy();
        mockWallJump = new MockJumpStrategy();
        jumpCommand = new JumpCommand(mockGroundJump, mockWallJump);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(player);
        Object.DestroyImmediate(playerOrientation);
    }

    [Test]
    public void testBufferTimeIsPositive()
    {
        Assert.Greater(jumpCommand.bufferTime, 0f);
    }

    [Test]
    public void testCanExecuteWhenReadyAndGroundJumpAvailableReturnsTrue()
    {
        mockGroundJump.canExecute = true;
        Assert.IsTrue(jumpCommand.CanExecute(ctx));
    }

    [Test]
    public void testCanExecuteWhenReadyAndWallJumpAvailableReturnsTrue()
    {
        mockWallJump.canExecute = true;
        Assert.IsTrue(jumpCommand.CanExecute(ctx));
    }

    [Test]
    public void testCanExecuteWhenNotReadyReturnsFalse()
    {
        ctx.readyToJump = false;
        mockGroundJump.canExecute = true;
        Assert.IsFalse(jumpCommand.CanExecute(ctx));
    }

    [Test]
    public void testCanExecuteWhenNoJumpAvailableReturnsFalse()
    {
        mockGroundJump.canExecute = false;
        mockWallJump.canExecute = false;
        Assert.IsFalse(jumpCommand.CanExecute(ctx));
    }

    [Test]
    public void testExecuteWallJumpAvailableExecutesWallJump()
    {
        mockWallJump.canExecute = true;
        jumpCommand.Execute(ctx);
        Assert.IsTrue(mockWallJump.wasExecuted);
        Assert.IsFalse(mockGroundJump.wasExecuted);
    }

    [Test]
    public void testExecuteWallJumpAvailableSetsWallJumpQueued()
    {
        mockWallJump.canExecute = true;
        jumpCommand.Execute(ctx);
        Assert.IsTrue(ctx.wallJumpQueued);
    }

    [Test]
    public void testExecuteGroundJumpOnlyExecutesGroundJump()
    {
        mockGroundJump.canExecute = true;
        jumpCommand.Execute(ctx);
        Assert.IsTrue(mockGroundJump.wasExecuted);
        Assert.IsFalse(mockWallJump.wasExecuted);
    }

    [Test]
    public void testExecuteGroundJumpOnlySetsJumpQueued()
    {
        mockGroundJump.canExecute = true;
        jumpCommand.Execute(ctx);
        Assert.IsTrue(ctx.jumpQueued);
    }

    [Test]
    public void testExecuteSetsReadyToJumpFalse()
    {
        mockGroundJump.canExecute = true;
        jumpCommand.Execute(ctx);
        Assert.IsFalse(ctx.readyToJump);
    }

    [Test]
    public void testExecuteInvokesResetJump()
    {
        bool resetInvoked = false;
        ctx.invokeResetJump = (_) => resetInvoked = true;
        mockGroundJump.canExecute = true;
        jumpCommand.Execute(ctx);
        Assert.IsTrue(resetInvoked);
    }
}