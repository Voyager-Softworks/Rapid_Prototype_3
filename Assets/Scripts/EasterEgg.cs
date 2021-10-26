using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EasterEgg : MonoBehaviour
{
    public List<PlayerInteracts> interactions;
    public UnityEvent OnEasterEgg;
    bool triggered = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (triggered == false)
        {
            
        
        bool allactivated = true;
        foreach (PlayerInteracts p in interactions)
        {
            if (!p.m_isComplete)
            {
                allactivated = false;
            }
        }
        if (allactivated)
        {
            triggered = true;
            OnEasterEgg.Invoke();
        }
        }

    }
}
