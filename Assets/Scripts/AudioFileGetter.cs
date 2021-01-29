using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFileGetter : MonoBehaviour
{
    private static AudioFileGetter _i;
    public static AudioFileGetter i
    { 
        get
        {
            if (_i == null)
            {
                _i = (Instantiate(Resources.Load("utils/AudioFileGetter")) as GameObject).GetComponent<AudioFileGetter>();
                
                
            }
            return _i;
        }
    }

    public AudioClip s;
    
    public AudioClip alexaSnarky1;
    public AudioClip alexaSnarky2;
    public AudioClip alexaSnarky3;
    public AudioClip alexaSnarky4;
    public AudioClip alexaSnarky5;
    public AudioClip alexaSnarky6;
    public AudioClip alexaSnarky7;
    public AudioClip alexaSnarky8;
    public AudioClip alexaSnarky9;
    public AudioClip alexaSnarky10;

    public AudioClip timeLeft90;
    public AudioClip timeLeft60;
    public AudioClip timeLeft30;
    public AudioClip timeLeft15;

    public AudioClip BackGroundMainMenu;
    public AudioClip BackGroundLevel;
    public AudioClip BackGroundLevelWon;
    public AudioClip BackGroundLevelLost;
    public AudioClip BackGroundEndGame;

}
