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
    public float m_viewVerticalRotation;

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

    Vector3[] vertices;
    int[] triangles;

    Vector2[] uv;

    public Mesh m_conemesh;
    public bool m_regenerateMesh;

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
            float rand1 = Random.Range(0, Mathf.PI * 2);
            Vector3 spawndir = (m_playerTransform.position - new Vector3((m_groupSpreadRadius * Mathf.Cos(rand1)) + origin.x, origin.y, m_groupSpreadRadius * Mathf.Sin(rand1) + origin.z));
            if(Vector3.Dot(spawndir, m_playerTransform.forward) < 0.0f) rand1 += Mathf.PI;
            float rand2 = Random.Range(0, Mathf.PI * 2);
            Vector3 groupOrigin = new Vector3((m_groupSpreadRadius * Mathf.Cos(rand1)) + origin.x, origin.y, m_groupSpreadRadius * Mathf.Sin(rand1) + origin.z);
            for (int j = 0; j < m_verminGroupSize; j++)
            {
                GameObject newRat = Instantiate(m_verminPrefab, 
                new Vector3((m_verminSpreadRadius * Mathf.Cos(rand2)) + groupOrigin.x, groupOrigin.y, m_verminSpreadRadius * Mathf.Sin(rand2) + groupOrigin.z),
                Quaternion.identity * Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up)
                );
                float rand3 = Random.Range(-0.3f, 0.3f);
                newRat.GetComponent<Animator>().speed += rand3;
                newRat.GetComponent<VerminAI>().m_speed += rand3;
                newRat.GetComponent<AudioSource>().pitch += rand3;
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
        if(m_conemesh == null)
        {
            m_conemesh = GenerateConeMesh();
            m_headTransform.gameObject.GetComponentInChildren<MeshFilter>().sharedMesh = m_conemesh;
        }
        float rand = Random.Range(0.0f, 1.0f);
        m_anim.SetFloat("IdleOffset", rand);
        m_lookBackTimer *= (1.0f - rand);
        
    }



    // Update is called once per frame
    
    void Update()
    {
        
        
            m_headTransform.localRotation = Quaternion.AngleAxis(m_viewVerticalRotation, Vector3.left);
            m_lookVector = m_headTransform.up;
            
            switch (m_state)
            {
            case SentryState.SEARCHING: 
                if((m_lookBackTimer -= Time.deltaTime) < 0.0f)
                {
                    m_anim.SetTrigger("LookBack");
                    m_lookBackTimer = m_lookBackInterval;
                }
                
                if((m_playerTransform.position - m_headTransform.position).magnitude <= m_viewDistance && Vector3.Angle(m_lookVector, (m_playerTransform.position - m_headTransform.position)) < m_viewAngle / 2)
                {
                    if(!Physics.Raycast(m_headTransform.position, 
                    (m_playerTransform.position - m_headTransform.position), 
                    (m_playerTransform.position - m_headTransform.position).magnitude, 
                    layerMask: LayerMask.GetMask("Obstacles")))
                    {
                        if(!(m_playerTransform.gameObject.GetComponent<PlayerMovement>().isSneaking && 
                        Random.Range(0, 100) > (m_playerTransform.gameObject.GetComponent<PlayerMovement>().sneakDetectionChance * 100.0f)))
                        {
                            m_state = SentryState.DETECTED;
                            m_screechTimer = m_screechDuration;
                            m_anim.SetTrigger("Detect");
                            m_ScreamFX.Play();
                            m_headTransform.GetComponentInChildren<MeshRenderer>().enabled = false;
                        }
                    }
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
                        m_headTransform.GetComponentInChildren<MeshRenderer>().enabled = true;
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
                transform.position += m_direction * Time.deltaTime * 10.0f;
                
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

    Mesh GenerateConeMesh()
    {
        float triangleAngle = (((Mathf.PI * 2)/10.0f)*(m_viewAngle/180.0f));
        m_headTransform.localRotation = Quaternion.AngleAxis(m_viewVerticalRotation, Vector3.left);
        
        Vector3 coneRay1 = Quaternion.AngleAxis(m_viewAngle/2, Vector3.forward) * Vector3.up;
        vertices = new Vector3[23];
        vertices[0] = Vector3.zero;
        uv = new Vector2[23];
        uv[0] = new Vector2(0.5f, 0.5f);
        
        for (int i = 1; i < 11; i++)
        {
            vertices[i] = (coneRay1.normalized * m_viewDistance);
            vertices[i+10] = vertices[i];
            Vector3 temp = coneRay1;
            coneRay1 = Quaternion.AngleAxis(36.0f, Vector3.up) * coneRay1;
            uv[i] = new Vector2((Mathf.Cos((i-1)*triangleAngle)/2.0f)+0.5f, (Mathf.Sin((i-1)*triangleAngle)/2.0f)+0.5f);
            uv[i+10] = new Vector2(((Mathf.Cos((i-1)*((Mathf.PI * 2)/10.0f))/2.0f)*(m_viewAngle/180.0f))+0.5f, ((Mathf.Sin((i-1)*((Mathf.PI * 2)/10.0f))/2.0f)*(m_viewAngle/180.0f))+0.5f);
            
        }
        vertices[22] = (Vector3.up * m_viewDistance);
        uv[22] = new Vector2(0.5f, 0.5f);
        vertices[21] = vertices[1];
        uv[21] = new Vector2((Mathf.Cos((10)*triangleAngle)/2.0f)+0.5f, (Mathf.Sin((10)*triangleAngle)/2.0f)+0.5f);
        triangles = new int[60] {
            //Cone Portion
            0, 2, 1, 
            0, 3, 2, 
            0, 4, 3, 
            0, 5, 4, 
            0, 6, 5, 
            0, 7, 6, 
            0, 8, 7, 
            0, 9, 8, 
            0, 10, 9, 
            0, 21, 10,
            //Face
            22, 11, 12, 
            22, 12, 13, 
            22, 13, 14, 
            22, 14, 15, 
            22, 15, 16, 
            22, 16, 17, 
            22, 17, 18, 
            22, 18, 19, 
            22, 19, 20, 
            22, 20, 11
            };
        Mesh outMesh;
        outMesh = new Mesh();
        outMesh.SetVertices(vertices);
        outMesh.triangles = triangles;
        outMesh.name = "Generated Cone";
        outMesh.SetUVs(0, uv);
        outMesh.RecalculateNormals();
        outMesh.RecalculateBounds();
        return outMesh;

    }

    void UpdateMesh(ref Mesh _mesh)
    {
        m_headTransform.localRotation = Quaternion.AngleAxis(m_viewVerticalRotation, Vector3.left);
        
        Vector3 coneRay1 = Quaternion.AngleAxis(m_viewAngle/2, Vector3.forward) * Vector3.up;
        vertices = new Vector3[23];
        vertices[0] = Vector3.zero;

        for (int i = 1; i < 11; i++)
        {
            vertices[i] = (coneRay1.normalized * m_viewDistance);
            vertices[i+10] = vertices[i];
            Vector3 temp = coneRay1;
            coneRay1 = Quaternion.AngleAxis(36.0f, Vector3.up) * coneRay1;
        }
        vertices[22] = (Vector3.up * m_viewDistance);
        vertices[21] = vertices[1];
        _mesh.SetVertices(vertices);
        _mesh.RecalculateNormals();
        
    }

    void OnDrawGizmosSelected()
    {
        m_lookVector = m_headTransform.up;
        m_headTransform.localRotation = Quaternion.AngleAxis(m_viewVerticalRotation, Vector3.left);
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
        if(m_conemesh == null)
        {
            m_conemesh = GenerateConeMesh();
            m_headTransform.gameObject.GetComponentInChildren<MeshFilter>().sharedMesh = m_conemesh;
        }
        else if(m_regenerateMesh)
        {
            m_conemesh = GenerateConeMesh();
            m_headTransform.gameObject.GetComponentInChildren<MeshFilter>().sharedMesh = m_conemesh;
            m_regenerateMesh = false;
        }
        

    }
}