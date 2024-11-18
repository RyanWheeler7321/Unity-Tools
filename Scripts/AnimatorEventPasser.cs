using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventPasser : MonoBehaviour
{
    public UnityEvent<string> stringEvent;

    public void StringEventInvoke(string value)
    {
        stringEvent.Invoke(value);
    }

    public UnityEvent event1;
    public UnityEvent event2;
    public UnityEvent event3;
    public UnityEvent event4;
    public UnityEvent event5;
    public UnityEvent event6;
    public UnityEvent event7;
    public UnityEvent event8;
    public UnityEvent event9;

    public void Event1Invoke()
    {
        event1.Invoke();
    }

    public void Event2Invoke()
    {
        event2.Invoke();
    }
    public void Event3Invoke()
    {
        event3.Invoke();
    }
    public void Event4Invoke()
    {
        event4.Invoke();
    }
    public void Event5Invoke()
    {
        event5.Invoke();
    }
    public void Event6Invoke()
    {
        event6.Invoke();
    }
    public void Event7Invoke()
    {
        event7.Invoke();
    }
    public void Event8Invoke()
    {
        event8.Invoke();
    }
    public void Event9Invoke()
    {
        event9.Invoke();
    }
}
