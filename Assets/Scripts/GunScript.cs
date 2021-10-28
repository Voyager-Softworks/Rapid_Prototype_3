using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GunScript : MonoBehaviour
{
    [Header("Parts")]
    [SerializeField] Transform r_end;
    [SerializeField] Transform l_end;

    [SerializeField] Renderer r_ind;
    [SerializeField] Renderer l_ind;

    [SerializeField] GameObject[] shells;

    [Header("Mats")]
    [SerializeField] Material red;
    [SerializeField] Material green;

    [Header("Effects")]
    [SerializeField] GameObject shotParticles;
    [SerializeField] AudioClip reloadSound;
    [SerializeField] NoiseMaker noiseMaker;

    [Header("Ammo")]
    [SerializeField] static float ammo = 0;
    [SerializeField] static float ammoStarting = 0;

    [SerializeField] static bool r_chamber = true;
    [SerializeField] static bool r_chamberStarting = true;

    [SerializeField] static bool l_chamber = true;
    [SerializeField] static bool l_chamberStarting = true;

    static private int currentLevel = 0;

    [Header("Reload")]
    [SerializeField] private float reloadTime = 2.0f;
    [SerializeField] private float reloadCooldown = 1.0f;
    private float reloadStart = 0.0f;

    EnemyAI[] m_enemies;
    VerminAI[] m_rats;
    SentryAI[] m_birds;

    GameObject m_meteor;
    GameObject m_jerrycanManager;

    GameObject m_boss;

    private void OnDrawGizmos()
    {
        float totalFOV = 40.0f;
        float rayRange = 20.0f;
        float halfFOV = totalFOV / 2.0f;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;
        Gizmos.DrawRay(transform.position, leftRayDirection * rayRange);
        Gizmos.DrawRay(transform.position, rightRayDirection * rayRange);
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetAmmo();

        reloadStart = -reloadTime;

        if (!m_meteor) m_meteor = GameObject.Find("Meteor");
        if (!m_boss) m_boss = GameObject.Find("BOSS_Bear_Finished");
        if (!m_jerrycanManager) m_jerrycanManager = GameObject.Find("JerrycanManager");
        m_enemies = GameObject.FindObjectsOfType<EnemyAI>();
        m_rats = GameObject.FindObjectsOfType<VerminAI>();
        m_birds = GameObject.FindObjectsOfType<SentryAI>();
        UpdateVisuals();
    }

    private static void ResetAmmo()
    {
        if (SceneManager.GetActiveScene().buildIndex <= 1)
        {
            ammo = 0;
            ammoStarting = 0;
        }

        if (currentLevel == SceneManager.GetActiveScene().buildIndex)
        {
            ammo = ammoStarting;
            r_chamber = r_chamberStarting;
            l_chamber = l_chamberStarting;
        }

        currentLevel = SceneManager.GetActiveScene().buildIndex;
        ammoStarting = ammo;
        r_chamberStarting = r_chamber;
        l_chamberStarting = l_chamber;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - reloadStart <= reloadTime)
        {
            GetComponent<LookAtCursor>().doLook = false;

            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0,0,-2), Time.deltaTime);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(90, 0, 0), Time.deltaTime);
            
        }
        else if (Time.time - reloadStart >= reloadTime + reloadCooldown)
        {
            if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.Locked)
            {
                Shoot();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Reload();
            }
        }
        else
        {
            GetComponent<LookAtCursor>().doLook = true;
        }
    }

    public void Reload()
    {
        bool didReload = false;

        if (!l_chamber && ammo > 0)
        {
            l_chamber = true;
            ammo--;
            didReload = true;
        }

        if (!r_chamber && ammo > 0)
        {
            r_chamber = true;
            ammo--;
            didReload = true;
        }

        if (didReload)
        {
            GetComponent<AudioSource>().PlayOneShot(reloadSound);
            reloadStart = Time.time;
        }

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        foreach (GameObject _shell in shells)
        {
            _shell.SetActive(true);
        }

        for (int i = 0; i < shells.Length - ammo; i++)
        {
            shells[i].SetActive(false);
        }

        r_ind.material = (r_chamber ? green : red);
        l_ind.material = (l_chamber ? green : red);
    }

    public void AddAmmo(float _amount)
    {
        ammo += _amount;

        UpdateVisuals();
    }

    public float GetAmmo()
    {
        return ammo;
    }

    public void Shoot()
    {
        Transform end = null;

        if (l_chamber)
        {
            l_chamber = false;
            end = l_end;
        }
        else if (r_chamber)
        {
            r_chamber = false;
            end = r_end;
        }
        else if (ammo > 0)
        {
            Reload();
            return;
        }
        else
        {
            return;
        }
        noiseMaker.PlayNoise();
        GameObject particles = Instantiate(shotParticles, end.position, end.rotation, null);
        Destroy(particles, 2);

        m_enemies = GameObject.FindObjectsOfType<EnemyAI>();
        foreach (EnemyAI _enemy in m_enemies)
        {
            Vector3 dir = (_enemy.transform.position - transform.position);
            float angle = Vector3.Angle(dir.normalized, transform.forward);
            float dist = dir.magnitude;
            if (dist <= 20 && angle <= 20)
            {
                _enemy.RecieveFlee();
            }
        }

        m_rats = GameObject.FindObjectsOfType<VerminAI>();
        foreach (VerminAI _enemy in m_rats)
        {
            Vector3 dir = (_enemy.transform.position - transform.position);
            float angle = Vector3.Angle(dir.normalized, transform.forward);
            float dist = dir.magnitude;
            if (dist <= 20 && angle <= 20)
            {
                _enemy.Shoot();
            }
        }

        m_birds = GameObject.FindObjectsOfType<SentryAI>();
        foreach (SentryAI _enemy in m_birds)
        {
            Vector3 dir = (_enemy.transform.position - transform.position);
            float angle = Vector3.Angle(dir.normalized, transform.forward);
            float dist = dir.magnitude;
            if (dist <= 20 && angle <= 20)
            {
                _enemy.Detect();
            }
        }

        if (m_meteor)
        {
            foreach (Transform child in m_meteor.transform)
            {
                if (child.name.ToLower().Contains("jerrycan"))
                {
                    Vector3 dir = (child.transform.position - transform.position);
                    float angle = Vector3.Angle(dir.normalized, transform.forward);
                    float dist = dir.magnitude;
                    if (dist <= 20 && angle <= 20)
                    {
                        if (m_jerrycanManager) m_jerrycanManager.GetComponent<JerrycanManager>().BlowMeteorCans();
                        break;
                    }
                }
            }
        }

        if (m_boss)
        {
            Vector3 dir = (m_boss.transform.position - transform.position);
            float angle = Vector3.Angle(dir.normalized, transform.forward);
            float dist = dir.magnitude;
            if (dist <= 20 && angle <= 20)
            {
                m_boss.GetComponent<BossBattleAI>().Shoot();
            }
        }

        transform.position -= transform.forward * 0.75f;
        transform.Rotate(new Vector3(-25,0,0), Space.Self);
        

        UpdateVisuals();
    }
}
