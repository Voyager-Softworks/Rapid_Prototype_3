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


    [Header("Speed")]
    public float m_wanderSpeed, m_patrolSpeed, m_searchSpeed, m_pursueSpeed, m_fleeSpeed;

    [Header("Awareness")]
    public float m_awareness = 0.0f;
    public float m_awarenessThreshhold;


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
        switch (m_currentState)
        {
            case AIState.WANDERING:
                agent.destination = GetWanderPosition(10.0f, 5.0f, gameObject.transform.forward);
                agent.speed = m_wanderSpeed;
                break;
            case AIState.PATROLLING:
                if (m_awareness == 0.0f)
                {
                    m_currentState = AIState.WANDERING;
                }
                else
                {
                    agent.speed = m_patrolSpeed;
                    agent.destination = GetWanderPosition(30.0f, 30.0f, mostRecentAwarePosition - gameObject.transform.position);
                }
                break;
            case AIState.SEARCHING:
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
                break;
            default:
                break;
        }
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
}
