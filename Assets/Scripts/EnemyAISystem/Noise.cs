using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise : MonoBehaviour
{
    public Color m_Near, m_Mid, m_Far;
    public bool m_triggerEnemyAlert = true;
    public float m_baseRange;
    public float m_multiplier;
    public float m_decayRate;
    public float m_currentAlertRadius;
    public AudioSource m_audioSource;
    public NoiseData m_data;
    public float m_timestamp = 0.0f;
    public float m_amountToIncrement;
    public float[] m_samples;

    public EnemyAI[] m_enemies;

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
        //m_enemies = GameObject.FindObjectsOfType<EnemyAI>();
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
        Color c = m_Far;
        c.a = 0.1f;
        Gizmos.color = c;
        Gizmos.DrawSphere(this.gameObject.transform.position, m_currentAlertRadius * 3);
        c = m_Mid;
        c.a = 0.3f;
        Gizmos.color = c;
        Gizmos.DrawSphere(this.gameObject.transform.position, m_currentAlertRadius);
        c = m_Near;
        c.a = 0.2f;
        Gizmos.color = c;
        Gizmos.DrawSphere(this.gameObject.transform.position, m_currentAlertRadius / 3);
    }

    void Alert()
    {
        if (!m_triggerEnemyAlert) return;
        foreach (var ai in m_enemies)
        {
            if (ai == null) continue;
            if ((this.gameObject.transform.position - ai.gameObject.transform.position).magnitude <= m_currentAlertRadius * 3)
            {
                if ((this.gameObject.transform.position - ai.gameObject.transform.position).magnitude <= m_currentAlertRadius)
                {
                    if ((this.gameObject.transform.position - ai.gameObject.transform.position).magnitude <= m_currentAlertRadius / 3)
                    {
                        ai.gameObject.GetComponent<EnemyAI>().RecievePursuit();
                    }
                    else
                    {
                        ai.gameObject.GetComponent<EnemyAI>().RecieveAlert(this.gameObject.transform);
                    }
                }
                else
                {
                    ai.gameObject.GetComponent<EnemyAI>().RecieveAware(this.gameObject.transform);
                }
            }
        }
        // RaycastHit[] hits = Physics.SphereCastAll(this.gameObject.transform.position, m_currentAlertRadius * 3, Vector3.up, 500, LayerMask.GetMask("Enemies"));
        // foreach (var hit in hits)
        // {
        //     if (hit.collider.gameObject.GetComponent<EnemyAI>())
        //     {
        //         hit.collider.gameObject.GetComponent<EnemyAI>().RecieveAware(this.gameObject.transform);
        //     }
        // }

        // hits = Physics.SphereCastAll(this.gameObject.transform.position, m_currentAlertRadius, Vector3.up, 500, LayerMask.GetMask("Enemies"));
        // foreach (var hit in hits)
        // {
        //     if (hit.collider.gameObject.GetComponent<EnemyAI>())
        //     {
        //         hit.collider.gameObject.GetComponent<EnemyAI>().RecieveAlert(this.gameObject.transform);
        //     }
        // }

        // hits = Physics.SphereCastAll(this.gameObject.transform.position, m_currentAlertRadius / 3, Vector3.up, 500, LayerMask.GetMask("Enemies"));
        // foreach (var hit in hits)
        // {
        //     if (hit.collider.gameObject.GetComponent<EnemyAI>())
        //     {
        //         hit.collider.gameObject.GetComponent<EnemyAI>().RecievePursuit();
        //     }
        // }
    }
}
