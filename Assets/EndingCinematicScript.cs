using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EndingCinematicScript : MonoBehaviour
{
    public Animator m_meteorAnim;
    public Transform m_bearTransform;
    public Vector3 m_bearTeleportPosition;
    public Vector3 m_bearTeleportRotation;
    public SFX_Effect m_meteorEffect;

    public SceneController m_scnCtrl;
    
    public void PlayMeteor()
    {
        m_meteorAnim.SetTrigger("DoDeath");
        m_meteorEffect.Play();
    }

    public void PlayBear()
    {
        m_bearTransform.gameObject.GetComponent<NavMeshAgent>().enabled = false;
        m_bearTransform.rotation = Quaternion.Euler(m_bearTeleportRotation);
        m_bearTransform.position = m_bearTeleportPosition;
        m_bearTransform.gameObject.GetComponent<BossBattleAI>().Kill();
    }

    public void GotoTitleScreen()
    {
        m_scnCtrl.LoadMenu();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(m_bearTeleportPosition, 0.5f);
        
    }
}
