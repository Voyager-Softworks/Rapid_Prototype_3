using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    [SerializeField] Transform end;

    [SerializeField] GameObject shotParticles;
    [SerializeField] AudioClip shotSound;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        GameObject particles = Instantiate(shotParticles, end.position, end.rotation, null);
        Destroy(particles, 2);
    }
}
