using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCursor : MonoBehaviour
{
    [SerializeField] Camera m_cam;
    [SerializeField] GameObject m_player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnDrawGizmos()
    {
        RaycastHit[] HitInfo = Physics.RaycastAll(m_cam.transform.position, m_cam.transform.forward, 100f);

        foreach (RaycastHit _hit in HitInfo)
        {
            if (_hit.transform.root == m_player) continue;
            Gizmos.DrawSphere(_hit.point, 0.1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit[] HitInfo = Physics.RaycastAll(m_cam.transform.position, m_cam.transform.forward, 100f);

        bool doesHit = false;

        foreach (RaycastHit _hit in HitInfo)
        {
            if (_hit.transform.root == m_player) continue;
            transform.LookAt(Vector3.Lerp(transform.position + transform.forward,_hit.point, 0.01f), Vector3.up);
            doesHit = true;
            break;
        }

        if (!doesHit)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, m_cam.transform.localRotation, 0.01f);
        }
    }
}
