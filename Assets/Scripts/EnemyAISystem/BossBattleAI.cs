using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossBattleAI : MonoBehaviour
{
    public bool shoottest = false;
    
    public Vector3 m_arenaPosition;

    public Vector3 m_nextSpawnPosition;
    public Vector3 m_nextRetreatPosition;
    public float m_arenaRadius;
    public float m_cooldownTime;

    public float m_chargeSpeed;
    float m_cooldownTimer;

    public float m_injureAnimLength;
    float m_injureAnimTimer;

    public float m_screenShakeRange;
    
    Animator m_anim;
    NavMeshAgent m_agent;

    Transform m_playerTransform;

    public AudioSource m_injuredSFX, m_dieSFX, m_footstepSFX;
    public List<AudioClip> m_footstepClips;
    SFX_Effect m_effect;

    public enum BossAIState
    {
        CHARGING,
        INJURED,
        RETREATING,
        WAITING,
        INACTIVE,
        DEAD,
    };
    
    void NewPositions()
    {
        float retreat = Random.Range(0, Mathf.PI * 2);
        float spawn = Random.Range(0, Mathf.PI * 2);
        m_nextSpawnPosition = new Vector3((m_arenaRadius * Mathf.Cos(spawn)) + m_arenaPosition.x, m_arenaPosition.y, m_arenaRadius * Mathf.Sin(spawn) + m_arenaPosition.z);
        m_nextRetreatPosition = new Vector3((m_arenaRadius * Mathf.Cos(retreat)) + m_arenaPosition.x, m_arenaPosition.y, m_arenaRadius * Mathf.Sin(retreat) + m_arenaPosition.z);
    }

    public BossAIState m_currState;
    // Start is called before the first frame update
    void Start()
    {
        NewPositions();
        m_currState = BossAIState.INACTIVE;
        m_playerTransform = GameObject.FindWithTag("Player").transform;
        m_anim = GetComponent<Animator>();
        m_agent = GetComponent<NavMeshAgent>();
        m_effect = GetComponent<SFX_Effect>();
    }

    public void StartBattle()
    {
        if (m_currState == BossAIState.INACTIVE)
        {
            this.transform.position = m_nextSpawnPosition;
            m_currState = BossAIState.CHARGING;
        }
    }

    public void Footstep()
    {
        m_footstepSFX.clip = m_footstepClips[Random.Range(0, m_footstepClips.Count)];
        m_footstepSFX.Play();
        if ((this.transform.position - m_playerTransform.position).magnitude < m_screenShakeRange)
        {
            m_effect.m_effectsList[0].m_screenShakeAmplitude = (new Vector2(0.2f, 0.2f) * (1.0f - ((this.transform.position - m_playerTransform.position).magnitude / m_screenShakeRange)));
            m_effect.Play();
        }
    }

    public void Shoot()
    {
        if (m_currState == BossAIState.CHARGING)
        {
            
            m_currState = BossAIState.INJURED;
            m_anim.SetTrigger("Shot");
            m_injureAnimTimer = m_injureAnimLength;
            m_injuredSFX.Play();
        }
    }

    public void Kill()
    {
        m_currState = BossAIState.DEAD;
        m_anim.SetTrigger("Dead");
        m_agent.speed = 0.0f;
        m_agent.angularSpeed = 0.0f;
        m_dieSFX.Play();
    }

    void OnTriggerEnter(Collider other)
    {
        if (m_currState == BossAIState.CHARGING)
        {
            if (other.transform.root.gameObject.CompareTag("Player"))
            {
                other.transform.root.gameObject.GetComponent<PlayerDeath>().KillPlayer(PlayerDeath.EnemyType.BEAR, this.transform.position);
                Destroy(this.gameObject);
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_currState)
        {
            case BossAIState.CHARGING:
                m_agent.SetDestination(m_playerTransform.position);
                m_agent.speed = m_chargeSpeed;
                if (shoottest)
                {
                    Shoot();
                }
            break;
            case BossAIState.INJURED:
                if ((m_injureAnimTimer -= Time.deltaTime) < 0.0f)
                {
                    m_currState = BossAIState.RETREATING;
                    m_agent.SetDestination(m_nextRetreatPosition);
                    m_agent.speed = m_chargeSpeed;
                }
                else
                {
                    m_agent.speed = 0.0f;
                }
            break;
            case BossAIState.RETREATING:
                if ((m_nextRetreatPosition - this.transform.position).magnitude < 1.0f)
                {
                    m_currState = BossAIState.WAITING;
                    m_cooldownTimer = m_cooldownTime;
                }
            break;
            case BossAIState.WAITING:
                if (((int)Time.timeSinceLevelLoad % 2) == 0)
                {
                    NewPositions();
                }
                if ((m_cooldownTimer -= Time.deltaTime) < 0.0f)
                {
                    m_currState = BossAIState.CHARGING;
                    this.transform.position = m_nextSpawnPosition;
                }
            break;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        if (m_currState == BossAIState.WAITING)
        {
            if(m_playerTransform != null) Gizmos.DrawLine(m_nextRetreatPosition, m_playerTransform.position);
        }
        else
        {
            Gizmos.DrawLine(this.transform.position, m_nextRetreatPosition);
        }
        
        Gizmos.color = Color.green;
        if (m_currState == BossAIState.WAITING)
        {
            if(m_playerTransform != null) Gizmos.DrawLine(m_nextSpawnPosition, m_playerTransform.position);
        }
        else
        {
            if(m_playerTransform != null) Gizmos.DrawLine(this.transform.position, m_playerTransform.position);
        }
    }
}
