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

    public float m_attackForceMult;

    public float m_attackHeightOffset;
    public float m_attackDuration;

    float m_attackTimer;

    public AudioSource m_attackSource, m_runSource;

    public float m_speed = 1.5f;
    Vector3 m_lookVector;
    NavMeshAgent m_agent;
    Animator m_anim;

    Rigidbody m_body;

    Transform playerTransform;

    public GameObject m_deathPrefab;
    // Start is called before the first frame update
    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_anim = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        m_body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if((m_attackTimer -= Time.deltaTime ) <= 0.0f)
        {
            m_agent.enabled = true;
            m_agent.speed = m_speed;
            m_attackTimer = 0.0f;
            m_body.isKinematic = false;
            if(!m_runSource.isPlaying)
            {
                m_runSource.Play();
            }
        }
        m_agent.destination = playerTransform.position;
        if ((playerTransform.position - m_headTransform.position).magnitude <= m_visionDistance && Vector3.Angle(transform.forward, (playerTransform.position - m_headTransform.position)) < m_visionAngle / 2)
        {
            if(m_attackTimer <= 0.0f)
            {
                m_anim.SetTrigger("Attack");
                Attack();
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

     void Attack()
    {
        m_body.isKinematic = false;
        Vector3 playerPos = new Vector3(playerTransform.position.x, playerTransform.position.y + m_attackHeightOffset, playerTransform.position.z);
        Vector3 direction = (playerPos - gameObject.transform.position).normalized;
        m_body.AddForce(direction * m_attackForceMult, ForceMode.Impulse);
        m_agent.enabled = false;
        
        //Debug.Log("Attacking!");
        m_attackTimer = m_attackDuration;
    }

    public void Shoot()
    {
        Instantiate(m_deathPrefab, this.transform.position, this.transform.rotation);
        Destroy(this.gameObject);
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
