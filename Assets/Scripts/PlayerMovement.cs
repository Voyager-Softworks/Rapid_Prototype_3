using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple player movement script
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] GameObject m_cam;
    float camY = 0;

    public CharacterController controller;
    public Transform groundCheck;
    public float groundDistance;
    public LayerMask groundMask;

    public float moveSpeed;
    public float jumpHeight;

    public float gravity;

    public Vector3 velocity;
    bool isGrounded;

    [SerializeField] float stepHeight = 0.05f;
    [SerializeField] float stepLength = 1.0f;
    [SerializeField] float distanceTraveled = 0;

    private void Start()
    {
        distanceTraveled = 0;
        camY = m_cam.transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        //only if local player
        

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
            (transform.up * camY * (1.0f - Mathf.Sin(distanceTraveled * 1.0f/stepLength)) * stepHeight)
            + (transform.up * camY)
            + (transform.right * (1.0f - Mathf.Sin(distanceTraveled * 1.0f / stepLength)) * stepHeight * 0.4f);
    }
}
