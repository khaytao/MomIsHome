using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuLogic : MonoBehaviour
{
    public GameObject main;
    public GameObject inst;
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.i.PlayBackGround(AudioFileGetter.i.BackGroundMainMenu);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            backToManu();
        }
    }

    public void toInst()
    {
        main.SetActive(false);
        inst.SetActive(true);    
    }
    
    public void backToManu()
    {
        main.SetActive(true);
        inst.SetActive(false);
    }
}
