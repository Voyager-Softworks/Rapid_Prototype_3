using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Simple player movement script
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    public enum Equipment
    {
        None,
        Shotgun,
        Flashlight,
        END
    }

    [Header("Camera")]
    [SerializeField] GameObject m_cam;
    float camY = 0;

    [Header("Equipment")]
    [SerializeField] GameObject m_flashlight;
    [SerializeField] GameObject m_shotgun;
    public Equipment m_currentlyEquipped = Equipment.None;
    public bool m_flashlightUnlocked = false;
    public bool m_shotgunUnlocked = false;

    [SerializeField] AudioClip EquipFlashlight;
    [SerializeField] AudioClip UnequipFlashlight;
    [SerializeField] AudioClip EquipShotgun;
    [SerializeField] AudioClip UnequipShotgun;

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

    [Header("Exertion")]
    public float maxExertion = 5.0f;
    public AnimationCurve exertionCurve;
    public float currExertion;
    bool forceSlow = false;

    [Header("Jumping")]
    public bool allowedJump = false;
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
    [SerializeField] AudioClip jump_sound;

    AudioSource source;
    [SerializeField] NoiseMaker landingNoiseMaker;

    [Header("Sneak")]
    public bool isSneaking = false;
    public float sneakNoiseReduction = 0.0f;
    public float sneakDetectionChance = 0.5f;

    NoiseMaker footstepNoise;
    public NoiseData normalFootstep;
    public NoiseData sneakFootstep;

    private void Start()
    {
        source = GetComponent<AudioSource>(); ;
        moveSpeed_Copy = moveSpeed;
        distanceTraveled = 0;
        camY = m_cam.transform.localPosition.y;
        currExertion = 0.0f;

        UpdateHeldEquipment();

        footstepNoise = GetComponent<NoiseMaker>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) || Input.mouseScrollDelta.y != 0)
        {
            NextEquipment(Input.mouseScrollDelta.y != 0 ? Input.mouseScrollDelta.y > 0 ? true : false : false);
            UpdateHeldEquipment();
        }

        if (Time.time - lastHit >= countdown)
        {
            forceSlow = false;
        }
        else
        {
            forceSlow = true;
        }

        bool hitsGround = false;

        Collider[] hits = Physics.OverlapSphere(groundCheck.position, groundDistance, groundMask);

        foreach (Collider _hit in hits)
        {
            if (_hit.transform.root == gameObject.transform || _hit.isTrigger)
            {
                continue;
            }

            hitsGround = true;
            break;
        }

        if (hitsGround)
        {
            if (!isGrounded)
            {
                landingNoiseMaker.PlayNoise();
            }
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        isSneaking = false;
        if (forceSlow || Input.GetKey(KeyCode.LeftControl))
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sneakSpeed, 2.0f * Time.deltaTime);
            if (Input.GetKey(KeyCode.LeftControl))
            {
                body.transform.localPosition = Vector3.Lerp(body.transform.localPosition, -body.transform.up * 0.5f, 3 * Time.deltaTime);
                isSneaking = true;
            }
        }
        else if (Input.GetKey(KeyCode.LeftShift) && currExertion < maxExertion)
        {
            body.transform.localPosition = Vector3.Lerp(body.transform.localPosition, Vector3.zero, 3 * Time.deltaTime);
            moveSpeed = Mathf.Lerp(moveSpeed, runSpeed, Time.deltaTime);
            currExertion += Time.deltaTime * 1.5f;
        }
        else
        {
            body.transform.localPosition = Vector3.Lerp(body.transform.localPosition, Vector3.zero, 3 * Time.deltaTime);
            moveSpeed = Mathf.Lerp(moveSpeed, moveSpeed_Copy, 3.0f * Time.deltaTime);
        }

        currExertion -= Time.deltaTime;
        currExertion = Mathf.Clamp(currExertion, 0, 10);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = Vector3.ClampMagnitude((transform.right * x) + (transform.forward * z), 1.0f);
        if (currExertion > (0.75f * maxExertion) && move.magnitude > 0.1f) currExertion += (Time.deltaTime * 0.5f);
        move *= (moveSpeed);

        velocity = new Vector3(move.x, velocity.y, move.z);

        if (Input.GetButtonDown("Jump") && isGrounded && allowedJump)
        {
            source.PlayOneShot(jump_sound);
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        velocity = new Vector3(velocity.x, Mathf.Clamp(velocity.y, -10, 10), velocity.z);

        if (isGrounded)
        {
            distanceTraveled += new Vector3(velocity.x, 0, velocity.z).magnitude * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime * exertionCurve.Evaluate(currExertion));

        m_cam.transform.localPosition =
            (transform.up * camY * (1.0f - Mathf.Sin(distanceTraveled * (1.0f / stepLength))) * stepHeight)
            + (transform.up * camY)
            + (transform.right * (1.0f - Mathf.Sin(distanceTraveled * (1.0f / stepLength) / 2)) * stepHeight * 0.4f);

        if ((1.0f - Mathf.Sin(distanceTraveled * (1.0f / stepLength))) <= 0.1f)
        {
            if (oneStep)
            {
                oneStep = false;
                footstepNoise.m_noise = (isSneaking ? sneakFootstep : normalFootstep);
                footstepNoise.PlayNoise();
                Step.Invoke();
            }
        }
        else
        {
            oneStep = true;
        }
    }

    private void UpdateHeldEquipment()
    {
        switch (m_currentlyEquipped)
        {
            case Equipment.None:
                if (m_flashlight.activeSelf)
                {
                    m_flashlight.SetActive(false);
                    GetComponent<AudioSource>().PlayOneShot(UnequipFlashlight);
                }
                if (m_shotgun.activeSelf)
                {
                    m_shotgun.SetActive(false);
                    GetComponent<AudioSource>().PlayOneShot(UnequipShotgun);
                }
                break;

            case Equipment.Flashlight:
                if (!m_flashlight.activeSelf) m_flashlight.SetActive(true);
                if (m_shotgun.activeSelf) m_shotgun.SetActive(false);
                GetComponent<AudioSource>().PlayOneShot(EquipFlashlight);
                break;

            case Equipment.Shotgun:
                if (m_flashlight.activeSelf) m_flashlight.SetActive(false);
                if (!m_shotgun.activeSelf) m_shotgun.SetActive(true);
                GetComponent<AudioSource>().PlayOneShot(EquipShotgun);
                break;

            default:
                break;
        }
    }

    private void NextEquipment(bool increase = true)
    {
        m_currentlyEquipped += increase ? 1 : -1;

        if (m_currentlyEquipped < 0) m_currentlyEquipped = Equipment.END - 1;
        if (m_currentlyEquipped > Equipment.END - 1) m_currentlyEquipped = 0;

        switch (m_currentlyEquipped)
        {
            case Equipment.Flashlight:
                if (!m_flashlightUnlocked) NextEquipment(increase);
                break;

            case Equipment.Shotgun:
                if (!m_shotgunUnlocked) NextEquipment(increase);
                break;
        }
    }

    public void UnlockShotgun()
    {
        m_shotgunUnlocked = true;
    }

    public void UnlockFlashlight()
    {
        m_flashlightUnlocked = true;
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
        if (other.name.Contains("Corn"))
        {
            cornCount++;
            Debug.Log("Corn: " + cornCount);
            CornHit.Invoke();
            lastHit = Time.time;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name.Contains("Corn"))
        {
            lastHit = Time.time;
        }
    }
}
