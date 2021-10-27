using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleEnabled : MonoBehaviour
{
    public GameObject m_toToggle;
    public void ToggleActive()
    {
        if(m_toToggle.activeSelf) m_toToggle.SetActive(false);
        else m_toToggle.SetActive(true);

    }
}
