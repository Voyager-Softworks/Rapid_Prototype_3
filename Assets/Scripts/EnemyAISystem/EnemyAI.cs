using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum AIState
    {
        WANDERING,
        PATROLLING,
        INVESTIGATING,
        SEARCHING,
        PURSUING,
        FLEEING,
        ATTACKING,
        IDLING,
        RECOVERING,
    };
    [Header("Prefab Setup Options")]
    public NavMeshAgent agent;
    public Transform playerTransform;
    public Rigidbody body;
    public Animator anim;



    [Header("Debugging")]
    public Vector3 mostRecentAlertPosition, mostRecentAwarePosition;
    public AIState m_currentState;

    [Header("Vision")]
    [Range(1.0f, 360.0f)]
    public float m_visionAngle;
    public float m_visionDistance;
    public Transform m_headTransform;
    Vector3 m_lookVector;

    [Header("Speed")]
    public float m_wanderSpeed;
    public float m_wanderTurnSpeed;
    public float m_investigateSpeed;
    public float m_investigateTurnSpeed;
    public float m_searchSpeed;
    public float m_searchTurnSpeed;
    public float m_pursueSpeed;
    public float m_pursueTurnSpeed;
    public float m_fleeSpeed;

    [Header("Awareness")]
    public float m_awareness = 0.0f;
    [Range(0.0f, 10.0f)]
    public float m_investigateThreshhold;
    [Range(0.0f, 10.0f)]
    public float m_pursueThreshhold;

    [Range(0.0f, 1.0f)]
    public float m_awarenessDecayRate;

    [Header("Behaviour")]
    public float m_minimumStateDuration;
    float m_stateTimer = 0;

    [Header("Patrol Behaviour")]
    public PatrolAgent m_patrolAgent;

    [Header("Flee Behaviour")]
    public float m_fleeDuration;
    float m_fleetimer;

    [Header("Search Behaviour")]
    public int m_searchChance;

    [Header("Attacking")]
    public float m_attackRadius;
    public float m_attackForceMult;

    public float m_attackHeightOffset;
    public float m_attackDuration;

    float m_attackTimer;
    public float m_attackCooldown;

    float m_cooldownTimer;

    [Header("Idling")]
    [Range(0, 100)]
    public int m_idleChance;
    public float m_idleCooldown;
    float m_idleTimer;

    [Header("Audio")]
    public AudioSource m_SFXsource;
    public AudioSource m_Stepsource;
    public AudioSource m_IdleSource;

    public List<AudioClip> m_WalkSFX;
    public AudioClip m_InjureSFX;
    public AudioClip m_LaunchSFX;
    public AudioClip m_LandSFX;
    public AudioClip m_RetreatSFX;


    // Start is called before the first frame update
    void Start()
    {
        m_currentState = (m_patrolAgent != null ? AIState.PATROLLING : AIState.WANDERING);
        m_idleTimer = m_idleCooldown;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        m_lookVector = m_headTransform.TransformVector(-m_headTransform.forward);
        

        m_cooldownTimer -= Time.deltaTime;
        m_stateTimer -= Time.deltaTime;
        m_awareness -= Time.deltaTime * m_awarenessDecayRate;
        if (m_awareness < 0.0f)
        {
            m_awareness = 0.0f;
        }
        if (m_awareness > 10.0f)
        {
            m_awareness = 10.0f;
        }
        if ((playerTransform.position - m_headTransform.position).magnitude <= m_visionDistance && 
            Vector3.Angle(m_lookVector, (playerTransform.position - m_headTransform.position)) < m_visionAngle / 2)
        {
            if(!Physics.Raycast(m_headTransform.position, 
                (playerTransform.position - m_headTransform.position), 
                (playerTransform.position - m_headTransform.position).magnitude, 
                layerMask: LayerMask.GetMask("Obstacles")))
                {
                    m_awareness += 4.0f * Time.deltaTime;
                }
        }
        if(m_patrolAgent != null)
            m_patrolAgent.SetMoving(m_currentState == AIState.PATROLLING);

        anim.SetBool("WANDERING", false);
        anim.SetBool("PATROLLING", false);
        anim.SetBool("SEARCHING", false);
        anim.SetBool("PURSUING", false);
        anim.SetBool("FLEEING", false);
        anim.SetBool("ATTACKING", false);
        anim.SetBool("RECOVERING", false);

        if (!m_Stepsource.isPlaying && m_currentState == AIState.PURSUING)
        {
            m_Stepsource.Play();
        }
        else if (m_Stepsource.isPlaying && m_currentState != AIState.PURSUING)
        {
            m_Stepsource.Stop();
        }
        if (m_currentState != AIState.ATTACKING)
        {
            agent.enabled = true;
            body.isKinematic = true;
        }

        switch (m_currentState)
        {
            case AIState.WANDERING:
                anim.SetBool("WANDERING", true);
                agent.enabled = true;
                m_idleTimer -= Time.deltaTime;
                if (m_awareness > m_pursueThreshhold && m_stateTimer <= 0)
                {
                    m_currentState = AIState.PURSUING;
                    m_stateTimer = m_minimumStateDuration;
                }

                agent.destination = GetWanderPosition(30.0f, 40.0f, gameObject.transform.forward);
                agent.speed = m_wanderSpeed;
                agent.angularSpeed = m_wanderTurnSpeed;
                if (m_idleTimer <= 0.0f)
                {
                    m_idleTimer = m_idleCooldown;
                    if (Random.Range(1, 100) <= m_idleChance)
                    {
                        m_IdleSource.clip = m_WalkSFX[Random.Range(0, m_WalkSFX.Count)];
                        m_IdleSource.Play();
                    }
                }
                break;
            case AIState.PATROLLING:
                anim.SetBool("WANDERING", true);
                agent.enabled = true;
                m_idleTimer -= Time.deltaTime;
                if (m_awareness > m_pursueThreshhold && m_stateTimer <= 0)
                {
                    m_currentState = AIState.PURSUING;
                    m_stateTimer = m_minimumStateDuration;
                }

                

                if (m_idleTimer <= 0.0f)
                {
                    m_idleTimer = m_idleCooldown;
                    if (Random.Range(1, 100) <= m_idleChance)
                    {
                        m_IdleSource.clip = m_WalkSFX[Random.Range(0, m_WalkSFX.Count)];
                        m_IdleSource.Play();
                    }
                }
                break;
            case AIState.INVESTIGATING:
                anim.SetBool("PATROLLING", true);
                agent.enabled = true;
                if (m_awareness < m_investigateThreshhold && m_stateTimer <= 0)
                {
                    m_currentState = (m_patrolAgent != null ? AIState.PATROLLING : AIState.WANDERING);
                    m_stateTimer = m_minimumStateDuration;
                }
                else if (m_awareness > m_pursueThreshhold && m_stateTimer <= 0)
                {
                    m_currentState = AIState.PURSUING;
                    m_stateTimer = m_minimumStateDuration;
                }
                else
                {
                    agent.speed = m_investigateSpeed;
                    agent.angularSpeed = m_investigateTurnSpeed;
                    agent.destination = GetWanderPosition(30.0f, 30.0f, mostRecentAwarePosition - gameObject.transform.position);
                }
                break;
            case AIState.SEARCHING:
                anim.SetBool("SEARCHING", true);
                agent.enabled = true;
                agent.stoppingDistance = 0.0f;
                agent.angularSpeed = m_searchTurnSpeed;
                agent.speed = m_searchSpeed;
                if (m_awareness > m_pursueThreshhold && m_stateTimer <= 0)
                {
                    m_currentState = AIState.PURSUING;
                    m_stateTimer = m_minimumStateDuration;
                }
                if ((mostRecentAwarePosition - gameObject.transform.position).magnitude <= m_attackRadius * 2 && m_stateTimer <= 0)
                {
                    m_currentState = AIState.INVESTIGATING;
                    m_stateTimer = m_minimumStateDuration;
                }
                agent.destination = mostRecentAlertPosition;
                break;
            case AIState.PURSUING:
                anim.SetBool("PURSUING", true);
                agent.enabled = true;
                agent.destination = playerTransform.position;
                agent.stoppingDistance = m_attackRadius;
                agent.speed = m_pursueSpeed;
                agent.angularSpeed = m_pursueTurnSpeed;
                if (m_awareness <= m_pursueThreshhold && m_stateTimer <= 0)
                {
                    m_currentState = (m_patrolAgent != null ? AIState.PATROLLING : AIState.WANDERING);
                    m_stateTimer = m_minimumStateDuration;
                }
                if ((playerTransform.position - gameObject.transform.position).magnitude <= m_attackRadius && Vector3.Dot((playerTransform.position - gameObject.transform.position).normalized, gameObject.transform.forward) > (1 - (m_visionAngle / 2)) && m_cooldownTimer <= 0 && m_stateTimer <= 0)
                {
                    m_currentState = AIState.ATTACKING;
                    m_stateTimer = m_minimumStateDuration;
                    Attack();
                    m_cooldownTimer = m_attackCooldown;
                }
                break;
            case AIState.FLEEING:
                anim.SetBool("FLEEING", true);
                agent.destination = GetWanderPosition(20.0f, 10.0f, gameObject.transform.position - playerTransform.position);
                agent.speed = m_fleeSpeed;
                m_fleetimer -= Time.deltaTime;
                if (m_fleetimer <= 0 && m_stateTimer <= 0)
                {
                    m_currentState = (m_patrolAgent != null ? AIState.PATROLLING : AIState.WANDERING);
                    m_stateTimer = m_minimumStateDuration;
                }
                break;
            case AIState.RECOVERING:
                anim.SetBool("RECOVERING", true);
                agent.speed = 0.0f;
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Armature|LAND") &&
                    anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && m_stateTimer <= 0)
                {
                    m_currentState = (m_patrolAgent != null ? AIState.PATROLLING : AIState.WANDERING);
                    m_stateTimer = m_minimumStateDuration;

                }
                break;
            case AIState.ATTACKING:
                anim.SetBool("ATTACKING", true);
                m_awareness = 10.0f;
                m_attackTimer -= Time.deltaTime;
                if (m_attackTimer <= 0)
                {
                    m_currentState = AIState.RECOVERING;
                    m_stateTimer = m_minimumStateDuration;

                    agent.enabled = true;
                    m_SFXsource.clip = m_LandSFX;
                    m_SFXsource.Play();
                }
                break;
            case AIState.IDLING:
                agent.speed = 0.0f;
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") &&
                    anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    m_currentState = (m_patrolAgent != null ? AIState.PATROLLING : AIState.WANDERING);
                }
                break;
            default:
                break;
        }
    }

    void Attack()
    {
        body.isKinematic = false;
        Vector3 playerPos = new Vector3(playerTransform.position.x, playerTransform.position.y + m_attackHeightOffset, playerTransform.position.z);
        Vector3 direction = (playerPos - gameObject.transform.position).normalized;
        body.AddForce(direction * m_attackForceMult, ForceMode.Impulse);
        agent.enabled = false;
        m_SFXsource.clip = m_LaunchSFX;
        m_SFXsource.Play();
        //Debug.Log("Attacking!");
        m_attackTimer = m_attackDuration;
    }

    public void RecieveFlee()
    {
        anim.SetTrigger("SHOT");
        m_fleetimer = m_fleeDuration;
        m_currentState = AIState.FLEEING;
        m_SFXsource.clip = m_RetreatSFX;
        m_SFXsource.Play();
    }
    public void RecieveAlert(Transform _alertPos)
    {
        if ((m_currentState == (m_patrolAgent != null ? AIState.PATROLLING : AIState.WANDERING) || m_currentState == AIState.INVESTIGATING) && Random.Range(1, 100) < m_searchChance)
        {
            m_currentState = AIState.SEARCHING;
            agent.destination = _alertPos.position;
            mostRecentAlertPosition = _alertPos.position;
        }

    }

    public void RecievePursuit()
    {
        if (m_currentState != AIState.FLEEING && m_currentState != AIState.ATTACKING)
        {
            m_currentState = AIState.PURSUING;
            m_awareness = 10.0f;
        }
    }

    public void RecieveAware(Transform _awarePos)
    {
        if (m_awareness + Time.deltaTime * 2 <= 7.0f)
        {
            m_awareness += Time.deltaTime * 2;
        }
        else
        {
            //m_awareness = 7.0f;
        }

        mostRecentAwarePosition = _awarePos.position;
        if (m_awareness > m_investigateThreshhold && m_currentState == (m_patrolAgent != null ? AIState.PATROLLING : AIState.WANDERING))
        {
            m_currentState = AIState.INVESTIGATING;
        }

    }

    Vector3 GetWanderPosition(float _offset, float _radius, Vector3 _dir)
    {
        Vector3 origin = this.gameObject.transform.position + (_dir.normalized * _offset);
        return new Vector3((_radius * Mathf.Cos(Random.Range(0, Mathf.PI * 2))) + origin.x, origin.y, _radius * Mathf.Sin(Random.Range(0, Mathf.PI * 2)) + origin.z);
    }

    // void OnTriggerEnter(Collider other)
    // {
    //     if (m_currentState == AIState.ATTACKING)
    //     {
    //         if (other.gameObject.CompareTag("Player"))
    //         {
    //             //Stuff for killing the player
    //             m_currentState = AIState.RECOVERING;

    //             agent.enabled = true;
    //             m_SFXsource.clip = m_LandSFX;
    //             m_SFXsource.Play();
    //         }
    //         else if (other.gameObject.CompareTag("Ground"))
    //         {
    //             m_currentState = AIState.RECOVERING;

    //             agent.enabled = true;
    //             m_SFXsource.clip = m_LandSFX;
    //             m_SFXsource.Play();
    //         }
    //     }
    // }


    /// <summary>
    /// Callback to draw gizmos only if the object is selected.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        
        m_lookVector = m_headTransform.TransformVector(-m_headTransform.forward);
        
        Gizmos.color = Color.yellow;
        Vector3 coneRay1 = Quaternion.AngleAxis(m_visionAngle/2, Vector3.up) * m_lookVector;
        Vector3 coneRay2 = Quaternion.AngleAxis(m_visionAngle/3, Vector3.up) * m_lookVector;
        Vector3 coneRay3 = Quaternion.AngleAxis(m_visionAngle/4, Vector3.up) * m_lookVector;
        Vector3 coneRay4 = Quaternion.AngleAxis(m_visionAngle/5, Vector3.up) * m_lookVector;
        Vector3 coneRay5 = Quaternion.AngleAxis(m_visionAngle/6, Vector3.up) * m_lookVector;
        Vector3 forwardRay = m_headTransform.position + (m_lookVector.normalized * m_visionDistance);
        for (int i = 0; i < 20; i++)
        {
            Gizmos.DrawRay(m_headTransform.position, coneRay1.normalized * m_visionDistance);
            Gizmos.DrawLine((coneRay1.normalized * m_visionDistance) + m_headTransform.position, (coneRay2.normalized * m_visionDistance) + m_headTransform.position);
            Gizmos.DrawLine((coneRay2.normalized * m_visionDistance) + m_headTransform.position, (coneRay3.normalized * m_visionDistance) + m_headTransform.position);
            Gizmos.DrawLine((coneRay3.normalized * m_visionDistance) + m_headTransform.position, (coneRay4.normalized * m_visionDistance) + m_headTransform.position);
            Gizmos.DrawLine((coneRay4.normalized * m_visionDistance) + m_headTransform.position, (coneRay5.normalized * m_visionDistance) + m_headTransform.position);
            Gizmos.DrawLine((coneRay5.normalized * m_visionDistance) + m_headTransform.position, forwardRay);
            Vector3 temp = coneRay1;
            coneRay1 = Quaternion.AngleAxis(18.0f, m_lookVector) * coneRay1;
            coneRay2 = Quaternion.AngleAxis(18.0f, m_lookVector) * coneRay2;
            coneRay3 = Quaternion.AngleAxis(18.0f, m_lookVector) * coneRay3;
            coneRay4 = Quaternion.AngleAxis(18.0f, m_lookVector) * coneRay4;
            coneRay5 = Quaternion.AngleAxis(18.0f, m_lookVector) * coneRay5;
            Gizmos.DrawLine((temp.normalized * m_visionDistance) + m_headTransform.position, (coneRay1.normalized * m_visionDistance) + m_headTransform.position);
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_attackRadius);

        Vector3 playerPos = new Vector3(playerTransform.position.x, playerTransform.position.y + m_attackHeightOffset, playerTransform.position.z);
        Vector3 direction = (playerPos - gameObject.transform.position);
        Gizmos.DrawRay(new Ray(transform.position, direction));
    }
    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    void OnDrawGizmos()
    {


        switch (m_currentState)
        {
            case AIState.WANDERING:
                Gizmos.color = Color.blue;
                break;
            case AIState.INVESTIGATING:
                Gizmos.color = Color.green;
                break;
            case AIState.SEARCHING:
                Gizmos.color = Color.yellow;
                break;
            case AIState.PURSUING:
                Gizmos.color = Color.magenta;
                break;
            case AIState.ATTACKING:
                Gizmos.color = Color.red;
                break;
            case AIState.IDLING:
                Gizmos.color = Color.cyan;
                break;
            case AIState.FLEEING:
                Gizmos.color = Color.white;
                break;
            case AIState.RECOVERING:
                Gizmos.color = Color.gray;
                break;
            default:
                break;
        }
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.up * 20.0f));

        Gizmos.DrawSphere(transform.position + (Vector3.up * 20.0f), 2.0f);
        Gizmos.color = Color.Lerp(Color.blue, Color.red, m_awareness / 10.0f);
        Gizmos.DrawCube(transform.position + (Vector3.up * m_awareness), new Vector3(3.0f, m_awareness * 2.0f, 3.0f));

    }

}

