using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DogScript : MonoBehaviour
{
    public UnityEvent PlayerReaches;

    [SerializeField] GameObject m_player;

    [SerializeField] bool hasPlayerReached = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasPlayerReached && other.transform.root.gameObject == m_player)
        {
            PlayerReaches.Invoke();
        }
    }
}
