using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;
    public PlayerMovement playerMovement;

    [Header("Slide Camera")]
    public float slideCameraOffset = -0.5f;
    public float slideLerpSpeed = 10f;

    private float currentYOffset = 0f;
    private bool isSliding;

    //subscribes the 2 handler methods to the events on playermovement such that whenever those 2 methods are ran the isSliding bool is updated
    // this is in fact a very small example of the observer pattern
    private void Start()
    {
        playerMovement.OnSlideStart += HandleSlideStart;
        playerMovement.OnSlideStop += HandleSlideStop;
    }

    private void OnDestroy()
    {
        playerMovement.OnSlideStart -= HandleSlideStart;
        playerMovement.OnSlideStop -= HandleSlideStop;
    }

    private void HandleSlideStart() => isSliding = true;
    private void HandleSlideStop() => isSliding = false;

    private void Update()
    {
        float targetYOffset = isSliding ? slideCameraOffset : 0f;
        currentYOffset = Mathf.Lerp(currentYOffset, targetYOffset, Time.deltaTime * slideLerpSpeed);
        transform.position = cameraPosition.position + new Vector3(0f, currentYOffset, 0f);
    }
}