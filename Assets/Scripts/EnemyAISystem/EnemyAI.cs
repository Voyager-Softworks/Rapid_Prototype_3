using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform playerTransform;

    public bool m_alerted;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RecieveAlert(Transform _alertPos)
    {
        m_alerted = true;
        agent.destination = _alertPos.position;
    }
}
