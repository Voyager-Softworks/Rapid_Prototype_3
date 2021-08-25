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
    };
    [Header("Prefab Setup Options")]
    public NavMeshAgent agent;
    public Transform playerTransform;

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

    // Start is called before the first frame update
    void Start()
    {
        m_currentState = AIState.WANDERING;
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
                if (m_awareness > 7.0f)
                {
                    m_currentState = AIState.PURSUING;
                }
                agent.destination = GetWanderPosition(10.0f, 5.0f, gameObject.transform.forward);
                agent.speed = m_wanderSpeed;
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
                agent.speed = m_pursueSpeed;
                if (m_awareness <= 7.0f)
                {
                    m_currentState = AIState.WANDERING;
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
            default:
                break;
        }
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

    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    void OnDrawGizmos()
    {
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

    }

}

