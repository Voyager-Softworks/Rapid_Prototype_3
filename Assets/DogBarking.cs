using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogBarking : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] AudioSource source;

    [SerializeField] AudioClip[] normalBarks;
    [SerializeField] AudioClip[] loudBarks;

    [Header("Bark Intervals")]
    [SerializeField] float averageBarkTime;
    [SerializeField] float randomOffset;

    public bool canbark = true;

    float nextBarkTime = 0;
    float lastBarkTime = 0;

    private void Start()
    {
        SetRandomTime();
        if (!source) source = GetComponent<AudioSource>();
    }

    private void SetRandomTime()
    {
        lastBarkTime = Time.time;
        nextBarkTime = averageBarkTime + Random.Range(-randomOffset, randomOffset);
    }

    private void Update()
    {
        if (!canbark) return;

        if (Time.time - lastBarkTime >= nextBarkTime)
        {
            SetRandomTime();
            PlayNormalBark();
        }
    }

    public void PlayNormalBark()
    {
        if (normalBarks.Length <= 0) return;
        if (!source) return;

        AudioClip sound = normalBarks[Random.Range(0, normalBarks.Length - 1)];

        source.PlayOneShot(sound);
    }

    public void PlayLoudBark()
    {
        if (loudBarks.Length <= 0) return;
        if (!source) return;

        AudioClip sound = loudBarks[Random.Range(0, loudBarks.Length - 1)];

        source.PlayOneShot(sound);
    }
}
