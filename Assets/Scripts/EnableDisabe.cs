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

    Vector3 origPos;
    Vector3 tempWorldPos = Vector3.zero;

    private void Start()
    {
        origPos = GetComponent<RectTransform>().position;
        startTime = Time.time;
        text = GetComponent<Text>();
    }

    private void Update()
    {
        if (tempWorldPos != Vector3.zero)
        {
            if (Vector3.Dot(Camera.main.transform.forward, tempWorldPos - Camera.main.transform.position) > 0)
            {
                Vector3 tempPos = Camera.main.WorldToScreenPoint(tempWorldPos);
                GetComponent<RectTransform>().position = tempPos;
            }
        }
        else
        {
            GetComponent<RectTransform>().position = origPos;
        }
        

        if (text.enabled && Time.time - startTime >= aliveTime)
        {
            Disable();
        }
    }

    public void SetPos(Vector3 _pos)
    {
        tempWorldPos = _pos;
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
        tempWorldPos = Vector3.zero;
        GetComponent<RectTransform>().position = origPos;

        if (text.enabled)
        {
            text.enabled = false;
            onDisable.Invoke();
        }
    }
}
