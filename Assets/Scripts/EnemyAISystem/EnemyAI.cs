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

    public Animator anim;

    Rigidbody body;

    [Header("Debugging")]
    public Vector3 mostRecentAlertPosition, mostRecentAwarePosition;
    public AIState m_currentState;

    [Header("Vision")]
    [Range(0.1f, 1)]
    public float m_visionCone;
    public float m_visionDistance;

    [Header("Speed")]
    public float m_wanderSpeed, m_patrolSpeed, m_searchSpeed, m_pursueSpeed, m_fleeSpeed;

    [Header("Awareness")]
    public float m_awareness = 0.0f;
    public float m_awarenessThreshhold;

    [Header("Flee Behaviour")]
    public float m_fleeDuration;
    float m_fleetimer;

    [Header("Attacking")]
    public float m_attackRadius;
    public float m_attackForceMult;

    public float m_attackHeightOffset;

    [Header("Idling")]
    [Range(0, 100)]
    public int m_idleChance;
    public float m_idleCooldown;
    float m_idleTimer;


    // Start is called before the first frame update
    void Start()
    {
        m_currentState = AIState.WANDERING;
        m_idleTimer = m_idleCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        m_awareness -= Time.deltaTime;
        if (m_awareness < 0.0f)
        {
            m_awareness = 0.0f;
        }
        if (m_awareness > 10.0f)
        {
            m_awareness = 10.0f;
        }
        if ((playerTransform.position - gameObject.transform.position).magnitude <= m_visionDistance && Vector3.Dot((playerTransform.position - gameObject.transform.position).normalized, gameObject.transform.forward) > (1 - (m_visionCone / 2)))
        {
            m_awareness = 10.0f;

        }
        switch (m_currentState)
        {
            case AIState.WANDERING:
                m_idleTimer -= Time.deltaTime;
                if (m_awareness > 7.0f)
                {
                    m_currentState = AIState.PURSUING;
                }

                agent.destination = GetWanderPosition(30.0f, 40.0f, gameObject.transform.forward);
                agent.speed = m_wanderSpeed;
                if (m_idleTimer <= 0.0f)
                {
                    m_idleTimer = m_idleCooldown;
                    if (Random.Range(0, 100) <= m_idleChance)
                    {
                        m_currentState = AIState.IDLING;
                    }
                }
                break;
            case AIState.PATROLLING:
                if (m_awareness == 0.0f)
                {
                    m_currentState = AIState.WANDERING;
                }
                else if (m_awareness > 7.0f)
                {
                    m_currentState = AIState.PURSUING;
                }
                else
                {
                    agent.speed = m_patrolSpeed;
                    agent.destination = GetWanderPosition(30.0f, 30.0f, mostRecentAwarePosition - gameObject.transform.position);
                }
                break;
            case AIState.SEARCHING:
                if (m_awareness > 7.0f)
                {
                    m_currentState = AIState.PURSUING;
                }
                agent.destination = mostRecentAlertPosition;
                break;
            case AIState.PURSUING:

                agent.destination = playerTransform.position;
                agent.stoppingDistance = m_attackRadius;
                agent.speed = m_pursueSpeed;
                if (m_awareness <= 7.0f)
                {
                    m_currentState = AIState.WANDERING;
                }
                if ((playerTransform.position - gameObject.transform.position).magnitude <= m_attackRadius)
                {
                    m_currentState = AIState.ATTACKING;
                    Attack();
                }
                break;
            case AIState.FLEEING:
                agent.destination = GetWanderPosition(20.0f, 10.0f, gameObject.transform.position - playerTransform.position);
                agent.speed = m_fleeSpeed;
                m_fleetimer -= Time.deltaTime;
                if (m_fleetimer <= 0)
                {
                    m_currentState = AIState.WANDERING;
                }
                break;
            case AIState.RECOVERING:
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Recovery") &&
                    anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    m_currentState = AIState.WANDERING;
                }
                break;
            case AIState.ATTACKING:
                m_awareness = 10.0f;
                break;
            case AIState.IDLING:
                agent.speed = 0.0f;
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") &&
                    anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    m_currentState = AIState.WANDERING;
                }
                break;
            default:
                break;
        }
    }

    void Attack()
    {
        body = gameObject.AddComponent<Rigidbody>();
        Vector3 playerPos = new Vector3(playerTransform.position.x, playerTransform.position.y + m_attackHeightOffset, playerTransform.position.z);
        Vector3 direction = (playerPos - gameObject.transform.position).normalized;
        body.AddForce(direction * m_attackForceMult, ForceMode.Impulse);
        agent.enabled = false;
    }

    public void RecieveFlee()
    {
        m_fleetimer = m_fleeDuration;
        m_currentState = AIState.FLEEING;
    }
    public void RecieveAlert(Transform _alertPos)
    {
        if (m_currentState == AIState.WANDERING || m_currentState == AIState.PATROLLING)
        {
            m_currentState = AIState.SEARCHING;
            agent.destination = _alertPos.position;
            mostRecentAlertPosition = _alertPos.position;
        }

    }

    public void RecievePursuit()
    {
        if (m_currentState != AIState.FLEEING)
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
            m_awareness = 7.0f;
        }

        mostRecentAwarePosition = _awarePos.position;
        if (m_awareness > m_awarenessThreshhold && m_currentState == AIState.WANDERING)
        {
            m_currentState = AIState.PATROLLING;
        }

    }

    Vector3 GetWanderPosition(float _offset, float _radius, Vector3 _dir)
    {
        Vector3 origin = this.gameObject.transform.position + (_dir.normalized * _offset);
        return new Vector3((_radius * Mathf.Cos(Random.Range(0, Mathf.PI * 2))) + origin.x, origin.y, _radius * Mathf.Sin(Random.Range(0, Mathf.PI * 2)) + origin.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (m_currentState == AIState.ATTACKING)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                //Stuff for killing the player
            }
            else if (other.gameObject.CompareTag("Ground"))
            {
                m_currentState = AIState.RECOVERING;
                Destroy(body);
                agent.enabled = true;
            }
        }
    }

    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 leftRay = Quaternion.AngleAxis(m_visionCone * -90.0f, Vector3.up) * transform.forward;
        Vector3 rightRay = Quaternion.AngleAxis(m_visionCone * 90.0f, Vector3.up) * transform.forward;
        Vector3[] curvepositions = new Vector3[5];
        curvepositions[0] = transform.position + (leftRay.normalized * m_visionDistance);
        curvepositions[4] = transform.position + (rightRay.normalized * m_visionDistance);
        curvepositions[2] = transform.position + (transform.forward.normalized * m_visionDistance);


        Gizmos.DrawRay(transform.position, leftRay.normalized * m_visionDistance);
        Gizmos.DrawRay(transform.position, rightRay.normalized * m_visionDistance);
        Gizmos.DrawLine(curvepositions[0], curvepositions[2]);
        Gizmos.DrawLine(curvepositions[2], curvepositions[4]);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_attackRadius);

        Vector3 playerPos = new Vector3(playerTransform.position.x, playerTransform.position.y + m_attackHeightOffset, playerTransform.position.z);
        Vector3 direction = (playerPos - gameObject.transform.position);
        Gizmos.DrawRay(new Ray(transform.position, direction));

    }

}

