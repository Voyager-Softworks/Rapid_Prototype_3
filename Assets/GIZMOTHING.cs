using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GIZMOTHING : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 2);
    }
}
