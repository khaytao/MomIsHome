using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    /*private void Start()
    {
        _AS = gameObject.AddComponent<AudioSource>();
    }*/

    private static AudioManager _i;

    public static AudioManager i
    {
        get
        {
            if (!_i)
            {
                _i = (Instantiate(Resources.Load("utils/AudioManager")) as GameObject).GetComponent<AudioManager>();
                
            }
            return _i;
        }
    }

    public bool isPlayingSound;
    
    public AudioSource AS_background;
    //public AudioSource AS_shorts;
    
    public void PlayBackGround(AudioClip s)
    {
        AS_background.clip = s;
        AS_background.loop = true;
        AS_background.Play();
    }

    private void donePlaying()
    {
        isPlayingSound = false;
    }

    public void PlaySound(AudioClip s)
    {
        isPlayingSound = true;
        AS_background.PlayOneShot(s);
        float t = s.length;
        Invoke(nameof(donePlaying), t);
    }
    
    public void PlaySound(AudioClip s, float scale)
    {
        isPlayingSound = true;
        AS_background.PlayOneShot(s, scale);
        float t = s.length;
        Invoke(nameof(donePlaying), t);
    }

}
