using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundDrowning : MonoBehaviour
{
    public AudioClip[] soundsLibrary;
    public AudioSource soundSource;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlaySounds()
    {
        soundSource.clip = soundsLibrary[Random.Range(0, soundsLibrary.Length - 1)];
        if (!soundSource.isPlaying)
        {
            soundSource.Play();
        }
    }
}
