using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SignalListener : MonoBehaviour
{
    public SignalSender signal;
    public UnityEvent signalEvent;
    public void OnSignalRaised()
    {
        signalEvent.Invoke();
    }
    public void OnEnable()
    {
        signal.RegisterListener(this);
    }
    public void OnDisable()
    {
        signal.DeregisterListener(this);
    }
}

