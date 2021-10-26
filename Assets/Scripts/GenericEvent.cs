using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericEvent : MonoBehaviour
{
    public UnityEvent OnEvent;
    public bool RunOnAwake = false;
    public float Delay = 0.0f;
    bool triggering = false;
    public void TriggerEvent()
    {
        OnEvent.Invoke();
    }

    void Awake()
    {
        if (RunOnAwake) triggering = true;
    }

    void Update()
    {
        if (triggering && (Delay -= Time.deltaTime) < 0.0f)
        {
            triggering = false;
            OnEvent.Invoke();
        }
    }
}
