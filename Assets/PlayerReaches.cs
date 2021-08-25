using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerReaches : MonoBehaviour
{
    public UnityEvent PlayerReached;

    [SerializeField] GameObject m_player;

    [SerializeField] bool canPlayerTrigger = false;

    [SerializeField] bool hasPlayerReached = false;

    private void Start()
    {
        if (!m_player) m_player = GameObject.Find("Player");
    }

    public void SetReached(bool _val)
    {
        hasPlayerReached = _val;
    }

    public bool GetReached()
    {
        return hasPlayerReached;
    }

    public void SetCanTrigger(bool _val)
    {
        canPlayerTrigger = _val;
    }

    public bool GetCanTrigger()
    {
        return canPlayerTrigger;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canPlayerTrigger && !hasPlayerReached && other.transform.root.gameObject == m_player)
        {
            PlayerReached.Invoke();
        }
    }
}
