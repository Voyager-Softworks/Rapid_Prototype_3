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

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
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
