using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise : MonoBehaviour
{
    public float m_baseRange;
    public float m_multiplier;
    public float m_decayRate;
    public float m_currentAlertRadius;
    public AudioSource m_audioSource;
    public NoiseData m_data;
    public float m_timestamp = 0.0f;
    public float m_amountToIncrement;
    public float[] m_samples;

    bool m_hasPlayed = false;
    void Start()
    {
        m_audioSource.volume = m_data.GetVolume();
        m_audioSource.clip = m_data.GetClip();
        m_audioSource.Play();
        m_currentAlertRadius = m_baseRange;
        m_hasPlayed = true;
        m_samples = new float[m_audioSource.clip.samples * m_audioSource.clip.channels];
        m_amountToIncrement = m_audioSource.clip.samples / m_audioSource.clip.length;
        m_audioSource.clip.GetData(m_samples, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_hasPlayed) return;
        if (!m_audioSource.isPlaying)
        {

            m_currentAlertRadius -= m_decayRate * Time.deltaTime;
        }
        else
        {
            if (m_currentAlertRadius > 0)
            {
                m_currentAlertRadius -= m_decayRate * Time.deltaTime;
            }
            m_timestamp += Time.deltaTime;
            int index = (int)(m_timestamp * m_amountToIncrement);
            Mathf.Clamp(index, 0, m_samples.Length);
            if (index == m_samples.Length || index == 0)
            {
                m_currentAlertRadius = Mathf.Abs(m_samples[index]) * m_multiplier * m_audioSource.volume;
            }
            else
            {
                m_currentAlertRadius = Mathf.Max(m_currentAlertRadius, Mathf.Lerp(m_currentAlertRadius, Mathf.Abs(m_samples[index]) * m_multiplier * m_audioSource.volume, Time.deltaTime * 5));
            }

        }
        Alert();
        if (m_currentAlertRadius < 0 && !m_audioSource.isPlaying)
        {
            m_currentAlertRadius = 0;

            Destroy(this.gameObject);
        }

    }

    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    void OnDrawGizmos()
    {
        Color c = Color.red;
        c.a = 0.2f;
        Gizmos.color = c;
        Gizmos.DrawSphere(this.gameObject.transform.position, m_currentAlertRadius);
    }

    void Alert()
    {
        RaycastHit[] hits = Physics.SphereCastAll(this.gameObject.transform.position, m_currentAlertRadius, Vector3.up);
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.GetComponent<EnemyAI>())
            {
                hit.collider.gameObject.GetComponent<EnemyAI>().RecieveAlert(this.gameObject.transform);
            }
        }
    }
}