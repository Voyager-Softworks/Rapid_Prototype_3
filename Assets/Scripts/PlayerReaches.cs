using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerReaches : MonoBehaviour
{
    public UnityEvent PlayerReached;
    public UnityEvent CantTrigger;
    public UnityEvent ConditionsMet;

    [SerializeField] List<PlayerReaches> Conditions;

    GameObject m_player;

    [SerializeField] bool canPlayerTrigger = false;

    [SerializeField] bool hasPlayerReached = false;

    [SerializeField] Text infoBox;

    [SerializeField] string worldMessage = "";

    private void Start()
    {
        if (!m_player) m_player = GameObject.Find("Player");
        if (!infoBox && GameObject.Find("InfoMessage")) infoBox = GameObject.Find("InfoMessage").GetComponent<Text>();

        foreach (PlayerReaches _condition in Conditions)
        {
            _condition.PlayerReached.AddListener(CheckCoditions);
        }

        PlayerReached.AddListener(()=> { 
            hasPlayerReached = true; 
        });
    }

    void CheckCoditions()
    {
        bool allreached = true;

        foreach (PlayerReaches _condition in Conditions)
        {
            if (!_condition.hasPlayerReached)
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
        if (!hasPlayerReached && other.transform.root.gameObject == m_player)
        {
            CheckCoditions();

            if (canPlayerTrigger)
            {
                PlayerReached.Invoke();
                HideWorldMessage();
            }
            else
            {
                CantTrigger.Invoke();
            }
        }
    }
}
