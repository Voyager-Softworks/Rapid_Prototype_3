using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;





public class LightingAdjuster : MonoBehaviour
{
    public VolumeProfile m_pp;
    public LiftGammaGain m_LGG;

    float m_gamma;
    // Start is called before the first frame update
    void Start()
    {
        m_pp.TryGet<LiftGammaGain>(out m_LGG);
        m_gamma = m_LGG.gamma.value.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Equals))
        {
            m_gamma += Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.Minus))
        {
            m_gamma -= Time.deltaTime;
        }
        SetGammaAlpha(m_gamma);
    }
    public void SetGammaAlpha(float gammaAlpha)
    {
        m_LGG.gamma.Override(new Vector4(1f, 1f, 1f, gammaAlpha));
    }

}
