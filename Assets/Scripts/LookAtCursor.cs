using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCursor : MonoBehaviour
{
    [SerializeField] Camera m_cam;
    [SerializeField] GameObject m_player;

    [SerializeField] float lookSpeed;

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

        Vector3 hitpos = Vector3.positiveInfinity;

        foreach (RaycastHit _hit in HitInfo)
        {
            if (_hit.transform.root == m_player) continue;
            if (_hit.collider.isTrigger) continue;

            if (Vector3.Distance(_hit.point, transform.position) <= Vector3.Distance(hitpos, transform.position))
            {
                hitpos = _hit.point;
                doesHit = true;
            }
        }


        if (doesHit)
        {
            transform.LookAt(Vector3.Lerp(transform.position + transform.forward, hitpos, lookSpeed * Time.deltaTime), Vector3.up);
        }
        else
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, m_cam.transform.localRotation, lookSpeed * Time.deltaTime);
        }
    }
}
