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
    public AudioSource AS_clock;
    
    public AudioSource AS_playerActions;
    public AudioSource AS_fire;
    public AudioSource AS_water;
    public AudioSource AS_mom;
    
    //public AudioSource AS_shorts;

    public void InitClockSound(AudioClip s, float scale)
    {
        AS_clock.clip = s;
        if (0 <= scale && scale <= 1)
        {
            AS_clock.volume = scale;
        }
        AS_clock.loop = true;
        AS_clock.Play();
    }

    public void StopClock()
    {
        AS_clock.Stop();
    }
    
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
        //isPlayingSound = true;
        AS_background.PlayOneShot(s);
        //float t = s.length;
        //Invoke(nameof(donePlaying), t);
    }
    
    public void PlaySound(AudioClip s, float scale)
    {
        isPlayingSound = true;
        AS_background.PlayOneShot(s, scale);
        float t = s.length;
        Invoke(nameof(donePlaying), t);
    }

    public void PlayLooped(AudioSource s, AudioClip c)
    {
        s.clip = c;
        s.loop = true;
        s.Play();
    }

    public void stopSound(AudioSource s)
    {
        s.Stop();
    }
    
    public void fireSound(AudioClip clip)
    {
        PlayLooped(AS_fire, clip);
    }

    public void stopfire()
    {
        stopSound(AS_fire);
    }
    public void waterSound(AudioClip clip)
    {
        PlayLooped(AS_water, clip);
    }

    public void stopWater()
    {
        stopSound(AS_water);
    }

    public void playerActionSound(AudioClip clip)
    {
        PlayLooped(AS_playerActions, clip);
    }

    public void stopPlayerActionSound()
    {
        stopSound(AS_playerActions);
    }


}
