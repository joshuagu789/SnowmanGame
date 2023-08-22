/*
 * Alternates between music (audio clips) for different occasions
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioSource;
    public List<AudioClip> gameStartMusic;
    public List<AudioClip> waveMusic;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayWaveMusic()
    {
        audioSource.clip = waveMusic[(int)Random.Range(0,waveMusic.Count)];
        audioSource.PlayDelayed(1f);
    }
}
