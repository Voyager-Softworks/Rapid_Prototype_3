using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EnableDisabe : MonoBehaviour
{
    public float aliveTime = 10;
    float startTime = float.PositiveInfinity;
    public bool isEnabled = false;

    [SerializeField] Image line;

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
        

        if (isEnabled && Time.time - startTime >= aliveTime)
        {
            Disable();
        }
        else if (isEnabled)
        {
            text.color += new Color(0, 0, 0, 2 * Time.deltaTime);
        }
        else
        {
            text.color -= new Color(0, 0, 0, 2 * Time.deltaTime);
        }

        text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Clamp(text.color.a, 0, 1));
        if (line && isEnabled) line.color = text.color;
    }

    public void SetPos(Vector3 _pos)
    {
        tempWorldPos = _pos;
    }

    public void Enable(float _time = 10)
    {
        aliveTime = _time;
        startTime = Time.time;
        isEnabled = true;
        onEnable.Invoke();
    }

    public void Disable()
    {
        if (tempWorldPos != Vector3.zero)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        }

        tempWorldPos = Vector3.zero;
        GetComponent<RectTransform>().position = origPos;

        if (isEnabled)
        {
            isEnabled = false;
            onDisable.Invoke();
        }
    }
}
