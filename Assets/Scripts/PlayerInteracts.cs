using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerInteracts : Condition
{
    public UnityEvent PlayerInteracted;
    public UnityEvent CantTrigger;
    public UnityEvent ConditionsMet;

    [SerializeField] List<Condition> Conditions;

    GameObject m_player;

    [SerializeField] public float m_interactDistance = 3.0f;

    [SerializeField] bool canPlayerTrigger = false;

    [SerializeField] Text infoBox;

    [SerializeField] string worldMessage = "";

    private void Start()
    {
        if (!m_player) m_player = GameObject.Find("Player");
        if (!infoBox && GameObject.Find("InfoMessage")) infoBox = GameObject.Find("InfoMessage").GetComponent<Text>();
    }

    bool once = true;
    private void LateUpdate()
    {
        if (once && GetComponent<Outline>()) { once = false; GetComponent<Outline>().OutlineWidth = 0.0f; }
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

    public void TryComplete()
    {
        if (!m_isComplete)
        {
            CheckCoditions();

            if (canPlayerTrigger)
            {
                m_isComplete = true;
                PlayerInteracted.Invoke();
            }
            else
            {
                CantTrigger.Invoke();
            }
        }
    }
}
