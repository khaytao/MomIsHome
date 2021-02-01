using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    
    public void StartPlaying()
    {
        Debug.Log("clicked Button");
        GameManager.Instance.FirstLevel();
        //Invoke(nameof(_StartPlaying), 0.35f);
        //GameManager.Instance.LoadLevelPrefabs(1);
        //Instantiate(Resources.Load("levels/level1", typeof(GameObject)) as GameObject);
    }

    private void _StartPlaying()
    {
        Debug.Log(" Time");
        GameManager.Instance.FirstLevel();
    }

    public void ShowInstructions()
    {
        FindObjectOfType<MainMenuLogic>().toInst();
    }
    
    public void back()
    {
        FindObjectOfType<MainMenuLogic>().backToManu();
    }
    
}
