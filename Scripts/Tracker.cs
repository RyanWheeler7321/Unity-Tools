using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker : MonoBehaviour
{
    public Transform trackedObject;

    public Vector3 offset;

    public bool lockY;

    float startY;

    public bool lookahead;

    public bool autoTrackPlayer;

    public float lookaheadFactor = 1.0f;

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
        Vector3 velocity = (trackedObject.position - lastTrackedPosition) / Time.deltaTime;
        lastTrackedPosition = trackedObject.position;

        Vector3 newPosition = trackedObject.position + offset;

        if (lookahead)
        {
            newPosition += velocity * lookaheadFactor;
        }

        if (lockY)
        {
            newPosition.y = startY;
        }

        transform.position = newPosition;
    }
}
