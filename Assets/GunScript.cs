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

    [SerializeField] float ammo = 6;
    [SerializeField] bool r_chamber = true;
    [SerializeField] bool l_chamber = true;


    // Start is called before the first frame update
    void Start()
    {
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

        GameObject particles = Instantiate(shotParticles, end.position, end.rotation, null);
        Destroy(particles, 2);

        UpdateVisuals();
    }
}