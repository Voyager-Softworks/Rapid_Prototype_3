﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Simple player movement script
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] GameObject m_cam;
    float camY = 0;

    [Header("Ground")]
    public CharacterController controller;
    public Transform groundCheck;
    public float groundDistance;
    public LayerMask groundMask;
    bool isGrounded;

    [Header("Movement")]
    [SerializeField] GameObject body;
    public float moveSpeed;
    float moveSpeed_Copy;
    public float sneakSpeed;
    public float runSpeed;
    bool forceSlow = false;
    public float jumpHeight;

    public float gravity;

    public Vector3 velocity;

    [Header("Steps")]
    [SerializeField] float stepHeight = 0.05f;
    [SerializeField] float stepLength = 1.0f;
    [SerializeField] float distanceTraveled = 0;
    public UnityEvent Step;
    public UnityEvent CornHit;
    bool oneStep = true;

    private void Start()
    {
        moveSpeed_Copy = moveSpeed;
        distanceTraveled = 0;
        camY = m_cam.transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastHit >= countdown)
        {
            forceSlow = false; 
        }
        else
        {
            forceSlow = true;
        }

        isGrounded = false;

        Collider[] hits = Physics.OverlapSphere(groundCheck.position, groundDistance, groundMask);

        foreach (Collider _hit in hits)
        {
            if (_hit.transform.root == gameObject.transform || _hit.isTrigger)
            {
                continue;
            }

            isGrounded = true;
            break;
        }

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (forceSlow || Input.GetKey(KeyCode.LeftControl))
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sneakSpeed,  2.0f * Time.deltaTime);
            body.transform.localPosition = Vector3.Lerp(body.transform.localPosition, -body.transform.up * 0.5f, 3 * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            body.transform.localPosition = Vector3.Lerp(body.transform.localPosition, Vector3.zero, 3 * Time.deltaTime);
            moveSpeed = Mathf.Lerp(moveSpeed, runSpeed, Time.deltaTime);
        }
        else
        {
            body.transform.localPosition = Vector3.Lerp(body.transform.localPosition, Vector3.zero, 3 * Time.deltaTime);
            moveSpeed = Mathf.Lerp(moveSpeed, moveSpeed_Copy, 3.0f * Time.deltaTime);
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = Vector3.ClampMagnitude((transform.right * x) + (transform.forward * z), 1.0f);
        move *= moveSpeed;

        velocity = new Vector3(move.x, velocity.y, move.z);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        if (isGrounded)
        {
            distanceTraveled += new Vector3(velocity.x, 0, velocity.z).magnitude * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);

        m_cam.transform.localPosition =
            (transform.up * camY * (1.0f - Mathf.Sin(distanceTraveled * (1.0f / stepLength))) * stepHeight)
            + (transform.up * camY)
            + (transform.right * (1.0f - Mathf.Sin(distanceTraveled * (1.0f / stepLength) / 2)) * stepHeight * 0.4f);

        if ((1.0f - Mathf.Sin(distanceTraveled * (1.0f / stepLength))) <= 0.1f)
        {
            if (oneStep)
            {
                oneStep = false;
                Step.Invoke();
            }
        }
        else
        {
            oneStep = true;
        }
    }

    public void ToggleSlowWalk()
    {
        forceSlow = !forceSlow;
    }

    float cornCount = 0;
    float lastHit = 0;
    float countdown = 0.1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("corn"))
        {
            cornCount++;
            Debug.Log("Corn: " + cornCount);
            CornHit.Invoke();
            lastHit = Time.time;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name.Contains("corn"))
        {
            lastHit = Time.time;
        }
    }
}
