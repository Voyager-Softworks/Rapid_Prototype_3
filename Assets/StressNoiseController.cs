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
    public AudioSource m_breathingSource;
    float m_timestamp = 0.0f;
    float m_amountToIncrement_Breathing;
    float[] m_breathingSamples;

    [Header("Levels")]
    public AnimationCurve exertionCurve;
    public float currExertion = 0.0f;






    void Start()
    {


        m_breathingSource.Play();
        m_currentAlertRadius = m_baseRange;


        m_breathingSamples = new float[m_breathingSource.clip.samples * m_breathingSource.clip.channels];
        m_amountToIncrement_Breathing = m_breathingSource.clip.samples / m_breathingSource.clip.length;
        m_breathingSource.clip.GetData(m_breathingSamples, 0);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currExertion += Time.deltaTime * 1.5f;
        }

        currExertion -= Time.deltaTime;

        currExertion = Mathf.Clamp(currExertion, 0, 10);


        m_breathingSource.volume = exertionCurve.Evaluate(currExertion);


        if (m_currentAlertRadius > 0)
        {
            m_currentAlertRadius -= m_decayRate * Time.deltaTime;
        }
        m_timestamp += Time.deltaTime;

        int index_B = (int)(m_timestamp * m_amountToIncrement_Breathing) % m_breathingSamples.Length;

        Mathf.Clamp(index_B, 0, m_breathingSamples.Length);
        if (index_B == m_breathingSamples.Length || index_B == 0)
        {
            m_currentAlertRadius = 0.0f;
        }
        else
        {
            m_currentAlertRadius = Mathf.Max(m_currentAlertRadius, Mathf.Lerp(m_currentAlertRadius, (Mathf.Abs(Mathf.Abs(m_breathingSamples[index_B])) * m_multiplier * ((m_breathingSource.volume) / 2.0f)), Time.deltaTime * 5));
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
