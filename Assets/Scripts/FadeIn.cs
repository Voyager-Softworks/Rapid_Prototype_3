using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    public UnityEvent fadeInDone;

    [SerializeField] bool startFade = false;
    [SerializeField] bool startFadeOut = false;
    Image img;

    float speed = 1;

    private void Start()
    {
        img = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (startFade)
        {
            img.color += new Color(0, 0, 0, 1 * Time.deltaTime);
            if (img.color.a >= 1)
            {
                fadeInDone.Invoke();
                startFade = false;
                img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
            }
        }
        else if (startFadeOut)
        {
            img.color -= new Color(0, 0, 0, 1 * Time.deltaTime);
            if (img.color.a <= 0)
            {
                startFade = false;
                img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
            }
        }

    }

    public void StartFade(float _speed)
    {
        speed = _speed;
        startFade = true;
        startFadeOut = false;
    }

    public void FadeOut(float _speed)
    {
        speed = _speed;
        startFade = false;
        startFadeOut = true;
    }
}
