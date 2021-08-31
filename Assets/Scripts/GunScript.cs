using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    [SerializeField] Transform r_end;
    [SerializeField] Transform l_end;

    [SerializeField] Renderer r_ind;
    [SerializeField] Renderer l_ind;

    [SerializeField] GameObject[] shells;

    [SerializeField] Material red;
    [SerializeField] Material green;

    [SerializeField] GameObject shotParticles;
    [SerializeField] AudioClip shotSound;
    [SerializeField] NoiseMaker noiseMaker;

    [SerializeField] float ammo = 6;
    [SerializeField] bool r_chamber = true;
    [SerializeField] bool l_chamber = true;

    EnemyAI[] m_enemies;

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
        m_enemies = GameObject.FindObjectsOfType<EnemyAI>();
        UpdateVisuals();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    public void Reload()
    {
        if (!l_chamber && ammo > 0)
        {
            l_chamber = true;
            ammo--;
        }

        if (!r_chamber && ammo > 0)
        {
            r_chamber = true;
            ammo--;
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

        transform.position -= transform.forward * 0.75f;
        transform.Rotate(new Vector3(-25,0,0), Space.Self);
        

        UpdateVisuals();
    }
}
