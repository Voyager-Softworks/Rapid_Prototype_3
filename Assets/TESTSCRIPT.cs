using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTSCRIPT : MonoBehaviour
{

    public GameObject[] corn;

    TreeInstance[] COPYDATA;

    // Use this for initialization
    void Start()
    {
        // Grab the island's terrain data
        TerrainData terrain;
        terrain = GetComponent<Terrain>().terrainData;
        COPYDATA = (TreeInstance[])terrain.treeInstances.Clone();
        List<TreeInstance> notCorn = new List<TreeInstance>();
        // For every tree on the island
        foreach (TreeInstance tree in terrain.treeInstances)
        {
            if (tree.prototypeIndex > 3)
            {
                notCorn.Add(tree);
                continue;
            }

            // Find its local position scaled by the terrain size (to find the real world position)
            Vector3 worldTreePos = Vector3.Scale(tree.position, terrain.size) + Terrain.activeTerrain.transform.position;
            GameObject tempcorn = Instantiate(corn[tree.prototypeIndex], worldTreePos, Quaternion.AngleAxis(Random.Range(0,360), Vector3.up)); // Create a prefab tree on its pos
            tempcorn.transform.localScale *= tree.heightScale;
        }
        // Then delete all trees on the island
        List<TreeInstance> newTrees = new List<TreeInstance>(0);
        newTrees.AddRange(notCorn);
        terrain.treeInstances = newTrees.ToArray();
    }

    private void OnDestroy()
    {
        GetComponent<Terrain>().terrainData.treeInstances = COPYDATA;
    }
}
