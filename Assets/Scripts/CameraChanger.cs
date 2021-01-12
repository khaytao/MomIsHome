using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChanger : MonoBehaviour
{
    public Camera camOver;
    public Camera camZoomed;

    private void Start()
    {
        camOver.enabled = true;
        camZoomed.enabled = false;
        StartCoroutine(ExecuteAfterTime(3));
    }

    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
 
        camOver.enabled = !camOver.enabled;
        camZoomed.enabled = !camZoomed.enabled;
    }
    
    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.C)) 
        {
            camOver.enabled = !camOver.enabled;
            camZoomed.enabled = !camZoomed.enabled;
        }
    }
}
