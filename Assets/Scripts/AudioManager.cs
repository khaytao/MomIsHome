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
    
    public AudioSource AS;

    public void PlayBackGround(AudioClip s)
    {
        AS.clip = s;
        AS.loop = true;
        AS.Play();
    }

}
