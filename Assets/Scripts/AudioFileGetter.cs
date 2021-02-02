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
    
    [Header("Alexa")]
    public AudioClip alexaIn;
    public AudioClip alexaout;
    public AudioClip alexaWin;
    public AudioClip alexaLose;
    
    
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

    [Header("screens")]
    public AudioClip BackGroundMainMenu;
    public AudioClip BackGroundLevel;
    public AudioClip BackGroundLevelWon;
    public AudioClip BackGroundLevelLost;
    public AudioClip BackGroundEndGame;

    
    public AudioClip winGame;
    public AudioClip loseGame;
    public AudioClip PlaceHolder1;

    [Header("UI")]
    public AudioClip clock;
    
    
    public AudioClip menuMove;
    public AudioClip menuSelect;
    
    [Header("Main Character")]
    public AudioClip run;
    public AudioClip pick;
    public AudioClip drop;
    public AudioClip fireEx;
    public AudioClip mop;
    public AudioClip fix;
    public AudioClip trash;
    public AudioClip angry;
    
    [Header("NPCs")]
    public AudioClip vomit;
    
    [Header("SFX")]
    public AudioClip Break;
    public AudioClip water;
    public AudioClip fire;
    
    [Header("Mom")]
    public AudioClip momCar;
    public AudioClip momPhone;
    public AudioClip momWin;
    public AudioClip momLose;
    
    
}
