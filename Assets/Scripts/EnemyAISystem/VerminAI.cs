using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VerminAI : MonoBehaviour
{

    [Header("Vision")]
    [Range(1.0f, 360.0f)]
    public float m_visionAngle;
    public float m_visionDistance;
    public Transform m_headTransform;

    public float m_attackCooldown = 0.0f;
    float m_attackCooldownTimer = 0.0f;

    public AudioSource m_attackSource, m_runSource;
    Vector3 m_lookVector;
    NavMeshAgent m_agent;
    Animator m_anim;

    Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_anim = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if((m_attackCooldownTimer -= Time.deltaTime ) <= 0.0f)
        {
            m_agent.speed = 1.5f;
            m_attackCooldownTimer = 0.0f;
            if(!m_runSource.isPlaying)
            {
                m_runSource.Play();
            }
        }
        m_agent.destination = playerTransform.position;
        if ((playerTransform.position - m_headTransform.position).magnitude <= m_visionDistance && Vector3.Angle(transform.forward, (playerTransform.position - m_headTransform.position)) < m_visionAngle / 2)
        {
            if(m_attackCooldownTimer <= 0.0f)
            {
                m_anim.SetTrigger("Attack");
                m_attackCooldownTimer = m_attackCooldown;
                m_agent.speed = 0.0f;
                m_attackSource.Play();
                m_runSource.Stop();
            }
        }
        
    }

     void OnTriggerEnter(Collider other)
    {
        
            if (other.transform.root.gameObject.CompareTag("Player"))
            {
                other.transform.root.gameObject.GetComponent<PlayerDeath>().KillPlayer(PlayerDeath.EnemyType.RAT, this.transform.position);
                Destroy(this.gameObject);
            }
            
        
    }

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
    }
}
