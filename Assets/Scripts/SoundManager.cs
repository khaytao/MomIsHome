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
}
