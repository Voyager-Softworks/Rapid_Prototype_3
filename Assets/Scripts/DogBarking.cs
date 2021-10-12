using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogBarking : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] AudioSource source;

    [SerializeField] AudioClip[] normalBarks;
    [SerializeField] AudioClip[] loudBarks;
    [SerializeField] AudioClip deathSound;

    [Header("Bark Intervals")]
    [SerializeField] float averageBarkTime;
    [SerializeField] float randomOffset;

    bool canbark = true;

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

    public void Die()
    {
        source.Stop();

        canbark = false;

        source.pitch = 1;

        source.PlayOneShot(deathSound);
    }

    public void PlayNormalBark()
    {
        if (!canbark) return;
        if (normalBarks.Length <= 0) return;
        if (!source) return;

        source.pitch = Random.Range(0.9f, 1.1f);

        AudioClip sound = normalBarks[Random.Range(0, normalBarks.Length - 1)];

        source.PlayOneShot(sound);
    }

    public void PlayLoudBark()
    {
        if (!canbark) return;
        if (loudBarks.Length <= 0) return;
        if (!source) return;

        source.pitch = Random.Range(0.9f, 1.1f);

        AudioClip sound = loudBarks[Random.Range(0, loudBarks.Length - 1)];

        source.PlayOneShot(sound);
    }
}
