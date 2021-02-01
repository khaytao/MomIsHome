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
    public AudioClip alexaPlaceHolder1;
    public AudioClip alexaPlaceHolder2;
    
    
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
    
        
    
    
    


    /*Screens:
    win game - Main/Title Screen + Win screen  (once)
        ???? - Game itself screen (once> timed per level)
    lose game - Lose game screen (once)

    UI/X: 
    clock - clock ticking (loop) > low volume idle > volume up when close to end
    buttons:
    Menu Move - mouse hover (once)
    Menu Select - clicked (once)

    Alexa:
    alexa in - before speak {turn on sound} (once)
        alexa out - after finished speaking {turn off} (once)
    ???? - Win screen, {Don't worry, I won't tell... We should do this again} (once)
    ???? - Lose screen, {I told you. But you didn't listen} (once)
        every 30 seconds reads the time:
    1 min 30 sec - 1 min and 30 sec till the end (once)
        1 min - 1 min till the end (once)
        30 sec - 30 sec till the end (once)
        15 sec - 15 sec till the end (once)
    speech triggered by events:
    snarky 1-10 - random (once)*/

}
