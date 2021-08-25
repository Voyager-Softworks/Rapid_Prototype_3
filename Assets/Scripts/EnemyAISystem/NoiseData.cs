using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sounds/NoiseData")]
public class NoiseData : ScriptableObject
{
    public List<AudioClip> m_clips;
    public float m_volume;
    public float m_volumeJitter;

    public AudioClip GetClip()
    {
        return m_clips[Random.Range(0, m_clips.Count)];
    }

    public float GetVolume()
    {
        return Mathf.Clamp(m_volume + Random.Range(-m_volumeJitter, m_volumeJitter), 0, 1);
    }
}
