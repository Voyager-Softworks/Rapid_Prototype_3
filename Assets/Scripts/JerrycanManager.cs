using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class JerrycanManager : MonoBehaviour
{
    public enum Stage
    {
        Arrived,
        Looked,
        Placed,
        Battle
    }

    public UnityEvent FirstBlown;
    public UnityEvent AllBlown;

    [Header("Cans")]
    public List<GameObject> m_jerryCans;

    [Header("Meteor")]
    public GameObject m_meteor;
    public List<GameObject> m_meteorCans;
    public AudioClip m_meteorScream;
    public GameObject m_meteorParticles;
    public float m_totalBlown;
    public int m_numToWin = 3;

    [Header("Player")]
    public GameObject m_player;
    public GameObject m_playerCan;

    [Header("Progression")]
    public Stage m_currentStage = Stage.Arrived;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void NextStage()
    {
        m_currentStage++;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCans();
    }

    public void UpdateCans()
    {
        switch (m_currentStage)
        {
            case Stage.Arrived:
                foreach (GameObject _can in m_jerryCans)
                {
                    if (_can.activeSelf)
                    {
                        PlayerInteracts pi = _can.GetComponent<PlayerInteracts>();
                        pi.SetCanTrigger(false);
                        pi.SetWorldMessage("This might be useful...");
                    }
                }
                break;

            case Stage.Looked:
                if (m_playerCan.activeSelf)
                {
                    foreach (GameObject _can in m_jerryCans)
                    {
                        if (_can.activeSelf)
                        {
                            PlayerInteracts pi = _can.GetComponent<PlayerInteracts>();
                            pi.SetCanTrigger(false);
                            pi.SetWorldMessage("I already have one");
                        }
                    }
                }
                else
                {
                    foreach (GameObject _can in m_jerryCans)
                    {
                        if (_can.activeSelf)
                        {
                            PlayerInteracts pi = _can.GetComponent<PlayerInteracts>();
                            if (pi.AreConditionsMet())
                            {
                                pi.SetCanTrigger(true);
                            }
                            else
                            {
                                pi.SetCanTrigger(false);
                            }
                            pi.SetWorldMessage("I already have one");
                        }
                    }

                    foreach (GameObject _can in m_meteorCans)
                    {
                        if (_can.activeSelf) { NextStage(); break; }
                    }
                }
                break;

            case Stage.Placed:
                foreach (GameObject _can in m_jerryCans)
                {
                    if (_can.activeSelf)
                    {
                        PlayerInteracts pi = _can.GetComponent<PlayerInteracts>();
                        pi.SetCanTrigger(false);
                        pi.SetWorldMessage("I already placed one");
                    }
                }

                if (m_totalBlown >= 1)
                {
                    FirstBlown.Invoke();
                    NextStage();
                }
                break;

            case Stage.Battle:
                if (m_playerCan.activeSelf)
                {
                    foreach (GameObject _can in m_jerryCans)
                    {
                        if (_can.activeSelf) _can.GetComponent<PlayerInteracts>().SetCanTrigger(false);
                    }
                }
                else
                {
                    foreach (GameObject _can in m_jerryCans)
                    {
                        if (_can.activeSelf)
                        {
                            PlayerInteracts pi = _can.GetComponent<PlayerInteracts>();
                            if (pi.AreConditionsMet())
                            {
                                pi.SetCanTrigger(true);
                            }
                            else
                            {
                                pi.SetCanTrigger(false);
                            }
                        }
                    }
                }
                break;
            default:
                break;
        }
    }

    public void TryPlaceCan()
    {
        if (m_playerCan.activeSelf)
        {
            GameObject closestCanSpot = null;
            foreach (GameObject _can in m_meteorCans)
            {
                if (_can.activeSelf) continue;

                if (closestCanSpot == null || 
                    Vector3.Distance(m_player.transform.position, _can.transform.position) < Vector3.Distance(m_player.transform.position, closestCanSpot.transform.position))
                {
                    closestCanSpot = _can;
                }
            }

            if (closestCanSpot)
            {
                closestCanSpot.SetActive(true);
                m_playerCan.SetActive(false);
            }
        }
        else
        {
            m_meteor.GetComponent<PlayerInteracts>().DisplayWorldMessage();
        }
    }

    public void BlowMeteorCans()
    {
        foreach (GameObject _can in m_meteorCans)
        {
            if (_can.activeSelf)
            {
                Destroy(Instantiate(m_meteorParticles, _can.transform.position, Quaternion.identity, null), 5.0f);
                _can.SetActive(false);
                m_totalBlown++;
            }
        }

        if (m_totalBlown > 0)
        {
            m_meteor.transform.parent.GetComponent<Animator>().SetTrigger("DoInjured");
            m_meteor.transform.parent.GetComponent<AudioSource>().PlayOneShot(m_meteorScream);
        }

        if (m_totalBlown >= m_numToWin)
        {
            m_meteor.transform.parent.GetComponent<Animator>().SetTrigger("DoDeath");
            //m_meteor.SetActive(false);
            AllBlown.Invoke();
        }
    }
}
