using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public GameObject midgame;
    public GameObject lost;
    public GameObject won;
    public GameObject pause;
    
    public static bool isPaused;
    public float menuDelay;

    private static CanvasManager _instance;

    public static CanvasManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (Instantiate(Resources.Load("utils/CanvasManager")) as GameObject).GetComponent<CanvasManager>();
            }

            return _instance;
        }
    }
    
    private void Start()
    {
        resetScreens();
        GameScreen();
    }

    public void resetScreens()
    {
        midgame.SetActive(false);
        lost.SetActive(false);
        won.SetActive(false);
        pause.SetActive(false);
    }

    public void GameScreen()
    {
        midgame.SetActive(true);
        lost.SetActive(false);
        won.SetActive(false);
        pause.SetActive(false);
    }
    
    public void WonScreen()
    {
        midgame.SetActive(false);
        won.SetActive(true);
    }
    public void LostScreen()
    {
        lost.SetActive(true);

    }

    public void Pause()
    {
        Time.timeScale = 0;
        pause.SetActive(true);
        isPaused = true;
    }
    
    public void Resume()
    {
        Time.timeScale = 1;
        pause.SetActive(false);
        isPaused = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !GameManager.Instance.gameOver)
        {
            if (isPaused)
            {
                int delayMS = (int) menuDelay * 1000;
                System.Threading.Thread.Sleep(delayMS);
                Resume();
            }
            else
            {
                Invoke(nameof(Pause), menuDelay);
            }
        }
    }
}
