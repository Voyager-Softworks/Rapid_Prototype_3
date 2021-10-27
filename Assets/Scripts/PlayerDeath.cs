using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerDeath : MonoBehaviour
{
    public enum EnemyType
    {
        CAT,
        RAT,
        BEAR,
    }
    public GameObject m_deathAnimationsObj;
    public GameObject m_catDeath, m_verminDeath, m_bearDeath;
    PlayerMovement m_pMovement;
    MouseLook m_mouseLook;

    public bool m_godmode = false;

    public UnityEvent OnDeathAnimComplete;

    float m_fadeTimer;
    bool m_fading = false;
    // Start is called before the first frame update
    void Start()
    {
        m_pMovement = GetComponent<PlayerMovement>();
        m_mouseLook = GetComponent<MouseLook>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ToggleGodMode()
    {
        if (m_godmode)
        {
            m_godmode = false;
        }
        else
        {
            m_godmode = true;
        }
    }
    public void KillPlayer(EnemyType _type, Vector3 _attackerPos)
    {
        if (m_godmode) return;
        
        switch (_type)
        {
            case EnemyType.CAT:
            m_catDeath.SetActive(true);
            break;
            case EnemyType.RAT:
            m_verminDeath.SetActive(true);
            break;
            case EnemyType.BEAR:
            m_bearDeath.SetActive(true);
            break;
            default:
            break;
        }
        m_deathAnimationsObj.transform.LookAt(_attackerPos, Vector3.up);
        m_pMovement.enabled = false;
        m_mouseLook.enabled = false;
        m_fading = true;
    }

    void BeginFade()
    {
        m_fading = true;
    }
}
