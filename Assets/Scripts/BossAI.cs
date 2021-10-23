using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour
{
    public bool m_isAwake;
    public bool m_isTriggered;
    public float m_wakeAnimDuration;
    public NavMeshAgent m_agent;
    public AudioSource m_source;
    public float m_audioDelay;
    float m_timer;
    Transform m_playerTransform;
    SkinnedMeshRenderer m_renderer;
    public Animator m_anim;

    public bool m_manualWake = false;
    // Start is called before the first frame update
    void Start()
    {
        m_timer = m_wakeAnimDuration;
        m_agent.enabled = false;
        m_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

    }

    // Update is called once per frame
    void Update()
    {
        if (m_isTriggered && !m_isAwake)
        {
            m_timer -= Time.deltaTime;
            if (m_timer <= 0)
            {
                m_isAwake = true;
                m_agent.enabled = true;
            }
        }
        if (m_isAwake)
        {
            m_agent.SetDestination(m_playerTransform.position);
        }
        if (m_manualWake)
        {
            m_manualWake = false;
            TriggerWake();
        }
    }

    public void TriggerWake()
    {
        if (m_isTriggered) return;
        m_isTriggered = true;
        m_anim.SetFloat("WakeSpeed", 1.0f);
        m_source.PlayDelayed(m_audioDelay);
    }
}
