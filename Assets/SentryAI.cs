using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryAI : MonoBehaviour
{
    public GameObject m_verminPrefab;
    public int m_verminGroupCount;
    public int m_verminGroupSize;

    public float m_verminSpreadRadius;
    public float m_groupSpreadRadius;

    public Transform m_headTransform;
    Transform m_playerTransform;
    Vector3 m_lookVector;

    public float m_viewDistance;
    public float m_viewAngle;

    public float m_lookBackInterval;
    float m_lookBackTimer;

    public float m_screechDuration;
    float m_screechTimer;

    public float m_flightDuration;

    public float m_flightAltitude;

    public float m_snappingDistance;
    float m_flightTimer;

    Vector3 m_direction;
    Vector3 m_origPos;
    Quaternion m_origRot;

    public Animator m_anim;

    public SFX_Effect m_ScreamFX;

    public enum SentryState
    {
        SEARCHING,
        DETECTED,
        FLYING,

    }

    public SentryState m_state;

    void SpawnVermin()
    {
        for (int i = 0; i < m_verminGroupCount; i++)
        {
            Vector3 origin = m_playerTransform.position;
            Vector3 groupOrigin = new Vector3((m_groupSpreadRadius * Mathf.Cos(Random.Range(0, Mathf.PI * 2))) + origin.x, origin.y, m_groupSpreadRadius * Mathf.Sin(Random.Range(0, Mathf.PI * 2)) + origin.z);
            for (int j = 0; j < m_verminGroupSize; j++)
            {
                Instantiate(m_verminPrefab, 
                new Vector3((m_verminSpreadRadius * Mathf.Cos(Random.Range(0, Mathf.PI * 2))) + groupOrigin.x, groupOrigin.y, m_verminSpreadRadius * Mathf.Sin(Random.Range(0, Mathf.PI * 2)) + groupOrigin.z),
                Quaternion.identity * Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up)
                );
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_state = SentryState.SEARCHING;
        m_lookBackTimer = m_lookBackInterval;
        m_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        m_origPos = transform.position;
        m_origRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        m_lookVector = m_headTransform.TransformVector(-m_headTransform.forward);
        
        switch (m_state)
        {
        case SentryState.SEARCHING: 
            if((m_lookBackTimer -= Time.deltaTime) < 0.0f)
            {
                m_anim.SetTrigger("LookBack");
                m_lookBackTimer = m_lookBackInterval;
            }
            
            if((m_playerTransform.position - m_headTransform.position).magnitude <= m_viewDistance && Vector3.Angle(transform.forward, (m_playerTransform.position - m_headTransform.position)) < m_viewAngle / 2)
            {
                m_state = SentryState.DETECTED;
                m_screechTimer = m_screechDuration;
                m_anim.SetTrigger("Detect");
                m_ScreamFX.Play();
            }
            break;
        case SentryState.DETECTED:
            if((m_screechTimer -= Time.deltaTime) < 0.0f)
            {
                m_state = SentryState.FLYING;
                SpawnVermin();
                m_flightTimer = m_flightDuration;
            }
            break;
        case SentryState.FLYING:
            if ((m_flightTimer -= Time.deltaTime) < 0.0f)
            {
                m_direction = (m_origPos - transform.position).normalized;
                if ((m_origPos - transform.position).magnitude < m_snappingDistance)
                {
                    transform.position = m_origPos;
                    transform.rotation = m_origRot;
                    m_state = SentryState.SEARCHING;
                    m_anim.SetTrigger("TouchPost");
                    break;
                }
            }
            else
            {
                m_direction += GetWanderPosition(20.0f, 5.0f, transform.forward) - transform.position;
                m_direction.Normalize();
                if(transform.position.y < m_flightAltitude) m_direction.y = 0.2f;
                if(transform.position.y > m_flightAltitude) m_direction.y = -0.2f;
            }
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(m_direction, Vector3.up), 10.0f);
            transform.position += m_direction * Time.deltaTime * 5.0f;
            
            break;
        default:
            break;
        }
    }


    Vector3 GetWanderPosition(float _offset, float _radius, Vector3 _dir)
    {
        Vector3 origin = this.gameObject.transform.position + (_dir.normalized * _offset);
        return new Vector3((_radius * Mathf.Cos(Random.Range(0, Mathf.PI * 2))) + origin.x, origin.y, _radius * Mathf.Sin(Random.Range(0, Mathf.PI * 2)) + origin.z);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (m_state == SentryState.FLYING && collision.collider.gameObject.CompareTag("Perch"))
        {
            
        }
    }

    void OnDrawGizmosSelected()
    {
        m_lookVector = m_headTransform.TransformVector(-m_headTransform.forward);
        
        Gizmos.color = Color.yellow;
        Vector3 coneRay1 = Quaternion.AngleAxis(m_viewAngle/2, Vector3.up) * m_lookVector;
        Vector3 coneRay2 = Quaternion.AngleAxis(m_viewAngle/3, Vector3.up) * m_lookVector;
        Vector3 coneRay3 = Quaternion.AngleAxis(m_viewAngle/4, Vector3.up) * m_lookVector;
        Vector3 coneRay4 = Quaternion.AngleAxis(m_viewAngle/5, Vector3.up) * m_lookVector;
        Vector3 coneRay5 = Quaternion.AngleAxis(m_viewAngle/6, Vector3.up) * m_lookVector;
        Vector3 forwardRay = m_headTransform.position + (m_lookVector.normalized * m_viewDistance);
        for (int i = 0; i < 10; i++)
        {
            Gizmos.DrawRay(m_headTransform.position, coneRay1.normalized * m_viewDistance);
            Gizmos.DrawLine((coneRay1.normalized * m_viewDistance) + m_headTransform.position, (coneRay2.normalized * m_viewDistance) + m_headTransform.position);
            Gizmos.DrawLine((coneRay2.normalized * m_viewDistance) + m_headTransform.position, (coneRay3.normalized * m_viewDistance) + m_headTransform.position);
            Gizmos.DrawLine((coneRay3.normalized * m_viewDistance) + m_headTransform.position, (coneRay4.normalized * m_viewDistance) + m_headTransform.position);
            Gizmos.DrawLine((coneRay4.normalized * m_viewDistance) + m_headTransform.position, (coneRay5.normalized * m_viewDistance) + m_headTransform.position);
            Gizmos.DrawLine((coneRay5.normalized * m_viewDistance) + m_headTransform.position, forwardRay);
            Vector3 temp = coneRay1;
            coneRay1 = Quaternion.AngleAxis(36.0f, m_lookVector) * coneRay1;
            coneRay2 = Quaternion.AngleAxis(36.0f, m_lookVector) * coneRay2;
            coneRay3 = Quaternion.AngleAxis(36.0f, m_lookVector) * coneRay3;
            coneRay4 = Quaternion.AngleAxis(36.0f, m_lookVector) * coneRay4;
            coneRay5 = Quaternion.AngleAxis(36.0f, m_lookVector) * coneRay5;
            Gizmos.DrawLine((temp.normalized * m_viewDistance) + m_headTransform.position, (coneRay1.normalized * m_viewDistance) + m_headTransform.position);
        }
        

    }
}
