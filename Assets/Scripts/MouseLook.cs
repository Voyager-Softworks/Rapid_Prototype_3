using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity;

    public GameObject player;
    public Transform cam;

    float xRoation;

    RaycastHit bestHit = new RaycastHit();

    private GameObject m_target;
    private float m_time = 2.0f;
    private float m_timeStarted = 0;
    private float m_speed = 1.0f;

    public void LookAt(GameObject _target)
    {
        m_target = _target;
        m_timeStarted = Time.time;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_timeStarted = -m_time;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - m_timeStarted <= m_time)
        {
            if (m_target && m_target.transform)
            {
                player.transform.LookAt(Vector3.Lerp(player.transform.position + player.transform.forward, m_target.transform.position, m_speed * Time.deltaTime));
                player.transform.localRotation = Quaternion.Euler(0, player.transform.rotation.eulerAngles.y, 0);

                if (Vector3.Dot(cam.transform.forward, m_target.transform.position - cam.transform.position) > 0)
                {
                    cam.transform.LookAt(Vector3.Lerp(cam.transform.position + cam.transform.forward, m_target.transform.position, m_speed * Time.deltaTime));
                    cam.transform.localRotation = Quaternion.Euler(cam.transform.localRotation.eulerAngles.x, 0, 0);
                    xRoation = cam.transform.localRotation.eulerAngles.x;
                }
            }
        }
        else if (Cursor.lockState == CursorLockMode.Locked)
        {
            
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            xRoation -= mouseY;
            xRoation = Mathf.Clamp(xRoation, -90f, 90f);

            cam.transform.localRotation = Quaternion.Euler(xRoation, 0f, 0f);

            player.transform.Rotate(transform.up * mouseX);
        }

        CheckLookingAt();

        if (Input.GetKeyDown(KeyCode.E)) 
        {
            if (bestHit.transform && bestHit.transform.GetComponent<PlayerInteracts>())
            {
                bestHit.transform.GetComponent<PlayerInteracts>().TryComplete();
            }
        }
    }

    private void CheckLookingAt()
    {
        if (bestHit.transform && bestHit.transform.GetComponent<Outline>()) bestHit.transform.GetComponent<Outline>().enabled = false;
        bestHit = new RaycastHit();
        bestHit.distance = Mathf.Infinity;

        RaycastHit[] HitInfo = Physics.SphereCastAll(Camera.main.transform.position, 0.1f, Camera.main.transform.forward, 100f);

        foreach (RaycastHit _hit in HitInfo)
        {
            if (!_hit.transform.GetComponent<PlayerInteracts>()) continue;
            PlayerInteracts pi = _hit.transform.GetComponent<PlayerInteracts>();
            if (_hit.transform.root == gameObject) continue;
            if (_hit.distance > pi.m_interactDistance) continue;

            if (_hit.distance <= bestHit.distance)
            {
                if (_hit.transform && _hit.transform.GetComponent<Outline>()) _hit.transform.GetComponent<Outline>().enabled = false;
                bestHit = _hit;
            }
        }

        if (bestHit.transform && bestHit.transform.GetComponent<Outline>())
        {
            Outline ol = bestHit.transform.GetComponent<Outline>();
            ol.enabled = true;
        }
    }
}
