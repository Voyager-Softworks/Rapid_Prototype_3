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
    }
    public GameObject m_catDeath;
    PlayerMovement m_pMovement;
    MouseLook m_mouseLook;

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

    public void KillPlayer(EnemyType _type)
    {
        switch (_type)
        {
            case EnemyType.CAT:
            m_catDeath.SetActive(true);
            break;
            case EnemyType.RAT:
            break;
            default:
            break;
        }
        m_pMovement.enabled = false;
        m_mouseLook.enabled = false;
        m_fading = true;
    }

    void BeginFade()
    {
        m_fading = true;
    }
}
