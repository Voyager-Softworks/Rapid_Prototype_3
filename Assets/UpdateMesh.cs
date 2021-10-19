using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateMesh : MonoBehaviour
{
    SkinnedMeshRenderer meshRenderer;
    MeshCollider meshCollider;
    Mesh tempMesh;

    private void Start()
    {
        meshRenderer = GetComponent<SkinnedMeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        tempMesh = new Mesh();
    }

    private float time = 0;
    private void FixedUpdate()
    {
        time += Time.deltaTime;

        if (time >= 0.5f)
        {
            time = 0;
            UpdateCollider();
        }
    }

    public void UpdateCollider()
    {
        meshRenderer.BakeMesh(tempMesh, true);
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = tempMesh;
    }
}
