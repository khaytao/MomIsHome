using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttons : MonoBehaviour
{
    /*public void ReplayLevel()
    {
        GameManager.Instance.endGame();
    }*/

    public void NextLevel()
    {
        Time.timeScale = 1;
        MyGameManager.Instance.NextLevel();
    }
    public void ResumeGame()
    {
        CanvasManager.instance.Resume();
    }

    public void Restart()
    {
        // if (CanvasManager.isPaused)
        // {
        //     CanvasManager.instance.Resume();
        // }
        CanvasManager.instance.Resume();
        MyGameManager.Instance.reloadLevel();
    }
    
    public void MainManu()
    {
        MyGameManager.Instance.MainMenu();
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }

    public void OnHover()
    {
        AudioManager.i.PlaySound(AudioFileGetter.i.menuMove);
    }
    
    public void OnClick()
    {
        AudioManager.i.PlaySound(AudioFileGetter.i.menuSelect);
    }
}
