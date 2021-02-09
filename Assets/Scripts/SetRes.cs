using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRes : MonoBehaviour
{
    
    public float PPU = 250;
    public float OSize = 12.71f;

    void Start()
    {
        setRes();
    }

    private void setRes()
    {
        float height = 2 * OSize * PPU;
        float width = height * 16 / 9;

        float screenRatio = (float) Screen.width / (float) Screen.height;
        float expectedRatio = 16f / 9f;
        float diffRatio = screenRatio / expectedRatio;

        float newOSize;
        if (screenRatio < expectedRatio)
        {
            newOSize = (height / (2 * PPU)) / diffRatio;
            //Camera.main.orthographicSize = (height / (2 * PPU)) / diffRatio;
        }
        else
        {
            newOSize = height / (2 * PPU);
            //Camera.main.orthographicSize = (height /(2 * PPU));
        }
        //CanvasManager.instance.UpdateRes(width, height);
        height = 2 * OSize;
        width = height * 16 / 9;
        Camera.main.orthographicSize = newOSize;
        CanvasManager.instance.UpdateRes(width, height);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("changing");
            setRes();
        }
    }
}