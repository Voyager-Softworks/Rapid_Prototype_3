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

    public bool manualsoundplay = false;
    public void PlayNoise()
    {
        Noise newNoise = Instantiate(m_noisePrefab, this.transform.position, this.transform.rotation).GetComponent<Noise>();
        newNoise.m_data = m_noise;
        newNoise.m_baseRange = m_baseRange;
        newNoise.m_multiplier = m_distanceMultiplier;
        newNoise.m_decayRate = m_decayRate;

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
}
