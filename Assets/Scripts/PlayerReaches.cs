using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerReaches : MonoBehaviour
{
    public UnityEvent PlayerReached;
    public UnityEvent CantTrigger;

    [SerializeField] GameObject m_player;

    [SerializeField] bool canPlayerTrigger = false;

    [SerializeField] bool hasPlayerReached = false;

    [SerializeField] Text infoBox;

    [SerializeField] string worldMessage = "";

    private void Start()
    {
        if (!m_player) m_player = GameObject.Find("Player");
        if (!infoBox && GameObject.Find("InfoMessage")) infoBox = GameObject.Find("InfoMessage").GetComponent<Text>();
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

    public void DisplayWorldMessage()
    {
        if (!infoBox) return;
        infoBox.text = worldMessage;
        EnableDisabe ed = infoBox.GetComponent<EnableDisabe>();
        ed.Enable();
        ed.SetPos(transform.position);
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
                CantTrigger.Invoke();
            }
        }
    }
}
