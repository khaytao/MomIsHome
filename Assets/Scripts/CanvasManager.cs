using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public GameObject midgame;
    public GameObject lost;
    public GameObject won;
    public GameObject pause;
    
    public GameObject clock;
    
    public static bool isPaused;
    public float menuDelay;

    private Transform Alexa;
    
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
        Alexa = midgame.transform.Find("Alexa");
        resetScreens();
        GameScreen();
    }

    public void resetScreens()
    {
        Alexa.gameObject.SetActive(false);
        
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

    public void WonScreenFade()
    {
        midgame.SetActive(false);
        
        Image BG_IM =  won.transform.GetChild(0).GetComponent<Image>();
        BG_IM.color = new Color(1, 1, 1, 0);
        won.SetActive(true);
        StartCoroutine(FadeBackGroundIn(BG_IM));
    }

    private IEnumerator FadeBackGroundIn(Image img)
    {
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            // set color with i as alpha
            img.color = new Color(1, 1, 1, i);
            yield return null;
        }
    }
    
    public void LostScreenFade()
    {
        midgame.SetActive(false);
        
        Image BG_IM =  lost.transform.GetChild(0).GetComponent<Image>();
        BG_IM.color = new Color(1, 1, 1, 0);
        won.SetActive(true);
        //StartCoroutine(FadeBackGroundIn(BG_IM));
    }

    public void LostScreen()
    {
        midgame.SetActive(false);
        lost.SetActive(true);
        //StartCoroutine(EaseScreenIn(lost.transform.GetChild(0).gameObject));
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

        if (Input.GetKeyDown(KeyCode.P))
        {
            GameManager.Instance.ActivateAlexa(AudioFileGetter.i.alexaSnarky1, "WOW!");
        }

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

        if (midgame.activeSelf && !GameManager.Instance.isAlexaPlaying)
        {
            Alexa.gameObject.SetActive(false);
        }
    }

    public void ResetClock()
    {
        clock = Instantiate(Resources.Load("utils/AudioFileGetter")) as GameObject;
    }

    public void alexa(string txt)
    {
        if (!midgame.activeSelf)
        {
            return;
        }
        
        Alexa.gameObject.SetActive(true);
        Alexa.Find("Text").GetComponent<TMP_Text>().text = txt;
        
    }
    
    private IEnumerator EaseScreenIn(GameObject im)
    {
        var  trans = im.transform;
        float x = trans.position.x;
        trans.position = new Vector3(x, start);
        float acceleration = Mathf.Abs(acc1);
        float t = 0;
        
        while (im.transform.position.y < goal)
        {
            
            if (im.transform.position.y > thresh1)
            {
                acceleration = 0;
            }
            else if (im.transform.position.y > thresh2)
            {
                acceleration = acc2;
            }
            im.transform.position = Vector3.right * x + Vector3.up * (float)(start + 0.5 * acceleration *(t*t));
            t += Time.deltaTime;
            Debug.Log("im moved: " + im.transform.position + "  ---- time---- " + t);
            yield return null;
        }
    }

    public float acc1;
    public float acc2;
    
    public float thresh1;
    public float thresh2;
    public float start;
    public float goal;
}
