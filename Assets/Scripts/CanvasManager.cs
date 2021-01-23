using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public GameObject midgame;
    public GameObject lost;
    public GameObject won;
    
    public void GameScreen()
    {
        midgame.SetActive(true);
        lost.SetActive(false);
        won.SetActive(false);
    }
    public void WonScreen()
    {
        midgame.SetActive(false);
        lost.SetActive(false);
        won.SetActive(true);
    }
    public void LostScreen()
    {
        midgame.SetActive(false);
        lost.SetActive(true);
        won.SetActive(false);
    }
}
