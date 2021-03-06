using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerReaches : Condition
{
    public UnityEvent PlayerReached;
    public UnityEvent CantTrigger;
    public UnityEvent ConditionsMet;

    [SerializeField] List<Condition> Conditions;

    GameObject m_player;

    [SerializeField] bool canPlayerTrigger = false;

    [SerializeField] Text infoBox;

    [SerializeField] string worldMessage = "";

    private void Start()
    {
        if (!m_player) m_player = GameObject.Find("Player");
        if (!infoBox && GameObject.Find("InfoMessage")) infoBox = GameObject.Find("InfoMessage").GetComponent<Text>();
    }

    void CheckCoditions()
    {
        bool allreached = true;

        foreach (Condition _condition in Conditions)
        {
            if (!_condition.m_isComplete)
            {
                allreached = false;
                break;
            }
        }

        if (allreached)
        {
            ConditionsMet.Invoke();
        }
    }

    public void SetCanTrigger(bool _val)
    {
        canPlayerTrigger = _val;
    }

    public bool GetCanTrigger()
    {
        return canPlayerTrigger;
    }

    public void SetWorldMessage(string _message)
    {
        worldMessage = _message;
    }

    public void DisplayWorldMessage()
    {
        if (!infoBox) return;
        infoBox.text = worldMessage;
        EnableDisabe ed = infoBox.GetComponent<EnableDisabe>();
        ed.Enable();
        ed.SetPos(transform.position);
    }

    public void HideWorldMessage()
    {
        if (!infoBox) return;
        infoBox.text = "";
        EnableDisabe ed = infoBox.GetComponent<EnableDisabe>();
        ed.Disable();
        ed.SetPos(transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!m_isComplete && other.transform.root.gameObject == m_player)
        {
            CheckCoditions();

            if (canPlayerTrigger)
            {
                m_isComplete = true;
                PlayerReached.Invoke();
            }
            else
            {
                CantTrigger.Invoke();
            }
        }
    }
}
