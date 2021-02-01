using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager
{
    private static GameObject _soundGameObject = new GameObject("Sound");
    private static AudioSource _AS = _soundGameObject.AddComponent<AudioSource>();
    
    public static void PlaySound(AudioClip s)
    {
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource AS = soundGameObject.AddComponent<AudioSource>();
        AS.PlayOneShot(s);
    }

    /*
    public static void PlayBackGround(AudioClip s)
    {
        /*if (_curPlaying)
        {
            _curPlaying.Stop();
        }
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource AS = soundGameObject.AddComponent<AudioSource>();
        AS.clip = s;
        AS.loop = true;
        AS.Play();
        _curPlaying = AS;#1#
        _AS.Stop();
        _AS.clip = s;
        _AS.loop = true;
        _AS.Play();
    }*/
}
