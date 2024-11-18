using UnityEngine;

public class Tracker : MonoBehaviour
{
    public Transform trackedObject;

    public Vector3 offset;

    public bool lockY;

    float startY;

    public bool lookahead;

    public bool autoTrackPlayer;

    public bool followRotation;

    public float lookaheadFactor = 1.0f;

    public float strength = 0.1f; // 0 = no lerp, higher = slower lerp
    public float rotationStrength = 0.1f;

    private Vector3 lastTrackedPosition;

    private void Start()
    {
        if (autoTrackPlayer)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                trackedObject = player.transform;
            }
        }
        if (lockY)
        {
            startY = transform.position.y;
        }
        lastTrackedPosition = trackedObject.position;
    }

    void Update()
    {
        // Position Tracking
        Vector3 velocity = (trackedObject.position - lastTrackedPosition) / Time.deltaTime;
        lastTrackedPosition = trackedObject.position;

        Vector3 targetPosition = trackedObject.position + offset;

        if (lookahead)
        {
            targetPosition += velocity * lookaheadFactor;
        }

        if (lockY)
        {
            targetPosition.y = startY;
        }

        // Smooth position update
        transform.position = Vector3.Lerp(transform.position, targetPosition, Mathf.Clamp01(strength * Time.deltaTime));

        // Rotation Tracking
        if (followRotation)
        {
            Quaternion targetRotation = trackedObject.rotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Mathf.Clamp01(rotationStrength * Time.deltaTime));
        }
    }
}
