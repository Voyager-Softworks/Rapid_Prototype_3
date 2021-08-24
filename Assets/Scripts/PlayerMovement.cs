using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple player movement script
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform groundCheck;
    public float groundDistance;
    public LayerMask groundMask;

    public float moveSpeed;
    public float jumpHeight;

    public float gravity;

    public Vector3 velocity;
    bool isGrounded;

    // Update is called once per frame
    void Update()
    {
        //only if local player
        

        isGrounded = false;

        Collider[] hits = Physics.OverlapSphere(groundCheck.position, groundDistance, groundMask);

        foreach (Collider _hit in hits)
        {
            if (_hit.transform.root == gameObject.transform)
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

        controller.Move(move * Time.deltaTime * moveSpeed);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
