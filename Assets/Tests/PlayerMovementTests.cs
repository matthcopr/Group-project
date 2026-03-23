using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerMovementTests
{
    private GameObject CreatePlayer()
    {
        var go = new GameObject("Player");
        go.AddComponent<Rigidbody>();
        go.AddComponent<CapsuleCollider>();

        var orientation = new GameObject("Orientation");
        orientation.transform.SetParent(go.transform);

        var pm = go.AddComponent<PlayerMovement>();
        pm.orientation = orientation.transform;
        pm.moveSpeed = 10f;
        pm.jumpForce = 12f;
        pm.jumpCooldown = 0.25f;
        pm.groundDrag = 6f;
        pm.slideDrag = 1f;
        pm.airMultiplier = 0.4f;
        pm.maxWallSlideSpeed = 3f;
        pm.wallJumpMultiplier = 0.5f;
        pm.turnSpeed = 90f;
        pm.coyoteTime = 0.15f;
        pm.jumpBufferTime = 0.1f;
        pm.playerHeight = 2f;
        pm.playerRadius = 0.4f;
        pm.maxJumps = 2;

        return go;
    }

    [UnityTest]
    public IEnumerator TestPlayerCreation()
    {
        var go = CreatePlayer();
        yield return null;

        Assert.IsNotNull(go.GetComponent<PlayerMovement>());
        Assert.IsNotNull(go.GetComponent<Rigidbody>());

        Object.Destroy(go);
    }

    [UnityTest]
    public IEnumerator TestRigidbody()
    {
        var go = CreatePlayer();
        yield return null;

        var rb = go.GetComponent<Rigidbody>();
        Assert.IsTrue(rb.freezeRotation);

        Object.Destroy(go);
    }

    [UnityTest]
    public IEnumerator TestMoveSpeed()
    {
        var go = CreatePlayer();
        yield return null;

        var pm = go.GetComponent<PlayerMovement>();
        Assert.AreEqual(10f, pm.moveSpeed);

        Object.Destroy(go);
    }

    [UnityTest]
    public IEnumerator TestSliding()
    {
        var go = CreatePlayer();
        yield return null;

        var pm = go.GetComponent<PlayerMovement>();
        Assert.IsFalse(pm.sliding);

        Object.Destroy(go);
    }

    [UnityTest]
    public IEnumerator TestOrientation()
    {
        var go = CreatePlayer();
        yield return null;

        var pm = go.GetComponent<PlayerMovement>();
        Assert.IsNotNull(pm.orientation);

        Object.Destroy(go);
    }
}