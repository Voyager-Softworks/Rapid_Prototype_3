using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMaker : MonoBehaviour
{
    public GameObject m_noisePrefab;
    public float m_baseRange;
    public float m_distanceMultiplier;
    public float m_decayRate;
    public NoiseData m_noise;
    public bool m_playOnCollision;
    public bool m_triggerEnemyAlert;
    public bool m_objectSpace;
    public bool manualsoundplay = false;
    public void PlayNoise()
    {
        Noise newNoise = Instantiate(m_noisePrefab, this.transform.position, this.transform.rotation).GetComponent<Noise>();
        if (m_objectSpace)
        {
            newNoise.gameObject.transform.SetParent(this.gameObject.transform);
        }
        newNoise.m_data = m_noise;
        newNoise.m_baseRange = m_baseRange;
        newNoise.m_multiplier = m_distanceMultiplier;
        newNoise.m_decayRate = m_decayRate;
        if (!m_triggerEnemyAlert)
        {
            newNoise.m_triggerEnemyAlert = false;
        }

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (manualsoundplay)
        {
            PlayNoise();
            manualsoundplay = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!m_playOnCollision) return;

        m_triggerEnemyAlert = other.gameObject.CompareTag("Player");


        PlayNoise();
    }

}
