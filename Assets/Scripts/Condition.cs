using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition : MonoBehaviour
{
    [SerializeField] public bool m_isComplete = false;

    public void SetComplete(bool _val)
    {
        m_isComplete = _val;
    }

    public bool GetComplete()
    {
        return m_isComplete;
    }
}