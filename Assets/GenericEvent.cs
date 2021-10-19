using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericEvent : MonoBehaviour
{
    public UnityEvent OnEvent;
    public void TriggerEvent()
    {
        OnEvent.Invoke();
    }
}
