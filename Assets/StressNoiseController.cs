using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StressNoiseController : MonoBehaviour
{
    [Header("AI Trigger")]
    public Color m_Near, m_Mid, m_Far;
    public bool m_triggerEnemyAlert = true;
    public float m_baseRange;
    public float m_multiplier;
    public float m_decayRate;
    public float m_currentAlertRadius;

    [Header("Audio Processing")]
    public AudioSource m_heartbeatSource, m_breathingSource;
    float m_timestamp = 0.0f;
    float m_amountToIncrement_Heartbeat, m_amountToIncrement_Breathing;
    float[] m_heartbeatSamples, m_breathingSamples;

    [Header("Levels")]
    public AnimationCurve exertionCurve;
    public float currExertion = 0.0f;
    public AnimationCurve stressCurve;

    public float currStress = 0.0f;

    [Header("Fear")]
    public float m_visionRadius;
    [Range(0.1f, 1)]
    public float m_visionCone;


    void Start()
    {

        m_heartbeatSource.Play();
        m_breathingSource.Play();
        m_currentAlertRadius = m_baseRange;
        m_heartbeatSamples = new float[m_heartbeatSource.clip.samples * m_heartbeatSource.clip.channels];
        m_amountToIncrement_Heartbeat = m_heartbeatSource.clip.samples / m_heartbeatSource.clip.length;
        m_heartbeatSource.clip.GetData(m_heartbeatSamples, 0);

        m_breathingSamples = new float[m_breathingSource.clip.samples * m_breathingSource.clip.channels];
        m_amountToIncrement_Breathing = m_breathingSource.clip.samples / m_breathingSource.clip.length;
        m_breathingSource.clip.GetData(m_breathingSamples, 0);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit[] hits = Physics.SphereCastAll(this.transform.position, m_visionRadius, Vector3.up);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.CompareTag("Enemy") && Vector3.Dot((hit.collider.gameObject.transform.position - gameObject.transform.position).normalized, gameObject.transform.forward) > (1 - (m_visionCone / 2)))
            {
                currStress += Time.deltaTime * 1.5f;
            }
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currExertion += Time.deltaTime * 1.5f;
        }

        currExertion -= Time.deltaTime;
        currStress -= Time.deltaTime;
        currExertion = Mathf.Clamp(currExertion, 0, 10);
        currStress = Mathf.Clamp(currStress, 0, 10);

        m_heartbeatSource.volume = stressCurve.Evaluate(currStress);
        m_breathingSource.volume = exertionCurve.Evaluate(currExertion);


        if (m_currentAlertRadius > 0)
        {
            m_currentAlertRadius -= m_decayRate * Time.deltaTime;
        }
        m_timestamp += Time.deltaTime;
        int index_H = (int)(m_timestamp * m_amountToIncrement_Heartbeat) % m_heartbeatSamples.Length;
        int index_B = (int)(m_timestamp * m_amountToIncrement_Breathing) % m_breathingSamples.Length;
        Mathf.Clamp(index_H, 0, m_heartbeatSamples.Length);
        Mathf.Clamp(index_B, 0, m_breathingSamples.Length);
        if (index_H == m_heartbeatSamples.Length || index_H == 0 || index_B == m_breathingSamples.Length || index_B == 0)
        {
            m_currentAlertRadius = 0.0f;
        }
        else
        {
            m_currentAlertRadius = Mathf.Max(m_currentAlertRadius, Mathf.Lerp(m_currentAlertRadius, (Mathf.Abs(m_heartbeatSamples[index_H]) + Mathf.Abs(m_breathingSamples[index_B])) * m_multiplier * ((m_heartbeatSource.volume + m_breathingSource.volume) / 2.0f), Time.deltaTime * 5));
        }

        if (m_currentAlertRadius > 0)
        {
            Alert();
        }

        if (m_currentAlertRadius < 0)
        {
            m_currentAlertRadius = 0.0f;
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
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(this.transform.position, m_visionRadius);
    }

    void Alert()
    {
        if (!m_triggerEnemyAlert) return;
        RaycastHit[] hits = Physics.SphereCastAll(this.gameObject.transform.position, m_currentAlertRadius * 3, Vector3.up);
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.GetComponent<EnemyAI>())
            {
                hit.collider.gameObject.GetComponent<EnemyAI>().RecieveAware(this.gameObject.transform);
            }
        }

        hits = Physics.SphereCastAll(this.gameObject.transform.position, m_currentAlertRadius, Vector3.up);
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.GetComponent<EnemyAI>())
            {
                hit.collider.gameObject.GetComponent<EnemyAI>().RecieveAlert(this.gameObject.transform);
            }
        }

        hits = Physics.SphereCastAll(this.gameObject.transform.position, m_currentAlertRadius / 3, Vector3.up);
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.GetComponent<EnemyAI>())
            {
                hit.collider.gameObject.GetComponent<EnemyAI>().RecievePursuit();
            }
        }
    }
}
