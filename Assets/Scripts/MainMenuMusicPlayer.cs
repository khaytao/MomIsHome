using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuMusicPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.i.PlayBackGround(AudioFileGetter.i.BackGroundMainMenu);
    }
}
