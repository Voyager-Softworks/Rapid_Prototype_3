using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JerrycanManager : MonoBehaviour
{
    [Header("Cans")]
    public List<GameObject> m_jerryCans;

    [Header("Meteor")]
    public GameObject m_meteor;
    public List<GameObject> m_meteorCans;
    public float m_totalBlown;
    public int m_numToWin = 3;

    [Header("Player")]
    public GameObject m_player;
    public GameObject m_playerCan;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
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
                if (_can.activeSelf) _can.GetComponent<PlayerInteracts>().SetCanTrigger(true);
            }
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
                _can.SetActive(false);
                m_totalBlown++;
            }
        }

        if (m_totalBlown >= m_numToWin)
        {
            m_meteor.SetActive(false);
        }
    }
}
