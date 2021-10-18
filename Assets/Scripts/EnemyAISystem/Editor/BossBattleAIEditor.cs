using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;

[CustomEditor(typeof(BossBattleAI))]
public class BossBattleAIEditor : Editor
{
    SerializedProperty nodes;
    
    BossBattleAI m_AI;


    private void OnEnable()
    {
        m_AI = (BossBattleAI)target;
        
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
       
    }

    void DrawCircle(float _radius, Vector3 _center)
    {
        Vector3 dir = Vector3.forward;
        int fidelity = (int)(50);
        for (int i = 0; i < fidelity-1; i++)
        {
            Vector3 newDir = Quaternion.AngleAxis(360/fidelity, Vector3.up) * dir;
            Handles.DrawLine((_center + (dir * _radius)), (_center + (newDir * _radius)));
            dir = newDir;
        }
        Handles.DrawLine((_center + (dir * _radius)), (_center + (Vector3.forward * _radius)));
    }
    private void OnSceneGUI()
    {
        Handles.color = Color.yellow;
        DrawCircle(m_AI.m_arenaRadius, m_AI.m_arenaPosition);
        DrawCircle(m_AI.m_arenaRadius + 1.0f, m_AI.m_arenaPosition);
        m_AI.m_arenaPosition = Handles.PositionHandle(m_AI.m_arenaPosition, Quaternion.identity);
        Handles.color = Color.green;
        DrawCircle(1.0f, m_AI.m_nextSpawnPosition);
        Handles.color = Color.red;
        DrawCircle(1.0f, m_AI.m_nextRetreatPosition);
    }
}

