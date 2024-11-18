using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    // Define a UnityEvent for customization in the Inspector.
    public UnityEvent onTriggerEnterEvent;

    public UnityEvent onTriggerStayEvent;

    public UnityEvent onTriggerExitEvent;

    public bool fireMultipleTimes = false;

    bool fired;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!fired)
            {
                fired = true;
                onTriggerEnterEvent.Invoke();
            }
            else if (fired && fireMultipleTimes)
            {
                onTriggerEnterEvent.Invoke();
            }
        }
    }

    bool exitFired;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!exitFired)
            {
                exitFired = true;
                onTriggerExitEvent.Invoke();
            }
            else if (exitFired && fireMultipleTimes)
            {
                onTriggerExitEvent.Invoke();
            }
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTriggerStayEvent.Invoke();
        }
    }
}
