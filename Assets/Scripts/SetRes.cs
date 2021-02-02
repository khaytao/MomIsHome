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
        float expectedRatio = height / width;
        float diffRatio = screenRatio / expectedRatio;

        if (screenRatio < expectedRatio)
        {
            Camera.main.orthographicSize = (height / 200f) / diffRatio;
        }
        else
        {
            Camera.main.orthographicSize = (height / 200f);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            setRes();
        }
    }
}