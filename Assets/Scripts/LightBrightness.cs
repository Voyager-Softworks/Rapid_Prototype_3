using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBrightness : MonoBehaviour
{
    [SerializeField] GameObject m_player;
    [SerializeField] Light m_light;

    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.Find("Player");
        m_light = GetComponent<Light>();


        m_light.innerSpotAngle = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit[] HitInfo = Physics.RaycastAll(transform.position, transform.forward, 100f);

        bool doesHit = false;

        Vector3 hitpos = Vector3.positiveInfinity;

        foreach (RaycastHit _hit in HitInfo)
        {
            if (_hit.transform.root == m_player) continue;
            if (_hit.collider.isTrigger) continue;

            if (Vector3.Distance(_hit.point, transform.position) <= Vector3.Distance(hitpos, transform.position))
            {
                hitpos = _hit.point;
                doesHit = true;
            }
        }


        if (doesHit)
        {
            float dist = Vector3.Distance(transform.position, hitpos);

            float newInt = Mathf.Pow(dist * 5.0f, 2.0f) / 5.0f;
            m_light.intensity = Mathf.Lerp(m_light.intensity, newInt, 2.0f * Time.deltaTime);

            float newAng = 200.0f / Mathf.Pow(dist, 0.707f);
            m_light.spotAngle = Mathf.Lerp(m_light.spotAngle, newAng, 2.0f * Time.deltaTime);
        }

        
    }
}
