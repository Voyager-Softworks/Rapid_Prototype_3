using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarnScript : MonoBehaviour
{
    public GameObject[] Doors;

    public bool manualExplode = false;

    private void Update()
    {
        if (manualExplode)
        {
            manualExplode = false;
            ExplodeDoors();
        }
    }

    public void ExplodeDoors()
    {
        foreach (GameObject _door in Doors)
        {
            Rigidbody rb = _door.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.velocity = transform.right * 10;

            Vector3 spin = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));

            spin *= 30;

            rb.AddTorque(spin, ForceMode.Impulse);
        }
    }
}
