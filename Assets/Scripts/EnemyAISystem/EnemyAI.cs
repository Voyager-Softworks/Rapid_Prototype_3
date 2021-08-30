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
    public float m_wanderSpeed;
    public float m_wanderTurnSpeed;
    public float m_patrolSpeed;
    public float m_patrolTurnSpeed;
    public float m_searchSpeed;
    public float m_searchTurnSpeed;
    public float m_pursueSpeed;
    public float m_pursueTurnSpeed;
    public float m_fleeSpeed;

    [Header("Awareness")]
    public float m_awareness = 0.0f;
    [Range(0.0f, 10.0f)]
    public float m_patrolThreshhold;
    [Range(0.0f, 10.0f)]
    public float m_pursueThreshhold;

    [Range(0.0f, 1.0f)]
    public float m_awarenessDecayRate;

    [Header("Flee Behaviour")]
    public float m_fleeDuration;
    float m_fleetimer;

    [Header("Search Behaviour")]
    public int m_searchChance;

    [Header("Attacking")]
    public float m_attackRadius;
    public float m_attackForceMult;

    public float m_attackHeightOffset;

    [Header("Idling")]
    [Range(0, 100)]
    public int m_idleChance;
    public float m_idleCooldown;
    float m_idleTimer;

    [Header("Audio")]
    public AudioSource m_SFXsource;
    public AudioSource m_Stepsource;

    public List<AudioClip> m_WalkSFX;
    public AudioClip m_InjureSFX;
    public AudioClip m_LaunchSFX;
    public AudioClip m_LandSFX;
    public AudioClip m_RetreatSFX;


    // Start is called before the first frame update
    void Start()
    {
        m_currentState = AIState.WANDERING;
        m_idleTimer = m_idleCooldown;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        m_awareness -= Time.deltaTime * m_awarenessDecayRate;
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
            m_awareness += 4.0f * Time.deltaTime;
        }

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
        }

        switch (m_currentState)
        {
            case AIState.WANDERING:
                anim.SetBool("WANDERING", true);
                m_idleTimer -= Time.deltaTime;
                if (m_awareness > m_pursueThreshhold)
                {
                    m_currentState = AIState.PURSUING;
                }

                agent.destination = GetWanderPosition(30.0f, 40.0f, gameObject.transform.forward);
                agent.speed = m_wanderSpeed;
                agent.angularSpeed = m_wanderTurnSpeed;
                if (m_idleTimer <= 0.0f)
                {
                    m_idleTimer = m_idleCooldown;
                    if (Random.Range(1, 100) <= m_idleChance)
                    {
                        m_currentState = AIState.IDLING;
                    }
                }
                break;
            case AIState.PATROLLING:
                anim.SetBool("PATROLLING", true);
                if (m_awareness < m_patrolThreshhold)
                {
                    m_currentState = AIState.WANDERING;
                }
                else if (m_awareness > m_pursueThreshhold)
                {
                    m_currentState = AIState.PURSUING;
                }
                else
                {
                    agent.speed = m_patrolSpeed;
                    agent.angularSpeed = m_patrolTurnSpeed;
                    agent.destination = GetWanderPosition(30.0f, 30.0f, mostRecentAwarePosition - gameObject.transform.position);
                }
                break;
            case AIState.SEARCHING:
                anim.SetBool("SEARCHING", true);
                agent.stoppingDistance = 0.0f;
                agent.angularSpeed = m_searchTurnSpeed;
                agent.speed = m_searchSpeed;
                if (m_awareness > m_pursueThreshhold)
                {
                    m_currentState = AIState.PURSUING;
                }
                if ((mostRecentAwarePosition - gameObject.transform.position).magnitude <= m_attackRadius * 2)
                {
                    m_currentState = AIState.PATROLLING;
                }
                agent.destination = mostRecentAlertPosition;
                break;
            case AIState.PURSUING:
                anim.SetBool("PURSUING", true);

                agent.destination = playerTransform.position;
                agent.stoppingDistance = m_attackRadius;
                agent.speed = m_pursueSpeed;
                agent.angularSpeed = m_pursueTurnSpeed;
                if (m_awareness <= m_pursueThreshhold)
                {
                    m_currentState = AIState.WANDERING;
                }
                if ((playerTransform.position - gameObject.transform.position).magnitude <= m_attackRadius && body == null)
                {
                    m_currentState = AIState.ATTACKING;
                    Attack();
                }
                break;
            case AIState.FLEEING:
                anim.SetBool("FLEEING", true);
                agent.destination = GetWanderPosition(20.0f, 10.0f, gameObject.transform.position - playerTransform.position);
                agent.speed = m_fleeSpeed;
                m_fleetimer -= Time.deltaTime;
                if (m_fleetimer <= 0)
                {
                    m_currentState = AIState.WANDERING;
                }
                break;
            case AIState.RECOVERING:
                anim.SetBool("RECOVERING", true);
                agent.speed = 0.0f;
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Armature|LAND") &&
                    anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    m_currentState = AIState.WANDERING;

                }
                break;
            case AIState.ATTACKING:
                anim.SetBool("ATTACKING", true);
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
        m_SFXsource.clip = m_LaunchSFX;
        m_SFXsource.Play();
    }

    public void RecieveFlee()
    {
        m_fleetimer = m_fleeDuration;
        m_currentState = AIState.FLEEING;
        m_SFXsource.clip = m_RetreatSFX;
        m_SFXsource.Play();
    }
    public void RecieveAlert(Transform _alertPos)
    {
        if ((m_currentState == AIState.WANDERING || m_currentState == AIState.PATROLLING) && Random.Range(1, 100) < m_searchChance)
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
        if (m_awareness > m_patrolThreshhold && m_currentState == AIState.WANDERING)
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
                m_currentState = AIState.RECOVERING;
                Destroy(body);
                agent.enabled = true;
                m_SFXsource.clip = m_LandSFX;
                m_SFXsource.Play();
            }
            else if (other.gameObject.CompareTag("Ground"))
            {
                m_currentState = AIState.RECOVERING;
                Destroy(body);
                agent.enabled = true;
                m_SFXsource.clip = m_LandSFX;
                m_SFXsource.Play();
            }
        }
    }


    /// <summary>
    /// Callback to draw gizmos only if the object is selected.
    /// </summary>
    void OnDrawGizmosSelected()
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
            case AIState.PATROLLING:
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

