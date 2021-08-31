using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerReaches : MonoBehaviour
{
    public UnityEvent PlayerReached;
    public UnityEvent<string> CantTriggerSTRING;
    public UnityEvent<Vector3> CantTriggerVEC3;

    [SerializeField] GameObject m_player;

    [SerializeField] bool canPlayerTrigger = false;

    [SerializeField] bool hasPlayerReached = false;

    [SerializeField] Text infoBox;

    [SerializeField] string cantTriggerMessage = "";

    private void Start()
    {
        if (!m_player) m_player = GameObject.Find("Player");
        if (!infoBox) infoBox = GameObject.Find("InfoMessage").GetComponent<Text>();
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
        if (!hasPlayerReached && other.transform.root.gameObject == m_player)
        {
            if (canPlayerTrigger)
            {
                PlayerReached.Invoke();
            }
            else
            {
                CantTriggerSTRING.Invoke(cantTriggerMessage);
                CantTriggerVEC3.Invoke(transform.position);
            }
        }
    }
}
