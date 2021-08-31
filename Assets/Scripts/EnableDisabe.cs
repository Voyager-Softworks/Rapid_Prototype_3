using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EnableDisabe : MonoBehaviour
{
    public float aliveTime = 10;
    float startTime = float.PositiveInfinity;

    Text text;

    public UnityEvent onEnable;
    public UnityEvent onDisable;

    private void Start()
    {
        startTime = Time.time;
        text = GetComponent<Text>();
    }

    private void Update()
    {
        if (text.enabled && Time.time - startTime >= aliveTime)
        {
            Disable();
        }
    }

    public void Enable(float _time = 10)
    {
        aliveTime = _time;
        startTime = Time.time;
        text.enabled = true;
        onEnable.Invoke();
    }

    public void Disable()
    {
        if (text.enabled)
        {
            text.enabled = false;
            onDisable.Invoke();
        }
    }
}
