using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class TeleportAfterDelay : MonoBehaviour
{
    public Vector3 m_teleportToPosition;
    public Vector3 m_teleportToRotation;
    public Transform m_objectToTeleport;
    public float m_teleportDelay;
    public bool m_started = false;

    public Mesh m_mesh;
    void Update()
    {
        if (m_started && (m_teleportDelay -= Time.deltaTime) < 0.0f)
        {   
            m_objectToTeleport.gameObject.GetComponent<CharacterController>().enabled = false;
            m_objectToTeleport.position = m_teleportToPosition;
            
            m_objectToTeleport.rotation = Quaternion.Euler(m_teleportToRotation);
            m_objectToTeleport.gameObject.GetComponent<CharacterController>().enabled = true;
            m_started = false;
        }
    }

    public void StartTeleport()
    {
        m_started = true;
        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        if (m_mesh == null)
        {
            if (m_objectToTeleport != null)
            {
                m_mesh = m_objectToTeleport.GetComponentInChildren<MeshFilter>().sharedMesh;
                if(m_mesh == null) m_mesh = m_objectToTeleport.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;

            }
        }
        else
        {
            Gizmos.DrawMesh(m_mesh, 0, m_teleportToPosition, Quaternion.Euler(m_teleportToRotation), Vector3.one);
            
        }
    }
    
}
