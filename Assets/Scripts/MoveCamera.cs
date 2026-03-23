using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;
    public PlayerMovement playerMovement;

    [Header("Slide Camera")]
    public float slideCameraOffset = -0.5f;
    public float slideLerpSpeed = 10f;

    private float currentYOffset = 0f;

    private void Update()
    {
        float targetYOffset = playerMovement.sliding ? slideCameraOffset : 0f;
        currentYOffset = Mathf.Lerp(currentYOffset, targetYOffset, Time.deltaTime * slideLerpSpeed);

        transform.position = cameraPosition.position + new Vector3(0f, currentYOffset, 0f);
    }
}
