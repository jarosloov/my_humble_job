using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Musicvolume : MonoBehaviour
{
    public AudioSource audioSrc;
    public float musicVolue = 0f;

    public void Start()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    public void Update()
    {
        audioSrc.volume = musicVolue;
    }

    public void SetVolume(float vol)
    {
        musicVolue = vol;
    }
}
