using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager
{
    public static void PlaySound(AudioClip s)
    {
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource AS = soundGameObject.AddComponent<AudioSource>();
        AS.PlayOneShot(s);
    }

    public static void PlayBackGround(AudioClip s)
    {
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource AS = soundGameObject.AddComponent<AudioSource>();
        AS.clip = s;
        AS.loop = true;
        AS.Play();
    }
}
