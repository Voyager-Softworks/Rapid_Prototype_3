using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornReplacer : MonoBehaviour
{

    public GameObject[] corn;

    // Use this for initialization
    void Start()
    {
        // Grab the island's terrain data
        TerrainData terrain;
        terrain = GetComponent<Terrain>().terrainData;
        terrain = TerrainDataCloner.Clone(terrain);
        List<TreeInstance> notCorn = new List<TreeInstance>();
        // For every tree on the island
        foreach (TreeInstance tree in terrain.treeInstances)
        {

            // Find its local position scaled by the terrain size (to find the real world position)
            Vector3 worldTreePos = Vector3.Scale(tree.position, terrain.size) + Terrain.activeTerrain.transform.position;
            string _name = terrain.treePrototypes[tree.prototypeIndex].prefab.name;
            GameObject m_realPrefab = null;
            foreach (GameObject _corn in corn)
            {
                if (_corn.name == _name)
                {
                    m_realPrefab = _corn;
                    break;
                }
            }

            if (m_realPrefab != null)
            {
                GameObject tempcorn = Instantiate(m_realPrefab, worldTreePos, Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up), transform); // Create a prefab tree on its pos
                tempcorn.transform.localScale *= tree.heightScale;
            }
        }
        // Then delete all trees on the island
        List<TreeInstance> newTrees = new List<TreeInstance>(0);
        newTrees.AddRange(notCorn);
        terrain.treeInstances = newTrees.ToArray();
    }
}
