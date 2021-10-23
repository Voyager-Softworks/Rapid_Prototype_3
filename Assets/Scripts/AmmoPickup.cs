using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Events;
using UnityEngine.Events;
using System;
using UnityEditor;
//using static UnityEditor.Events.UnityEventTools;

[ExecuteInEditMode]
public class AmmoPickup : MonoBehaviour
{
    [Header("Editable")]
    public int m_amount = 5;

    [Header("External")]
    public GameObject m_player;
    public GunScript m_playerGun;
    public PlayerMovement m_playerMovement;

    [Header("Internal")]
    public PlayerInteracts m_pi;
    public Outline m_outline;
    public MeshCollider m_collider;

    bool recheck = false;


    // Start is called before the first frame update
    void Start()
    {

    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        UpdateScript();
    }
#endif

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (recheck)
        {
            Debug.LogWarning("Rechecking Ammo Scripts");
            recheck = false;
            UpdateScript();
        }
#endif

        if (m_pi && m_player && m_playerGun && m_playerMovement)
        {
            if (!m_playerMovement.m_shotgunUnlocked || m_pi.GetCanTrigger() && m_playerGun.GetAmmo() >= 5)
            {
                m_pi.SetCanTrigger(false);
            }
            else if (m_playerMovement.m_shotgunUnlocked && !m_pi.GetCanTrigger() && m_playerGun.GetAmmo() < 5)
            {
                m_pi.SetCanTrigger(true);
            }
        }
    }

    public void UpdateScript()
    {
#if UNITY_EDITOR
        Undo.RecordObject(this, "UpdateScript");

        m_player = GameObject.Find("Player");

        m_pi = GetComponent<PlayerInteracts>();
        m_outline = GetComponent<Outline>();
        m_collider = GetComponent<MeshCollider>();


        if (!m_pi)
        {
            Debug.LogWarning("No <PlayerInteracts> found on " + gameObject.name + ", adding one.");
            m_pi = gameObject.AddComponent<PlayerInteracts>();
            recheck = true;
            return;
        }

        if (!m_outline)
        {
            Debug.LogWarning("No <Outline> found on " + gameObject.name + ", adding one.");
            m_outline = gameObject.AddComponent<Outline>();
            m_outline.OutlineColor = new Color(1.0f, 0.707f, 1.0f);
            m_outline.OutlineWidth = 10;
        }

        if (!m_collider)
        {
            Debug.LogWarning("No <MeshCollider> found on " + gameObject.name + ", adding one.");
            m_collider = gameObject.AddComponent<MeshCollider>();
        }

        if (!m_player)
        {
            Debug.LogError("No 'Player' found in scene.");
        }
        else
        {
            m_playerGun = m_player.GetComponentInChildren<GunScript>(true);
            m_playerMovement = m_player.GetComponent<PlayerMovement>();
        }

        if (!m_playerGun)
        {
            Debug.LogError("No <GunScript> found on 'Player' children.");
        }

        if (!m_playerMovement)
        {
            Debug.LogError("No <PlayerMovement> found on 'Player'.");
        }

        if (m_pi && m_player && m_playerGun && m_playerMovement)
        {

            while (m_pi.PlayerInteracted.GetPersistentEventCount() > 0)
            {
                UnityEventTools.RemovePersistentListener(m_pi.PlayerInteracted, 0);
            }
            while (m_pi.CantTrigger.GetPersistentEventCount() > 0)
            {
                UnityEventTools.RemovePersistentListener(m_pi.CantTrigger, 0);
            }

            UnityEventTools.AddBoolPersistentListener(m_pi.PlayerInteracted, gameObject.SetActive, false);
            UnityEventTools.AddFloatPersistentListener(m_pi.PlayerInteracted, m_playerGun.AddAmmo, m_amount);
            UnityEventTools.AddPersistentListener(m_pi.PlayerInteracted, m_pi.HideWorldMessage);

            UnityEventTools.AddPersistentListener(m_pi.CantTrigger, m_pi.DisplayWorldMessage);

            if (m_playerMovement.m_shotgunUnlocked)
            {
                m_pi.SetWorldMessage("Cant hold anymore");
                m_pi.SetCanTrigger(true);
            }
            else
            {
                m_pi.SetWorldMessage("Need my gun to use these");
                m_pi.SetCanTrigger(false);
            }


                PrefabUtility.RecordPrefabInstancePropertyModifications(m_pi);
            PrefabUtility.RecordPrefabInstancePropertyModifications(this);

            //PrefabUtility.RecordPrefabInstancePropertyModifications(GetComponent<AmmoPickup>());
        }
#endif
    }
}
