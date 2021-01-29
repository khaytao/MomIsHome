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
        GameManager.Instance.NextLevel();
    }
    public void ResumeGame()
    {
        FindObjectOfType<CanvasManager>().Resume();
    }

    public void Restart()
    {
        if (CanvasManager.isPaused)
        {
            FindObjectOfType<CanvasManager>().Resume();
        }
        GameManager.Instance.reloadLevel();
    }
    
    public void MainManu()
    {
        GameManager.Instance.MainMenu();
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
}
