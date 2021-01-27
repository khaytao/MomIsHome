using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{

    public void StartPlaying()
    {
        SceneManager.LoadScene("Levels");
        //GameManager.Instance.LoadLevelPrefabs(1);
        //Instantiate(Resources.Load("levels/level1", typeof(GameObject)) as GameObject);
    }
}
